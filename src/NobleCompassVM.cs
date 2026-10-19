using System.Collections.Generic;
using TaleWorlds.Library;

namespace NobleCompass
{
    /// <summary>Data for the NobleCompassOverlay movie. NobleCompassMapView fills it every frame.</summary>
    public sealed class NobleCompassVM : ViewModel
    {
        private bool _isShown;
        private Vec2 _centerPosition;
        private float _needleAngle;
        private Color _arrowColor = Color.ConvertStringToColor("#F2C97AFF");
        private float _diameter = 150f;
        private string _labelText = "";

        [DataSourceProperty]
        public bool IsShown { get => _isShown; set => Set(ref _isShown, value, nameof(IsShown)); }

        /// <summary>The party's position on screen, in pixels.</summary>
        [DataSourceProperty]
        public Vec2 CenterPosition { get => _centerPosition; set => Set(ref _centerPosition, value, nameof(CenterPosition)); }

        /// <summary>Direction to the target on screen, in radians (0 = right, clockwise, screen y points down).</summary>
        [DataSourceProperty]
        public float NeedleAngle { get => _needleAngle; set => Set(ref _needleAngle, value, nameof(NeedleAngle)); }

        [DataSourceProperty]
        public Color ArrowColor { get => _arrowColor; set => Set(ref _arrowColor, value, nameof(ArrowColor)); }

        /// <summary>Width of the circle the arrow travels, in UI units.</summary>
        [DataSourceProperty]
        public float Diameter { get => _diameter; set => Set(ref _diameter, value, nameof(Diameter)); }

        /// <summary>"Looking for ..."; empty hides the label.</summary>
        [DataSourceProperty]
        public string LabelText { get => _labelText; set => Set(ref _labelText, value, nameof(LabelText)); }

        private void Set<T>(ref T field, T value, string name)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            OnPropertyChanged(name); // the generic OnPropertyChangedWithValue<T> only takes reference types
        }
    }
}
