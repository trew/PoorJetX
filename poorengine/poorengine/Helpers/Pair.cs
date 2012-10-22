using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoorEngine.Helpers
{
    [Serializable]
    public class Pair<T, U>
    {
        public T First { get; set; }
        public U Second { get; set; }

        public Pair() { }

        public Pair(T first, U second)
        {
            First = first;
            Second = second;
        }

        public override int GetHashCode()
        {
            return (First != null ? First.GetHashCode() : 0) + 29 * (Second != null ? Second.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("First: {0}, Second: {1}", this.First,  this.Second);
        }

        public override bool Equals(object obj)
        {
            try
            {
                Pair<T, U> ob = (Pair<T, U>)obj;
                return (Equals(ob.First, First) && Equals(ob.Second, Second)) || (Equals(ob.Second, First) && Equals(ob.First, Second));
            }
            catch
            {
                return false;
            }
        }

        public static bool operator ==(Pair<T, U> p1, Pair<T, U> p2)
        {
            if (Equals(p1, null) && Equals(p2, null)) return true;
            if (Equals(p1, null) || Equals(p2, null)) return false;
            return (Equals(p1.First, p2.First) && Equals(p1.Second, p2.Second)) ||
                   (Equals(p1.Second, p2.First) && Equals(p1.First, p2.Second));
        }
    
        public static bool operator !=(Pair<T, U> p1, Pair<T, U> p2)
        {
            if (Equals(p1, null) && Equals(p2, null)) return false;
            if (Equals(p1, null) || Equals(p2, null)) return true;
            return !((Equals(p1.First, p2.First) && Equals(p1.Second, p2.Second)) ||
                   (Equals(p1.Second, p2.First) && Equals(p1.First, p2.Second)));
        }
    }
}
