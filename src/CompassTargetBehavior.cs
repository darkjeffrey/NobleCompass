using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NobleCompass
{
    /// <summary>
    /// Remembers who the compass follows. Only a vanilla Hero reference is saved, so removing the mod is safe.
    /// </summary>
    public sealed class CompassTargetBehavior : CampaignBehaviorBase
    {
        private Hero _target;

        public static CompassTargetBehavior Current => Campaign.Current?.GetCampaignBehavior<CompassTargetBehavior>();

        public Hero Target => _target;

        /// <summary>Whether the trail was in range last frame, for the found/lost messages. Not saved.</summary>
        public bool WasInRange { get; set; }

        public override void RegisterEvents()
        {
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, OnHeroKilled);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("nc_target", ref _target);
        }

        /// <summary>Whether the encyclopedia offers the compass for this hero.</summary>
        public static bool CanTrack(Hero hero) =>
            hero != null && hero.IsAlive && hero != Hero.MainHero && (NcSettings.AnyHero || hero.IsLord);

        /// <summary>False when "Only heroes you have met" is on and the player hasn't met this hero.</summary>
        public static bool IsKnownEnough(Hero hero) => hero != null && (!NcSettings.MetOnly || hero.HasMet);

        public bool IsTracking(Hero hero) => hero != null && _target == hero;

        public void Toggle(Hero hero)
        {
            if (hero == null) return;

            if (_target == hero)
            {
                Stop();
                Say("{=nc_msg_stopped}Your compass no longer follows {NAME}.", hero, Colors.White);
            }
            else if (!IsKnownEnough(hero))
            {
                Say("{=nc_msg_not_met}You haven't met {NAME} yet, so your scouts wouldn't know whom to look for.", hero, Colors.Yellow);
            }
            else
            {
                _target = hero;
                WasInRange = false;
                Say("{=nc_msg_started}Your compass now follows {NAME}. It appears around your party once your scouts are close enough to pick up the trail.", hero, Colors.Green);
            }
        }

        public void Stop()
        {
            _target = null;
            WasInRange = false;
        }

        private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
        {
            if (victim == null || victim != _target) return;
            Stop();
            MBInformationManager.AddQuickInformation(
                new TextObject("{=nc_msg_died}{NAME} has died. Your compass needle falls still.").SetTextVariable("NAME", victim.Name));
        }

        private static void Say(string text, Hero hero, Color color) =>
            Log.Info(new TextObject(text).SetTextVariable("NAME", hero.Name), color);

        /// <summary>Scouting of the party's scout (the game falls back to the leader when none is assigned).</summary>
        public static int ScoutingSkill()
        {
            Hero scout = MobileParty.MainParty?.EffectiveScout ?? Hero.MainHero;
            return scout?.GetSkillValue(DefaultSkills.Scouting) ?? 0;
        }

        /// <summary>The scouting range in map distance, ignoring "Unlimited range".</summary>
        public static float ScoutRange() => NcSettings.BaseRange + NcSettings.RangePerSkill * ScoutingSkill();

        /// <summary>How close the target must be for the compass to show.</summary>
        public static float Range() => NcSettings.UnlimitedRange ? float.PositiveInfinity : ScoutRange();
    }
}
