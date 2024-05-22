using Elements;
using Elements.Geometry;
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
            // Initialize output
            var output = new IndVizOutputs(0, 10, 10, 10, 10, 10, 10, 10, 10);

            // Define the height of the cubes
            var height = 1.0;

            // Define individual lengths for each cube
            double[] lengths = { 2.0, 3.0, 1.5, 2.5, 4.0 };

            // Define individual colors for each cube
            Material[] materials = 
            {
                new Material("Red", new Color(1.0, 0.0, 0.0, 0.5)),
                new Material("Green", new Color(0.0, 1.0, 0.0, 0.5)),
                new Material("Blue", new Color(0.0, 0.0, 1.0, 0.5)),
                new Material("Yellow", new Color(1.0, 1.0, 0.0, 0.5)),
                new Material("Purple", new Color(0.5, 0.0, 0.5, 0.5))
            };

            // Calculate the total volume based on the sum of individual lengths
            var totalLength = 0.0;
            foreach (var l in lengths)
            {
                totalLength += l;
            }
            var volume = totalLength * input.Width * height;

            // Generate 5 cubes and add to the model
            double offsetX = 0.0;
            for (int i = 0; i < 5; i++)
            {
                // Define the rectangle for each cube using individual lengths
                var rectangle = new Polygon(new List<Vector3>
                {
                    new Vector3(offsetX, 0, 0),
                    new Vector3(offsetX + lengths[i], 0, 0),
                    new Vector3(offsetX + lengths[i], input.Width, 0),
                    new Vector3(offsetX, input.Width, 0)
                });

                // Create the mass (cube) with the defined rectangle and height
                var mass = new Mass(rectangle, height, materials[i]);

                // Add the mass to the model
                output.Model.AddElement(mass);

                // Update the offset for the next cube
                offsetX += lengths[i];
            }

            // Set the output volume
            output.Volume = volume;
            return output;
        }
      }
}