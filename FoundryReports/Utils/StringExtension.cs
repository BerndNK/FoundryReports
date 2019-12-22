﻿namespace FoundryReports.Utils
{
    internal static class StringExtension
    {
        /// <summary>
        /// See https://stackoverflow.com/questions/36845430/persistent-hashcode-for-strings
        /// </summary>
        public static int StableHashCode(this string input)
        {
            unchecked
            {
                var hash1 = 5381;
                var hash2 = hash1;

                for(var i = 0; i < input.Length && input[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ input[i];
                    if (i == input.Length - 1 || input[i+1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ input[i+1];
                }

                return hash1 + (hash2*1566083941);
            }
        }
    }
}
