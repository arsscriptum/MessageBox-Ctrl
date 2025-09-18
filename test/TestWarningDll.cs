using FastDownloader;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;      // Important
class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        try
        {
            if (System.Windows.Application.Current == null)
                new System.Windows.Application();

            var d = new MessageBox.WarningDialog();
            //d.ShowWarningDialog("The Current Temperature Has Reached 100!","WARNING");
            //d.ShowWarningDialogWithMaxSound("The Current Temperature Has Reached 100!", "WARNING");
            var temp = args[0];
            d.ShowWarningDialogWithSound($"The Current Temperature Has Reached {temp}!", "WARNING");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("[Exception] " + ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
            Environment.Exit(2);
        }
    }
}
