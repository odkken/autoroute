using System;

namespace Routing
{
    public static class HungarianAlgorithm
    {
        public static int[] FindAssignments(this int[,] costs)
        {
            if (costs == null)
                throw new ArgumentNullException(nameof(costs));
            var length1 = costs.GetLength(0);
            var length2 = costs.GetLength(1);
            for (var index1 = 0; index1 < length1; ++index1)
            {
                var val1 = int.MaxValue;
                for (var index2 = 0; index2 < length2; ++index2)
                    val1 = Math.Min(val1, costs[index1, index2]);
                for (var index2 = 0; index2 < length2; ++index2)
                    costs[index1, index2] -= val1;
            }
            var masks = new byte[length1, length2];
            var rowsCovered = new bool[length1];
            var colsCovered = new bool[length2];
            for (var index1 = 0; index1 < length1; ++index1)
            {
                for (var index2 = 0; index2 < length2; ++index2)
                {
                    if (Math.Abs(costs[index1, index2]) < Tolerance && !rowsCovered[index1] && !colsCovered[index2])
                    {
                        masks[index1, index2] = 1;
                        rowsCovered[index1] = true;
                        colsCovered[index2] = true;
                    }
                }
            }
            ClearCovers(rowsCovered, colsCovered, length2, length1);
            var path = new Location[length2 * length1];
            var pathStart = new Location();
            var num = 1;
            while (num != -1)
            {
                switch (num - 1)
                {
                    case 0:
                        num = RunStep1(masks, colsCovered, length2, length1);
                        continue;
                    case 1:
                        num = RunStep2(costs, masks, rowsCovered, colsCovered, length2, length1, ref pathStart);
                        continue;
                    case 2:
                        num = RunStep3(masks, rowsCovered, colsCovered, length2, length1, path, pathStart);
                        continue;
                    case 3:
                        num = RunStep4(costs, rowsCovered, colsCovered, length2, length1);
                        continue;
                    default:
                        continue;
                }
            }
            var numArray = new int[length1];
            for (var index1 = 0; index1 < length1; ++index1)
            {
                for (var index2 = 0; index2 < length2; ++index2)
                {
                    if (masks[index1, index2] == 1)
                    {
                        numArray[index1] = index2;
                        break;
                    }
                }
            }
            return numArray;
        }

        public static double Tolerance = double.Epsilon * 5;

        private static int RunStep1(byte[,] masks, bool[] colsCovered, int w, int h)
        {
            for (var index1 = 0; index1 < h; ++index1)
            {
                for (var index2 = 0; index2 < w; ++index2)
                {
                    if (masks[index1, index2] == 1)
                        colsCovered[index2] = true;
                }
            }
            var num = 0;
            for (var index = 0; index < w; ++index)
            {
                if (colsCovered[index])
                    ++num;
            }
            return num == h ? -1 : 2;
        }

        private static int RunStep2(int[,] costs, byte[,] masks, bool[] rowsCovered, bool[] colsCovered, int w, int h, ref Location pathStart)
        {
            Location zero;
            while (true)
            {
                zero = FindZero(costs, rowsCovered, colsCovered, w, h);
                if (zero.Row != -1)
                {
                    masks[zero.Row, zero.Column] = 2;
                    var starInRow = FindStarInRow(masks, w, zero.Row);
                    if (starInRow != -1)
                    {
                        rowsCovered[zero.Row] = true;
                        colsCovered[starInRow] = false;
                    }
                    else
                        goto label_4;
                }
                else
                    break;
            }
            return 4;
            label_4:
            pathStart = zero;
            return 3;
        }

        private static int RunStep3(byte[,] masks, bool[] rowsCovered, bool[] colsCovered, int w, int h, Location[] path, Location pathStart)
        {
            var index1 = 0;
            path[0] = pathStart;
            while (true)
            {
                var starInColumn = FindStarInColumn(masks, h, path[index1].Column);
                if (starInColumn != -1)
                {
                    var index2 = index1 + 1;
                    path[index2] = new Location(starInColumn, path[index2 - 1].Column);
                    var primeInRow = FindPrimeInRow(masks, w, path[index2].Row);
                    index1 = index2 + 1;
                    path[index1] = new Location(path[index1 - 1].Row, primeInRow);
                }
                else
                    break;
            }
            ConvertPath(masks, path, index1 + 1);
            ClearCovers(rowsCovered, colsCovered, w, h);
            ClearPrimes(masks, w, h);
            return 1;
        }

        private static int RunStep4(int[,] costs, bool[] rowsCovered, bool[] colsCovered, int w, int h)
        {
            var minimum = FindMinimum(costs, rowsCovered, colsCovered, w, h);
            for (var index1 = 0; index1 < h; ++index1)
            {
                for (var index2 = 0; index2 < w; ++index2)
                {
                    if (rowsCovered[index1])
                        costs[index1, index2] += minimum;
                    if (!colsCovered[index2])
                        costs[index1, index2] -= minimum;
                }
            }
            return 2;
        }

        private static void ConvertPath(byte[,] masks, Location[] path, int pathLength)
        {
            for (var index = 0; index < pathLength; ++index)
            {
                if (masks[path[index].Row, path[index].Column] == 1)
                    masks[path[index].Row, path[index].Column] = 0;
                else if (masks[path[index].Row, path[index].Column] == 2)
                    masks[path[index].Row, path[index].Column] = 1;
            }
        }

        private static Location FindZero(int[,] costs, bool[] rowsCovered, bool[] colsCovered, int w, int h)
        {
            for (var row = 0; row < h; ++row)
            {
                for (var col = 0; col < w; ++col)
                {
                    if (Math.Abs(costs[row, col]) < Tolerance && !rowsCovered[row] && !colsCovered[col])
                        return new Location(row, col);
                }
            }
            return new Location(-1, -1);
        }

        private static int FindMinimum(int[,] costs, bool[] rowsCovered, bool[] colsCovered, int w, int h)
        {
            var val1 = int.MaxValue;
            for (var index1 = 0; index1 < h; ++index1)
            {
                for (var index2 = 0; index2 < w; ++index2)
                {
                    if (!rowsCovered[index1] && !colsCovered[index2])
                        val1 = Math.Min(val1, costs[index1, index2]);
                }
            }
            return val1;
        }

        private static int FindStarInRow(byte[,] masks, int w, int row)
        {
            for (var index = 0; index < w; ++index)
            {
                if (masks[row, index] == 1)
                    return index;
            }
            return -1;
        }

        private static int FindStarInColumn(byte[,] masks, int h, int col)
        {
            for (var index = 0; index < h; ++index)
            {
                if (masks[index, col] == 1)
                    return index;
            }
            return -1;
        }

        private static int FindPrimeInRow(byte[,] masks, int w, int row)
        {
            for (var index = 0; index < w; ++index)
            {
                if (masks[row, index] == 2)
                    return index;
            }
            return -1;
        }

        private static void ClearCovers(bool[] rowsCovered, bool[] colsCovered, int w, int h)
        {
            for (var index = 0; index < h; ++index)
                rowsCovered[index] = false;
            for (var index = 0; index < w; ++index)
                colsCovered[index] = false;
        }

        private static void ClearPrimes(byte[,] masks, int w, int h)
        {
            for (var index1 = 0; index1 < h; ++index1)
            {
                for (var index2 = 0; index2 < w; ++index2)
                {
                    if (masks[index1, index2] == 2)
                        masks[index1, index2] = 0;
                }
            }
        }

        private struct Location
        {
            public readonly int Row;
            public readonly int Column;

            public Location(int row, int col)
            {
                Row = row;
                Column = col;
            }
        }
    }
}
