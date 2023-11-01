using System;
using System.Collections.Generic;
using System.Linq;

namespace Chartboost.Editor.EditorWindows.Adapters.Comparers
{
    /// <summary>
    /// Comparer for Dictionaries, allows to compare Dictionary items based on an IEqualityComparer. 
    /// </summary>
    /// <typeparam name="TKey">Key type in dictionary.</typeparam>
    /// <typeparam name="TValue">Value type in dictionary. IEqualityComparer must be provided</typeparam>
    public class DictionaryComparer<TKey, TValue> : IEqualityComparer<Dictionary<TKey, TValue>>
    {
        private readonly IEqualityComparer<TValue> _valueComparer;

        /// <summary>
        /// Dictionary constructor, IEqualityComparer for Value type should be provided.
        /// </summary>
        /// <param name="valueComparer"></param>
        public DictionaryComparer(IEqualityComparer<TValue> valueComparer)
            => _valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

        /// <summary>
        /// Indicates equality between both Dictionaries.
        /// </summary>
        /// <param name="x">First Dictionary.</param>
        /// <param name="y">Second Dictionary.</param>
        /// <returns></returns>
        public bool Equals(Dictionary<TKey, TValue> x, Dictionary<TKey, TValue> y)
        {
            if (x.Count != y.Count)
                return false;
            if (x.Keys.Except(y.Keys).Any())
                return false;
            if (y.Keys.Except(x.Keys).Any())
                return false;
            foreach (var pair in x)
                if (!_valueComparer.Equals(pair.Value, y[pair.Key]))
                    return false;
            return true;
        }
        
        public int GetHashCode(Dictionary<TKey, TValue> obj)
        {
            throw new NotImplementedException();
        }
    }
}
