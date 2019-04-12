using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine.Networking;

namespace Sandbox.Command {
    public class SetShared : Command {
        public static bool itemsShared = false;

        public override string key() {
            return "SetShared";
        }

        protected override void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) {
            string[] argStrings = arguments.ToArray();
            if (argStrings.Length >= 1) {
                conVars.Add("Share", bool.TryParse(argStrings[0], out bool shared) ? shared : !itemsShared);
            } else {
                conVars.Add("Share", !itemsShared);
            }
        }

        public override PreparedResult prepare(Dictionary<string, object> conVars,
                                               ref Dictionary<string, string> packetContents) {
            bool.TryParse(conVars["Share"] as string, out bool result);
            packetContents.Add("Share", result.ToString());
            return PreparedResult.Replicate;
        }

        [Server]
        public override void invoke_server(Dictionary<string, string> contents) {
            bool.TryParse(contents["Share"], out itemsShared);
            Chat.AddMessage(itemsShared ? "Item pickups are now shared" : "Item pickups are no longer shared");
        }
    }
}