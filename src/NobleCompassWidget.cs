using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace NobleCompass
{
    /// <summary>
    /// An invisible circle centred on CenterPosition (screen pixels, as the game's MobilePartyTrackerItemWidget uses
    /// its Position), with the arrow on its edge pointing outwards along NeedleAngle. The arrow must be aligned
    /// Left/Top. Sizes come from ScaledSuggestedWidth/Height (everything here is Fixed) because Widget.Size is a
    /// System.Numerics.Vector2, which would need an extra reference to build.
    /// </summary>
    public class NobleCompassWidget : Widget
    {
        private const float RadToDeg = (float)(180.0 / Math.PI);

        public NobleCompassWidget(UIContext context) : base(context)
        {
        }

        public Vec2 CenterPosition { get; set; }

        /// <summary>Radians on screen: 0 = right, positive turns clockwise (screen y points down).</summary>
        public float NeedleAngle { get; set; }

        public Color NeedleColor { get; set; } = Color.ConvertStringToColor("#F2C97AFF");

        /// <summary>The arrow. Its sprite points up at Rotation 0.</summary>
        public Widget NeedleWidget { get; set; }

        protected override void OnLateUpdate(float dt)
        {
            base.OnLateUpdate(dt);

            float radius = ScaledSuggestedWidth * 0.5f;
            ScaledPositionXOffset = CenterPosition.x - radius;
            ScaledPositionYOffset = CenterPosition.y - ScaledSuggestedHeight * 0.5f;

            Widget arrow = NeedleWidget;
            if (arrow == null) return;

            float distance = Math.Max(0f, radius - arrow.ScaledSuggestedHeight * 0.5f);
            arrow.ScaledPositionXOffset = radius + (float)Math.Cos(NeedleAngle) * distance - arrow.ScaledSuggestedWidth * 0.5f;
            arrow.ScaledPositionYOffset = ScaledSuggestedHeight * 0.5f + (float)Math.Sin(NeedleAngle) * distance - arrow.ScaledSuggestedHeight * 0.5f;

            // Widget.Rotation is in degrees, clockwise, around the pivot (a fraction of the size). The sprite points
            // up, which is -90 degrees.
            arrow.PivotX = 0.5f;
            arrow.PivotY = 0.5f;
            arrow.Rotation = NeedleAngle * RadToDeg + 90f;
            if (arrow.Color != NeedleColor) arrow.Color = NeedleColor;
        }
    }
}
