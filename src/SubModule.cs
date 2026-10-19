using System;
using Bannerlord.UIExtenderEx;
using Bannerlord.UIExtenderEx.ResourceManager;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NobleCompass
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            try
            {
                WidgetFactoryManager.Register(typeof(NobleCompassWidget));

                // The compass button on the encyclopedia hero page (HeroPagePrefabPatch + HeroPageCompassMixin).
                UIExtender uiExtender = UIExtender.Create("NobleCompass");
                uiExtender.Register(typeof(SubModule).Assembly);
                uiExtender.Enable();

                // MapScreenPatch adds the compass view to the campaign map.
                _harmony = new Harmony("darkjeffrey.noblecompass");
                _harmony.PatchAll(typeof(SubModule).Assembly);
            }
            catch (Exception e)
            {
                Log.Error("OnSubModuleLoad", e);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign && gameStarterObject is CampaignGameStarter starter)
                starter.AddBehavior(new CompassTargetBehavior());
        }
    }
}
