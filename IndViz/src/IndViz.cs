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
            var output = new IndVizOutputs(0);

            // Validate input
            if (input.VolumePercentages.Length != 5)
            {
                throw new ArgumentException("Input must contain exactly 5 volume percentages.");
            }

            // Calculate total volume
            var totalVolume = input.Length * input.Width * input.Height;

            // Calculate individual volumes
            var volumes = new double[5];
            for (int i = 0; i < 5; i++)
            {
                volumes[i] = totalVolume * (input.VolumePercentages[i] / 100.0);
            }

            // Calculate individual heights
            var heights = new double[5];
            for (int i = 0; i < 5; i++)
            {
                heights[i] = volumes[i] / (input.Length * input.Width);
            }

            // Generate masses and add to model
            for (int i = 0; i < 5; i++)
            {
                var offsetX = i * input.Length; // Position each mass side by side
                var rectangle = new Polygon(new List<Vector3>
                {
                    new Vector3(offsetX, 0, 0),
                    new Vector3(offsetX + input.Length, 0, 0),
                    new Vector3(offsetX + input.Length, input.Width, 0),
                    new Vector3(offsetX, input.Width, 0)
                });
                var mass = new Mass(rectangle, heights[i]);
                output.Model.AddElement(mass);
            }

            // Set the output volume
            output.TotalVolume = totalVolume;
            return output;
        }
    }

    public class IndVizInputs
    {
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double[] VolumePercentages { get; set; } // Array of 5 percentages
    }

    public class IndVizOutputs
    {
        public double TotalVolume { get; set; }
        public Model Model { get; set; }

        public IndVizOutputs(double totalVolume)
        {
            TotalVolume = totalVolume;
            Model = new Model();
        }
    }
}
