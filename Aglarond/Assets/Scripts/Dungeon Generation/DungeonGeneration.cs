using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FourthDimension.Dungeon {
    enum EDungeonTiles {
        WALL,
        FLOOR,
        DOOR
    }

    public class DungeonGeneration : MonoBehaviour {
        [Header("Tilemaps for Debugging Generation")]
        public Tilemap carvedRooms;
        public Tile solidRock;
        public Tile roomTile;
        public Tile mazeTile;
        public Tile possibleConnector;

        [Header("GameObjects for Tilemap")]
        public GameObject groundGameObjectTile;
        public GameObject wallGameObjectTile;

        [Header("Organization")]
        public Transform groundTilesParent;
        public Transform wallTilesParent;

        [Header("Monster Spawning")]
        public int monstersToSpawn = 50;
        public GameObject[] enemyPrefabs;
        public Transform enemiesParent;

        //---------------------------------------- Configuration
        // TODO Make a ScriptableObject with the config variables
        private const int km_stageWidth = 40;
        private const int km_stageHeight = 40;

        private const int km_attemptsToPlaceRoom = 300;
        private const float km_extraConnectorChance = .0f;
        private const int km_roomExtraSize = 2;
        private const int km_minRoomSize = 8;
        private const int km_maxRoomSize = 12;
        private const int km_maxTunnelSize = 10;
        private const int km_amountAlloweedToExceedTilemap = 0;
        // -----------------------------------------------------

        private List<Room> m_rooms;
        private EDungeonTiles[,] m_abstractedDungeonTiles;
        private DungeonTile[,] m_consolidatedDungeonTiles;

        private readonly List<Vector2> m_cardinalMoves = new List<Vector2> {
            Vector2.left,
            Vector2.up,
            Vector2.right,
            Vector2.down
        };

        private void Awake() {
            m_rooms = new List<Room>();
            GenerateDungeon();
        }

        private void GenerateDungeon() {
            // Start with a room
            // add another room
            // repeat until level is full
        }
    }
}
