using System;
using HarmonyLib;
using SandBox.GauntletUI.Map;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NobleCompass
{
    /// <summary>
    /// The compass arrow on the campaign map, built like the game's party trackers (GauntletMapTrackersView): the
    /// movie sits on the map's nameplate layer and the party's position is projected to the screen every frame.
    /// </summary>
    public sealed class NobleCompassMapView : MapView
    {
        private const float DebugEverySeconds = 3f;

        private NobleCompassVM _dataSource;
        private GauntletLayer _layer;
        private GauntletMovieIdentifier _movie;
        private bool _ownsLayer;
        private bool _inConversation;
        private float _debugTimer;

        protected override void CreateLayout()
        {
            base.CreateLayout();
            _dataSource = new NobleCompassVM();

            // The nameplate layer keeps the arrow under menus and the encyclopedia. If another mod replaced the
            // basic map view, fall back to a layer of our own.
            _layer = MapScreen.GetMapView<GauntletMapBasicView>()?.GauntletNameplateLayer;
            if (_layer == null)
            {
                _layer = new GauntletLayer("NobleCompass", 1);
                MapScreen.AddLayer(_layer);
                _ownsLayer = true;
            }
            Layer = _layer;
            _movie = _layer.LoadMovie("NobleCompassOverlay", _dataSource);
        }

        protected override void OnFinalize()
        {
            if (_movie != null) _layer?.ReleaseMovie(_movie);
            if (_ownsLayer && MapScreen != null && MapScreen.HasLayer(_layer)) MapScreen.RemoveLayer(_layer);
            _dataSource?.OnFinalize();
            _dataSource = null;
            _movie = null;
            _layer = null;
            Layer = null;
            base.OnFinalize();
        }

        protected override void OnMapConversationStart()
        {
            base.OnMapConversationStart();
            _inConversation = true;
        }

        protected override void OnMapConversationOver()
        {
            base.OnMapConversationOver();
            _inConversation = false;
        }

        protected override void OnMapScreenUpdate(float dt)
        {
            base.OnMapScreenUpdate(dt);
            if (_dataSource == null) return;
            try
            {
                _dataSource.IsShown = Update(dt);
            }
            catch (Exception e)
            {
                _dataSource.IsShown = false;
                Log.Error("MapScreenUpdate", e);
            }
        }

        /// <summary>Fills the view model; false hides the arrow.</summary>
        private bool Update(float dt)
        {
            CompassTargetBehavior behavior = CompassTargetBehavior.Current;
            Hero target = behavior?.Target;
            MobileParty party = MobileParty.MainParty;
            Camera camera = MapScreen?.MapCameraView?.Camera;
            if (!NcSettings.Enabled || _inConversation || target == null || party == null || camera == null) return false;

            if (!target.IsAlive)
            {
                behavior.Stop(); // died in a way HeroKilledEvent didn't report
                return false;
            }
            if (!CompassTargetBehavior.IsKnownEnough(target)) return false; // "Only heroes you have met" turned on later

            CampaignVec2 targetPos = target.GetCampaignPosition();
            CampaignVec2 partyPos = party.Position;
            if (!targetPos.IsValid() || !partyPos.IsValid()) return false;

            float distance = partyPos.Distance(targetPos);
            float range = CompassTargetBehavior.Range();
            bool inRange = distance <= range;
            DebugReport(dt, target, distance, range, inRange);
            AnnounceTrail(behavior, target, inRange);
            if (!inRange) return false;

            // Project the party and a point a step towards the target; the angle between them on screen follows the
            // camera's turn and tilt.
            Vec3 center = partyPos.AsVec3();
            if (!ToScreen(camera, center, out Vec2 centerOnScreen)) return false;

            Vec2 toTarget = targetPos.ToVec2() - partyPos.ToVec2();
            if (toTarget.Length > 0.001f)
            {
                Vec2 dir = toTarget.Normalized();
                if (ToScreen(camera, center + new Vec3(dir.x, dir.y), out Vec2 ahead))
                    _dataSource.NeedleAngle = (float)Math.Atan2(ahead.y - centerOnScreen.y, ahead.x - centerOnScreen.x);
            }

            _dataSource.CenterPosition = centerOnScreen;
            _dataSource.Diameter = NcSettings.Size;
            _dataSource.ArrowColor = ArrowColors.For(distance);
            _dataSource.LabelText = NcSettings.ShowLabel
                ? new TextObject("{=nc_label}Looking for {NAME}").SetTextVariable("NAME", target.Name).ToString()
                : "";
            return true;
        }

        /// <summary>World to screen pixels; false when the point is behind the camera.</summary>
        private static bool ToScreen(Camera camera, Vec3 world, out Vec2 screen)
        {
            float x = 0f, y = 0f, w = 0f;
            MBWindowManager.WorldToScreenInsideUsableArea(camera, world, ref x, ref y, ref w);
            screen = new Vec2(x, y);
            return w > 0f && !float.IsNaN(x) && !float.IsNaN(y);
        }

        private static void AnnounceTrail(CompassTargetBehavior behavior, Hero target, bool inRange)
        {
            if (inRange == behavior.WasInRange) return;
            behavior.WasInRange = inRange;
            if (!NcSettings.AnnounceTrail) return;

            TextObject text = inRange
                ? new TextObject("{=nc_trail_found}Your scouts have picked up {NAME}'s trail.")
                : new TextObject("{=nc_trail_lost}Your scouts have lost {NAME}'s trail.");
            MBInformationManager.AddQuickInformation(text.SetTextVariable("NAME", target.Name));
        }

        private void DebugReport(float dt, Hero target, float distance, float range, bool inRange)
        {
            if (!NcSettings.DebugMessages || (_debugTimer -= dt) > 0f) return;
            _debugTimer = DebugEverySeconds;
            string rangeText = float.IsInfinity(range) ? "unlimited" : range.ToString("0.0");
            Log.Debug($"{target.Name}: distance {distance:0.0}, range {rangeText} (Scouting {CompassTargetBehavior.ScoutingSkill()}), {(inRange ? "shown" : "hidden")}");
        }
    }

    /// <summary>Adds the compass view whenever the campaign map screen is built (new game or load).</summary>
    [HarmonyPatch(typeof(MapScreen), "OnInitialize")]
    internal static class MapScreenPatch
    {
        [HarmonyPostfix]
        private static void AfterOnInitialize(MapScreen __instance)
        {
            try
            {
                if (__instance.GetMapView<NobleCompassMapView>() == null)
                    __instance.AddMapView<NobleCompassMapView>(new object[0]);
            }
            catch (Exception e)
            {
                Log.Error("MapScreen.OnInitialize", e);
            }
        }
    }
}
