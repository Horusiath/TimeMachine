using System;

namespace TimeMachine
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DoNotTrackAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class IdentifierAttribute : Attribute
    {
        
    }
}