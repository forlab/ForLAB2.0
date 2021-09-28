using System;
using System.Resources;

namespace ForLab.Core.Common
{
    public static class EnumExtension
    {     
        public static string GetDisplayName(Type resourceType, string resourceKey)
        {

            var _resourceManager = new ResourceManager(resourceType);
            string displayName = _resourceManager.GetString(resourceKey);
            return string.IsNullOrWhiteSpace(displayName) ? string.Format("[[{0}]]", resourceKey) : displayName;
        }
    }
}
