using System;
using System.Threading.Tasks;

namespace Routing
{
    public abstract class PermutationSolverBase
    {
        protected abstract double GetCost();
        protected abstract double GetSwapImprovement(int i, int j);
        public bool Finished { get; private set; }
        public int[] Solution { get; protected set; }
        public double Cost { get; private set; }
        public void Iterate()
        {
            if (Finished)
                return;
            var initialCost = GetCost();
            var cost = initialCost;

            var locker = new object();
            var bestSwapImprovement = 0.0;
            var bestSwapIndex1 = -1;
            var bestSwapIndex2 = -1;
            Parallel.For(0, Solution.Length, i =>
            {
                for (var j = i; j < Solution.Length; j++)
                {
                    var improvement = GetSwapImprovement(i, j);
                    if (improvement > bestSwapImprovement)
                    {
                        lock (locker)
                        {
                            if(improvement <= bestSwapImprovement)
                                continue;
                            bestSwapImprovement = improvement;
                            bestSwapIndex1 = i;
                            bestSwapIndex2 = j;
                        }
                    }
                }
            });
            if (bestSwapIndex1 == -1)
            {
                Finished = true;
                return;
            }
            Swap(Tuple.Create(bestSwapIndex1, bestSwapIndex2));
            cost = GetCost();
            Console.WriteLine("improvement of " + bestSwapImprovement + ". new cost = " + cost);
            Cost = cost;
        }

        protected abstract void Swap(Tuple<int, int> pair);
    }
}