namespace OTAPI.Patcher.Extensions
{
    public static partial class StringExtensions
    {
        public static string TrimEnd(this string input, string suffixToRemove)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove))
                return input.Substring(0, input.Length - suffixToRemove.Length);

            return input;
        }
    }
}
