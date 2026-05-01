using System;

namespace MessageBox
{
    public enum AlertSound
    {
        TwentyEightFiftyFour,
        AirgapOff,
        Alert1,
        Alert2,
        AlertVoice,
        AnomalyDetected,
        CivilEmergency,
        Digital,
        Intruder,
        LifeSupportSysFailure,
        ManualOverrideInit,
        MechanicalSiren,
        NukeAlert,
        Ringing,
        SystemError,
        TempAlert,
        TornadoSiren,
        TornadoSirenLong,
        VideoGameAlarm
    }

    public static class AlertSoundHelper
    {
        public static string ToPackUri(AlertSound sound)
        {
            string file;
            switch (sound)
            {
                case AlertSound.TwentyEightFiftyFour:   file = "28_54.wav";               break;
                case AlertSound.AirgapOff:              file = "AirgapOff.wav";            break;
                case AlertSound.Alert1:                 file = "alert1.wav";               break;
                case AlertSound.Alert2:                 file = "alert2.wav";               break;
                case AlertSound.AlertVoice:             file = "alertvoice.wav";           break;
                case AlertSound.AnomalyDetected:        file = "AnomalyDetected.wav";      break;
                case AlertSound.CivilEmergency:         file = "CivilEmergency.wav";       break;
                case AlertSound.Digital:                file = "Digital.wav";              break;
                case AlertSound.Intruder:               file = "intruder.wav";             break;
                case AlertSound.LifeSupportSysFailure:  file = "LifeSupportSysFailure.wav";break;
                case AlertSound.ManualOverrideInit:     file = "ManualOverrideInit.wav";   break;
                case AlertSound.MechanicalSiren:        file = "MechanicalSiren.wav";      break;
                case AlertSound.NukeAlert:              file = "NukeAlert.wav";            break;
                case AlertSound.Ringing:                file = "Ringing.wav";              break;
                case AlertSound.SystemError:            file = "SystemError.wav";          break;
                case AlertSound.TempAlert:              file = "TempAlert.wav";            break;
                case AlertSound.TornadoSiren:           file = "tornadoSiren.wav";         break;
                case AlertSound.TornadoSirenLong:       file = "tornadoSirenLong.wav";     break;
                case AlertSound.VideoGameAlarm:         file = "VideoGameAlarm.wav";       break;
                default:                                file = "LifeSupportSysFailure.wav";break;
            }
            return "pack://application:,,,/MessageBox;component/res/sounds/" + file;
        }
    }
}
