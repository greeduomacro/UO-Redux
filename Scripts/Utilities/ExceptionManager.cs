using System;

namespace Server.Utilities {
    public class ExceptionManager {
        public static void Configure() {
            Console.WriteLine("Exception Manager: Enabled to log exceptions to the system event log.");
        }

        public static void LogException(string sourceFile, Exception e) {
            try {
                string eventString = "#############\n";
                eventString += String.Format("[{0}]: Exception caught in {1}:\n\n", sourceFile, e.TargetSite);
                eventString += e.ToString();
                eventString += "\n#############";

                EventLog.Warning(0, eventString);
            } catch { }
        }
    }
}