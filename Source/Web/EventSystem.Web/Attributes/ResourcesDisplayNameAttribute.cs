using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace EventSystem.Web.Attributes
{
    public class ResourcesDisplayNameAttribute : DisplayNameAttribute
    {
        private PropertyInfo nameProperty;
        private Type resourceType;

        public ResourcesDisplayNameAttribute(string displayNameKey)
            : base(displayNameKey)
        {

        }

        public Type NameResourceType
        {
            get { return resourceType; }
            set
            {
                resourceType = value;
                nameProperty = resourceType.GetProperty(base.DisplayName, BindingFlags.Static | BindingFlags.Public);
            }
        }

        public override string DisplayName
        {
            get
            {
                if (nameProperty == null)
                {
                    return base.DisplayName;
                }

                return (string)nameProperty.GetValue(nameProperty.DeclaringType, null);
            }
        }

    }
}