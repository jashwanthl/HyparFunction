using Elements;
using Elements.Geometry;
using Elements.Geometry.Solids;
using System;
using System.Collections.Generic;

namespace IndViz
{

    public static class IndViz
    {
        // Constants for the site and parking
        private static readonly Material SITE_MATERIAL = new Material("site", "#7ECD9F", 0.0f, 0.0f);
        private static readonly Material PARKING_MATERIAL = new Material("parking", "#CCCCCC", 0.0f, 0.0f);
        private static readonly Material CAR_MATERIAL = new Material("car", "#FF0000", 0.0f, 0.0f);
        private static readonly Material GREEN_RECTANGLE_MATERIAL = new Material("GreenRectangle", new Color(0.0, 1.0, 0.0, 0.5));
        private const string LEGACY_IDENTITY_PREFIX = "legacy";

        /// <summary>
        /// The IndViz function.
        /// </summary>
        /// <param name="inputModels">The input models.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A IndVizOutputs instance containing computed results and the model with any new elements.</returns>
        public static IndVizOutputs Execute(Dictionary<string, Model> inputModels, IndVizInputs input)
        {
            var Area = input.GigaWatts * 83911.75931 * 0.092903;
            // round the area to the nearest 10 and remove the decimals
            Area = Math.Round(Area / 10) * 10;
            var Gas = input.GigaWatts * 11.91380039;
            var Nitrogen = input.GigaWatts * 10.45070994;
            // round up the Nitrogen to the nearest 100 and remove the decimals
            Nitrogen = Math.Round(Nitrogen / 100) * 100;
            var CompressedAir = input.GigaWatts * 1741.6473;
            // round up the compressed air to the nearest 100 and remove the decimals
            CompressedAir = Math.Round(CompressedAir / 100) * 100;
            var PowerCap = input.GigaWatts * 2.972249627;
            // round up the power cap to the nearest 10 and remove the decimals
            PowerCap = Math.Round(PowerCap / 10) * 10;
            // Calculate the base cost
            var baseCost = (Area * 516.6449231) / 0.092903;
            // round the cost to the nearest 10000000 and remove the decimals
            baseCost = Math.Round(baseCost / 10000000) * 10000000;

            var minCost = baseCost * 0.7;
            var maxCost = baseCost * 1.5;
            var Cost = $"{minCost:C2} - {maxCost:C2}";

            var CellArea = Math.Round(Area * 0.000247105, 2);
            // round the cost to the nearest 10 and remove the decimals
            CellArea = Math.Round(CellArea / 10) * 10;
            var chilledWater = input.GigaWatts * 552.695309;
            // round the cost to the nearest 100 and remove the decimals
            chilledWater = Math.Round(chilledWater / 100) * 100;
            var onePercent = (Area * 0.146779239) / 0.092903;
            var tenPercent = (Area * 0.059151171) / 0.092903;
            var thirtyFivePercent = (Area * 0.087909453) / 0.092903;

            // Initialize output
            var output = new IndVizOutputs(0, Area, CellArea, Cost, PowerCap, Nitrogen, CompressedAir, Gas, chilledWater, onePercent, tenPercent, thirtyFivePercent);

            // Define the height of the cubes
            var height = 20.0;

            // Define individual lengths for each cube
            double[] lengths = { Area * 0.05331213567 / 450, Area * 0.1066242713 / 450, Area * 0.03267973856 / 450, Area * 0.159936407 / 450, Area * 0.108814697 / 450, Area * 0.2132485427 / 450, Area * 0.05882352941 / 450, Area * 0.267973856 / 450 };

            // Create a variable to store the opacity of the cubes
            var GableOpacity = input.GableRoofAndMassOpacity ? 0.6 : 1.0;
            var AreaOpacity = input.AreaMassOpacity ? 0.6 : 1.0;

            // Define individual colors for each cube
            
            Material[] materials = 
            {
                new Material("Red", new Color(1.0, 0.0, 0.0, AreaOpacity)),
                new Material("Green", new Color(0.0, 1.0, 0.0, AreaOpacity)),
                new Material("Blue", new Color(0.0, 0.0, 1.0, AreaOpacity)),
                new Material("Yellow", new Color(1.0, 1.0, 0.0,AreaOpacity)),
                new Material("Purple", new Color(0.5, 0.0, 0.5, AreaOpacity)),
                new Material("Orange", new Color(1.0, 0.5, 0.0, AreaOpacity)),
                new Material("Pink", new Color(1.0, 0.0, 1.0, AreaOpacity)),
                new Material("Cyan", new Color(0.0, 1.0, 1.0, AreaOpacity))
            };

            // Define labels for each cube
            string[] labels = 
            {
                "Area 100", "Area 200", "Area 250", "Area 300", "Area 350", "Area 400", "Area 450", "Area 500"
            };

            // Calculate the total length based on the sum of individual lengths
            var totalLength = 0.0;
            foreach (var l in lengths)
            {
                totalLength += l;
            }
            var totalWidth = 450.0;
            var totalHeight = height;

            // Define site dimensions and offset
            var siteOffset = 25.0;
            var siteLength = totalLength + 2 * siteOffset;
            var siteWidth = totalWidth + 2 * siteOffset;

            // Define site rectangle
            var siteBoundary = new Polygon(new List<Vector3>
            {
                new Vector3(-siteOffset, -siteOffset, 0),
                new Vector3(siteLength - siteOffset, -siteOffset, 0),
                new Vector3(siteLength - siteOffset, siteWidth - siteOffset, 0),
                new Vector3(-siteOffset, siteWidth - siteOffset, 0)
            });

            //Create the site element using the CreateSite function
            //  if SiteVisibility is true then the site will be visible
            if (input.SiteVisibility)
            {
                var site = CreateSite(siteBoundary);
                output.Model.AddElement(site);
            }


            // Generate 8 cubes and add to the model
            double offsetX = siteOffset;
            Mass firstMass = null;
            Mass lastMass = null;
            for (int i = 0; i < 8; i++)
            {
                // Define the rectangle for each cube using individual lengths
                var rectangle = new Polygon(new List<Vector3>
                {
                    new Vector3(offsetX, siteOffset, 0),
                    new Vector3(offsetX + lengths[i], siteOffset, 0),
                    new Vector3(offsetX + lengths[i], 450 + siteOffset, 0),
                    new Vector3(offsetX, 450 + siteOffset, 0)
                });

                // Create the mass (cube) with the defined rectangle and height
                var mass = new Mass(rectangle, height, materials[i]);

                // Add the custom property (label) to the mass
                mass.AdditionalProperties["Label"] = labels[i];

                // Add the mass to the model
                output.Model.AddElement(mass);

                // Save the first mass for the triangle roof
                if (i == 0)
                {
                    firstMass = mass;
                }
                lastMass = mass;
                
                if (input.FootballFieldVisibility)
                {
                    // Add a green rectangle 50' up from the starting cube
                    AddGreenRectanglesWithGaps(output.Model, siteOffset, totalWidth, totalLength);
                    AddGreenRectanglesAlongWidth(output.Model, siteOffset + 450 + 50, siteOffset, 450, 110, totalLength);
                }
                

                // Update the offset for the next cube
                offsetX += lengths[i];

            }

             // Add the encompassing bounding box after the cubes are created
            if (firstMass != null && lastMass != null)
            {
                // After creating the 8 cubes
                CreateEncompassingBox(output.Model, siteOffset, siteOffset, totalLength, totalWidth, height, GableOpacity);

            }
            // Add triangle roof over the first mass
            if (firstMass != null)
            {
                AddTriangleRoofManually(output.Model,offsetX, siteOffset, lengths[0], height, totalWidth, totalLength, GableOpacity);
            }

            
            
            // Set the output volume
            output.Volume = totalLength * totalWidth * totalHeight;
            return output;
        }

