using System.Collections.Generic;
using UnityEngine;

namespace Sandbox.UI {
    public class HUDContainer : MonoBehaviour {
        private List<Line> lines;

        public HUDContainer() {
            lines = new List<Line>();
        }

        public void Add(string line) {
            lines.Add(new Line(5, line));
        }

        private void LateUpdate() {
            foreach (Line line in lines) {
                line.lifeRemaining -= Time.deltaTime;
            }

            lines.RemoveAll(isLineDead);
        }

        private void OnGUI() {
            if (lines.Count == 0) {
                return;
            }

            Vector2 size = new Vector2(384f, Screen.height / 3f);
            float y = Screen.height * 0.25f;

            Rect rect = new Rect(Screen.width - (size.x + 10), y, size.x, size.y);
            GUILayout.BeginArea(rect);

            GUILayout.BeginVertical();

            foreach (Line line in lines) {
                GUILayout.Label(line.text);
            }

            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        private bool isLineDead(Line line) {
            return line.lifeRemaining <= 0;
        }

        private class Line {
            public float lifeRemaining;
            public string text;

            public Line(float lifeRemaining, string text) {
                this.lifeRemaining = lifeRemaining;
                this.text = text;
            }
        }
    }
}