#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ProfanityChecker.Domain;

// ReSharper disable once CheckNamespace
namespace ProfanityChecker.Logic
{
    public class AhoCorasickAlgorithm : ISearchingAlgorithm
    {
        private AhoCorasickTreeNode Root { get; }

        private AhoCorasickAlgorithm(List<string> dictionary)
        {
            if (!dictionary.Any())
                throw new ArgumentException("Parameter cannot be empty", nameof(dictionary));
            
            Root = AhoCorasickTreeNode.CreateNode();
            
            foreach (var phrase in dictionary)
            {
                AddPatternToTree(phrase);
            }
            SetFailureNodes();
        }

        public static AhoCorasickAlgorithm Create(IEnumerable<string> dictionary)
        {
            return new(dictionary.ToList());
        }
        
        public IEnumerable<ProfanityItem> FindAll(string data, CancellationToken ct)
        {
            var result = new List<ProfanityItem>();
            
            var node = Root;

            try
            {

                for (var i = 0; i < data.Length; i++)
                {
                    ct.ThrowIfCancellationRequested();

                    var c = data[i];
                    var transition = GetTransition(c, ref node);

                    if (transition != null) node = transition;

                    foreach (var resultValue in node.Result)
                    {
                        // get whole word or phrase from left to right until spaces
                        var startIndex = i - resultValue.Length + 1;
                        var fullBounds = GetFullBounds(startIndex, i);
                        var profanityItem = new ProfanityItem(resultValue, fullBounds, startIndex);

                        var existing = result.FirstOrDefault(x => x.Equals(profanityItem));
                        if (existing != null)
                        {
                            existing.FullBounds.Add(fullBounds);
                            existing.Indexes.Add(startIndex);
                        }
                        else
                        {
                            result.Add(profanityItem);
                        }
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                return new List<ProfanityItem>(0);
            }

            return result;
            
            string GetFullBounds(int startIndex, int endIndex)
            {
                // find left bound
                var leftBound = startIndex;
                for (var i=startIndex; i>=0; i--)
                {
                    var c = data[i];
                    if (!IsWhiteSpace(c)) leftBound = i;
                    else break;
                }
                
                // find right bound
                var rightBound = endIndex;
                for (var i = endIndex; i < data.Length; i++)
                {
                    var c = data[i];
                    if (!IsWhiteSpace(c)) rightBound = i;
                    else break;
                }

                return data.Substring(leftBound, rightBound - leftBound+1);
            }

            bool IsWhiteSpace(char c)
            {
                return char.IsWhiteSpace(c) || c.ToString() == Environment.NewLine || c == '\t';
            }
        }
        
        public bool ContainsAny(string data, CancellationToken ct)
        {
            var node = Root;

            try
            {
                foreach (var c in data)
                {
                    ct.ThrowIfCancellationRequested();
                    
                    var transition = GetTransition(c, ref node);

                    if (transition != null) node = transition;

                    if (node.Result.Any()) return true;
                }
            }
            catch (OperationCanceledException e)
            {
                return false;
            }


            return false;
        }

        private AhoCorasickTreeNode? GetTransition(char c, ref AhoCorasickTreeNode node)
        {
            AhoCorasickTreeNode? transition = null;
            do
            {
                transition = node.GetTransition(c);
                if (node == Root) break;
                if (transition == null) node = node.Failure;
            } while (transition == null);

            return transition;
        }

        private void SetFailureNodes()
        {
            var nodes = FailToRootNode();
            FailUsingBfs(nodes);
            Root.Failure = Root;
        }

        private void AddPatternToTree(string pattern)
        {
            var node = Root;
            foreach (var c in pattern.ToLower())
            {
                node = node.GetTransition(c) ?? node.AddTransition(c);
            }

            node.AddResult(pattern);
        }

        private List<AhoCorasickTreeNode> FailToRootNode()
        {
            var nodes = new List<AhoCorasickTreeNode>();
            foreach (var node in Root.Transactions)
            {
                node.Failure = Root;
                nodes.AddRange(node.Transactions);
            }

            return nodes;
        }

        private void FailUsingBfs(List<AhoCorasickTreeNode> nodes)
        {
            while (nodes.Count > 0)
            {
                var newNodes = new List<AhoCorasickTreeNode>();
                foreach (var node in nodes)
                {
                    var failure = node.ParentFailure;
                    var value = node.Value;

                    while (failure != null && !failure.ContainsTransition(value))
                    {
                        failure = failure.Failure;
                    }

                    if (failure == null)
                    {
                        node.Failure = Root;
                    }
                    else
                    {
                        node.Failure = failure.GetTransition(value);
                        node.AddResult(node.Failure.Result);
                    }
                    
                    newNodes.AddRange(node.Transactions);
                }

                nodes = newNodes;
            }
        }
        
    }
}
