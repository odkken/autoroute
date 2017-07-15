using System;
using System.Collections.Generic;
using System.Linq;

namespace Routing
{
    public class IntWrapper
    {
        public int Value { get; set; }
        public IntWrapper(int value) { Value = value; }
    }
    public class TreeLevel
    {
        private readonly List<int> _values;
        private readonly int _depth;
        private int _i;
        private TreeLevel _nextLevel;
        private readonly TreeLevel[] _levels;
        private readonly IntWrapper _maxDepth;
        public TreeLevel(List<int> values, int depth)
        {
            _values = values;
            _depth = depth;
            _levels = new TreeLevel[values.Count];
            _levels[0] = this;
            _maxDepth = new IntWrapper(0);
        }

        private TreeLevel(List<int> values, TreeLevel[] levels, int depth, IntWrapper maxDepth)
        {
            _values = values;
            _depth = depth;
            levels[depth] = this;
            _levels = levels;
            _maxDepth = maxDepth;
            _maxDepth.Value = depth;
        }

        public void WalkOver(int depth)
        {
            _levels[depth]._i++;
            _levels[depth].Prune();
            for (int i = _maxDepth.Value - 1; i >= 0; i--)
            {
                if (!_levels[i + 1].Exhausted) break;

                _levels[i]._i++;
                _levels[i].Prune();
            }
        }

        public void Prune()
        {
            _nextLevel?.Prune();
            _nextLevel = null;
            _maxDepth.Value = _depth;
        }

        public void WalkDown()
        {
            if (_nextLevel != null)
                _nextLevel.WalkDown(); 
            else if (_values.Count > 1)
            {
                var lowerValues = _values.ToList();
                lowerValues.RemoveAt(_i);
                _nextLevel = new TreeLevel(lowerValues, _levels, _depth + 1, _maxDepth);
            }
        }

        public List<int> GetCurrentValues()
        {
            var vals = new List<int>();
            for (int i = 0; i <= _maxDepth.Value; i++)
            {
                vals.Add(_levels[i].MyValue);
            }
            return vals;
        }

        public void PrintValues()
        {
            Console.WriteLine(string.Join(",", GetCurrentValues()));
        }

        private int MyValue => _values[_i];

        public bool Exhausted => _i == _values.Count;

    }
}