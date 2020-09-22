using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoRV
{
    class TimeManage
    {
        private static string tzId = "";
        private static bool daylight = false;
        private static int offset = 0;
        public static void setTimezone(string tz)
        {
            Config.getInstance().getTimezone(tz, ref tzId, ref daylight, ref offset);
        }
        public static DateTime getCurrentTime()
        {
            if (tzId != "")
            {
                try
                {
                    TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
                    DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                    if (tz.IsDaylightSavingTime(now) && !daylight)
                        now = now.AddHours(-1);
                    return now;
                }
                catch { }
            }
            return DateTime.UtcNow.AddHours(offset);
        }
    }
}
