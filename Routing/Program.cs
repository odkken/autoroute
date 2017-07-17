using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using OpenTK.Graphics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Keyboard = SFML.Window.Keyboard;

namespace Routing
{
    public static class Program
    {

        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        public static double GetRN()
        {
            var result = new byte[4];
            lock (Rng)
            {
                Rng.GetBytes(result);
            }
            return 1.0 * BitConverter.ToUInt32(result, 0) / uint.MaxValue;
        }
        public static void Shuffle<T>(this IList<T> array)
        {
            var n = array.Count;
            for (var i = 0; i < n; i++)
            {
                var rn = GetRN();
                var r = i + (int)(rn * (n - i));
                var t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        static double EvaluateSwapBenefit(int[] guess, int[] solution, int index1, int index2)
        {
            var initialCost = Math.Abs(guess[index1] - solution[index1]) + Math.Abs(guess[index2] - solution[index2]);
            var swappedCost = Math.Abs(guess[index2] - solution[index1]) + Math.Abs(guess[index1] - solution[index2]);
            return 1.0 * initialCost - swappedCost;
        }

        static int EvaluateFitness(int[] guess, int[] solution)
        {
            var sum = 0;
            for (var i = 0; i < guess.Length; i++)
            {
                sum += Math.Abs(guess[i] - solution[i]);
            }
            return sum;
        }


        public static void PrintSolution(int[] solution)
        {
            Console.WriteLine(string.Join(",", solution));
        }

        public interface IPermutationSolver
        {
            void Iterate();
            int[] Solution { get; }
        }



        public static double GetCost(this int[] solution, double[,] costMatrix)
        {
            return solution.Select((t, i) => costMatrix[i, t]).Sum() * 0.01;
        }

        enum Problem
        {
            Routing,
            TSP
        }

        public static void Main(string[] args)
        {
            var problem = Problem.Routing;
            var numPorts = 500;
            if (args.Length > 0)
            {
                int.TryParse(args[0], out numPorts);
            }

            var screenSize = new Vector2u(640, 480);
            var inputPorts = Enumerable.Range(0, numPorts).Select(a => new Node { X = GetSeededRN(screenSize.X * 4), Y = GetSeededRN(screenSize.Y * 3) }).ToArray();
            var outputPorts = Enumerable.Range(0, numPorts + 100).Select(a => new Node { X = GetSeededRN(screenSize.X * 4), Y = GetSeededRN(screenSize.Y * 3) }).ToArray();
            //var count = (int)Math.Round(Math.Sqrt(numPorts));
            //var inputPorts = Enumerable.Range(0, numPorts).Select(a => new Node { X = (float)((a % count) * count * 1.0 / numPorts * 4 * screenSize.X), Y = (float)((a / count) * count * 1.0 / numPorts * 3 * screenSize.Y) }).ToArray();
            //var outputPorts = inputPorts.Select(a => new Node { X = a.X + 20, Y = a.Y + 130 }).ToArray();
            //File.WriteAllLines(@"c:\temp\in.txt", inputPorts.Select(a=> $"{a.X},{a.Y}"));
            //File.WriteAllLines(@"c:\temp\out.txt", outputPorts.Select(a=> $"{a.X},{a.Y}"));
            //var inputPorts = File.ReadAllLines(@"c:\temp\in.txt").Select(a => new Node { X = float.Parse(a.Split(',')[0]), Y = float.Parse(a.Split(',')[1]) }).ToArray();
            //var outputPorts = File.ReadAllLines(@"c:\temp\out.txt").Select(a => new Node { X = float.Parse(a.Split(',')[0]), Y = float.Parse(a.Split(',')[1]) }).ToArray();
            //var particles = new List<Particle>();
            //var numParticles = 100;
            //var costs = new List<double>();
            //Parallel.For(0, 100, i =>
            //{
            //    var solver = new RoutingSolver(inputPorts, outputPorts);
            //    while (!solver.Finished)
            //    {
            //        solver.Iterate();
            //    }
            //    lock (particles)
            //    {
            //        particles.Add(new Particle(solver.Solution));
            //        costs.Add(solver.Cost);
            //    }md
            //});
            //Console.WriteLine($"Initial Best solution: {costs.Min()}.  Average solution cost: {costs.Average()}");
            //var swarm = new Swarm(particles, Solution => Solution.Select((t, i) => inputPorts[i].DistanceTo(outputPorts[t])).Sum());
            var costMatrix = new int[inputPorts.Length, outputPorts.Length];
            var originalCost = new int[costMatrix.GetLength(0), costMatrix.GetLength(1)];
            for (int i = 0; i < inputPorts.Length; i++)
            {
                for (int j = 0; j < outputPorts.Length; j++)
                {
                    costMatrix[i, j] = (int)(1000 * inputPorts[i].DistanceTo(outputPorts[j]));
                    originalCost[i, j] = costMatrix[i, j];
                }
            }


            //var hungTask = Task.Run(() => costMatrix.FindAssignments());
            //var swarm = new RoutingSolver(inputPorts, costMatrix);
            //var swarmTask = Task.Run(() =>
            //{
            //    while (!swarm.Finished)
            //    {
            //        swarm.Iterate();
            //    }
            //});

            //swarmTask.Wait();
            //hungTask.Wait();
            //var hung = hungTask.Result;
            //Console.WriteLine($"swarm: {swarm.Solution.GetCost(originalCost)}\nhungarian: {hung.GetCost(originalCost)}");

            costMatrix.FindAssignments();


            //var w = new RenderWindow(VideoMode.DesktopMode, "asdf", Styles.Default) { Size = screenSize };
            //w.KeyPressed += OnKeyPressed;
            //w.Closed += OnClosed;
            //var allNodes = inputPorts.Select(a => new DrawableNode(a) { Color = Color.Magenta }).ToList();
            //if (problem == Problem.Routing)
            //    allNodes.AddRange(outputPorts.Select(a => new DrawableNode(a) { Color = Color.Green }));

            //var displaySwarm = true;
            //w.KeyPressed += (sender, eventArgs) =>
            //{
            //    if (eventArgs.Code == Keyboard.Key.Space)
            //        displaySwarm = !displaySwarm;
            //};

            //while (w.IsOpen)
            //{
            //    w.DispatchEvents();
            //    w.Clear();
            //    foreach (var drawableNode in allNodes)
            //    {
            //        w.Draw(drawableNode);
            //    }
            //    for (int j = 0; j < numPorts; j++)
            //    {
            //        var vertices = new Vertex[2];
            //        vertices[0] = new Vertex(new Vector2f(inputPorts[j].X, inputPorts[j].Y));
            //        if (displaySwarm)
            //            vertices[1] = new Vertex(new Vector2f(outputPorts[swarm.Solution[j]].X, outputPorts[swarm.Solution[j]].Y));
            //        else
            //            vertices[1] = new Vertex(new Vector2f(outputPorts[hung[j]].X, outputPorts[hung[j]].Y));
            //        w.Draw(vertices, PrimitiveType.Lines, RenderStates.Default);
            //    }

            //    w.Display();


            //    //Console.ReadKey();


            //}
        }

        private static readonly Random rng = new Random(0);
        private static float GetSeededRN(uint u)
        {
            return (float)(u * rng.NextDouble());
        }

        private static float GetRN(uint scale)
        {
            return (float)(scale * GetRN());
        }

        /// <summary>
        /// Function called when the window is closed
        /// </summary>
        static void OnClosed(object sender, EventArgs e)

        {
            var window = (Window)sender;
            window.Close();
        }

        /// <summary>
        /// Function called when a key is pressed
        /// </summary>
        static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            var window = (Window)sender;
            if (e.Code == Keyboard.Key.Escape)
                window.Close();
        }

        /// <summary>
        /// Function called when the window is resized
        /// </summary>
        static void OnResized(object sender, SizeEventArgs e)
        {
            GL.Viewport(0, 0, (int)e.Width, (int)e.Height);
        }
    }

    public class NodeLayout
    {
        public IEnumerable<Node> GetOutputNodes()
        {
            return new List<Node>
{
    new Node {X = 0, Y = 50},
    new Node {X = 50, Y = 80},
    new Node {X = 100, Y = 110},
    new Node {X = 150, Y = 110},
    new Node {X = 200, Y = 80},
    new Node {X = 250, Y = 50}
};
        }
        public IEnumerable<Node> GetInputNodes()
        {
            return new List<Node>
{
    new Node {X = 0, Y = 0},
    new Node {X = 50, Y = 0},
    new Node {X = 100, Y = 0},
    new Node {X = 150, Y = 0},
    new Node {X = 200, Y = 0},
    new Node {X = 250, Y = 0}
};
        }
    }

    public class DrawableNode : Node, Drawable
    {
        private readonly CircleShape _shape;
        public DrawableNode(Node node)
        {
            X = node.X;
            Y = node.Y;
            _shape = new CircleShape { FillColor = Color.Magenta, Radius = 5, Origin = new Vector2f(5f, 5f), Position = new Vector2f(X, Y) };
        }

        public Color Color
        {
            get { return _shape.FillColor; }
            set { _shape.FillColor = value; }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_shape, states);
        }
    }
}
