using DbmsApi.API;
using MathPackage;
using ObjParser;
using RuleAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing;
using Xbim.Ifc;
using Xbim.Ifc2x3.Interfaces;
using static ModelConverter.Dataset3DFRONTClasses;
using static ModelConverter.DatasetCoohomClassesModel;

namespace ModelConverter
{
    public class TurtleBOTConverter
    {

        public static StringBuilder sb = new StringBuilder();
        public static List<RuleCheckObject> extractedSpaces = new List<RuleCheckObject>();
        public static int buildingLevels { get; set; }


        public static string buildingName { get; set; }
        public static string siteName { get; set; } 
        public TurtleBOTConverter(string buildingName)
        {
            TurtleBOTConverter.buildingName = buildingName;   
        }


        public enum DoorSwingDirection
        {
            Inside,
            Outside,
            Unknown
        }

        //given that I am facing the room from outside
        public static string GetCardinalDirection(Vector3D forwardDirectionY, Vector3D rightDirectionX, Vector3D upDirectionZ)
        {
            // Check the primary component of ForwardDirectionY since it denotes the forward direction
            if (Math.Abs(forwardDirectionY.x) > Math.Abs(forwardDirectionY.y))
            {
                // X component is dominant
                return forwardDirectionY.x > 0 ? "East" : "West";
            }
            else
            {
                // Y component is dominant
                return forwardDirectionY.y > 0 ? "North" : "South";
            }
        }

        public static void TTL_preProcess()
        {
            
            string rdf = $@"
                @prefix {siteName}: <http://{siteName}namespace.com/>.
                @prefix bot: <https://w3id.org/bot#>.
                @prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>.
                @prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#>.
                @prefix owl: <http://www.w3.org/2002/07/owl#>.
                @prefix ex: <http://example.org/>.

                # General {siteName} Information
                {siteName}:UofA rdf:type bot:{siteName}.
                {siteName}:{buildingName} rdf:type bot:Building.
                {siteName}:UofA bot:hasBuilding {siteName}:{buildingName}.

                    #  Defining Door as a subclass of bot:Interface
                        {siteName}:Door rdf:type owl:Class ;
                       rdfs:subClassOf bot:Interface.
                ";

            sb.AppendLine(rdf);
        }

        public static int GetBuildingLevels(string ifcFilePath)
        {
            using (var model = IfcStore.Open(ifcFilePath))
            {
                var buildingStoreys = model.Instances.OfType<IIfcBuildingStorey>().ToList();
                return buildingLevels = buildingStoreys.Count;
            }
        }
        private static string GetDirectionFromVector(MathPackage.Vector3D vector)
        {
            double absX = Math.Abs(vector.x);
            double absY = Math.Abs(vector.y);

            if (absX > absY)  // X is dominant
            {
                return vector.x  > 0 ? "East" : "West";
            }
            else if (absY > absX) // Y is dominant
            {
                return vector.y > 0 ? "North" : "South";
            }
            else // Near equal influence (we'll take this to mean it's a diagonal direction)
            {
                if (vector.x > 0 && vector.y > 0) return "North-East";
                if (vector.x < 0 && vector.y > 0) return "North-West";
                if (vector.x > 0 && vector.y < 0) return "South-East";
                return "South-West";
            }
        }

        public static string GetDoorEntryDirection(MathPackage.Vector3D forwardDirectionY)
        {
            string toDirection = GetDirectionFromVector(forwardDirectionY);
            string fromDirection;

            switch (toDirection)
            {
                case "North":
                    fromDirection = "South";
                    break;
                case "South":
                    fromDirection = "North";
                    break;
                case "East":
                    fromDirection = "West";
                    break;
                case "West":
                    fromDirection = "East";
                    break;
                case "North-East":
                    fromDirection = "South-West";
                    break;
                case "North-West":
                    fromDirection = "South-East";
                    break;
                case "South-East":
                    fromDirection = "North-West";
                    break;
                case "South-West":
                    fromDirection = "North-East";
                    break;
                default:
                    throw new ArgumentException("Invalid direction");
            }

            return $"{fromDirection} to {toDirection}";
        }

