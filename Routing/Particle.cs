using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MoreLinq;

namespace Routing
{
    public class SwapOp
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public double Probability { get; set; }

        public void Swap(int[] array)
        {
            var t = array[Index1];
            array[Index1] = array[Index2];
            array[Index2] = t;
        }
    }

    public class Particle
    {
        public int[] Position { get; set; }
        public int[] BestPosition { get; set; }

        public Particle(int[] initialPosition)
        {
            Position = initialPosition;
            BestPosition = new int[Position.Length];
            Position.CopyTo(BestPosition, 0);
        }
    }
    public class Swarm
    {
        private readonly List<Particle> _particles;
        private readonly Func<int[], double> _calculatePerformance;
        private readonly int _solutionLength;
        public double Alpha = 0.02;
        public double Beta = 0.02;



        public Swarm(List<Particle> particles, Func<int[], double> calculatePerformance)
        {
            _particles = particles;
            _calculatePerformance = calculatePerformance;
            _solutionLength = _particles.First().Position.Length;
        }
        public int[] Solution { get; private set; }
        public double Cost { get; private set; }
        public void Update()
        {
            var gBest = _particles.MinBy(a => _calculatePerformance(a.BestPosition)).BestPosition.ToList();
            Solution = gBest.ToArray();
            Cost = _calculatePerformance(Solution);
            Parallel.ForEach(_particles, particle =>
            //foreach (var particle in _particles)
            {
                var myGbestList = gBest.ToList();
                var tempVelocities = new List<SwapOp>();
                for (var i = 0; i < _solutionLength; i++)
                {
                    if (particle.Position[i] != particle.BestPosition[i])
                    {
                        var swap = new SwapOp
                        {
                            Probability = Alpha,
                            Index1 = i,
                            Index2 = particle.BestPosition.ToList().IndexOf(particle.Position[i])
                        };
                        if (swap.Index2 == -1)
                        {
                            var x = 0;
                        }
                        tempVelocities.Add(swap);

                        swap.Swap(particle.BestPosition);
                    }
                    if (particle.Position[i] != myGbestList[i])
                    {
                        var swap = new SwapOp
                        {
                            Probability = Beta,
                            Index1 = i,
                            Index2 = myGbestList.IndexOf(particle.Position[i])
                        };
                        if (swap.Index2 == -1)
                        {
                            var x = 0;
                        }
                        tempVelocities.Add(swap);

                        swap.Swap(particle.BestPosition);
                    }
                }
                foreach (var tempVelocity in tempVelocities)
                {
                    if (Program.GetRN() < tempVelocity.Probability)
                    {
                        tempVelocity.Swap(particle.Position);
                    }
                }
                var cost = _calculatePerformance(particle.Position);
                var bestCost = _calculatePerformance(particle.BestPosition);
                if (cost < bestCost)
                    particle.Position.CopyTo(particle.BestPosition, 0);
            });
        }
    }

    public class Pso
    {
        private readonly int _numParticles;
        private readonly int[] _trueSolution;

        public Pso(int numParticles, int[] trueSolution)
        {
            _numParticles = numParticles;
            _trueSolution = trueSolution;
        }
        public List<Particle> Run(int numIterations)
        {
            var solutionLength = _trueSolution.Length;
            var particles = new List<Particle>();

            for (int i = 0; i < _numParticles; i++)
            {
                var solution = Enumerable.Range(1, solutionLength).ToArray();
                solution.Shuffle();
                particles.Add(new Particle(solution));
            }
            var costFunc = new Func<int[], double>(ints =>
            {
                var sum = 0;
                for (var i = 0; i < solutionLength; i++)
                {
                    sum += Math.Abs(_trueSolution[i] - ints[i]);
                }
                return sum;
            });
            var swarm = new Swarm(particles, costFunc)
            {
                Alpha = 0,
                Beta = 0
            };
            for (int i = 0; i < numIterations; i++)
            {
                swarm.Alpha = 0.5 * i * 1.0 / numIterations;
                swarm.Beta = 0.5 * i * 1.0 / numIterations;
                swarm.Update();
            }
            return particles;
        }
    }

}