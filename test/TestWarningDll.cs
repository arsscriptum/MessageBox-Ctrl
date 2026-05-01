using System;
using System.Text;
using System.Windows;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        try
        {
            if (Application.Current == null)
                new Application();

            if (args.Length == 0)
            {
                ShowDefault();
                return;
            }

            string message = null;
            string title   = null;
            string alertId = null;
            bool   doList  = false;

            int i = 0;
            while (i < args.Length)
            {
                string arg = args[i];

                if (arg == "-l" || arg == "--list")
                {
                    doList = true;
                    i++;
                }
                else if (arg == "-m" || arg == "--message")
                {
                    if (i + 1 >= args.Length)
                        Die("--message requires a value.");
                    message = args[++i];
                    i++;
                }
                else if (arg == "-t" || arg == "--title")
                {
                    if (i + 1 >= args.Length)
                        Die("--title requires a value.");
                    title = args[++i];
                    i++;
                }
                else if (arg == "-a" || arg == "--alert")
                {
                    if (i + 1 >= args.Length)
                        Die("--alert requires a value.");
                    alertId = args[++i];
                    i++;
                }
                else
                {
                    Die("Unknown argument: " + arg);
                }
            }

            if (doList)
            {
                ShowList();
                return;
            }

            if (alertId != null)
            {
                MessageBox.AlertSound sound;
                if (!TryParseAlertSound(alertId, out sound))
                    Die("Unknown alert ID: " + alertId + "\nRun with -l to list valid IDs.");

                string msg = message ?? "A System Error Occurred.";
                string ttl = title   ?? "Error";

                var d = new MessageBox.DuneDialog();
                d.ShowWithSound(msg, ttl, sound);
                return;
            }

            // -m / -t only, no alert
            {
                string msg = message ?? "A System Error Occurred.";
                string ttl = title   ?? "Error";
                var d = new MessageBox.WarningDialog();
                d.ShowWarningDialog(msg, ttl);
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                ex.Message + "\r\n\r\n" + ex.StackTrace,
                "Unhandled Exception",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Environment.Exit(2);
        }
    }

    static void ShowDefault()
    {
        var d = new MessageBox.WarningDialog();
        d.ShowWarningDialog("A System Error Occurred.", "Error");
    }

    static void ShowList()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Available alert IDs:\r\n");

        var values = (MessageBox.AlertSound[])Enum.GetValues(typeof(MessageBox.AlertSound));
        foreach (var v in values)
            sb.AppendLine("  " + (int)v + "  " + v.ToString());

        System.Windows.MessageBox.Show(
            sb.ToString(),
            "Alert Sounds",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    static bool TryParseAlertSound(string input, out MessageBox.AlertSound result)
    {
        // accept numeric ID or name
        int numeric;
        if (int.TryParse(input, out numeric) &&
            Enum.IsDefined(typeof(MessageBox.AlertSound), numeric))
        {
            result = (MessageBox.AlertSound)numeric;
            return true;
        }

        try
        {
            result = (MessageBox.AlertSound)Enum.Parse(
                typeof(MessageBox.AlertSound), input, ignoreCase: true);
            return true;
        }
        catch
        {
            result = MessageBox.AlertSound.LifeSupportSysFailure;
            return false;
        }
    }

    static void Die(string msg)
    {
        System.Windows.MessageBox.Show(
            msg,
            "Argument Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        Environment.Exit(1);
    }
}