        public static string ConvertFormat(string input)
        {
            var match = Regex.Match(input, @"^(.*?_)(\d)(.*)$");
            if (match.Success)
            {
                return $"{match.Groups[1].Value.TrimEnd('_')} {match.Groups[2].Value}-{match.Groups[3].Value}";
            }
            return input;
        }


        public static string GetFloorSuffix(int i)
        {
            if (i == 1) return "st";
            if (i == 2) return "nd";
            if (i == 3) return "rd";
            return "th";
        }

        public static string GetClosestDoor(RuleCheckObject currentDoor, string direction, IEnumerable<RuleCheckObject> allDoors)
        {
            RuleCheckObject closestDoor = null;
            double minDistance = double.MaxValue;
            double threshold = 0.2; // Adjust this value as needed for your data's precision

            foreach (var door in allDoors)
            {
                if (currentDoor == door) continue;

                bool isCandidate = false;

                double distance = Math.Sqrt(Math.Pow(currentDoor.Location.x - door.Location.x, 2) + Math.Pow(currentDoor.Location.y - door.Location.y, 2));

                switch (direction)
                {
                    case "North":
                        isCandidate = door.Location.y > currentDoor.Location.x && Math.Abs(currentDoor.Location.x - door.Location.x) < threshold;
                        break;
                    case "South":
                        isCandidate = door.Location.y < currentDoor.Location.y && Math.Abs(currentDoor.Location.x - door.Location.x) < threshold;
                        break;
                    case "East":
                        isCandidate = door.Location.y > currentDoor.Location.x && Math.Abs(currentDoor.Location.x - door.Location.y) < threshold;
                        break;
                    case "West":
                        isCandidate = door.Location.x < currentDoor.Location.x && Math.Abs(currentDoor.Location.x - door.Location.y) < threshold;
                        break;
                }

                if (isCandidate && distance < minDistance)
                {
                    minDistance = distance;
                    closestDoor = door;
                }
            }
            return closestDoor?.Name;
        }


        public static void GenerateBuildingFloors(int numberOfLevels)
        {
            sb.AppendLine($"# Stories in {buildingName}");
            for (int i = 1; i <= numberOfLevels; i++)
            {
                string floorSuffix = GetFloorSuffix(i);
                sb.AppendLine($"{siteName}:{buildingName}_Floor_{i} rdf:type bot:Storey; rdfs:label \"{i}{floorSuffix} Floor\".");
            }

            // Linking the stories to the building
            sb.AppendLine();
            sb.Append($"{siteName}:{buildingName} bot:hasStorey ");
            for (int i = 1; i <= numberOfLevels; i++)
            {
                sb.Append($"{siteName}:{buildingName}_Floor_{i}");
                if (i < numberOfLevels)
                {
                    sb.Append(", ");
                }
            }
            sb.AppendLine(" .");
            sb.AppendLine();

        }

        public static string ConvertDoorName(string doorName)
        {
            if (doorName == null) return null;
            Regex regex = new Regex(@"(?<=:)(\d+)$");
            Match match = regex.Match(doorName);
            return match.Value;
        }

