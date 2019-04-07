using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RoR2;
using Sandbox.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sandbox.Command {
    public class SpawnItem : ICommand {
        public string key() {
            return "spawnItem";
        }

        public void parseArguments(IEnumerable<string> arguments, ref Dictionary<string, object> conVars) {
            string[] argStrings = arguments.ToArray();
            conVars.Add("ItemId", argStrings[0]);

            if (argStrings.Length >= 2) {
                conVars.Add("Amount", int.TryParse(argStrings[1], out int amt) ? amt : 1);
            } else {
                conVars.Add("Amount", 1);
            }

        }

        public void invoke(Dictionary<string, object> conVars) {
            Camera camera = Camera.main;

            if (camera == null) {
                Chat.AddMessage("Unable to find camera reference.");
                return;
            }

            Transform transform = camera.transform;
            Ray ray = new Ray {
                origin = transform.position,
                direction = transform.forward
            };

            if (!Physics.Raycast(ray, out RaycastHit hit)) {
                Chat.AddMessage("Unable to find spawn location.");
                return;
            }

            string itemId = conVars["ItemId"].ToString();
            if (!Enum.TryParse(itemId, out ItemIndex idx)) {
                Chat.AddMessage("Unable to find Item ID: " + itemId);

                List<Tuple<string,int>> tuples = Enum.GetNames(typeof(EquipmentIndex))
                                                     .Select(x => new Tuple<string, int>(x, DamerauLevenshtein.CalculateDistance(x, itemId)))
                                                     .Where(x => x.Item2 < 5)
                                                     .ToList();
                tuples.Sort((a, b) => a.Item2.CompareTo(b.Item2));

                if (tuples.Count <= 0) {
                    return;
                }

                Chat.AddMessage("Did you mean:");
                for (int i = 0; i < tuples.Count && i < 3; i++) {
                    Chat.AddMessage("    " + tuples[i].Item1);
                }
                return;
            }

            string amtStr = conVars["Amount"].ToString();
            if (!int.TryParse(amtStr, out int amt)) {
                amt = 1;
            }

            for (int i = 0; i < amt; i++) {
                Vector3 randomDirection = new Vector3(Random.value, Random.value, Random.value);
                PickupDropletController.CreatePickupDroplet(new PickupIndex(idx),
                                                            hit.point + Vector3.up * 1.5f,
                                                            Vector3.up * 20f + randomDirection * 5f);
            }
        }
    }
}