        private static void CreateEncompassingBox(Model model, double startX, double startY, double totalLength, double totalWidth, double height, double MaterialOpacity)
        {
            // Define the vertices for the bounding box
            var boundingBoxVertices = new List<Vector3>
            {
                new Vector3(startX, startY, 0),  // Bottom left corner
                new Vector3(startX + totalLength, startY, 0),  // Bottom right corner
                new Vector3(startX + totalLength, startY + totalWidth, 0),  // Top right corner
                new Vector3(startX, startY + totalWidth, 0)  // Top left corner
            };

            // Ensure we have enough vertices to create a valid polygon
            if (boundingBoxVertices.Count >= 3)
            {
                var boundingBoxPolygon = new Polygon(boundingBoxVertices);
                var boundingBoxMaterial = new Material("BoundingBox", new Color(0.5, 0.5, 0.5, MaterialOpacity));  // Semi-transparent material

                // Create the mass for the bounding box
                var boundingBoxMass = new Mass(boundingBoxPolygon, height, boundingBoxMaterial);

                // Add the bounding box to the model
                model.AddElement(boundingBoxMass);
            }
            else
            {
                Console.WriteLine("Error: Not enough vertices to create a valid polygon.");
            }
        }

        private static void AddGreenRectanglesAlongWidth(Model model, double startY, double startX, double totalWidth, double rectangleWidth, double totalLength)
        {
            var rectangleLength = 50.0;  // The width of the rectangles
            var gap = 5.0;

            double currentY = startY;

            while (currentY + rectangleLength <= startY + totalWidth)
            {
                // Define the rectangle vertices
                var rectangleVertices = new List<Vector3>
                {
                    new Vector3(startX, currentY, 0),
                    new Vector3(startX + rectangleWidth, currentY, 0),
                    new Vector3(startX + rectangleWidth, currentY + rectangleLength, 0),
                    new Vector3(startX, currentY + rectangleLength, 0)
                };

                var rectanglePolygon = new Polygon(rectangleVertices);

                // Create a small extrusion to represent the rectangle as a flat surface
                var rectangleMass = new Mass(rectanglePolygon, 0.1, GREEN_RECTANGLE_MATERIAL);

                // Add the rectangle to the model
                model.AddElement(rectangleMass);

                // Move to the next position
                currentY += rectangleLength + gap;
            }

            // If there is space left but not enough for a full rectangle, add a smaller one
            double remainingLength = (startY + totalWidth) - currentY;
            if (remainingLength > 0)
            {
                var rectangleVertices = new List<Vector3>
                {
                    new Vector3(startX, currentY, 0),
                    new Vector3(startX + rectangleWidth, currentY, 0),
                    new Vector3(startX + rectangleWidth, currentY + remainingLength, 0),
                    new Vector3(startX, currentY + remainingLength, 0)
                };

                var rectanglePolygon = new Polygon(rectangleVertices);

                // Create a small extrusion to represent the rectangle as a flat surface
                var rectangleMass = new Mass(rectanglePolygon, 0.1, GREEN_RECTANGLE_MATERIAL);

                // Add the rectangle to the model
                model.AddElement(rectangleMass);
            }
        }