        public static string ExtractAfterColonForUri(string input)
        {
            Regex regex = new Regex(@":\s*(.*)");
            Match match = regex.Match(input);

            if (match.Success)
            {

                return match.Groups[1].Value;
            }
            else
            {
                return string.Empty;
            }
        }
        public static string ModelToTurtle(Model model)
        {
                
            sb.AppendLine("# Building Rooms Data");
            RuleCheckModel rcModel = new RuleCheckModel(model, false);

            foreach (RuleCheckObject space in rcModel.Objects.Where(o => o.Type == "Room" || o.Type == "Space"))
            {
                var SpaceLocation = space.Location;
                string roomName = space.Name;
                string roomCategory = roomName.Split(':').First().Trim().Replace(' ', '_');

                //ignore stairwells or elevators as they will be generated as interfaces not rooms
                //if (roomCategory == "Stairwell" || roomCategory == "Elevator")
                //    continue;

               

                string roomType = space.Type; // e.g., "Washroom", "Office", etc.

                // Constructing the main room identifier 
                string roomID = $"Room_{roomName.Replace("-", "_")}";

                string roomNumber = new string(roomName.Where(char.IsDigit).ToArray());
                string roomCode = buildingName.Substring(0, 3).ToUpper();
                roomID = $"{roomCode}-{roomNumber}";
                

              
                // Parsing floor information from the space name
                string floorNumber = roomName.Split(' ').Last().Split('_').First();
                // Extracts "1" from "Office: Ath 1_01"


                // Parsing floor information from the space name

                string floorID = $"{buildingName}_Floor_{roomNumber}";

                // Appending the statements to the StringBuilder
                string uri = ExtractAfterColonForUri(roomName.ToUpper()).Replace(' ', '_');
                switch (roomCategory)
                {
                    case "Hallway":
                        uri += "HLWY";
                        break;
                    case "Stairwell":
                        uri += "STRWL";
                        break;
                    case "Elevator":
                        uri += "ELVTR";
                        break;
                    default:

                        break;
                }
                sb.AppendLine($"{siteName}:{uri} a ex:{roomCategory};");
                sb.AppendLine($"    bot:isSpaceOf {siteName}:{buildingName}_Floor_{floorNumber.Split('-').First()};");
                sb.AppendLine($"    bot:IndoorLocation '{SpaceLocation}';");
                sb.AppendLine($"    rdfs:label \"{roomName.ToUpper()}\".");

                // Add a newline for separation 
                sb.AppendLine();

                extractedSpaces.Add(space);

               
            }
           
            //doors

            foreach (RuleCheckObject door in rcModel.Objects.Where(o => o.Type == "Door"))
            {

                var doorLocation = door.Location;
                string doorID = door.Name;
                

                Vector4D doorOrientation = door.Orientation;
               

                    //regex to get the id
                    string doorName = ConvertDoorName(door.Name);
                sb.AppendLine($"{siteName}:door-{doorName} a {siteName}:Door;");
          

                List<string> interfacedSpacesList = new List<string>(); 
                List<string> interfacedSpacesListLabels = new List<string>();

                foreach (RuleCheckObject space in extractedSpaces)
                {
                    string roomName = space.Name;
                    string roomCategory = roomName.Split(':').First().Trim();

                    //if (roomCategory == "Stairwell" || roomCategory == "Elevator")
                    //    continue;


                    if (Utils.MeshOverlap(door.GetGlobalMesh(), space.GetGlobalMesh()))
                    {
                        string overlappedSpaceName = space.Name;
                        var SpaceLocation =  space.Location;
                    
                        string roomNumber = new string(overlappedSpaceName.Where(char.IsDigit).ToArray());
                        string roomCode = buildingName.Substring(0, 3).ToUpper() ;
                        string overlappedRoomId = ExtractAfterColonForUri(roomName.ToUpper()).Replace(' ', '_');
                        string overlappedRoomCategory = overlappedSpaceName.Split(':').First().Trim();

                        switch (overlappedRoomCategory)
                        {
                            case "Hallway":
                                overlappedRoomId += "HLWY";
                                break;
                            case "Stairwell":
                                overlappedRoomId += "STRWL";
                                break;
                            case "Elevator":
                                overlappedRoomId += "ELVTR";
                                break;
                            default:
                             
                                break;
                        }

                        interfacedSpacesList.Add($"{siteName}:{overlappedRoomId}");

                        if (interfacedSpacesList.Count == 1)
                            interfacedSpacesListLabels.Add(overlappedRoomId.Replace('_', ' '));
                        else { 
                        if (roomCategory != "Hallway")
                            interfacedSpacesListLabels.Add(overlappedRoomId.Replace('_', ' '));
                        }
                        
                    }
                }

    

                // Append the bot:interfaceOf relationships and directions to the StringBuilder
                if (interfacedSpacesList.Any())
                {
                    sb.AppendLine($"    rdfs:label \"Door of {string.Join(", ", interfacedSpacesListLabels)}\";");
                    sb.AppendLine($"    bot:interfaceOf {string.Join(", ", interfacedSpacesList)};");
                   
                }
                sb.AppendLine($"    bot:IndoorLocation '{doorLocation}'.");
                //sb.AppendLine($"    {absoluteDirections}.");

                // Add a newline for clarity between entries
                sb.AppendLine();
            }

            string output = sb.ToString();
            File.WriteAllText("output.rdf", output);
            
            return output;
        }
    }
}