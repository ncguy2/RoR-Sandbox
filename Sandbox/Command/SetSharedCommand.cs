using System.Collections.Generic;
using System.Linq;
using RoR2;
using Sandbox.Command.Attribute;
using UnityEngine.Networking;

namespace Sandbox.Command {
    [SandboxCommand]
    public class SetSharedCommand : Command {
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

        protected override PreparedResult prepare(Dictionary<string, object> conVars,
                                                  ref Dictionary<string, string> packetContents) {
            bool.TryParse(conVars["Share"] as string, out bool result);
            packetContents.Add("Share", result.ToString());
            return PreparedResult.Replicate;
        }

        [Server]
        public override void invoke_server(Dictionary<string, string> contents) {
            if (!bool.TryParse(contents["Share"], out bool result)) {
                result = !itemsShared;
            }

            itemsShared = result;
            Chat.AddMessage(itemsShared ? "Item pickups are now shared" : "Item pickups are no longer shared");
        }
    }
}