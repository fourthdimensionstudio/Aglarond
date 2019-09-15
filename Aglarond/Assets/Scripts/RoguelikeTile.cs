using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps {
    public class RoguelikeTile : Tile {
        [Header("Roguelike Tiles")]
        public Sprite visibleSprite;
        public Sprite notVisibleSprite;

        public bool isVisible = true;

        // Refreshes itself to see if it is visible or not.
        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            base.RefreshTile(position, tilemap);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);

            if(isVisible) {
                tileData.sprite = this.visibleSprite;
            } else {
                tileData.sprite = this.notVisibleSprite;
            }

            tileData.transform = this.transform;
            tileData.gameObject = this.gameObject;
            tileData.flags = this.flags;
            tileData.colliderType = this.colliderType;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Aglarond/RoguelikeTile")]
        public static void CreateRoguelikeTile() {
            string path = EditorUtility.SaveFilePanelInProject("Save Roguelike Tile", "New Roguelike Tile", "Asset", "Save Roguelike Tile", "Assets");
            if(path == "") {
                return;
            }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RoguelikeTile>(), path);
        }
#endif
    }


}
