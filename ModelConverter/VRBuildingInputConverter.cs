using DbmsApi;
using DbmsApi.API;
using MathPackage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverter
{
    public class VRBuildingInputConverter
    {
        public static Model VRBuildingInputToModel(string path)
        {
            VRBuildingInputClass newVrBuilding = DBMSReadWrite.JSONReadFromFile<VRBuildingInputClass>(path);
            return VRBuildingInputToModel(newVrBuilding);
        }

        public static Model VRBuildingInputToModel(VRBuildingInputClass vrBuildingInput)
        {
            Model newModel = new Model();
            newModel.Name = "VRModel";

            float floorHeight = vrBuildingInput.walls.SelectMany(c => c.corners).Min(v => v.y);
            List<Vector3D> floorVerts = new List<Vector3D>();
            foreach (Wall wall in vrBuildingInput.walls)
            {
                Component co = new Component()
                {
                    Vertices = wall.corners.Select(c => new Vector3D(c.x, c.z, c.y)).ToList(),
                    Triangles = new List<int[]>()
                    {
                        new int[]{ 0,1,2 },
                        new int[]{ 0,2,3 },
                        new int[]{ 0,2,1 },
                        new int[]{ 0,3,2 }
                    }
                };

                bool addedAFloorItem = false;
                List<Vector3D> bottomPoints = new List<Vector3D>();
                foreach (Corner c in wall.corners)
                {
                    if (Math.Abs(c.y - floorHeight) < 0.01)
                    {
                        Vector3D addingVect = new Vector3D(c.x, c.z, floorHeight);
                        bottomPoints.Add(addingVect);
                        if (floorVerts.Any(v => Vector3D.Distance(addingVect, v) < 0.01))
                        {
                            continue;
                        }
                        if (!addedAFloorItem)
                        {
                            floorVerts.Add(addingVect);
                            addedAFloorItem = true;
                        }
                    }
                }

                Vector3D wallForwardDir = null;
                if (bottomPoints.Count == 2)
                {
                    Vector3D wallDir = Vector3D.Subract(bottomPoints[0], bottomPoints[1]);
                    wallForwardDir = new Vector3D(wallDir.y, wallDir.x, wallDir.z).Norm();
                }
                else
                {
                    // weird wall??
                }

                List<Component> cos = new List<Component>() { co };
                Vector3D upDir = new Vector3D(0, 0, 1);
                Vector3D wallRightDir = Vector3D.Cross(wallForwardDir, upDir);
                Utils.GetLocationAndOrientationFromTranslationMatrix(new Matrix4(wallRightDir.Get4D(0), wallForwardDir.Get4D(0), upDir.Get4D(0), new Vector4D(0, 0, 0, 1)), out Vector3D locTemp, out Vector4D orient);
                Vector3D loc = ConverterGeneral.CenterObject(cos, 1.0, orient);

                ModelObject mo = new ModelObject()
                {
                    TypeId = "Wall",
                    Id = wall.id.ToString(),
                    Components = cos,
                    Location = loc,
                    Orientation = orient
                };


                newModel.ModelObjects.Add(mo);
            }

            floorVerts.Reverse();
            List<int[]> floorTris = Utils.EarClip(floorVerts, new Vector3D(0, 0, 1), false);
            List<int[]> floorTrisFlip = Utils.EarClip(floorVerts, new Vector3D(0, 0, 1), true);
            List<int[]> allTris = new List<int[]>();
            allTris.AddRange(floorTris);
            allTris.AddRange(floorTrisFlip);
            Component floorCo = new Component()
            {
                Vertices = floorVerts,
                Triangles = floorTris
            };
            List<Component> floorCos = new List<Component>() { floorCo };
            Vector4D floorOrient = new Vector4D(0, 0, 0, 1);
            Vector3D floorLoc = ConverterGeneral.CenterObject(floorCos, 1.0, floorOrient);

            ModelObject floorMo = new ModelObject()
            {
                TypeId = "Floor",
                Id = vrBuildingInput.walls.Count().ToString(),
                Components = floorCos,
                Location = floorLoc,
                Orientation = floorOrient
            };

            newModel.ModelObjects.Add(floorMo);

            return newModel;
        }
    }
}