using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        System.Media.SoundPlayer _internal_player = null;

        public WarningDialog(string message, string title)
        {
            InitializeComponent();
            MessageText.Text = message;
            TitleText.Text = title;
            DataContext = this;

            Uri uri = new Uri(intruderSoundResource, UriKind.Absolute);
            var streamInfo = Application.GetResourceStream(uri);
            if (streamInfo != null)
            {
                _internal_player = new System.Media.SoundPlayer(streamInfo.Stream);
                _internal_player.PlayLooping();
            }
        }

        public void OK_Click(object sender, RoutedEventArgs e)
        {
            if (_internal_player != null)
            {
                _internal_player.Stop();
            }
            this.DialogResult = true;
            this.Close();
        }
    }
}
