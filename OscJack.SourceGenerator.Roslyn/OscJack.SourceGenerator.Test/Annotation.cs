using System;

namespace OscJack.Annotation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class OscPackableAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class OscElementOrderAttribute : Attribute
    {
        public int Index { get; private set; }

        public OscElementOrderAttribute(int index)
        {
            this.Index = index;
        }
    }
}