using System;
using System.Collections.Generic;
using System.Text;

namespace maze
{
   
    public class ExampleSolution
    {
        public string directions { get; set; }
    }

    public class JsonGetClass
    {
        public string name { get; set; }
        public string mazePath { get; set; }
        public List<int> startingPosition { get; set; }
        public List<int> endingPosition { get; set; }
        public string message { get; set; }
        public ExampleSolution exampleSolution { get; set; }
        public List<List<string>> map { get; set; }
    }

    public class JsonStartRaceClass
    {
        public string message { get; set; }
        public string nextMaze { get; set; }
    }

    public class JsonRaceResultClass
    {
        public string result { get; set; }
        public double elapsed { get; set; }
        public int shortestSolutionLength { get; set; }
        public int yourSolutionLength { get; set; }
        public string nextMaze { get; set; }
    }
}
