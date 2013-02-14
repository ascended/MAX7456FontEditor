using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MaxFontEditor.Services
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SingletonServiceAttribute : Attribute
    {
        public SingletonServiceAttribute()
        {
        }

        public static bool HasAttribute(ICustomAttributeProvider pi)
        {
            var result = pi.GetCustomAttributes(typeof(SingletonServiceAttribute), true)
                .Where(a => a is SingletonServiceAttribute)
                .Any();

            return result;
        }
    }
}
