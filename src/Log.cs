using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NobleCompass
{
    internal static class Log
    {
        private static readonly HashSet<string> Reported = new HashSet<string>();

        /// <summary>Message-log line, only when "Debug messages" is on. Debug text isn't translated.</summary>
        public static void Debug(string text)
        {
            if (!NcSettings.DebugMessages) return;
            InformationManager.DisplayMessage(new InformationMessage("[NobleCompass] " + text, Colors.Gray));
        }

        /// <summary>Shows only the first error from each place, since the compass update runs every frame.</summary>
        public static void Error(string where, Exception e)
        {
            if (!Reported.Add(where)) return;
            InformationManager.DisplayMessage(new InformationMessage(
                new TextObject("{=nc_err_generic}[NobleCompass] error ({WHERE}): {ERROR}")
                    .SetTextVariable("WHERE", where)
                    .SetTextVariable("ERROR", e.Message)
                    .ToString(),
                Colors.Red));
        }

        public static void Info(TextObject text, Color color)
        {
            InformationManager.DisplayMessage(new InformationMessage(text.ToString(), color));
        }
    }
}
