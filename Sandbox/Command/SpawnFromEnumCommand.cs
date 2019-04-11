using System.Collections.Generic;
using System.Linq;
using RoR2;
using Sandbox.Utilities;
using UnityEngine;

namespace Sandbox.Command {
    public abstract class SpawnFromEnumCommand<T> : ICommand where T : struct {
        public override void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) {
            string[] argStrings = arguments.ToArray();
            conVars.Add("Id", argStrings[0]);

            if (argStrings.Length >= 2) {
                conVars.Add("Amount", int.TryParse(argStrings[1], out int amt) ? amt : 1);
            } else {
                conVars.Add("Amount", 1);
            }
        }

        public override PreparedResult prepare(Dictionary<string, object> conVars,
                                               ref Dictionary<string, string> packetContents) {
            if (!UnityUtils.RayTrace(out RaycastHit hit)) {
                return PreparedResult.Stop;
            }

            string id = conVars["Id"].ToString();
            string[] nearbyNames = DataUtils.SelectEnum(id, out T idx);

            if (nearbyNames != null) {
                Chat.AddMessage("Unable to find  ID: " + id);

                // ReSharper disable once InvertIf
                if (nearbyNames.Length > 0) {
                    Chat.AddMessage("Did you mean:");
                    foreach (string t in nearbyNames) {
                        Chat.AddMessage("    " + t);
                    }
                }

                return PreparedResult.Stop;
            }

            string amtStr = conVars["Amount"].ToString();
            if (!int.TryParse(amtStr, out int amt)) {
                amt = 1;
            }

            packetContents.Add("Id", toInteger(idx).ToString());
            packetContents.Add("Amount", amt.ToString());
            DataUtils.WriteVector3ToDictionary("Location", hit.point, ref packetContents);

            return PreparedResult.Replicate;
        }

        public override void invoke_server(Dictionary<string, string> contents) {
            if (!int.TryParse(contents["Id"], out int idx)) {
                SandboxMain.Log($"Invalid  ID \"{contents["Id"]}\" Replicated");
                return;
            }

            if (!int.TryParse(contents["Amount"], out int amt)) {
                SandboxMain.Log($"Invalid amount \"{contents["Amount"]}\" Replicated, overriding to 1");
                amt = 1;
            }

            if (!DataUtils.ReadVector3FromDictionary("Location", ref contents, out Vector3 hit)) {
                SandboxMain.Log(
                    $"Invalid location \"[{contents["Location.x"]}, {contents["Location.y"]}, {contents["Location.z"]}] Replicated");
                return;
            }

            for (int i = 0; i < amt; i++) {
                Vector3 randomDirection = new Vector3(Random.value, Random.value, Random.value);
                PickupDropletController.CreatePickupDroplet(makeIndex(idx),
                                                            hit + Vector3.up * 1.5f,
                                                            Vector3.up * 20f + randomDirection * 5f);
            }
        }

        protected abstract int toInteger(T obj);
        protected abstract PickupIndex makeIndex(int idx);
    }
}