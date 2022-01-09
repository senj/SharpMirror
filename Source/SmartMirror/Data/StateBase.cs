using System;

namespace SmartMirror.Data
{
    public class StateBase : Displayable
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        public StateBase(string name, Type type) : base()
        {
            Name = name;
            Type = type;
        }
    }
}
