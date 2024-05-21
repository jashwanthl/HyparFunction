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
        /// <param name="model">The input model.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A IndVizOutputs instance containing computed results and the model with any new elements.</returns>
        public static IndVizOutputs Execute(Dictionary<string, Model> inputModels, IndVizInputs input)
        {
            // Your code here.
          
            var gw = input.GigaWatts;
            var height = 1.0;
            var volume = input.Length * input.Width * height;
            var output = new IndVizOutputs(volume);
            var rectangle = Polygon.Rectangle(input.Length, input.Width);
            var mass = new Mass(rectangle, height);
            output.Model.AddElement(mass);
            return output;
        }
      }
}