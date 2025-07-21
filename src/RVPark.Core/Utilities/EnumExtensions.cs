using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RVPark.Core.Utilities
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var member = value.GetType()
                          .GetMember(value.ToString())
                          .FirstOrDefault();
            if (member == null) return value.ToString();
            var displayAttr = member.GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.GetName() ?? value.ToString();
        }
    }
}
