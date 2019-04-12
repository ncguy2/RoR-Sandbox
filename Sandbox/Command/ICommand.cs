using System.Collections.Generic;
using UnityEngine.Networking;

namespace Sandbox.Command {
    public abstract class Command {
        public string Key => key();

        public void invoke(IEnumerable<string> arguments) {
            Dictionary<string, object> conVars = new Dictionary<string, object>();
            parseArguments(arguments, ref conVars);
            CommandPacket pkt = new CommandPacket();
            prepare(conVars, ref pkt.contents);

            // TODO If invoked on client, send to server

            if (NetworkServer.active) {
                SandboxMain.Log($"Invoking command \"{Key}\" on server");
                invoke_server(pkt.contents);
            } else {
                SandboxMain.NetHandler.Replicate(pkt);
            }
        }

        public abstract string key();

        protected abstract void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars);

        [Client]
        public abstract PreparedResult prepare(Dictionary<string, object> conVars,
                                               ref Dictionary<string, string> packetContents);

        [Server]
        public abstract void invoke_server(Dictionary<string, string> contents);
    }

    public enum PreparedResult {
        Replicate,
        Stop
    }

    public class CommandPacket : MessageBase {
        public string cmdKey;
        public Dictionary<string, string> contents;

        public CommandPacket() {
            contents = new Dictionary<string, string>();
        }

        public override void Deserialize(NetworkReader reader) {
            base.Deserialize(reader);
            cmdKey = reader.ReadString();
            int size = reader.ReadInt32();

            for (int i = 0; i < size; i++) {
                string key = reader.ReadString();
                string val = reader.ReadString();
                contents.Add(key, val);
            }
        }

        public override void Serialize(NetworkWriter writer) {
            base.Serialize(writer);
            writer.Write(cmdKey);
            writer.Write(contents.Count);
            foreach (KeyValuePair<string, string> pair in contents) {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }
    }
}