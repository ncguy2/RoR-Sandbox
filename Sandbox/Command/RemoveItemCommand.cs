using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using Sandbox.Command.Attribute;
using Sandbox.Utilities;

namespace Sandbox.Command {
    [SandboxCommand(true)]
    public class RemoveItemCommand : Command {
        public override string key() {
            return "removeItem";
        }

        protected override void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) {
            string[] argStrings = arguments.ToArray();
            string displayName = UnityUtils.GetLocalNetworkUserName();
            string id = argStrings[0];


            int amt = 1;
            if (argStrings.Length >= 2) {
                if (!int.TryParse(argStrings[1], out amt)) {
                    amt = 1;
                }
            }

            conVars.Add("Target", displayName);
            conVars.Add("Id", id);
            conVars.Add("Amount", amt);
        }

        public override PreparedResult prepare(Dictionary<string, object> conVars,
                                               ref Dictionary<string, string> packetContents) {
            preparePassthrough(conVars, ref packetContents);
            return PreparedResult.Replicate;
        }

        public override void invoke_server(Dictionary<string, string> contents) {
            string username = contents["Target"];
            string idStr = contents["Id"];
            string amtStr = contents["Amount"];

            if (!int.TryParse(amtStr, out int amt)) {
                amt = 1;
            }

            NetworkUser user = UnityUtils.GetNetworkUserWithName(username);
            if (user == null) {
                SandboxMain.Log($"Unable to find user with name {username}");
                return;
            }

            if (Enum.TryParse(idStr, true, out ItemIndex item)) {
                user.GetCurrentBody().inventory.RemoveItem(item, amt);
            }
        }
    }
}