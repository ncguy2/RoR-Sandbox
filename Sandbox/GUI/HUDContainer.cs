using System;
using System.Collections.Generic;
using System.Linq;
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

            Rect safeArea = Screen.safeArea;

            Vector2 size = new Vector2(384f, safeArea.y / 3f);
            float y = safeArea.y * 0.25f + size.y;

            Rect rect = new Rect(safeArea.x - size.x, y, size.x, size.y);

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