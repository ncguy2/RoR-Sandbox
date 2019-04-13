using System.Collections.Generic;
using Sandbox.Command.Attribute;

namespace Sandbox.Command {
    [SandboxCommand]
    public class HelpCommand : Command {
        public override string Key() {
            return "help";
        }

        protected override void
            ParseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) { }

        protected override PreparedResult Prepare(Dictionary<string, object> conVars,
                                                  ref Dictionary<string, string> packetContents) {
            foreach (Command cmd in SandboxMain.CmdHandler.GetCommands()) {
                SandboxMain.ToHud(" -- " + cmd.Key());
            }

            return PreparedResult.Stop;
        }

        public override void InvokeServer(Dictionary<string, string> contents) { }
    }
}