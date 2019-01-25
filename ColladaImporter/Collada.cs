using System;
using System.Collections.Generic;

namespace ColladaSharp
{
    public partial class Collada
    {
        /// <summary>
        /// Event called every time a debug line is printed.
        /// </summary>
        public static event Action<string> OutputLine;
        public static void WriteLine(string line)
        {
            Console.WriteLine(line);
            OutputLine?.Invoke(line);
        }

        public class SceneCollection
        {
            public List<Scene> Scenes { get; } = new List<Scene>();
        }

    }
}
