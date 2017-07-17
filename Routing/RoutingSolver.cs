using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Routing
{
    public class RoutingSolver : PermutationSolverBase
    {
        private readonly double[,] _costMatrix;

        public RoutingSolver(IReadOnlyCollection<Node> inputPorts, double[,] costMatrix)
        {
            _costMatrix = costMatrix;
            Solution = Enumerable.Range(0, inputPorts.Count).ToArray();
            Solution.Shuffle();
        }

        protected override double GetCost()
        {
            return Solution.Select((t, i) => _costMatrix[i, t]).Sum();
        }

        protected override double GetSwapImprovement(int i, int j)
        {
            var initialCost = _costMatrix[i, Solution[i]] + _costMatrix[j, Solution[j]];
            var swappedCost = _costMatrix[j, Solution[i]] + _costMatrix[i, Solution[j]];
            return initialCost - swappedCost;
        }

        protected override void Swap(Tuple<int, int> pair)
        {
            new SwapOp { Index1 = pair.Item1, Index2 = pair.Item2 }.Swap(Solution);
        }
    }
}