using Elements;
using Elements.Geometry;
using Elements.Geometry.Solids;
using System;
using System.Collections.Generic;

namespace IndViz
{
    public static class IndViz
    {
        // Constants for the site
        private static readonly Material SITE_MATERIAL = new Material("site", "#7ECD9F", 0.0f, 0.0f);
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
            var Gas = input.GigaWatts * 11.91380039;
            var Nitrogen = input.GigaWatts * 10.45070994;
            var CompressedAir = input.GigaWatts * 1741.6473;
            var PowerCap = input.GigaWatts * 2.972249627;
            var Cost = (Area * 516.6449231) / 0.092903;
            var CellArea = Math.Round(Area * 0.000247105,2);
            var chilledWater = input.GigaWatts * 552.695309;
            var onePercent = (Area * 0.146779239) / 0.092903;
            var tenPercent = (Area * 0.059151171) / 0.092903;
            var thirtyFivePercent = (Area * 0.087909453) / 0.092903;

            // Initialize output
            var output = new IndVizOutputs(0, Area, CellArea, Cost, PowerCap, Nitrogen, CompressedAir, Gas, chilledWater, onePercent, tenPercent, thirtyFivePercent);

            // Define the height of the cubes
            var height = 20.0;

            // Define individual lengths for each cube
            double[] lengths = { Area*0.05331213567/450, Area*0.1066242713/450, Area*0.03267973856/450, Area*0.159936407/450, Area*0.108814697/450, Area*0.2132485427/450, Area*0.05882352941/450, Area*0.267973856/450};

            // Define individual colors for each cube
            Material[] materials = 
            {
                new Material("Red", new Color(1.0, 0.0, 0.0, 0.5)),
                new Material("Green", new Color(0.0, 1.0, 0.0, 0.5)),
                new Material("Blue", new Color(0.0, 0.0, 1.0, 0.5)),
                new Material("Yellow", new Color(1.0, 1.0, 0.0, 0.5)),
                new Material("Purple", new Color(0.5, 0.0, 0.5, 0.5)),
                new Material("Orange", new Color(1.0, 0.5, 0.0, 0.5)),
                new Material("Pink", new Color(1.0, 0.0, 1.0, 0.5)),
                new Material("Cyan", new Color(0.0, 1.0, 1.0, 0.5))
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
            var siteOffset = 70.0;
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

            // Create the site element using the CreateSite function
            var site = CreateSite(siteBoundary);

            // Add the site to the model
            output.Model.AddElement(site);

            // Generate 8 cubes and add to the model
            double offsetX = siteOffset;
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

                // Update the offset for the next cube
                offsetX += lengths[i];
            }

            // Set the output volume
            output.Volume = totalLength * totalWidth * totalHeight;
            return output;
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
