using System.Collections.Generic;
using System.Linq;
using RoR2;

namespace Sandbox.Command {
    public class SetShared : ICommand {

        public static bool itemsShared = false;

        public string key() {
            return "SetShared";
        }

        public void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) {
            string[] argStrings = arguments.ToArray();
            if (argStrings.Length >= 1) {
                conVars.Add("Share", bool.TryParse(argStrings[0], out bool shared) ? shared : !itemsShared);
            } else {
                conVars.Add("Share", !itemsShared);
            }
        }

        public void invoke(Dictionary<string, object> conVars) {
            itemsShared = conVars["Share"] as bool? ?? !itemsShared;
            Chat.AddMessage(itemsShared ? "Item pickups are now shared" : "Item pickups are no longer shared");
        }
    }
}