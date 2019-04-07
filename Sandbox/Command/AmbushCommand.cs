using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using PingFixer;
using RoR2;
using Sandbox.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Sandbox.Command {
    public class AmbushCommand : ICommand {
        public string key() {
            return "ambush";
        }

        public void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) {
            string[] argStrings = arguments.ToArray();
            conVars.Add("Player name", argStrings[0]);
        }

        public void invoke(Dictionary<string, object> conVars) {
            CombatDirector director = getDirector();
            if (director == null) {
                Chat.AddMessage("No Combat Director found, cannot force ambush");
                return;
            }

            string victimName = conVars["Player name"] as string;

            if (victimName == null) {
                Chat.AddMessage("No player name provided");
                return;
            }

            PlayerCharacterMasterController victim = null;

            foreach (PlayerCharacterMasterController playerController in PlayerCharacterMasterController.instances) {
                if (victimName.Equals(playerController.networkUser.userName, StringComparison.OrdinalIgnoreCase)) {
                    victim = playerController;
                    break;
                }
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