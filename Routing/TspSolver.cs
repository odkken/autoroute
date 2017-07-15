using System;
using System.Collections.Generic;
using System.Linq;

namespace Routing
{
    public class TspSolver : PermutationSolverBase
    {
        private readonly Node[] _nodes;

        public TspSolver(IEnumerable<Node> nodes)
        {
            _nodes = nodes.ToArray();
            Solution = Enumerable.Range(0, _nodes.Length).ToArray();
            Solution.Shuffle();
        }

        protected override double GetCost()
        {
            var sum = 0.0;
            for (var i = 0; i < _nodes.Length - 1; i++)
            {
                sum += _nodes[Solution[i]].DistanceTo(_nodes[Solution[i + 1]]);
            }
            return sum;
        }

        private double GetCostOfNode(int index)
        {
            if (index == 0)
                return _nodes[Solution[0]].DistanceTo(_nodes[Solution[1]]);
            if (index == Solution.Length - 1)
                return _nodes[Solution[index]].DistanceTo(_nodes[Solution[index - 1]]);
            return _nodes[Solution[index]].DistanceTo(_nodes[Solution[index - 1]]) + _nodes[Solution[index]].DistanceTo(_nodes[Solution[index + 1]]);
        }

        protected override double GetSwapImprovement(int i, int j)
        {
            var original = GetCost();
            var originalSolution = Solution.ToList();
            var swapper = new SwapOp { Index1 = i, Index2 = j };
            swapper.Swap(Solution);
            var newCost = GetCost();
            Solution = originalSolution.ToArray();
            return original - newCost;
        }

        protected override void Swap(Tuple<int, int> pair)
        {
            var firstIndex = pair.Item1 > pair.Item2 ? pair.Item2 : pair.Item1;
            var secondIndex = pair.Item1 > pair.Item2 ? pair.Item1 : pair.Item2;

            var list = Solution.ToList();
            var firstChunk = list.GetRange(0, firstIndex);
            var secondChunk = list.GetRange(firstIndex, secondIndex - firstIndex);
            var thirdChunk = list.GetRange(secondIndex, Solution.Length - secondIndex);
            secondChunk.Reverse();
            firstChunk.AddRange(secondChunk);
            firstChunk.AddRange(thirdChunk);
            lock (Solution)
            {
                Solution = firstChunk.ToArray();
            }
        }
    }
}