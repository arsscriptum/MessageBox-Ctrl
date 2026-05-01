using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace MessageBox
{
    public static class ShowWarningDialog
    {
        public static Action<bool> OnCompleted  { get; set; }
        public static Action<bool> OnCancelled  { get; set; }
        public static Action<int>  OnProgress   { get; set; }

        public static void ShowDialog(string message, string title)
        {
            var app = System.Windows.Application.Current ?? new System.Windows.Application();
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            var win = new MessageBox.WarningDialog();
            win.ShowWarningDialogWithSound(message, title);
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            app.MainWindow = win;
            app.Run(win);
        }

        public static void ShowDuneDialog(string message, string title, AlertSound sound, DialogStyle style = DialogStyle.Normal, double volume = 0.3)
        {
            var app = System.Windows.Application.Current ?? new System.Windows.Application();
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            var win = new MessageBox.DuneDialog();
            win.ShowWithSound(message, title, sound, style, volume);
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            app.MainWindow = win;
            app.Run(win);
        }
    }
}
