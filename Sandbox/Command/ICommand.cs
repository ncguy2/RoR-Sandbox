using System.Collections.Generic;

namespace Sandbox.Command {
    public interface ICommand {
        string key();
        void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars);
        void invoke(Dictionary<string, object> conVars);
    }
}