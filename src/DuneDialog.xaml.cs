using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace MessageBox
{
    public partial class DuneDialog : Window
    {
        public string Message { get; set; }

        private MediaPlayer _player = null;

        public DuneDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void ShowWithSound(string message, string title, AlertSound sound, double volume = 0.3)
        {
            string packUri = AlertSoundHelper.ToPackUri(sound);
            string tempWav = ExtractWavToTempFile(packUri);

            MessageText.Text = message;
            TitleText.Text   = title;

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

        public void ShowSilent(string message, string title)
        {
            MessageText.Text = message;
            TitleText.Text   = title;
            ShowDialog();
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
