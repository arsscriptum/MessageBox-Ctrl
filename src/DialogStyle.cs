namespace MessageBox
{
    public enum DialogStyle
    {
        // 0 - deep teal on near-black  (default / current look)
        Normal = 0,

        // 1 - hard red on very dark grey
        Critical = 1,

        // 2 - amber/gold on dark charcoal
        Warning = 2,

        // 3 - hot orange on deep brown-black
        TempAlert = 3,

        // 4 - flat Win10 blue on white-ish grey
        Windows10 = 4
    }

    public static class DialogStyleHelper
    {
        public static string Description(DialogStyle style)
        {
            switch (style)
            {
                case DialogStyle.Normal:    return "Normal    - teal on near-black (default)";
                case DialogStyle.Critical:  return "Critical  - red on dark grey";
                case DialogStyle.Warning:   return "Warning   - amber/gold on charcoal";
                case DialogStyle.TempAlert: return "TempAlert - orange on deep brown-black";
                case DialogStyle.Windows10: return "Windows10 - blue on light grey (flat)";
                default:                    return style.ToString();
            }
        }
    }
}
