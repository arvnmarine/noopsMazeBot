using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace maze
{



    class Program
    {
        static void Main(string[] args)
        {
            //GETData("https://api.noopschallenge.com/mazebot/random?minSize=100&maxSize=900");

            StartRace();
        }

        static void StartRace()
        {
            var nextMazePath = LogIn("arvnmarine");
            do {
                
                nextMazePath = GETData("https://api.noopschallenge.com" + nextMazePath);
                Console.WriteLine("Next maze: " + nextMazePath);
                if (nextMazePath == null)
                {
                    break;
                }
            } while (true) ;
        }


        static string LogIn(string githubUsername)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.noopschallenge.com/mazebot/race/start");

            var postData = "{ \"login\" : \"" + githubUsername + "\" }";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var ParsedJsonData = JsonConvert.DeserializeObject<JsonStartRaceClass>(responseString);
            


            Console.WriteLine(ParsedJsonData.message);
            return ParsedJsonData.nextMaze;
        }

        static string GETData(string url)
        {
            var JsonData = GetDataFromAPI(url);
            var Maze = JsonData.map;
            var StartingPos = JsonData.startingPosition;
            var EndingPos = JsonData.endingPosition;
            var path = A_star_pathFinder(Maze, StartingPos[1], StartingPos[0], EndingPos[1], EndingPos[0]);
            //DisplayMaze(Maze);
            //Console.WriteLine(path);
            return POSTData(JsonData.mazePath, path);

        }

        static string POSTData(string url, string postedString)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.noopschallenge.com" + url);

            var postData = "{ \"directions\" : \""+ postedString + "\" }";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();


            var ParsedJsonData = JsonConvert.DeserializeObject<JsonRaceResultClass>(responseString);
            Console.WriteLine(ParsedJsonData.result + " " + ParsedJsonData.elapsed + " " + ParsedJsonData.shortestSolutionLength + " " + ParsedJsonData.yourSolutionLength);

            return ParsedJsonData.nextMaze ;
        }

        static void DisplayMaze(List<List<string>> Maze)
        {
            for (int i=0; i< Maze.Count; i++)
            {
                for (int j = 0; j < Maze[i].Count; j++)
                {
                    Console.Write(Maze[i][j]);
                }
                Console.WriteLine();
            }
        }

        static JsonGetClass GetDataFromAPI(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var ParsedJsonData = JsonConvert.DeserializeObject<JsonGetClass>(responseString);
            
            //Console.WriteLine(responseString);

            return ParsedJsonData;
        }
        static string TracePath(Maze_element elem)
        {
            //Console.WriteLine("Tracing...");
            string path = "";
            var tmp = elem;
            
            while (tmp.fromNode != null)
            {
                
                var par = tmp.fromNode;
                if (par.x < tmp.x)
                {
                    path = "S" + path;
                } else if (par.x > tmp.x)
                {
                    path = "N" + path;
                }
                else if (par.y < tmp.y)
                {
                    path = "E" + path;
                }
                else if (par.y > tmp.y)
                {
                    path = "W" + path;
                }
                tmp = par;
                
            }
            
            return path;
        }

        static string A_star_pathFinder(List<List<string>> Maze, int Ax, int Ay, int Bx, int By)
        {
            //Console.WriteLine("Start A*");
            string pathResult = "";
            List<List<Maze_element>> MazeObj = new List<List<Maze_element>>();
            int n = Maze.Count;
            int m = Maze[0].Count;
            MinHeapClass MinHeap = new MinHeapClass(n*m);

            
            for (int i=0; i < n; i++)
            {
                List<Maze_element> tmpArr = new List<Maze_element>();
                for (int j=0; j < m; j++)
                {
                    Maze_element elem = new Maze_element(i,j);
                    elem.h = Math.Abs(i-Bx) + Math.Abs(j-By);
                    if (Maze[i][j] == "A")
                    {
                        elem.g = 0;
                        elem.f = elem.g + elem.h;   
                    } 

                    tmpArr.Add(elem);
                    if (Maze[i][j] != "X")
                    {
                        MinHeap.Add(elem);
                    }
                    
                }
                MazeObj.Add(tmpArr);
            }

            Console.WriteLine(MinHeap.Peek().f);

            bool stop = false;
            do
            {
                Maze_element elem = MinHeap.Pop();
                //Scan 4 adjacent block
                int[] compass = { -1, 0, 1, 0, -1 };
                for (int i=0;i < 4; i++)
                {
                    int x = elem.x + compass[i];
                    int y = elem.y + compass[i + 1];

                    if ((x < 0) || (y < 0))
                        continue;
                    if ((x >= n) || (y >= m))
                        continue;
                    if (Maze[x][y] == "X")
                        continue;

                    if (MazeObj[x][y].f > elem.g + 1 + MazeObj[x][y].h)
                    {
                        MazeObj[x][y].g = elem.g + 1;
                        MazeObj[x][y].f = MazeObj[x][y].g + MazeObj[x][y].h;
                        MazeObj[x][y].fromNode = elem;
                        var heapIndex = MazeObj[x][y].heapIndex;
                        MinHeap.ReCalculateUp(heapIndex);
                        MinHeap.ReCalculateDown(heapIndex);
                    }
                     

                    if (Maze[x][y] == "B")
                    {
                        stop = true;
                        //Console.WriteLine("Found B");
                        pathResult = TracePath(MazeObj[x][y]);
                        break;
                    }

                }
                if (stop)
                    break;
            } while (!MinHeap.IsEmpty());
            
            return pathResult; 
        }
    }
}
