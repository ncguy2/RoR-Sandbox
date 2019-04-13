using System.Collections.Generic;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace Sandbox.UI {
    public class HudContainer : MonoBehaviour {
        private readonly List<Line> _lines;

        public HudContainer() {
            _lines = new List<Line>();
        }

        public void Add(string line) {
            _lines.Add(new Line(5, line));
        }

        private void LateUpdate() {
            foreach (Line line in _lines) {
                line.LifeRemaining -= Time.deltaTime;
            }

            _lines.RemoveAll(IsLineDead);
        }

        // ReSharper disable once InconsistentNaming
        private void OnGUI() {
            if (_lines.Count == 0) {
                return;
            }

            Vector2 size = new Vector2(384f, Screen.height / 1.5f);
            float y = Screen.height * 0.25f;

            Rect rect = new Rect(Screen.width - (size.x + 10), y, size.x, size.y);

            GUI.Box(rect, "");

            GUILayout.BeginArea(rect);

            GUILayout.BeginVertical();

            foreach (Line line in _lines) {
                GUILayout.Label(line.Text);
            }

            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        private static bool IsLineDead(Line line) {
            return line.LifeRemaining <= 0;
        }

        private class Line {
            public readonly string Text;

            public Line(float lifeRemaining, string text) {
                LifeRemaining = lifeRemaining;
                Text = text;
            }

            public float LifeRemaining { get; set; }
        }
    }
}