using System;
using System.Linq;
using MoreLinq;

namespace Routing
{
    public class RoutingSolver : PermutationSolverBase
    {
        private readonly Node[] _inputPorts;
        private readonly Node[] _outputPorts;

        public RoutingSolver(Node[] inputPorts, Node[] outputPorts)
        {
            _inputPorts = inputPorts;
            var unusedInputs = _inputPorts.ToList();
            _outputPorts = outputPorts;
            Solution = Enumerable.Range(0, _inputPorts.Length).ToArray();
            Solution.Shuffle();
            var unusedIndeces = Enumerable.Range(0, _inputPorts.Length).ToList();
            var outList = outputPorts.ToList();
            while (unusedIndeces.Any())
            {
                var cheapest = unusedIndeces.AsParallel().MinBy(a => unusedInputs.Min(b => b.DistanceTo(outList[a])));
                var closestPort = unusedInputs.AsParallel().MinBy(a => a.DistanceTo(outList[cheapest]));
                unusedInputs.Remove(closestPort);
                unusedIndeces.Remove(cheapest);
                Solution[_inputPorts.ToList().IndexOf(closestPort)] = cheapest;
            }

        }

        protected override double GetCost()
        {
            return Solution.Select((t, i) => _inputPorts[i].DistanceTo(_outputPorts[t])).Sum();
        }

        protected override double GetSwapImprovement(int i, int j)
        {
            var initialCost = _inputPorts[i].DistanceTo(_outputPorts[Solution[i]]) + _inputPorts[j].DistanceTo(_outputPorts[Solution[j]]);
            var swappedCost = _inputPorts[j].DistanceTo(_outputPorts[Solution[i]]) + _inputPorts[i].DistanceTo(_outputPorts[Solution[j]]);
            return initialCost - swappedCost;
        }

        protected override void Swap(Tuple<int, int> pair)
        {
            new SwapOp { Index1 = pair.Item1, Index2 = pair.Item2 }.Swap(Solution);
        }
    }
}