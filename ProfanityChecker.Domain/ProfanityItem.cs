using System;
using System.Collections.Generic;

namespace ProfanityChecker.Domain
{
    public sealed class ProfanityItem
    {
        private string _data;
        
        public string Data => _data;

        public List<int> Indexes { get; set; } = new();

        public long Count { get; set; } = 1;
        
        public string FullBounds { get; }

        public ProfanityItem(string data, string fullBounds, int index)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data), "Parameter cannot be null or empty");

            _data = data;
            FullBounds = fullBounds;
            Indexes.Add(index);
        }

        public override bool Equals(object? obj)
        {
            var item = obj as ProfanityItem;
            
            if (item == null)
                return false;
            
            return Data == item.Data && FullBounds == item.FullBounds;
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode() ^ FullBounds.GetHashCode();
        }
    }
}