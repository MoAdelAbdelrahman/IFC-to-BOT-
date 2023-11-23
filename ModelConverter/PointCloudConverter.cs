using DbmsApi.API;
using g3;
using MathPackage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverter
{
    public class PointCloudConverter
    {
        private static double SCALE = 1.0f;
        //private static string DefaultType = "N/A";

        public static Model ConvertPointFilesToModel(string modelName, string[] fileNames, bool flat, bool flip)
        {
            List<CreatedObject> createdObjects = new List<CreatedObject>();
            foreach (string fileName in fileNames)
            {
                CreatedObject newObj = GetPointsFromFile(Path.GetFileNameWithoutExtension(fileName), File.ReadAllLines(fileName), flip);
                if (newObj != null)
                {
                    createdObjects.Add(newObj);
                }
            }

            List<Vector3D> points = createdObjects.SelectMany(o => o.Points).ToList();
            //GetFarthestPoints(points, out Vector3 maxV1, out Vector3 maxV2);
            //Vector3 pointDirection = (maxV1 - maxV2).normalized;
            Vector3D pointDirection = flip ? Vector3D.X.Neg() : Vector3D.X;
            Vector3D averagePoint = new Vector3D(points.Sum(p => p.x) / (double)points.Count, points.Sum(p => p.y) / (double)points.Count, points.Sum(p => p.z) / (double)points.Count);

            createdObjects.ForEach(o => o.MoveAllPoints(averagePoint));
            createdObjects.ForEach(o => o.ReorderPointsByDir(pointDirection));

            Model finalModel = new Model()
            {
                Name = modelName + (flat ? "_flat" : ""),
                ModelObjects = new List<ModelObject>(),
                Properties = new DbmsApi.API.Properties(),
                Tags = new List<KeyValuePair<string, string>>(),
                Relations = new List<Relation>()
            };

            List<ModelObject> modelObjects = new List<ModelObject>();
            foreach (CreatedObject co in createdObjects)
            {
                if (co.Name == "Marking")
                {
                    modelObjects.AddRange(GetRoadObject(co, flat, flip));
                }
                else
                {
                    modelObjects.Add(GetSignObject(co, flat, flip));
                }
            }

            finalModel.ModelObjects.AddRange(modelObjects);

            return finalModel;
        }

        private static List<ModelObject> GetRoadObject(CreatedObject co, bool flat, bool flip)
        {
            List<ModelObject> returnList = new List<ModelObject>();
            foreach (CreatedObject splitObj in CreatedObject.SplitCreatedObject(co, flip))
            {
                List<Component> components = new List<Component>();
                splitObj.CreateShapeFromPointsWrap(flat);
                if (splitObj.Mesh == null)
                {
                    continue;
                }

                components.Add(new Component()
                {
                    Properties = new DbmsApi.API.Properties(),
                    Tags = new List<KeyValuePair<string, string>>(),
                    Triangles = splitObj.Mesh.TriangleList,
                    Vertices = splitObj.Mesh.VertexList,
                    MaterialId = Material.Default().Name
                });

                Vector4D orientation = flip ? Utils.GetQuaterion(Vector3D.Z, -Math.PI / 2.0) : Utils.GetQuaterion(Vector3D.Z, Math.PI / 2.0);
                Vector3D location = ConverterGeneral.CenterObject(components, 1.0, orientation);

                ModelObject modelObject = new ModelObject()
                {
                    Name = co.Name,
                    Id = Guid.NewGuid().ToString(),
                    Components = components,
                    Location = location,
                    Orientation = orientation,
                    Properties = new DbmsApi.API.Properties(),
                    Tags = new List<KeyValuePair<string, string>>(),
                    TypeId = "Road"
                };

                returnList.Add(modelObject);
            }

            return returnList;
        }

        private static ModelObject GetSignObject(CreatedObject co, bool flat, bool flip)
        {
            List<Component> components = new List<Component>();
            foreach (CreatedObject splitObj in CreatedObject.SplitCreatedObject(co, flip))
            {
                splitObj.CreateShapeFromPointsWrap(flat);
                if (splitObj.Mesh == null)
                {
                    continue;
                }

                components.Add(new Component()
                {
                    Properties = new DbmsApi.API.Properties(),
                    Tags = new List<KeyValuePair<string, string>>(),
                    Triangles = splitObj.Mesh.TriangleList,
                    Vertices = splitObj.Mesh.VertexList,
                    MaterialId = Material.Default().Name
                });
            }

            Vector4D orientation = Utils.GetQuaterion(Vector3D.Z, Math.PI / 2.0);
            Vector3D location = ConverterGeneral.CenterObject(components, 1.0, orientation);

            ModelObject modelObject = new ModelObject()
            {
                Name = co.Name,
                Id = Guid.NewGuid().ToString(),
                Components = components,
                Location = location,
                Orientation = orientation,
                Properties = new DbmsApi.API.Properties(),
                Tags = new List<KeyValuePair<string, string>>(),
                TypeId = "Sign"
            };

            return modelObject;
        }

        private static CreatedObject GetPointsFromFile(string name, string[] dataLines, bool flip)
        {
            List<Vector3D> points = new List<Vector3D>();
            for (int i = 0; i < dataLines.Length; i++)
            {
                if (i == 0)
                {
                    continue;
                }
                try
                {
                    string line = dataLines[i];
                    string[] lineSplit = line.Split(',');
                    double x = Convert.ToSingle(lineSplit[0]);
                    double y = Convert.ToSingle(lineSplit[1]);
                    double z = Convert.ToSingle(lineSplit[2]);

                    points.Add(new Vector3D(x / SCALE, y / SCALE, z / SCALE));
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                }
            }

            if (points.Count == 0)
            {
                return null;
            }

            return new CreatedObject(points, name, flip);
        }
    }

    public class CreatedObject
    {
        private static int NUMBER_OF_POINTS = 1000;

        public List<Vector3D> Points;
        public string Name;
        public Mesh Mesh;

        public CreatedObject(List<Vector3D> points, string name, bool flip)
        {
            Points = flip ? points.OrderByDescending(p => p.x).ToList() : points.OrderBy(p => p.x).ToList();
            Name = name;
        }

        public void MoveAllPoints(Vector3D moveVect)
        {
            Points = Points.Select(p => Vector3D.Subract(p, moveVect)).ToList();
        }

        public void CreateShapeFromPointsWrap(bool flat, bool flipTriangles = false)
        {
            if (flat)
            {
                CreateShapeFromPointsWrapFlat(flipTriangles);
            }
            else
            {
                CreateShapeFromPointsWrap3D(flipTriangles);
            }
        }

        public void CreateShapeFromPointsWrap3D(bool flipTriangles = false)
        {
            try
            {
                double top = Points.Max(p => p.z);
                double bottom = Points.Min(p => p.z);

                // Find convex hull of points and triagulate
                ConvexHull2 cHull = new ConvexHull2(Points.Select(p => new Vector2d(p.x, p.y)).ToList(), double.Epsilon, QueryNumberType.QT_DOUBLE);
                List<Vector3D> hullTop = cHull.HullIndices.Select(index => new Vector3D(Points[index].x, Points[index].y, top)).ToList();
                List<Vector3D> hullBottom = cHull.HullIndices.Select(index => new Vector3D(Points[index].x, Points[index].y, bottom)).ToList();

                //List<Vector3D> cHull = Utils.GetConvexHull(Points.Select(p => new Vector3D(p.x, p.y, 0.0)).ToList());
                //List<Vector3D> hullTop = cHull.Select(p => new Vector3D(p.x, p.y, top)).ToList();
                //List<Vector3D> hullBottom = cHull.Select(p => new Vector3D(p.x, p.y, bottom)).ToList();

                // Extrude solid based on points Z range
                List<int[]> triangles = new List<int[]>();
                for (int i = 2; i < hullTop.Count; i++)
                {
                    triangles.Add(new int[] { 0, i - 1, i });
                    triangles.Add(new int[] { hullTop.Count, hullTop.Count + i, hullTop.Count + i - 1 });
                }
                for (int i = 0; i < hullTop.Count; i++)
                {
                    int j = (i + 1) % hullTop.Count;
                    triangles.Add(new int[] { i, hullTop.Count + i, j });
                    triangles.Add(new int[] { hullTop.Count + i, hullTop.Count + j, j });
                }

                if (flipTriangles)
                {
                    triangles = triangles.Select(t => new int[] { t[0], t[2], t[1] }).ToList();
                }

                List<Vector3D> verts = new List<Vector3D>();
                verts.AddRange(hullTop);
                verts.AddRange(hullBottom);

                Mesh = new Mesh(verts, triangles);
            }
            catch (Exception ex)
            {
                Utils.GetXYZDimentions(Points, out Vector3D center, out Vector3D dims);
                Mesh = Utils.CreateBoundingBox(center, dims, FaceSide.FRONT);
                Debug.Log(ex.ToString());
            }
        }

        public void CreateShapeFromPointsWrapFlat(bool flipTriangles = false)
        {
            try
            {
                // Find convex hull of points and triagulate
                ConvexHull2 cHull = new ConvexHull2(Points.Select(p => new Vector2d(p.x, p.y)).ToList(), double.Epsilon, QueryNumberType.QT_DOUBLE);
                List<Vector3D> hullTop = cHull.HullIndices.Select(index => new Vector3D(Points[index].x, Points[index].y, 0.0)).ToList();

                //List<Vector3D> cHull = Utils.GetConvexHull(Points.Select(p => new Vector3D(p.x, p.y, 0.0)).ToList());
                //List<Vector3D> hullTop = cHull.Select(p => new Vector3D(p.x, p.y, 0.0)).ToList();

                List<int[]> triangles = new List<int[]>();
                for (int i = 2; i < hullTop.Count; i++)
                {
                    triangles.Add(new int[] { 0, i - 1, i });
                }

                if (flipTriangles)
                {
                    triangles = triangles.Select(t => new int[] { t[0], t[2], t[1] }).ToList();
                }

                List<Vector3D> verts = new List<Vector3D>();
                verts.AddRange(hullTop);

                Mesh = new Mesh(verts, triangles);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());

                Vector3D center = Vector3D.Average(Points.ToArray());
                List<Vector3D> verts = new List<Vector3D>()
                {
                    Vector3D.Add(center, new Vector3D(0.1, 0.1, 0.0)),
                    Vector3D.Add(center, new Vector3D(0.1, -0.1, 0.0)),
                    Vector3D.Add(center, new Vector3D(-0.1, -0.1, 0.0)),
                    Vector3D.Add(center, new Vector3D(-0.1, 0.1, 0.0))
                };
                List<int[]> triangles = new List<int[]>()
                {
                    new int[]{0,3,1},
                    new int[]{3,2,1},
                };
                if (flipTriangles)
                {
                    triangles = triangles.Select(t => new int[] { t[0], t[2], t[1] }).ToList();
                }
                Mesh = new Mesh(verts, triangles);
            }
        }

        public void ReorderPointsByDir(Vector3D directionVect)
        {
            double angle = Vector3D.AngleRad(directionVect, Vector3D.X);
            Vector3D axis = Vector3D.Cross(directionVect, Vector3D.X).Norm();
            Vector4D orientation = Utils.GetQuaterion(axis, angle);
            Matrix4 T = Utils.GetTranslationMatrixFromLocationOrientation(new Vector3D(), orientation);
            Matrix4 TInv = T.GetInverse(out bool hasInverse);

            if (hasInverse)
            {
                Points = Points.OrderBy(p => Matrix4.Multiply(TInv, new Vector4D(p.x, p.y, p.z, 1.0f)).x).ToList();
            }
        }

        public static List<CreatedObject> SplitCreatedObject(CreatedObject original, bool flip)
        {
            List<CreatedObject> splitObjects = new List<CreatedObject>();
            List<Vector3D> copyOfPoints = original.Points.Select(p => new Vector3D(p.x, p.y, p.z)).ToList();
            int counter = 0;
            while (copyOfPoints.Count > 0)
            {
                int numberOfPointsToRemove = Math.Min(NUMBER_OF_POINTS, copyOfPoints.Count);
                splitObjects.Add(new CreatedObject(copyOfPoints.GetRange(0, numberOfPointsToRemove), original.Name + "_" + counter, flip));
                copyOfPoints.RemoveRange(0, numberOfPointsToRemove);
                counter++;
            }

            return splitObjects;
        }
    }
}