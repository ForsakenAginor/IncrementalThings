using System;
using Lean.Localization;

namespace Source.Scripts.Utility.Extensions
{
    public static class StringExtensions
    {
        public static string Translate(this string str)
        {
            if(string.IsNullOrEmpty(str))
                throw new ArgumentException("String cannot be null or empty");
        
        
            return LeanLocalization.GetTranslationText(str);
        }
    }
}