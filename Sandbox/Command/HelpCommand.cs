using System.Collections.Generic;
using RoR2;

namespace Sandbox.Command {
    public class HelpCommand : ICommand {
        public string key() {
            return "help";
        }

        public void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) {
        }

        public void invoke(Dictionary<string, object> conVars) {
            foreach (ICommand cmd in SandboxMain.CmdHandler.getCommands()) {
                Chat.AddMessage(" -- " + cmd.key());
            }
        }
    }
}