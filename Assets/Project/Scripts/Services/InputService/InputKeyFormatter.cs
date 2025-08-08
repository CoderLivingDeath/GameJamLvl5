using System.Text.RegularExpressions;

namespace GameJamLvl5.Project.Scripts.Services.InputService
{
    public static class InputKeyFormatter
    {
        private static readonly Regex pattern = new Regex(@"^([^/]+)(/[^/]+)*$", RegexOptions.Compiled);

        /// <summary>
        /// Checks if the string matches the pattern "segment/segment/..." 
        /// </summary>
        public static bool IsValid(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            return pattern.IsMatch(input);
        }

        /// <summary>
        /// Formats a string to the pattern "segment/segment/..." (no spaces, lowercase).
        /// Returns the formatted string or null if the format is invalid.
        /// </summary>
        public static string Format(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var segments = input.Split('/');

            for (int i = 0; i < segments.Length; i++)
            {
                segments[i] = segments[i].Trim();

                if (string.IsNullOrEmpty(segments[i]))
                    return null; // empty segment - invalid format
            }

            string formatted = string.Join("/", segments).ToLowerInvariant();

            if (!IsValid(formatted))
                return null;

            return formatted;
        }

        /// <summary>
        /// Converts a string to the format "name/call_type" (no spaces, lowercase, slash separated)
        /// and checks if it matches this format.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="result">Output formatted string if successful.</param>
        /// <returns>True if the string is successfully formatted to "name/call_type", otherwise false.</returns>
        public static bool TryFormatKey(string input, out string result)
        {
            result = InputKeyFormatter.Format(input);
            return result != null;
        }
    }
}
