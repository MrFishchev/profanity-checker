using System;
using System.Collections.Generic;

namespace ProfanityChecker.Domain
{
    public sealed class ProfanityItem
    {
        private string _data;
        
        public string Data => _data;

        public HashSet<int> Indexes { get; private set; } = new();

        public HashSet<string> FullBounds { get; private set; } = new();

        public ProfanityItem(string data, string fullBounds, int index)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data), "Parameter cannot be null or empty");

            _data = data;
            FullBounds.Add(fullBounds);
            Indexes.Add(index);
        }

        public override bool Equals(object? obj)
        {
            var item = obj as ProfanityItem;
            
            if (item == null)
                return false;
            
            return Data == item.Data;
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
    }
}