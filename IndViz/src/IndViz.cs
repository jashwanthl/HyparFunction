using Elements;
using Elements.Geometry;
using Elements.Geometry.Solids;
using System.Collections.Generic;

namespace IndViz
{
    public static class IndViz
    {   
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
            var Cost = Area * 516.6449231;
            var CellArea = Area * 7.856666667;
            var chilledWater = input.GigaWatts * 552.695309;
            var onePercent = Area * 0.146779239;
            var tenPercent = Area * 0.059151171;
            var thirtyFivePercent = Area * 0.087909453;
;
            // Initialize output
            var output = new IndVizOutputs(0, Area, CellArea, Cost, PowerCap, Nitrogen, CompressedAir, Gas, chilledWater,onePercent,tenPercent,thirtyFivePercent);

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

            // Calculate the total volume based on the sum of individual lengths
            var totalLength = 0.0;
            foreach (var l in lengths)
            {
                totalLength += l;
            }
            var volume = totalLength * 450 * height;

            // Generate 5 cubes and add to the model
            double offsetX = 0.0;
            for (int i = 0; i < 8; i++)
            {
                // Define the rectangle for each cube using individual lengths
                var rectangle = new Polygon(new List<Vector3>
                {
                    new Vector3(offsetX, 0, 0),
                    new Vector3(offsetX + lengths[i], 0, 0),
                    new Vector3(offsetX + lengths[i], 450, 0),
                    new Vector3(offsetX, 450, 0)
                });

                // Create the mass (cube) with the defined rectangle and height
                var mass = new Mass(rectangle, height, materials[i]);

                // Add the mass to the model
                output.Model.AddElement(mass);

                // Add a label to the mass
                var label = new Annotation(labels[i], new Vector3(offsetX + lengths[i] / 2, 225, height + 1), Vector3.ZAxis, fontSize: 1.0);
                output.Model.AddElement(label);

                // Update the offset for the next cube
                offsetX += lengths[i];
            }

            // Set the output volume
            output.Volume = volume;
            return output;
        }
      }
}