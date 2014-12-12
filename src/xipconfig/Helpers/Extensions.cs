using System;
using System.ComponentModel;
using System.Net;

namespace X_ToolZ.Helpers
{
    static class Extensions
    {
        public static string GetDescription(this Enum e)
        {
            var type = e.GetType();
            var memInfo = type.GetMember(e.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute),
                false);
            if (attributes.Length > 0)
                return ((DescriptionAttribute)attributes[0]).Description;
            return e.ToString();
        }

        public static string GetText(this bool? b)
        {
            if (b.HasValue)
            {
                if (b.Value)
                    return "Yes";
                return "No";
            }
            return "unknown";
        }

        public static string GetIPOrEmpty(this IPAddress ip)
        {
            if (ip == null)
                return string.Empty;
            return ip.ToString();
        }

        public static string GetDateTimeOrEmpty(this DateTime? dt)
        {
            if (dt == null)
                return string.Empty;
            return dt.Value.ToString();
        }
    }
}
