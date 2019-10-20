using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FourthDimension.Dungeon {
    enum EDungeonTile {
        WALL,
        FLOOR,
        DOOR_CANDIDATE,
        DOOR
    }

    [System.Serializable]
    public class Room {
        public int width;
        public int height;
        private Vector2 m_centerPosition;
        public Vector2 CenterPosition {
            get {
                return m_centerPosition;
            }
            set {
                m_centerPosition = value;
            }
        }

        public List<Vector2> positionsInRoom;
        public List<Vector2> doorCandidates;

        public Room(int _width, int _height) {
            width = _width;
            height = _height;
            m_centerPosition = Vector2.zero;
            positionsInRoom = new List<Vector2>();
            doorCandidates = new List<Vector2>();

            int halfWidth = Mathf.RoundToInt(((float)width / 2.0f));
            int halfHeight = Mathf.RoundToInt(((float)height / 2.0f));

            for(int x = -halfWidth; x <= halfWidth; x++) {
                for(int y = -halfHeight; y <= halfHeight; y++) {
                    positionsInRoom.Add(new Vector2(x,y));
                }
            }

            // at this point, this is a square room, adding door candidates
            doorCandidates.Add(new Vector2(Random.Range(-halfWidth + 1, halfWidth - 2), halfHeight + 1));
            doorCandidates.Add(new Vector2(Random.Range(-halfWidth + 1, halfWidth - 2), -halfHeight - 1));
            doorCandidates.Add(new Vector2(halfWidth + 1, Random.Range(-halfHeight + 2, halfHeight - 1)));
            doorCandidates.Add(new Vector2(-halfWidth - 1, Random.Range(-halfHeight + 2, halfHeight - 1)));
        }

        public List<Vector2> GetWorldPositions() {
            List<Vector2> worldPositions = new List<Vector2>();

            foreach(Vector2 position in positionsInRoom) {
                worldPositions.Add(m_centerPosition + position);
            }

            return worldPositions;
        }

        public List<Vector2> GetDoorsInWorldPosition() {
            List<Vector2> doorWorldPositions = new List<Vector2>();

            foreach(Vector2 door in doorCandidates) {
                doorWorldPositions.Add(m_centerPosition + door);
            }

            return doorWorldPositions;
        }
    }

    public class DungeonGeneration : MonoBehaviour {
        [Header("GameObjects for Tilemap")]
        public GameObject groundGameObjectTile;
        public GameObject wallGameObjectTile;
        public GameObject doorTile;

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
        private const int km_minRoomSize = 6;
        private const int km_maxRoomSize = 10;
        private const int km_maxTunnelSize = 10;
        private const int km_amountAlloweedToExceedTilemap = 0;
        // -----------------------------------------------------

        private Room m_starterRoom;
        public Vector2 StartingPosition {
            get {
                return m_starterRoom.CenterPosition;
            }
        }

        private List<Room> m_rooms;
        private EDungeonTile[,] m_abstractedDungeonTiles;
        private DungeonTile[,] m_consolidatedDungeonTiles;

        private readonly List<Vector2> m_cardinalMoves = new List<Vector2> {
            Vector2.left,
            Vector2.up,
            Vector2.right,
            Vector2.down
        };

        private void Awake() {
            m_rooms = new List<Room>();
            m_abstractedDungeonTiles = new EDungeonTile[km_stageWidth, km_stageHeight];
            m_consolidatedDungeonTiles = new DungeonTile[km_stageWidth, km_stageHeight];
            GenerateDungeon();
        }

        private void GenerateDungeon() {
            for(int i = 0; i < km_stageWidth; i++) {
                for(int j = 0; j < km_stageHeight; j++) {
                    m_abstractedDungeonTiles[i, j] = EDungeonTile.WALL;
                }
            }

            Room starterRoom = CreateARoom();
            starterRoom.CenterPosition = new Vector2(km_stageWidth / 2, km_stageHeight / 2);
            ConsolidateRoom(starterRoom);
            m_rooms.Add(starterRoom);
            m_starterRoom = starterRoom;
            // add another room
            // repeat until level is full

            for(int i = 0; i < km_attemptsToPlaceRoom; i++) {
                Room roomBeingAdded = CreateARoom();
                Room roomToAttachTo = m_rooms.RandomOrDefault();
                PlaceRoom(roomBeingAdded, roomToAttachTo);
            }

            CleanupDungeon();
            GenerateDungeonTiles();
        }

        private Room CreateARoom() {
            int roomWidth = Random.Range(km_minRoomSize, km_maxRoomSize);
            int roomHeight = Random.Range(km_minRoomSize, km_maxRoomSize);
            Room createdRoom = new Room(roomWidth, roomHeight);

            // TODO have a chance to create a hallway

            return createdRoom;
        }

        private void ConsolidateRoom(Room _roomToConsolidate) {
            List<Vector2> positionsToConsolidate = _roomToConsolidate.GetWorldPositions();
            List<Vector2> doorPositions = _roomToConsolidate.GetDoorsInWorldPosition();

            foreach (Vector2 position in positionsToConsolidate) {
                if (position.x < 0 || position.x >= km_stageWidth || position.y < 0 || position.y >= km_stageHeight) {
                    continue;
                }

                m_abstractedDungeonTiles[(int)position.x, (int)position.y] = EDungeonTile.FLOOR;
            }

            foreach(Vector2 position in doorPositions) {
                if (position.x < 0 || position.x >= km_stageWidth || position.y < 0 || position.y >= km_stageHeight) {
                    continue;
                }

                m_abstractedDungeonTiles[(int)position.x, (int)position.y] = EDungeonTile.DOOR_CANDIDATE;
            }
        }

        private void PlaceRoom(Room _roomBeingPlaced, Room _connectedRoom) {
            List<Vector2> connectedRoomDoorPositions = _connectedRoom.GetDoorsInWorldPosition();
            List<Vector2> roomBeingPlacedDoorPositions = _roomBeingPlaced.GetDoorsInWorldPosition(); // right now, the room being placed is centered on (0,0)
            List<Vector2> roomBeingPlacedPositions = _roomBeingPlaced.GetWorldPositions();


            for(int i = 0; i < roomBeingPlacedDoorPositions.Count; i++) {
                for(int j = 0; j < connectedRoomDoorPositions.Count; j++) {
                    Vector2 centerCandidate = connectedRoomDoorPositions[j] - roomBeingPlacedDoorPositions[i];
                    // Debug.Log($"Center Candidate: {centerCandidate}");
                    // Debug.Log($"Door Position on Room Being Placed: {centerCandidate + roomBeingPlacedDoorPositions[i]} - Door Position on Room being connected: {connectedRoomDoorPositions[j]}");

                    bool canRoomBePlaced = true;
                    foreach(Vector2 position in roomBeingPlacedPositions) {
                        int xPositionToCheck = (int)(centerCandidate.x + position.x);
                        int yPositionToCheck = (int)(centerCandidate.y + position.y);

                        if(xPositionToCheck < 0 || xPositionToCheck >= km_stageWidth || yPositionToCheck < 0 || yPositionToCheck >= km_stageHeight) {
                            // Debug.Log($"Cannot be placed because it is out of bounds!");
                            canRoomBePlaced = false;
                            break;
                        }

                        if(m_abstractedDungeonTiles[xPositionToCheck, yPositionToCheck] != EDungeonTile.WALL) {
                            // Debug.Log($"Cannot place room here because there is a {m_abstractedDungeonTiles[xPositionToCheck, yPositionToCheck]}");
                            canRoomBePlaced = false;
                            break;
                        }
                    }

                    if(canRoomBePlaced) {
                        _roomBeingPlaced.CenterPosition = centerCandidate;
                        ConsolidateRoom(_roomBeingPlaced);
                        m_abstractedDungeonTiles[(int)connectedRoomDoorPositions[j].x, (int)connectedRoomDoorPositions[j].y] = EDungeonTile.DOOR;
                        _roomBeingPlaced.doorCandidates.RemoveAt(i);
                        _connectedRoom.doorCandidates.RemoveAt(j);
                        m_rooms.Add(_roomBeingPlaced);
                        return;
                    }
                }
            }
        }

        private void CleanupDungeon() {
            for(int x = 0; x < km_stageWidth; x++) {
                for(int y = 0; y < km_stageHeight; y++) {
                    if(m_abstractedDungeonTiles[x,y] == EDungeonTile.DOOR_CANDIDATE) {
                        m_abstractedDungeonTiles[x, y] = EDungeonTile.WALL;
                    }
                }
            }
        }

        private void GenerateDungeonTiles() {
            // Generating Borders
            for(int x = -20; x < km_stageWidth + 20; x++) {
                for(int y = -20; y < km_stageHeight + 20; y++) {
                    if(x >= 0 && x < km_stageWidth && y >= 0 && y < km_stageHeight) {
                        continue;
                    }

                    DungeonTile tile;
                    tile = Instantiate(wallGameObjectTile, new Vector3(x, y, 0), Quaternion.identity).GetComponent<DungeonTile>();
                    tile.transform.SetParent(wallTilesParent);
                    tile.InitializeTile(new Vector2(x, y), true);
                    tile.WasTileDiscovered = true;
                    tile.UpdateTile();
                }
            }

            for(int x = 0; x < km_stageWidth; x++) {
                for(int y = 0; y < km_stageHeight; y++) {
                    DungeonTile dungeonTile = null;

                    if(m_abstractedDungeonTiles[x,y] == EDungeonTile.WALL) {
                        dungeonTile = Instantiate(wallGameObjectTile, new Vector3(x, y, 0), Quaternion.identity).GetComponent<DungeonTile>();
                        dungeonTile.transform.SetParent(wallTilesParent);
                        dungeonTile.InitializeTile(new Vector2(x, y), true);
                    } else if(m_abstractedDungeonTiles[x,y] == EDungeonTile.DOOR) {
                        dungeonTile = Instantiate(doorTile, new Vector3(x, y, 0), Quaternion.identity).GetComponent<DungeonTile>();
                        dungeonTile.transform.SetParent(groundTilesParent);
                        dungeonTile.InitializeTile(new Vector2(x, y));
                    }  else if(m_abstractedDungeonTiles[x,y] == EDungeonTile.FLOOR) {
                        dungeonTile = Instantiate(groundGameObjectTile, new Vector3(x, y, 0), Quaternion.identity).GetComponent<DungeonTile>();
                        dungeonTile.transform.SetParent(groundTilesParent);
                        dungeonTile.InitializeTile(new Vector2(x, y));
                    }

                    m_consolidatedDungeonTiles[x, y] = dungeonTile;
                }
            }
        }

        public void RevealAllTiles() {
            for(int x = 0; x < km_stageWidth; x++) {
                for(int y = 0; y < km_stageHeight; y++) {
                    m_consolidatedDungeonTiles[x, y].IsVisible = true;
                    m_consolidatedDungeonTiles[x, y].WasTileDiscovered = true;
                    m_consolidatedDungeonTiles[x, y].UpdateTile();
                }
            }
        }

        public DungeonTile GetTile(int _x, int _y) {
            if (_x >= 0 && _x < m_consolidatedDungeonTiles.GetLength(0) && _y >= 0 && _y < m_consolidatedDungeonTiles.GetLength(1)) {
                return m_consolidatedDungeonTiles[_x, _y];
            }

            return null;
        }
    }
}
