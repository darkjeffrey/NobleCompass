using System;
using System.Linq;
using System.Xml;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NobleCompass
{
    /// <summary>Adds the compass button to the encyclopedia hero page, right of the bookmark star.</summary>
    [PrefabExtension("EncyclopediaHeroPage", "descendant::ButtonWidget[@Id='BookmarkButton']")]
    public sealed class HeroPagePrefabPatch : CustomPatch<XmlDocument>
    {
        private const string ButtonXml = @"
<ButtonWidget Id=""NobleCompassButton"" WidthSizePolicy=""Fixed"" HeightSizePolicy=""Fixed"" SuggestedWidth=""42"" SuggestedHeight=""42""
              HorizontalAlignment=""Left"" VerticalAlignment=""Top"" MarginLeft=""60"" MarginTop=""9"" Brush=""NobleCompass.Encyclopedia.Button""
              Command.Click=""ExecuteNcToggleCompass"" IsSelected=""@NcIsTracking"" IsVisible=""@NcCanTrack"" UpdateChildrenStates=""true"">
  <Children>
    <BrushWidget WidthSizePolicy=""Fixed"" HeightSizePolicy=""Fixed"" SuggestedWidth=""14"" SuggestedHeight=""18"" HorizontalAlignment=""Center""
                 VerticalAlignment=""Center"" Brush=""NobleCompass.Encyclopedia.Button.Arrow"" DoNotAcceptEvents=""true"" />
    <HintWidget DataSource=""{NcCompassHint}"" WidthSizePolicy=""StretchToParent"" HeightSizePolicy=""StretchToParent""
                Command.HoverBegin=""ExecuteBeginHint"" Command.HoverEnd=""ExecuteEndHint"" />
  </Children>
</ButtonWidget>";

        public override string Id => "NobleCompassHeroPageButton";

        public override void Apply(XmlDocument document)
        {
            XmlElement bookmark = document.SelectNodes("//ButtonWidget").OfType<XmlElement>()
                .FirstOrDefault(e => e.GetAttribute("Id") == "BookmarkButton");
            if (bookmark?.ParentNode == null) return;

            var fragment = new XmlDocument();
            fragment.LoadXml(ButtonXml);
            bookmark.ParentNode.InsertAfter(document.ImportNode(fragment.DocumentElement, true), bookmark);
        }
    }

    /// <summary>The compass button's data on the encyclopedia hero page.</summary>
    [ViewModelMixin]
    public sealed class HeroPageCompassMixin : BaseViewModelMixin<EncyclopediaHeroPageVM>
    {
        private bool _canTrack;
        private bool _isTracking;
        private HintViewModel _hint;

        public HeroPageCompassMixin(EncyclopediaHeroPageVM vm) : base(vm)
        {
            Refresh();
        }

        private Hero PageHero => ViewModel?.Obj as Hero;

        [DataSourceProperty]
        public bool NcCanTrack { get => _canTrack; set => SetField(ref _canTrack, value, nameof(NcCanTrack)); }

        [DataSourceProperty]
        public bool NcIsTracking { get => _isTracking; set => SetField(ref _isTracking, value, nameof(NcIsTracking)); }

        [DataSourceProperty]
        public HintViewModel NcCompassHint { get => _hint; set => SetField(ref _hint, value, nameof(NcCompassHint)); }

        public override void OnRefresh()
        {
            base.OnRefresh();
            Refresh();
        }

        [DataSourceMethod]
        public void ExecuteNcToggleCompass()
        {
            Hero hero = PageHero;
            if (!CompassTargetBehavior.CanTrack(hero)) return;
            CompassTargetBehavior.Current?.Toggle(hero);
            Refresh();
        }

        private void Refresh()
        {
            try
            {
                CompassTargetBehavior behavior = CompassTargetBehavior.Current;
                Hero hero = PageHero;
                NcCanTrack = NcSettings.Enabled && behavior != null && CompassTargetBehavior.CanTrack(hero);
                NcIsTracking = behavior != null && behavior.IsTracking(hero);

                TextObject hint = NcIsTracking
                    ? new TextObject("{=nc_hint_stop}Stop following {NAME} with your compass.")
                    : !CompassTargetBehavior.IsKnownEnough(hero)
                        ? new TextObject("{=nc_hint_not_met}You haven't met {NAME} yet. Meet them first to follow them with your compass.")
                        : new TextObject("{=nc_hint_start}Follow {NAME} with your compass. It appears around your party on the map once your scouts are close enough; better Scouting finds the trail from farther away.");
                NcCompassHint = new HintViewModel(hint.SetTextVariable("NAME", hero?.Name));
            }
            catch (Exception e)
            {
                Log.Error("HeroPageRefresh", e);
            }
        }
    }
}
