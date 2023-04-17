using System.Collections.Generic;
using Chartboost.Editor.Adapters.Serialization;

namespace Chartboost.Editor.Adapters.Comparers
{
    public class SelectedVersionsComparer : IEqualityComparer<AdapterSelection>
    {
        public bool Equals(AdapterSelection x, AdapterSelection y)
        {
            if (x != null && x.id != y.id)
                return false;
            if (x.android != y.android)
                return false;
            if (x.ios != y.ios)
                return false;
            return true;
        }

        public int GetHashCode(AdapterSelection obj)
        {
            unchecked
            {
                var hashCode = (obj.id != null ? obj.id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.android != null ? obj.android.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.ios != null ? obj.ios.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
