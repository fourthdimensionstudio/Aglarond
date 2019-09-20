using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FourthDimension.Dungeon.DungeonGeneration))]
public class DungeonGenerationInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        FourthDimension.Dungeon.DungeonGeneration dungeon = FindObjectOfType<FourthDimension.Dungeon.DungeonGeneration>();

        GUILayout.Space(20);
        if (GUILayout.Button("Reveal All Tiles")) {
            dungeon.RevealAllTiles();
        }
    }
}
