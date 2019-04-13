using System.Collections.Generic;
using UnityEngine.Networking;

namespace Sandbox.Command {
    public abstract class Command {
        public void Invoke(IEnumerable<string> arguments) {
            Dictionary<string, object> conVars = new Dictionary<string, object>();
            ParseArguments(arguments, ref conVars);
            CommandPacket pkt = new CommandPacket();
            if (Prepare(conVars, ref pkt.Contents) == PreparedResult.Stop) {
                return;
            }

            if (NetworkServer.active) {
                SandboxMain.Log($"Invoking command \"{Key()}\" on server");
                InvokeServer(pkt.Contents);
            } else {
                SandboxMain.NetHandler.Replicate(pkt);
            }
        }

        public abstract string Key();

        protected abstract void ParseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars);

        [Client]
        protected abstract PreparedResult Prepare(Dictionary<string, object> conVars,
                                                  ref Dictionary<string, string> packetContents);

        [Server]
        public abstract void InvokeServer(Dictionary<string, string> contents);

        protected static void PreparePassthrough(Dictionary<string, object> conVars,
                                                 ref Dictionary<string, string> packetContents) {
            foreach (KeyValuePair<string, object> pair in conVars) {
                packetContents.Add(pair.Key, pair.Value.ToString());
            }
        }
    }

    public enum PreparedResult {
        Replicate,
        Stop
    }

    public class CommandPacket : MessageBase {
        public string CmdKey;
        public Dictionary<string, string> Contents;

        public CommandPacket() {
            Contents = new Dictionary<string, string>();
        }

        public override void Deserialize(NetworkReader reader) {
            base.Deserialize(reader);
            CmdKey = reader.ReadString();
            int size = reader.ReadInt32();

            for (int i = 0; i < size; i++) {
                string key = reader.ReadString();
                string val = reader.ReadString();
                Contents.Add(key, val);
            }
        }

        public override void Serialize(NetworkWriter writer) {
            base.Serialize(writer);
            writer.Write(CmdKey);
            writer.Write(Contents.Count);
            foreach (KeyValuePair<string, string> pair in Contents) {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }
    }
}