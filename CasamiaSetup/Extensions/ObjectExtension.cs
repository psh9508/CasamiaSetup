using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CasamiaSetup.Extensions
{
    public static class ObjectExtension
    {
        public static Dictionary<string, string> ToDictionary(this object src)
        {
            return src.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                .ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(src, null));
        }
    }
}
