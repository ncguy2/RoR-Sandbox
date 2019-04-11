using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PingFixer;
using RoR2;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Sandbox.Command {
    public class AmbushCommand : ICommand {
        public override string key() {
            return "ambush";
        }

        public override void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) {
            string[] argStrings = arguments.ToArray();
            conVars.Add("Player name", argStrings[0]);
        }

        public override PreparedResult prepare(Dictionary<string, object> conVars,
                                               ref Dictionary<string, string> packetContents) {
            string victimName = conVars["Player name"] as string;
            packetContents.Add("Player name", victimName);
            return string.IsNullOrEmpty(victimName) ? PreparedResult.Stop : PreparedResult.Replicate;
        }

        [Server]
        public override void invoke_server(Dictionary<string, string> contents) {
            CombatDirector director = getDirector();
            if (director == null) {
                Chat.AddMessage("No Combat Director found, cannot force ambush");
                return;
            }

            if (!(contents["Player name"] is string victimName)) {
                Chat.AddMessage("No player name provided");
                return;
            }

            PlayerCharacterMasterController victim = null;

            foreach (PlayerCharacterMasterController playerController in PlayerCharacterMasterController.instances) {
                if (!victimName.Equals(playerController.networkUser.userName, StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                victim = playerController;
                break;
            }

            if (victim == null) {
                Chat.AddMessage("No player with name: " + victimName + " found");
                return;
            }

            director.InvokePrivateMethod("GenerateAmbush", victim.transform.position);
        }

        [CanBeNull]
        private CombatDirector getDirector() {
            return Object.FindObjectsOfType(typeof(CombatDirector)).First() as CombatDirector;
        }
    }
}