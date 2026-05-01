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

            string             message  = null;
            string             title    = null;
            string             alertId  = null;
            string             styleId  = null;
            bool               doList   = false;
            bool               doHelp   = false;
            bool               doStyles = false;

            int i = 0;
            while (i < args.Length)
            {
                string arg = args[i];

                if (arg == "-l" || arg == "--list")
                {
                    doList = true;
                    i++;
                }
                else if (arg == "-h" || arg == "--help")
                {
                    doHelp = true;
                    i++;
                }
                else if (arg == "-x" || arg == "--stylelist")
                {
                    doStyles = true;
                    i++;
                }
                else if (arg == "-m" || arg == "--message")
                {
                    if (i + 1 >= args.Length) Die("--message requires a value.");
                    message = args[++i];
                    i++;
                }
                else if (arg == "-t" || arg == "--title")
                {
                    if (i + 1 >= args.Length) Die("--title requires a value.");
                    title = args[++i];
                    i++;
                }
                else if (arg == "-a" || arg == "--alert")
                {
                    if (i + 1 >= args.Length) Die("--alert requires a value.");
                    alertId = args[++i];
                    i++;
                }
                else if (arg == "-s" || arg == "--style")
                {
                    if (i + 1 >= args.Length) Die("--style requires a value.");
                    styleId = args[++i];
                    i++;
                }
                else
                {
                    Die("Unknown argument: " + arg);
                }
            }

            if (doHelp)   { ShowHelp();      return; }
            if (doList)   { ShowSoundList(); return; }
            if (doStyles) { ShowStyleList(); return; }

            MessageBox.DialogStyle style = MessageBox.DialogStyle.Normal;
            if (styleId != null && !TryParseStyle(styleId, out style))
                Die("Unknown style ID: " + styleId + "\nRun with -x to list valid styles.");

            if (alertId != null)
            {
                MessageBox.AlertSound sound;
                if (!TryParseAlertSound(alertId, out sound))
                    Die("Unknown alert ID: " + alertId + "\nRun with -l to list valid IDs.");

                string msg = message ?? "A System Error Occurred.";
                string ttl = title   ?? "Error";
                var d = new MessageBox.DuneDialog();
                d.ShowWithSound(msg, ttl, sound, style);
                return;
            }

            // style or message/title only, no sound
            {
                string msg = message ?? "A System Error Occurred.";
                string ttl = title   ?? "Error";

                if (styleId != null)
                {
                    var d = new MessageBox.DuneDialog();
                    d.ShowSilent(msg, ttl, style);
                }
                else
                {
                    var d = new MessageBox.WarningDialog();
                    d.ShowWarningDialog(msg, ttl);
                }
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

    static void ShowHelp()
    {
        string text =
            "Usage: TestWarningDll [options]\r\n\r\n" +
            "  (no args)              Default silent WarningDialog\r\n" +
            "  -m, --message  <text>  Message body text\r\n" +
            "  -t, --title    <text>  Title bar text\r\n" +
            "  -a, --alert    <id>    Alert sound ID or name  (implies DuneDialog)\r\n" +
            "  -s, --style    <id>    Visual style ID or name (implies DuneDialog)\r\n" +
            "  -l, --list             List available alert sound IDs\r\n" +
            "  -x, --stylelist        List available style IDs\r\n" +
            "  -h, --help             Show this help\r\n\r\n" +
            "Examples:\r\n" +
            "  TestWarningDll -m \"Drive failure\" -t \"CRITICAL\" -a 12 -s 1\r\n" +
            "  TestWarningDll -s Warning -m \"Threshold exceeded\"";

        System.Windows.MessageBox.Show(
            text, "Help",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    static void ShowSoundList()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Available alert sound IDs:\r\n");
        var values = (MessageBox.AlertSound[])Enum.GetValues(typeof(MessageBox.AlertSound));
        foreach (var v in values)
            sb.AppendLine("  " + (int)v + "\t" + v.ToString());

        System.Windows.MessageBox.Show(
            sb.ToString(), "Alert Sounds",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    static void ShowStyleList()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Available style IDs:\r\n");
        var values = (MessageBox.DialogStyle[])Enum.GetValues(typeof(MessageBox.DialogStyle));
        foreach (var v in values)
            sb.AppendLine("  " + (int)v + "\t" + MessageBox.DialogStyleHelper.Description(v));

        System.Windows.MessageBox.Show(
            sb.ToString(), "Dialog Styles",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    static bool TryParseAlertSound(string input, out MessageBox.AlertSound result)
    {
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

    static bool TryParseStyle(string input, out MessageBox.DialogStyle result)
    {
        int numeric;
        if (int.TryParse(input, out numeric) &&
            Enum.IsDefined(typeof(MessageBox.DialogStyle), numeric))
        {
            result = (MessageBox.DialogStyle)numeric;
            return true;
        }
        try
        {
            result = (MessageBox.DialogStyle)Enum.Parse(
                typeof(MessageBox.DialogStyle), input, ignoreCase: true);
            return true;
        }
        catch
        {
            result = MessageBox.DialogStyle.Normal;
            return false;
        }
    }

    static void Die(string msg)
    {
        System.Windows.MessageBox.Show(
            msg, "Argument Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        Environment.Exit(1);
    }
}
