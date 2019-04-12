using System.Collections.Generic;
using System.Linq;
using Sandbox.Utilities;

namespace Sandbox.Command {
    public class GiveItem : Command {
        public override string key() {
            return "giveItem";
        }

        protected override void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) {
            string[] argStrings = arguments.ToArray();
            string displayName = UnityUtils.GetLocalPlayerController().GetDisplayName();
            string id = argStrings[0];
        }

        public override PreparedResult prepare(Dictionary<string, object> conVars,
                                               ref Dictionary<string, string> packetContents) {
            throw new System.NotImplementedException();
        }

        public override void invoke_server(Dictionary<string, string> contents) {
            throw new System.NotImplementedException();
        }
    }
}