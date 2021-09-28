using System;
namespace ForLab.Core.Common
{
    public sealed class DisplayNameAttribute: Attribute
    {
        public DisplayNameAttribute(Type resourceType, string resourceName)
        {
            ResourceType = resourceType;
            ResourceName = resourceName;
        }

        public string ResourceName
        {
            get;
            private set;
        }

        public Type ResourceType
        {
            get;
            private set;
        }
    }
}
