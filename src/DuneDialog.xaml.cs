using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MessageBox
{
    public partial class DuneDialog : Window
    {
        public string Message { get; set; }

        private MediaPlayer _player = null;

        // ---- per-style palette ----
        private struct StylePalette
        {
            public string WindowBg;
            public string Accent;
            public string GlowColor;
            public string TitleGlow;
            public string MessageBg;
            public string MessageBorder;
            public string MessageFg;
            public string ButtonBg;
            public string ButtonHover;
            public string ButtonPressed;
        }

        private static StylePalette GetPalette(DialogStyle style)
        {
            StylePalette p;
            switch (style)
            {
                case DialogStyle.Critical:
                    p.WindowBg      = "#CC120408";
                    p.Accent        = "#FFFF2D2D";
                    p.GlowColor     = "#FFFF0000";
                    p.TitleGlow     = "#FFFF2D2D";
                    p.MessageBg     = "#1AFF2D2D";
                    p.MessageBorder = "#66FF2D2D";
                    p.MessageFg     = "#FFFFC8C8";
                    p.ButtonBg      = "#1AFF2D2D";
                    p.ButtonHover   = "#33FF2D2D";
                    p.ButtonPressed = "#55FF2D2D";
                    break;

                case DialogStyle.Warning:
                    p.WindowBg      = "#CC0F0C02";
                    p.Accent        = "#FFFFC107";
                    p.GlowColor     = "#FFFFB300";
                    p.TitleGlow     = "#FFFFC107";
                    p.MessageBg     = "#1AFFC107";
                    p.MessageBorder = "#66FFC107";
                    p.MessageFg     = "#FFFFF8DC";
                    p.ButtonBg      = "#1AFFC107";
                    p.ButtonHover   = "#33FFC107";
                    p.ButtonPressed = "#55FFC107";
                    break;

                case DialogStyle.TempAlert:
                    p.WindowBg      = "#CC110600";
                    p.Accent        = "#FFFF6D00";
                    p.GlowColor     = "#FFFF4500";
                    p.TitleGlow     = "#FFFF6D00";
                    p.MessageBg     = "#1AFF6D00";
                    p.MessageBorder = "#66FF6D00";
                    p.MessageFg     = "#FFFFD8B0";
                    p.ButtonBg      = "#1AFF6D00";
                    p.ButtonHover   = "#33FF6D00";
                    p.ButtonPressed = "#55FF6D00";
                    break;

                case DialogStyle.Windows10:
                    p.WindowBg      = "#FFF0F0F0";
                    p.Accent        = "#FF0078D7";
                    p.GlowColor     = "#FF0078D7";
                    p.TitleGlow     = "#FF0078D7";
                    p.MessageBg     = "#FFE8F4FF";
                    p.MessageBorder = "#FF99CCF3";
                    p.MessageFg     = "#FF1A1A1A";
                    p.ButtonBg      = "#FFE1F0FB";
                    p.ButtonHover   = "#FFCCE4F7";
                    p.ButtonPressed = "#FF99C9EF";
                    break;

                default: // Normal
                    p.WindowBg      = "#CC0A0F1A";
                    p.Accent        = "#FF4ECDC4";
                    p.GlowColor     = "#FF00FFFF";
                    p.TitleGlow     = "#FF4ECDC4";
                    p.MessageBg     = "#1A4ECDC4";
                    p.MessageBorder = "#664ECDC4";
                    p.MessageFg     = "#FFCCE8E6";
                    p.ButtonBg      = "#1A4ECDC4";
                    p.ButtonHover   = "#334ECDC4";
                    p.ButtonPressed = "#554ECDC4";
                    break;
            }
            return p;
        }

        public DuneDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void ShowWithSound(string message, string title,
                                  AlertSound sound,
                                  DialogStyle style = DialogStyle.Normal,
                                  double volume     = 0.3)
        {
            ApplyStyle(style);

            MessageText.Text = message;
            TitleText.Text   = title;

            string tempWav = ExtractWavToTempFile(AlertSoundHelper.ToPackUri(sound));
            if (tempWav != null)
            {
                _player = new MediaPlayer();
                _player.Open(new Uri(tempWav, UriKind.Absolute));
                _player.Volume = volume;
                _player.MediaEnded += (s, e) =>
                {
                    _player.Position = TimeSpan.Zero;
                    _player.Play();
                };
                _player.Play();
            }

            ShowDialog();
        }

        public void ShowSilent(string message, string title,
                               DialogStyle style = DialogStyle.Normal)
        {
            ApplyStyle(style);
            MessageText.Text = message;
            TitleText.Text   = title;
            ShowDialog();
        }

        private void ApplyStyle(DialogStyle style)
        {
            StylePalette p = GetPalette(style);

            Color accentColor = (Color)ColorConverter.ConvertFromString(p.Accent);
            Color glowColor   = (Color)ColorConverter.ConvertFromString(p.GlowColor);
            Color titleGlow   = (Color)ColorConverter.ConvertFromString(p.TitleGlow);

            OuterBorder.Background  = new SolidColorBrush((Color)ColorConverter.ConvertFromString(p.WindowBg));
            OuterBorder.BorderBrush = new SolidColorBrush(accentColor);
            OuterBorder.Effect = new DropShadowEffect
            {
                BlurRadius  = 32,
                ShadowDepth = 0,
                Color       = glowColor,
                Opacity     = 0.30
            };

            TopBar.Fill    = new SolidColorBrush(accentColor);
            BottomBar.Fill = new SolidColorBrush(accentColor);

            TitleText.Foreground = new SolidColorBrush(accentColor);
            TitleText.Effect = new DropShadowEffect
            {
                BlurRadius  = 10,
                ShadowDepth = 0,
                Color       = titleGlow,
                Opacity     = 0.85
            };

            MessageBorder.Background  = new SolidColorBrush((Color)ColorConverter.ConvertFromString(p.MessageBg));
            MessageBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(p.MessageBorder));
            MessageText.Foreground    = new SolidColorBrush((Color)ColorConverter.ConvertFromString(p.MessageFg));

            btnOk.Background  = new SolidColorBrush((Color)ColorConverter.ConvertFromString(p.ButtonBg));
            btnOk.Foreground  = new SolidColorBrush(accentColor);
            btnOk.BorderBrush = new SolidColorBrush(accentColor);

            RebuildButtonTemplate(p);
        }

        private void RebuildButtonTemplate(StylePalette p)
        {
            Color bgNormal  = (Color)ColorConverter.ConvertFromString(p.ButtonBg);
            Color bgHover   = (Color)ColorConverter.ConvertFromString(p.ButtonHover);
            Color bgPressed = (Color)ColorConverter.ConvertFromString(p.ButtonPressed);
            Color accent    = (Color)ColorConverter.ConvertFromString(p.Accent);

            var template = new ControlTemplate(typeof(System.Windows.Controls.Button));

            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.Name = "bd";
            borderFactory.SetValue(Border.BackgroundProperty,      new SolidColorBrush(bgNormal));
            borderFactory.SetValue(Border.BorderBrushProperty,     new SolidColorBrush(accent));
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            borderFactory.SetValue(Border.CornerRadiusProperty,    new CornerRadius(2));

            var cpFactory = new FrameworkElementFactory(
                typeof(System.Windows.Controls.ContentPresenter));
            cpFactory.SetValue(
                System.Windows.Controls.ContentPresenter.HorizontalAlignmentProperty,
                HorizontalAlignment.Center);
            cpFactory.SetValue(
                System.Windows.Controls.ContentPresenter.VerticalAlignmentProperty,
                VerticalAlignment.Center);
            borderFactory.AppendChild(cpFactory);
            template.VisualTree = borderFactory;

            var hoverTrigger = new Trigger
            {
                Property = System.Windows.Controls.Button.IsMouseOverProperty,
                Value    = true
            };
            hoverTrigger.Setters.Add(
                new Setter(Border.BackgroundProperty, new SolidColorBrush(bgHover), "bd"));

            var pressedTrigger = new Trigger
            {
                Property = System.Windows.Controls.Button.IsPressedProperty,
                Value    = true
            };
            pressedTrigger.Setters.Add(
                new Setter(Border.BackgroundProperty, new SolidColorBrush(bgPressed), "bd"));

            template.Triggers.Add(hoverTrigger);
            template.Triggers.Add(pressedTrigger);

            btnOk.Template = template;
        }

        private string ExtractWavToTempFile(string packUri)
        {
            Uri uri = new Uri(packUri, UriKind.Absolute);
            var streamInfo = Application.GetResourceStream(uri);
            if (streamInfo == null) return null;

            string tempPath = Path.GetTempFileName() + ".wav";
            using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                streamInfo.Stream.CopyTo(fs);
            }
            return tempPath;
        }

        public void OK_Click(object sender, RoutedEventArgs e)
        {
            _player?.Stop();
            this.DialogResult = true;
            this.Close();
        }
    }
}
