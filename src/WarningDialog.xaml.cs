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
        public WarningDialog(string message, string title)
        {
            InitializeComponent();
            Message = message;
            Title = title;
            DataContext = this;
        }

        public void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}