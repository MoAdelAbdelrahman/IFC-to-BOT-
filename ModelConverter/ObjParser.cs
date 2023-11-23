using DbmsApi.API;
using DbmsApi.Mongo;
using MathPackage;
using ObjParser;
using ObjParser.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModelConverter
{
    public static class ObjParser
    {
        public enum FaceSide { FRONT, BACK, BOTH }

        public static CatalogObject ConvertObjFile(FileStream fileStream, string objectName,double scale, bool flipTriangles, bool flipYZ)
        {
            // Load and parse the obj file
            Obj obj = new Obj();
            obj.LoadObj(fileStream);

            CatalogObject newModelObject = ConvertObjFile(obj, objectName, scale, flipTriangles, flipYZ);
            newModelObject.Tags.Add(new KeyValuePair<string, string>("Dataset", ConverterGeneral.Datasets.OBJ.ToString()));
            return newModelObject;
        }

        public static CatalogObject ConvertObjFile(Obj obj, string objectName, double scale, bool flipTriangles, bool flipYZ)
        {
            List<Component> components = new List<Component>();
            //  Get faces (list of faces and normals)
            List<Tuple<Vector3D, List<Vector3D>>> faces = obj.FaceList.Select(face => GetFace(face, obj, flipYZ)).ToList();

            // Attempt to tesellate the faces
            List<int> triangles = new List<int>();
            List<Vector3D> finalVertices = new List<Vector3D>();
            foreach (Tuple<Vector3D, List<Vector3D>> face in faces)
            {
                if (face.Item2.Count < 3)
                {
                    continue;
                }

                List<int> tempTriangleInts = Utils.EarClip(face.Item2, face.Item1);
                foreach (int intVal in tempTriangleInts)
                {
                    triangles.Add(intVal + finalVertices.Count);
                }
                finalVertices.AddRange(face.Item2);
            }

            List<int[]> trianglesTuples = new List<int[]>();
            for (int index = 0; index < triangles.Count; index += 3)
            {
                if (flipTriangles)
                {
                    trianglesTuples.Add(new int[3] { triangles[index], triangles[index + 1], triangles[index + 2] });
                }
                else
                {
                    trianglesTuples.Add(new int[3] { triangles[index], triangles[index + 2], triangles[index + 1] });
                }
            }

            Component component = new Component()
            {
                Triangles = trianglesTuples,
                Vertices = finalVertices.Select(v => v.Copy()).ToList(),
                MaterialId = DbmsApi.API.Material.Default().Name,
                Properties = new DbmsApi.API.Properties(),
                Tags = new List<KeyValuePair<string, string>>()
            };
            components.Add(component);

            ConverterGeneral.CenterObject(components, scale, new Vector4D(0, 0, 0, 1));

            CatalogObject modelSpecificObject = new CatalogObject()
            {
                Name = objectName,
                CatalogID = null,
                Components = components,
                Properties = new DbmsApi.API.Properties(),
                TypeId = "N/A"
            };

            return modelSpecificObject;
        }

        public static Tuple<Vector3D, List<Vector3D>> GetFace(Face face, Obj obj, bool flipYZ)
        {
            List<Vector3D> vertices = new List<Vector3D>();
            Vector3D normal = new Vector3D(0, 0, 0);
            int counter = 0;
            bool fixNormals = false;
            for (int i = 0; i < face.VertexIndexList.Length; i++)
            {
                //if (counter > 0 && !NormalsEqual(normal, obj.Normals[face[i].NormalIndex - 1]))
                //{
                //    // weird case when not all verticies have the same normal...
                //    fixNormals = true;
                //}
                //normal = objectLoadResult.Normals[face[i].NormalIndex - 1];             // Every vertex has a normal?
                //Vertex vertex = obj.VertexList[face[i].VertexIndex - 1];     // Check why its -1?
                //vertices.Add(new Vector3D(vertex.X, flipYZ ? vertex.Z : vertex.Y, flipYZ ? vertex.Y : vertex.Z));
                counter++;
            }

            Vector3D norm = new Vector3D(normal.x, flipYZ ? normal.z : normal.y, flipYZ ? normal.y : normal.z);
            if (fixNormals)
            {
                // weird case when not all verticies have the same normal. Just find it ourselves...
                norm = Utils.EstimateFaceNormal(vertices);
            }

            return new Tuple<Vector3D, List<Vector3D>>(norm, vertices);
        }

        //public static bool NormalsEqual(Normal n1, Normal n2)
        //{
        //    return n1.X == n2.X && n1.Y == n2.Y && n1.Z == n2.Z;
        //}

    }
}