        private static void AddGreenRectanglesWithGaps(Model model, double siteOffset, double totalWidth, double totalLength)
        {
            var rectangleWidth = 50.0;
            var rectangleLength = 110.0;
            var gap = 5.0;

            double currentX = siteOffset;
            double startY = siteOffset + 450 + 50;  // 50' up from the end of the Y-axis of the masses

            while (currentX + rectangleLength <= siteOffset + totalLength)
            {
                // Define the rectangle vertices
                var rectangleVertices = new List<Vector3>
                {
                    new Vector3(currentX, startY, 0),
                    new Vector3(currentX + rectangleLength, startY, 0),
                    new Vector3(currentX + rectangleLength, startY + rectangleWidth, 0),
                    new Vector3(currentX, startY + rectangleWidth, 0)
                };

                var rectanglePolygon = new Polygon(rectangleVertices);

                // Create a small extrusion to represent the rectangle as a flat surface
                var rectangleMass = new Mass(rectanglePolygon, 0.1, GREEN_RECTANGLE_MATERIAL);

                // Add the rectangle to the model
                model.AddElement(rectangleMass);

                // Move to the next position
                currentX += rectangleLength + gap;
            }
            double remainingLength = siteOffset + totalLength - currentX;
            if (remainingLength > 0)
            {
                var rectangleVertices = new List<Vector3>
                {
                    new Vector3(currentX, startY, 0),
                    new Vector3(currentX + remainingLength, startY, 0),
                    new Vector3(currentX + remainingLength, startY + rectangleWidth, 0),
                    new Vector3(currentX, startY + rectangleWidth, 0)
                };

                var rectanglePolygon = new Polygon(rectangleVertices);

                // Create a small extrusion to represent the rectangle as a flat surface
                var rectangleMass = new Mass(rectanglePolygon, 0.1, GREEN_RECTANGLE_MATERIAL);

                // Add the rectangle to the model
                model.AddElement(rectangleMass);
            }
        }



