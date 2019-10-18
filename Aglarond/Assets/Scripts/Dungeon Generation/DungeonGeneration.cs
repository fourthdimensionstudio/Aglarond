using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FourthDimension.Dungeon {
    /*
     * Dungeon Generation
     * Possible Optimizations:
     *      1. 
     */
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
        private const int km_amountAlloweedToExceedTilemap = 0;
        // -----------------------------------------------------

        private List<Room> m_rooms;
        private DungeonTile[,] m_dungeonTiles;

        private int m_currentRegion = -1;

        private List<Vector3Int> m_cardinalMoves = new List<Vector3Int> {
            Vector3Int.left,
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down
        };

        private List<Vector3Int> m_diagonalMoves = new List<Vector3Int> {
            Vector3Int.left + Vector3Int.up,
            Vector3Int.up + Vector3Int.right,
            Vector3Int.right + Vector3Int.down,
            Vector3Int.down + Vector3Int.left
        };

        private List<Vector3Int> m_allMovesWithDiagonals = new List<Vector3Int> {
            Vector3Int.left,
            Vector3Int.left + Vector3Int.up,
            Vector3Int.up,
            Vector3Int.up + Vector3Int.right,
            Vector3Int.right,
            Vector3Int.right + Vector3Int.down,
            Vector3Int.down,
            Vector3Int.down + Vector3Int.left
        };

        private void Awake() {
            m_rooms = new List<Room>();

            GenerateDungeon();
        }

        private void GenerateDungeon() {
            // I'm happy with this
            CreateRooms();

            // I'm happy with this
            // ValidateDungeon();

            // I'm happy with this
            // GenerateDefinitiveTilemap();

            // Last Step
            // SpawnMonsters();
        }

        #region ROOM GENERATION
        private void CreateRooms() {
            for(int i = 0; i < km_attemptsToPlaceRoom; i++) {
                // 1. Pick a random room size
                Vector2 roomPosition = new Vector2(Random.Range(0, km_stageWidth), Random.Range(0, km_stageHeight));
                int roomWidth = Random.Range(km_minRoomSize, km_maxRoomSize);
                if(roomWidth % 2 == 0) {
                    roomWidth++;
                }

                int roomHeight = Random.Range(km_minRoomSize, km_maxRoomSize);
                if(roomHeight % 2 == 0) {
                    roomHeight++;
                }
                // TODO Avoid rooms that are even-sized
                // TODO Avoid rooms too rectangular
                Room roomToAdd = new Room(roomPosition, roomWidth, roomHeight);
                bool overlaps = false;

                foreach (Room room in m_rooms) {
                    if(roomToAdd.CollidesWith(room)) {
                        overlaps = true;
                        break;
                    }
                }

                if(overlaps) {
                    continue;
                }

                m_rooms.Add(roomToAdd);

                // Creating a new region for this room
                CarveRoom(roomToAdd);
            }

            Debug.Log($"Room Generation finished, {m_currentRegion} regions and {m_rooms.Count} rooms");
        }

        private void CarveRoom(Room _room) {
            for(int x = (int)_room.roomBoundaries.bottomLeft.x; x < _room.roomBoundaries.bottomRight.x; x++) {
                for(int y = (int)_room.roomBoundaries.bottomLeft.y; y < _room.roomBoundaries.topLeft.y; y++) {
                    carvedRooms.SetTile(new Vector3Int(x, y, 0), roomTile);
                }
            }
        }
        #endregion

        #region VALIDATE
        /// <summary>
        /// Validating the dungeon using a simple flood fill algorithm
        /// </summary>
        private void ValidateDungeon() {
            Debug.Log("Validating with Flood Fill");
            Room startingRoom = m_rooms.RandomOrDefault();
            Vector3Int startingPoint = new Vector3Int((int)startingRoom.roomPosition.x, (int)startingRoom.roomPosition.y, 0);

            Stack<Vector3Int> nodesToVisit = new Stack<Vector3Int>();
            List<Vector3Int> nodesVisited = new List<Vector3Int>();

            nodesToVisit.Push(startingPoint);

            do {
                Vector3Int currentPoint = nodesToVisit.Pop();
                nodesVisited.Add(currentPoint);
                // TODO Change to Ground tile
                carvedRooms.SetTile(currentPoint, roomTile);

                foreach(Vector3Int movement in m_cardinalMoves) {
                    Vector3Int tileToVisit = currentPoint + movement;

                    if(carvedRooms.GetTile(tileToVisit) != null && !nodesVisited.Contains(tileToVisit)) {
                        nodesToVisit.Push(tileToVisit);
                    }
                }
            } while (nodesToVisit.Count > 0);
        }
        #endregion

        #region GENERATING TILEMAPS
        private void GenerateDefinitiveTilemap() {

            m_dungeonTiles = new DungeonTile[carvedRooms.size.x, carvedRooms.size.y];

            for(int x = 0; x < carvedRooms.size.x; x++) {
                for(int y = 0; y < carvedRooms.size.y; y++) {
                    DungeonTile dungeonTile;
                    if(carvedRooms.GetTile(new Vector3Int(x, y, 0)) == null) {
                        dungeonTile = Instantiate(wallGameObjectTile, new Vector3(x, y, 0), Quaternion.identity).GetComponent<DungeonTile>();
                        dungeonTile.transform.SetParent(wallTilesParent);
                        dungeonTile.InitializeTile(new Vector2(x,y), true);
                    } else {
                        dungeonTile = Instantiate(groundGameObjectTile, new Vector3(x, y, 0), Quaternion.identity).GetComponent<DungeonTile>();
                        dungeonTile.transform.SetParent(groundTilesParent);
                        dungeonTile.InitializeTile(new Vector2(x,y));
                    }

                    m_dungeonTiles[x, y] = dungeonTile;
                    
                }
            }

            Destroy(carvedRooms.gameObject);
        }

        private void SpawnMonsters() {
            for(int i = 0; i < monstersToSpawn; i++) {
                Vector2 positionToSpawn = m_rooms.RandomOrDefault().GetRandomPositionInRoom();
                Instantiate(enemyPrefabs.RandomOrDefault(), positionToSpawn, Quaternion.identity, enemiesParent);
            }
        }
        #endregion

        #region Handling
        public DungeonTile GetTile(int x, int y) {
            if(x >= 0 && x < m_dungeonTiles.GetLength(0) && y >= 0 && y < m_dungeonTiles.GetLength(1)) {
                return m_dungeonTiles[x, y];
            }

            return null;
        }

        public int GetDungeonSize(int _dimension) {
            return m_dungeonTiles.GetLength(_dimension);
        }

        public void RevealAllTiles() {
            for(int x = 0; x < m_dungeonTiles.GetLength(0); x++) {
                for(int y = 0; y < m_dungeonTiles.GetLength(1); y++) {
                    m_dungeonTiles[x, y].IsVisible = true;
                    m_dungeonTiles[x, y].WasTileDiscovered = true;
                    m_dungeonTiles[x, y].UpdateTile();
                }
            }
        }
        #endregion

        // ...
        public Vector3 GetStartingPosition() {
            return m_rooms.RandomOrDefault().GetRoomCenter();
        }
    }
}
