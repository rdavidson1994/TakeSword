using System.Collections.Generic;

namespace TakeSword
{
    public abstract class Hypothetical
    {
        public string Name { get; set; }
    }

    public class Hypothetical<T> : Hypothetical
    {
        public ICollection<T> PossibleValues { get; set; }
        public T Value { get; set; }
    }
}