        private static Site CreateSite(Polygon perimeter)
        {
            var area = perimeter.Area();
            var geomRep = new Lamina(perimeter, false);
            var site = new Site
            {
                Perimeter = perimeter,
                Area = area,
                Material = SITE_MATERIAL,
                Representation = geomRep,
                AddId = LEGACY_IDENTITY_PREFIX
            };
            return site;
        }

        private static void AddTriangleRoofManually(Model model, double startX, double startY, double length, double height, double width, double totalLength, double MaterialOpacity)
        {
            // Manually define the points for the triangle based on known dimensions
            var roofHeight = 30.0;

            var trianglePoints = new List<Vector3>
            {
                new Vector3(startX, startY, height),  // Bottom left corner at the top of the mass
                new Vector3(startX , width + startY, height),  // Bottom right corner at the top of the mass
                new Vector3(startX , width/2, height + roofHeight)  // Peak of the triangle
            };

            // Create the triangle profile
            var triangleProfile = new Polygon(trianglePoints);

            // Create a transparent material for the ghosted roof
            var ghostedMaterial = new Material("GhostedRoof", new Color(1.0, 1.0, 1.0, MaterialOpacity));

            // Create the extrusion along the Y-axis
            var triangleSolid = new Extrude(triangleProfile, totalLength, Vector3.XAxis * -1, false);

            // Create the mass using the solid and add it to the model
            var triangleMass = new Mass(triangleProfile, totalLength, ghostedMaterial);
            triangleMass.Representation = new Representation(new List<SolidOperation> { triangleSolid });

            model.AddElement(triangleMass);
        }

        private static void AddParkingLot(Model model, Polygon siteBoundary, double buildingLength)
        {
            // Define parking lot dimensions and offset from the building
            double parkingLotOffset = 10.0;
            double parkingSpaceWidth = 2.5;
            double parkingSpaceLength = 5.0;
            int numberOfSpaces = 10; // Number of parking spaces

            // Calculate the start position for the parking lot
            double parkingLotStartX = -buildingLength / 2 + parkingLotOffset;
            double parkingLotStartY = siteBoundary.Vertices[0].Y + parkingLotOffset;

            for (int i = 0; i < numberOfSpaces; i++)
            {
                // Calculate the position for each parking space
                double x = parkingLotStartX + (i % 10) * parkingSpaceWidth;
                double y = parkingLotStartY + (i / 10) * parkingSpaceLength;

                // Define the rectangle for each parking space
                var parkingSpace = new Polygon(new List<Vector3>
                {
                    new Vector3(x, y, 0),
                    new Vector3(x + parkingSpaceWidth, y, 0),
                    new Vector3(x + parkingSpaceWidth, y + parkingSpaceLength, 0),
                    new Vector3(x, y + parkingSpaceLength, 0)
                });

                // Create the parking space mass
                var parkingMass = new Mass(parkingSpace, 0.1, PARKING_MATERIAL);

                // Add the parking space to the model
                model.AddElement(parkingMass);

                // Optionally add cars to some of the parking spaces
                if (i % 2 == 0)
                {
                    var car = new Mass(parkingSpace, 1.5, CAR_MATERIAL);
                    model.AddElement(car);
                }
            }
        }
    }

    public class Site : GeometricElement
    {
        public Polygon Perimeter { get; set; }
        public double Area { get; set; }
        public string AddId { get; set; }

        public Site() : base(new Transform(), BuiltInMaterials.Default, null, false, Guid.NewGuid(), null)
        {
        }
    }
}
