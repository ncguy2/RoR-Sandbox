using Sandbox.Command;
using UnityEngine.Networking;

namespace Sandbox.Network {
    public class NetworkCommandListener {
        private const short CommandPacketType = 0x4000;

        public void StartServer() {
            SandboxMain.Log("NetworkCommandListener starting");
            NetworkServer.RegisterHandler(CommandPacketType, HandlePacket);
            SandboxMain.Log("NetworkCommandListener started");
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void StopServer() {
            SandboxMain.Log("NetworkCommandListener stopping");
            NetworkServer.UnregisterHandler(CommandPacketType);
            SandboxMain.Log("NetworkCommandListener stopped");
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void Replicate(CommandPacket pkt) {
            NetworkClient client = NetworkManager.singleton.client;
            if (client == null) {
                SandboxMain.Log("NetworkManager.singleton.client is null", true);
                return;
            }

            if (!client.Send(CommandPacketType, pkt)) {
                SandboxMain.Log($"Failed to replicate command {pkt.CmdKey}", true);
            }
        }

        private void HandlePacket(NetworkMessage msg) {
            if (msg.msgType != CommandPacketType) {
                return;
            }

            CommandPacket pkt = msg.ReadMessage<CommandPacket>();
            Invoke(pkt);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void Invoke(CommandPacket cmdPacket) {
            Command.Command cmd = SandboxMain.CmdHandler.GetCommand(cmdPacket.CmdKey);
            cmd?.InvokeServer(cmdPacket.Contents);
        }
    }
}