using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestockManagementSystem.Utilities
{
    public class ValidationHelper
    {
        private static readonly string[] AllowedColours = { "red", "black", "white" };
        private static readonly string[] AllowedTypes = { "cow", "sheep", "all" };

        public static bool IsValidColour(string colour)
            => AllowedColours.Contains(colour.ToLower());

        public static bool IsValidType(string type)
            => AllowedTypes.Contains(type.ToLower());

        public static bool IsPositive(double value)
            => value >= 0;

        public static bool IsValidId(int id)
            => id > 0;
    }
}
