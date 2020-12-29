using System;
using System.Collections.Generic;

namespace LinqExtensions.Extensions {
    public class KeyEqualityComparer<T> : IEqualityComparer<T> {
        private readonly Func<T, object> _keyExtractor;

        public KeyEqualityComparer(Func<T, object> keyExtractor) {
            this._keyExtractor = keyExtractor;
        }

        public bool Equals(T x, T y) {
            return this._keyExtractor(x).Equals(this._keyExtractor(y));
        }

        public int GetHashCode(T obj) {
            return this._keyExtractor(obj).GetHashCode();
        }
    }
}