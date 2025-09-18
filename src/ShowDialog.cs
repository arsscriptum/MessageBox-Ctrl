using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace MessageBox
{

    public static class ShowWarningDialog
    {
        public static Action<bool>? OnCompleted { get; set; }
        public static Action<bool>? OnCancelled { get; set; }
        public static Action<int>? OnProgress { get; set; }

        public static void ShowDialog(string message, string title)
        {
            var app = System.Windows.Application.Current ?? new System.Windows.Application();
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;
            var win = new MessageBox.WarningDialog(message, title);
         
            app.ShutdownMode = ShutdownMode.OnMainWindowClose;

            // This ensures closing win will shutdown app
            app.MainWindow = win;
            // Wire up the events to static callbacks
      

            app.Run(win);
        }
    }
}
