using System.Collections.Generic;

namespace ProfanityChecker.Logic
{
    internal class AhoCorasickTreeNode
    {
        private readonly HashSet<string> _result = new();
        private readonly Dictionary<char, AhoCorasickTreeNode> _transitionsDictionary = new();
        private readonly AhoCorasickTreeNode _parent;

        public HashSet<string> Result => _result;
        public AhoCorasickTreeNode ParentFailure => _parent?.Failure;
        public IEnumerable<AhoCorasickTreeNode> Transactions => _transitionsDictionary.Values;
        public char Value { get; private set; }
        public AhoCorasickTreeNode Failure { get; set; }

        private AhoCorasickTreeNode(AhoCorasickTreeNode parent, char value)
        {
            Value = value;
            _parent = parent;
        }

        public static AhoCorasickTreeNode CreateNode() => new(null, ' ');

        public void AddResult(string result) => _result.Add(result);

        public void AddResult(IEnumerable<string> result)
        {
            foreach (var value in result)
                _result.Add(value);
        }

        public AhoCorasickTreeNode AddTransition(char c)
        {
            var node = new AhoCorasickTreeNode(this, c);
            _transitionsDictionary.Add(node.Value, node);
            return node;
        }

        public AhoCorasickTreeNode GetTransition(char c) => _transitionsDictionary.GetValueOrDefault(char.ToLower(c));

        public bool ContainsTransition(char c) => _transitionsDictionary.ContainsKey(char.ToLower(c));
    }
}