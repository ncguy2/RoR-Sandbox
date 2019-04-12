using System.Collections.Generic;

namespace Sandbox.Command {
    public class HelpCommand : Command {
        public override string key() {
            return "help";
        }

        protected override void
            parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) { }

        public override PreparedResult prepare(Dictionary<string, object> conVars,
                                               ref Dictionary<string, string> packetContents) {
            foreach (Command cmd in SandboxMain.CmdHandler.getCommands()) {
                SandboxMain.toHud(" -- " + cmd.key());
            }

            return PreparedResult.Stop;
        }

        public override void invoke_server(Dictionary<string, string> contents) { }
    }
}