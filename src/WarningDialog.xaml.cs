using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace MessageBox
{
    public partial class WarningDialog : Window
    {
        public string Message { get; set; }
        private string intruderSoundResource = "pack://application:,,,/MessageBox;component/res/sounds/intruder.wav";
        MediaPlayer _internal_player = null;
        System.Media.SoundPlayer _internal_sound_player = null;
        public WarningDialog()
        {
            InitializeComponent();
            DataContext = this;
        }
        public void ShowWarningDialogWithSound(string message, string title,double volume = 0.3)
        {

            MessageText.Text = message;
            TitleText.Text = title;
           
            Uri uri = new Uri(intruderSoundResource, UriKind.Absolute);
            var streamInfo = Application.GetResourceStream(uri);
            if (streamInfo != null)
            {
                // In your dialog class:
                string tempWav = ExtractWavToTempFile("pack://application:,,,/MessageBox;component/res/sounds/intruder.wav");
                if (tempWav != null)
                {
                    _internal_player = new MediaPlayer();
                    _internal_player.Open(new Uri(tempWav, UriKind.Absolute));
                    _internal_player.Volume = volume;
                    _internal_player.MediaEnded += (s, e) => {
                        _internal_player.Position = TimeSpan.Zero;
                        _internal_player.Play();
                    };
                    _internal_player.Play();
                }
            }
            ShowDialog();
        }
        public void ShowWarningDialogWithMaxSound(string message, string title)
        {
            InitializeComponent();
            MessageText.Text = message;
            TitleText.Text = title;
            DataContext = this;

            Uri uri = new Uri(intruderSoundResource, UriKind.Absolute);
            var streamInfo = Application.GetResourceStream(uri);
            if (streamInfo != null)
            {
                _internal_sound_player = new System.Media.SoundPlayer(streamInfo.Stream);
                _internal_sound_player.PlayLooping();
            }
            ShowDialog();
        }
        public void ShowWarningDialog(string message, string title)
        {
           
            MessageText.Text = message;
            TitleText.Text = title;
            ShowDialog();

        }
        // Helper to extract resource to temp file and return path
        string ExtractWavToTempFile(string resourcePackUri)
        {
            Uri uri = new Uri(resourcePackUri, UriKind.Absolute);
            var streamInfo = Application.GetResourceStream(uri);
            if (streamInfo == null) return null;

            string tempPath = System.IO.Path.GetTempFileName() + ".wav";
            using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                streamInfo.Stream.CopyTo(fs);
            }
            return tempPath;
        }
      
        public void OK_Click(object sender, RoutedEventArgs e)
        {
            _internal_sound_player?.Stop();
            _internal_player?.Stop();
            this.DialogResult = true;
            this.Close();
        }
    }
}
