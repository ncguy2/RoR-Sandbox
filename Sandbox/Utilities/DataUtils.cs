using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Sandbox.Utilities {
    public static class DataUtils {
        public static string[] SelectEnum<T>(string query, out T result, int maxNearby = 3, bool caseSensitive = false,
                                             T defVal = default(T))
            where T : struct {
            if (Enum.TryParse(query, out result)) {
                result = defVal;
                return null;
            }

            List<Tuple<string, int>> tuples = Enum.GetNames(typeof(T))
                                                  .Select(x => new Tuple<string, int>(
                                                              x,
                                                              DamerauLevenshtein.CalculateDistance(
                                                                  x, query, caseSensitive)))
                                                  .Where(x => x.Item2 < 5)
                                                  .ToList();
            tuples.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            if (tuples.Count <= 0) {
                return null;
            }

            int amt = Math.Min(maxNearby, tuples.Count);

            string[] arr = new string[amt];

            for (int i = 0; i < amt; i++) {
                arr[i] = tuples[i].Item1;
            }

            return arr;
        }


        public static void WriteVector3ToDictionary(string key, Vector3 vec, ref Dictionary<string, string> dest) {
            dest.Add(key + ".x", vec.x.ToString(CultureInfo.InvariantCulture));
            dest.Add(key + ".y", vec.y.ToString(CultureInfo.InvariantCulture));
            dest.Add(key + ".z", vec.z.ToString(CultureInfo.InvariantCulture));
        }

        public static bool ReadVector3FromDictionary(string key, ref Dictionary<string, string> dest, out Vector3 vec) {
            vec = new Vector3();
            return float.TryParse(dest[key + ".x"], out vec.x) &&
                   float.TryParse(dest[key + ".y"], out vec.y) &&
                   float.TryParse(dest[key + ".z"], out vec.z);
        }
    }
}