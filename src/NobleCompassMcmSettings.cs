using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace NobleCompass
{
    /// <summary>The "Noble Compass" page in Mod Options (MCM). Saved to Documents\Mount and Blade II Bannerlord\Configs\ModSettings.</summary>
    public sealed class NobleCompassMcmSettings : AttributeGlobalSettings<NobleCompassMcmSettings>
    {
        public override string Id => "NobleCompass";
        public override string FolderName => "NobleCompass";
        public override string FormatType => "json2";

        // MCM doesn't translate DisplayName. Before the text system is ready TextObject returns the raw "{=id}text",
        // so drop the id to keep the English name.
        public override string DisplayName
        {
            get
            {
                string name = new TextObject("{=nc_mcm_title}Noble Compass").ToString();
                return name.StartsWith("{=", System.StringComparison.Ordinal) ? name.Substring(name.IndexOf('}') + 1) : name;
            }
        }

        // ---------------- General ----------------
        private const string General = "{=nc_mcm_group_general}General";

        [SettingPropertyBool("{=nc_mcm_Enabled}Enable", Order = 0, RequireRestart = false,
            HintText = "{=nc_mcm_Enabled_hint}Turn the mod on or off. Off hides the compass and the encyclopedia button; the noble you picked is kept.")]
        [SettingPropertyGroup(General, GroupOrder = 0)]
        public bool Enabled { get; set; } = true;

        [SettingPropertyBool("{=nc_mcm_AnyHero}Track any hero, not only nobles", Order = 1, RequireRestart = false,
            HintText = "{=nc_mcm_AnyHero_hint}Off: the compass button only shows on the encyclopedia pages of lords and ladies. On: it shows for every living hero, such as companions, wanderers and notables.")]
        [SettingPropertyGroup(General, GroupOrder = 0)]
        public bool AnyHero { get; set; } = false;

        [SettingPropertyBool("{=nc_mcm_MetOnly}Only heroes you have met", Order = 2, RequireRestart = false,
            HintText = "{=nc_mcm_MetOnly_hint}Your scouts can only follow someone you have met in person. The compass button still shows for others, but tells you that you haven't met them yet, and an arrow already following someone you haven't met stays hidden.")]
        [SettingPropertyGroup(General, GroupOrder = 0)]
        public bool MetOnly { get; set; } = false;

        // ---------------- Range ----------------
        private const string Range = "{=nc_mcm_group_range}Range (Scouting)";

        [SettingPropertyFloatingInteger("{=nc_mcm_BaseRange}Range with 0 Scouting", 0f, 300f, "0", Order = 0, RequireRestart = false,
            HintText = "{=nc_mcm_BaseRange_hint}How close, in map distance, the noble must be for the compass to appear when your party's scout has no Scouting skill. For scale, a party usually covers about 4 to 6 map distance per hour. Turn on Debug messages to see the current distance and range.")]
        [SettingPropertyGroup(Range, GroupOrder = 1)]
        public float BaseRange { get; set; } = 20f;

        [SettingPropertyFloatingInteger("{=nc_mcm_RangePerSkill}Extra range per Scouting point", 0f, 3f, "0.00", Order = 1, RequireRestart = false,
            HintText = "{=nc_mcm_RangePerSkill_hint}Range added for every point of Scouting your party's scout has (or your own, if no scout is assigned). With the defaults: 20 at 0 Scouting, 50 at 100, 110 at 300.")]
        [SettingPropertyGroup(Range, GroupOrder = 1)]
        public float RangePerSkill { get; set; } = 0.3f;

        [SettingPropertyBool("{=nc_mcm_UnlimitedRange}Unlimited range", Order = 2, RequireRestart = false,
            HintText = "{=nc_mcm_UnlimitedRange_hint}Show the compass at any distance and ignore Scouting.")]
        [SettingPropertyGroup(Range, GroupOrder = 1)]
        public bool UnlimitedRange { get; set; } = false;

        // ---------------- Look ----------------
        private const string Look = "{=nc_mcm_group_look}Look";

        [SettingPropertyInteger("{=nc_mcm_Size}Compass size", 60, 400, "0", Order = 0, RequireRestart = false,
            HintText = "{=nc_mcm_Size_hint}Width of the circle the arrow travels around your party, in UI units (it follows the game's UI scale). Smaller keeps the arrow closer to your party.")]
        [SettingPropertyGroup(Look, GroupOrder = 2)]
        public int Size { get; set; } = 150;

        [SettingPropertyBool("{=nc_mcm_ShowLabel}Show who you're looking for", Order = 1, RequireRestart = false,
            HintText = "{=nc_mcm_ShowLabel_hint}Show \"Looking for\" and the noble's name under your party while the arrow is shown.")]
        [SettingPropertyGroup(Look, GroupOrder = 2)]
        public bool ShowLabel { get; set; } = true;

        [SettingPropertyText("{=nc_mcm_ArrowColor}Arrow color", Order = 2, RequireRestart = false,
            HintText = "{=nc_mcm_ArrowColor_hint}Color of the arrow as #RRGGBB (or #RRGGBBAA), used when it isn't colored by distance.")]
        [SettingPropertyGroup(Look, GroupOrder = 2)]
        public string ArrowColor { get; set; } = "#F2C97A";

        [SettingPropertyBool("{=nc_mcm_ColorByDistance}Color the arrow by distance", Order = 3, RequireRestart = false,
            HintText = "{=nc_mcm_ColorByDistance_hint}Blend the arrow from the far color, at the edge of your scouting range, to the near color as you close in on the noble.")]
        [SettingPropertyGroup(Look, GroupOrder = 2)]
        public bool ColorByDistance { get; set; } = false;

        [SettingPropertyText("{=nc_mcm_NearColor}Near color", Order = 4, RequireRestart = false,
            HintText = "{=nc_mcm_NearColor_hint}Arrow color when the noble is right next to you, as #RRGGBB. Only used when coloring by distance.")]
        [SettingPropertyGroup(Look, GroupOrder = 2)]
        public string NearColor { get; set; } = "#5FE05F";

        [SettingPropertyText("{=nc_mcm_FarColor}Far color", Order = 5, RequireRestart = false,
            HintText = "{=nc_mcm_FarColor_hint}Arrow color when the noble is at the edge of your scouting range, as #RRGGBB. Only used when coloring by distance. In between, the colors blend around the color wheel (green to red passes through yellow).")]
        [SettingPropertyGroup(Look, GroupOrder = 2)]
        public string FarColor { get; set; } = "#E04A3A";


        // ---------------- Messages ----------------
        private const string Messages = "{=nc_mcm_group_messages}Messages";

        [SettingPropertyBool("{=nc_mcm_AnnounceTrail}Announce when the trail is found or lost", Order = 0, RequireRestart = false,
            HintText = "{=nc_mcm_AnnounceTrail_hint}Show a message at the top of the screen when the noble comes within range and the compass appears, and when they move out of range again.")]
        [SettingPropertyGroup(Messages, GroupOrder = 3)]
        public bool AnnounceTrail { get; set; } = true;

        // ---------------- Testing ----------------
        private const string Testing = "{=nc_mcm_group_testing}Testing";

        [SettingPropertyBool("{=nc_mcm_DebugMessages}Debug messages", Order = 0, RequireRestart = false,
            HintText = "{=nc_mcm_DebugMessages_hint}Every few seconds while tracking, show the distance to the noble, your scout's Scouting skill and the compass range in the chat log. Useful for tuning the range; turn it off for normal play.")]
        [SettingPropertyGroup(Testing, GroupOrder = 4)]
        public bool DebugMessages { get; set; } = false;
    }
}
