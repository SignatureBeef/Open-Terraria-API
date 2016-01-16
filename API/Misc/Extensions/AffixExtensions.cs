using System;

namespace OTA.Misc
{
    public static class AffixExtensions
    {
        public static bool Parse(string str, out Affix affix, bool ignoreCase = false)
        {
            affix = Affix.None;

            foreach (var val in Enum.GetValues(typeof(Affix)))
            {
                var afx = (Affix)val;
                if (afx.ToString() == str || (ignoreCase && afx.ToString().ToLower() == str.ToLower()))
                {
                    affix = afx;
                    return true;
                }
            }

            return false;
        }
    }
}

