using System.Collections.Generic;
using System.Linq;
using RoR2;
using Sandbox.Command.Attribute;
using Sandbox.Utilities;
using UnityEngine;

namespace Sandbox.Command {
    [SandboxCommand(true)]
    public class SpawnEnemyCommand : Command {
        public override string key() {
            return "spawnEnemy";
        }

        protected override void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) {
            string[] argStrings = arguments.ToArray();
            conVars.Add("SpawnCard", argStrings[0]);
        }


        public override PreparedResult prepare(Dictionary<string, object> conVars,
                                               ref Dictionary<string, string> packetContents) {
            if (!UnityUtils.RayTrace(out RaycastHit hit)) {
                return PreparedResult.Stop;
            }

            packetContents.Add("SpawnCard", conVars["SpawnCard"] as string);
            DataUtils.WriteVector3ToDictionary("Location", hit.point, ref packetContents);

            return PreparedResult.Replicate;
        }

        public override void invoke_server(Dictionary<string, string> contents) {
            string spawnCard = contents["SpawnCard"];
            if (!DataUtils.ReadVector3FromDictionary("Location", ref contents, out Vector3 hit)) {
                SandboxMain.Log(
                    $"Invalid location \"[{contents["Location.x"]}, {contents["Location.y"]}, {contents["Location.z"]}] Replicated",
                    true);
                return;
            }

            SpawnCard card = Resources.Load<SpawnCard>("SpawnCards/" + spawnCard);
            if (card == null) {
                SandboxMain.Log($"Unable to load a spawnCard called \"{spawnCard}\"", true);
                return;
            }

            card.DoSpawn(hit, Quaternion.identity);
        }
    }
}