using System;

namespace NobleCompass
{
    /// <summary>
    /// Read-only view of the current settings. Values come from the MCM page (NobleCompassMcmSettings);
    /// the defaults there are used if MCM isn't loaded yet.
    /// </summary>
    public static class NcSettings
    {
        private static readonly NobleCompassMcmSettings Defaults = new NobleCompassMcmSettings();

        private static NobleCompassMcmSettings S
        {
            get
            {
                try { return NobleCompassMcmSettings.Instance ?? Defaults; }
                catch (Exception) { return Defaults; }
            }
        }

        public static bool Enabled => S.Enabled;
        public static bool AnyHero => S.AnyHero;
        public static bool MetOnly => S.MetOnly;
        public static float BaseRange => Math.Max(0f, S.BaseRange);
        public static float RangePerSkill => Math.Max(0f, S.RangePerSkill);
        public static bool UnlimitedRange => S.UnlimitedRange;
        public static int Size => Math.Max(20, S.Size);
        public static bool ShowLabel => S.ShowLabel;
        public static string ArrowColor => S.ArrowColor;
        public static bool ColorByDistance => S.ColorByDistance;
        public static string NearColor => S.NearColor;
        public static string FarColor => S.FarColor;
        public static bool AnnounceTrail => S.AnnounceTrail;
        public static bool DebugMessages => S.DebugMessages;
    }
}
