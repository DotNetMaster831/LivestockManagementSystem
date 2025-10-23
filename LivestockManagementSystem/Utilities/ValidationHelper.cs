using System;
using System.Linq;

namespace LivestockManagementSystem.Utilities
{
    public static class ValidationHelper
    {
        private static readonly string[] AllowedColours = { "red", "black", "white" };
        private static readonly string[] AllowedTypes = { "cow", "sheep", "all" };

        // ✅ Validate colour — ignores case and null/empty safely
        public static bool IsValidColour(string colour)
        {
            if (string.IsNullOrWhiteSpace(colour))
                return false;
            return AllowedColours.Contains(colour.Trim().ToLower());
        }

        // ✅ Validate type — same safety and case-insensitivity
        public static bool IsValidType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return false;
            return AllowedTypes.Contains(type.Trim().ToLower());
        }

        // ✅ Positive check (works for both weight and expense)
        public static bool IsPositive(double value)
            => value > 0;

        // ✅ Overload for string inputs (handles parsing safely)
        public static bool IsPositive(string value, out double parsed)
        {
            if (double.TryParse(value, out parsed))
                return parsed > 0;
            return false;
        }

        // ✅ Valid numeric ID check
        public static bool IsValidId(string id, out int parsed)
        {
            if (int.TryParse(id, out parsed))
                return parsed > 0;
            return false;
        }

        // ✅ Direct int version (for already parsed IDs)
        public static bool IsValidId(int id) => id > 0;
    }
}
