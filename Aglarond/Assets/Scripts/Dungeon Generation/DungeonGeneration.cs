using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FourthDimension.Dungeon {
    enum EDungeonTile {
        WALL,
        FLOOR,
        DOOR_CANDIDATE,
        DOOR
    }

    public class DoorCandidate {
        public Vector2 doorPosition;
        public Vector2 doorDirection;

        public DoorCandidate(Vector2 _doorPosition, Vector2 _doorDirection) {
            doorPosition = _doorPosition;
            doorDirection = _doorDirection;
        }
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
        private bool m_hasHallway = false;
        public bool HasHallway {
            get {
                return m_hasHallway;
            }
        }

        public List<Vector2> positionsInRoom;
        public List<DoorCandidate> doorCandidates;
        private List<DoorCandidate> m_unusedDoors;

        public Room(int _width, int _height) {
            width = _width;
            height = _height;
            m_centerPosition = Vector2.zero;
            positionsInRoom = new List<Vector2>();
            doorCandidates = new List<DoorCandidate>();
            m_unusedDoors = new List<DoorCandidate>();

            int halfWidth = Mathf.RoundToInt(((float)width / 2.0f));
            int halfHeight = Mathf.RoundToInt(((float)height / 2.0f));

            for(int x = -halfWidth; x <= halfWidth; x++) {
                for(int y = -halfHeight; y <= halfHeight; y++) {
                    positionsInRoom.Add(new Vector2(x,y));
                }
            }

            // at this point, this is a square room, adding door candidates
            doorCandidates.Add(new DoorCandidate(new Vector2(Random.Range(-halfWidth + 1, halfWidth - 2), halfHeight + 1), Vector2.up));
            doorCandidates.Add(new DoorCandidate(new Vector2(Random.Range(-halfWidth + 1, halfWidth - 2), -halfHeight - 1), Vector2.down));
            doorCandidates.Add(new DoorCandidate(new Vector2(halfWidth + 1, Random.Range(-halfHeight + 2, halfHeight - 1)), Vector2.right));
            doorCandidates.Add(new DoorCandidate(new Vector2(-halfWidth - 1, Random.Range(-halfHeight + 2, halfHeight - 1)), Vector2.left));
        }

        public void MakeCellullarAutomata() {
            // TODO remove all the magic numbers here
            width = 8;
            height = 8;
            positionsInRoom.Clear();
            doorCandidates.Clear();

            Vector2 startingPosition = Vector2.zero;
            int halfWidth = Mathf.RoundToInt( ((float)width / 2.0f) );
            int halfHeight = Mathf.RoundToInt( ((float)height / 2.0f) );
            for(int x = -halfWidth; x <= halfWidth; x++) {
                for(int y = -halfHeight; y <= halfHeight; y++) {
                    if(Random.value < 0.7f) {
                        positionsInRoom.Add(new Vector2(x, y));
                    }
                }
            }

            for(int i = 0; i < 5; i++) {
                for (int x = -halfWidth; x <= halfWidth; x++) {
                    for (int y = -halfHeight; y <= halfHeight; y++) {
                        Vector2 currentPosition = new Vector2(x, y);

                        if(positionsInRoom.Contains(currentPosition)) {
                            if(GetNumberOfNeighbors(currentPosition) < 4) {
                                positionsInRoom.Remove(currentPosition);
                            }
                        } else {
                            if(GetNumberOfNeighbors(currentPosition) >= 5) {
                                positionsInRoom.Add(currentPosition);
                            }
                        }

                    }
                }
            }

            // Placing Doors
            Vector2 tileMostToTheRight = Vector2.zero;
            Vector2 tileMostToTheLeft = Vector2.zero;
            Vector2 tileMostUp = Vector2.zero;
            Vector2 tileMostDown = Vector2.zero;

            foreach(Vector2 tilePosition in positionsInRoom) {
                if(tilePosition.x > tileMostToTheRight.x) {
                    tileMostToTheRight = tilePosition;
                }

                if (tilePosition.x < tileMostToTheLeft.x) {
                    tileMostToTheLeft = tilePosition;
                }

                if (tilePosition.y > tileMostUp.y) {
                    tileMostUp = tilePosition;
                }

                if (tilePosition.y < tileMostDown.y) {
                    tileMostDown = tilePosition;
                }
            }

            doorCandidates.Add(new DoorCandidate(tileMostToTheRight + Vector2.right, Vector2.right));
            doorCandidates.Add(new DoorCandidate(tileMostDown + Vector2.down, Vector2.down));
            doorCandidates.Add(new DoorCandidate(tileMostToTheLeft + Vector2.left, Vector2.left));
            doorCandidates.Add(new DoorCandidate(tileMostUp + Vector2.up, Vector2.up));
        }

        private int GetNumberOfNeighbors(Vector2 _position) {
            int numberOfNeighbours = 0;

            if (positionsInRoom.Contains(_position + Vector2.up)) numberOfNeighbours++;
            if (positionsInRoom.Contains(_position + Vector2.up + Vector2.right)) numberOfNeighbours++;
            if (positionsInRoom.Contains(_position + Vector2.right)) numberOfNeighbours++;
            if (positionsInRoom.Contains(_position + Vector2.right + Vector2.down)) numberOfNeighbours++;
            if (positionsInRoom.Contains(_position + Vector2.down)) numberOfNeighbours++;
            if (positionsInRoom.Contains(_position + Vector2.down + Vector2.left)) numberOfNeighbours++;
            if (positionsInRoom.Contains(_position + Vector2.left)) numberOfNeighbours++;
            if (positionsInRoom.Contains(_position + Vector2.left + Vector2.up)) numberOfNeighbours++;

            return numberOfNeighbours;
        }

        public void PlaceHallway() {
            if(doorCandidates.Count == 0) {
                return;
            }

            m_hasHallway = true;
            DoorCandidate doorToStartTheHallway = doorCandidates.RandomOrDefault();
            doorCandidates.Remove(doorToStartTheHallway);
            foreach(DoorCandidate doorCandidate in doorCandidates) {
                m_unusedDoors.Add(doorCandidate);
            }
            doorCandidates.Clear();

            // TODO make these constants up the class
            int minHallwayLength = 1;
            int maxHallwayLength = 5;
            int hallwayLength = Random.Range(minHallwayLength, maxHallwayLength);


            for(int i = 0; i < hallwayLength; i++) {
                positionsInRoom.Add(doorToStartTheHallway.doorPosition + (i * doorToStartTheHallway.doorDirection));
            }

            Vector2 lastPlacedPosition = positionsInRoom.Last();
            if(doorToStartTheHallway.doorDirection == Vector2.up) {
                doorCandidates.Add(new DoorCandidate(lastPlacedPosition + Vector2.up, Vector2.up));
            } else if(doorToStartTheHallway.doorDirection == Vector2.right) {
                doorCandidates.Add(new DoorCandidate(lastPlacedPosition + Vector2.right, Vector2.right));
            } else if(doorToStartTheHallway.doorDirection == Vector2.down) {
                doorCandidates.Add(new DoorCandidate(lastPlacedPosition + Vector2.down, Vector2.down));
            } else if(doorToStartTheHallway.doorDirection == Vector2.left) {
                doorCandidates.Add(new DoorCandidate(lastPlacedPosition + Vector2.left, Vector2.left));
            }
        }

        public void RestoreUnusedDoors() {
            foreach(DoorCandidate door in m_unusedDoors) {
                doorCandidates.Add(door);
            }

            m_unusedDoors.Clear();
        }

        public List<Vector2> GetWorldPositions() {
            List<Vector2> worldPositions = new List<Vector2>();

            foreach(Vector2 position in positionsInRoom) {
                worldPositions.Add(m_centerPosition + position);
            }

            return worldPositions;
        }

        public List<DoorCandidate> GetDoorsInWorldPosition() {
            List<DoorCandidate> doorWorldPositions = new List<DoorCandidate>();

            foreach(DoorCandidate door in doorCandidates) {
                doorWorldPositions.Add(new DoorCandidate(m_centerPosition + door.doorPosition, door.doorDirection));
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

            Room starterRoom = new Room(Random.Range(km_minRoomSize, km_maxRoomSize), Random.Range(km_minRoomSize, km_maxRoomSize));
            starterRoom.CenterPosition = new Vector2(km_stageWidth / 2, km_stageHeight / 2);
            ConsolidateRoom(starterRoom);
            m_rooms.Add(starterRoom);
            m_starterRoom = starterRoom;

            // add another room
            // repeat until level is full
            for(int i = 0; i < km_attemptsToPlaceRoom; i++) {
                Room roomBeingAdded = CreateARoom();

                if(roomBeingAdded.positionsInRoom.Count < 20) {
                    Debug.Log($"Room is too small... will not be placed.");
                    continue;
                }

                Room roomToAttachTo = m_rooms.RandomOrDefault();
                PlaceRoom(roomBeingAdded, roomToAttachTo);
            }

            CleanupDungeon();
            GenerateDungeonTiles();
        }

        #region Room Generation and Placement
        private Room CreateARoom() {
            int roomWidth = Random.Range(km_minRoomSize, km_maxRoomSize);
            int roomHeight = Random.Range(km_minRoomSize, km_maxRoomSize);
            Room createdRoom = new Room(roomWidth, roomHeight);

            if(Random.value < 0.33f) {
                Debug.Log($"Making a Celullar Automata");
                createdRoom.MakeCellullarAutomata();
            }

            if(Random.value < 0.8f) {
                Debug.Log($"Placing Hallway on Room");
                createdRoom.PlaceHallway();
            }

            return createdRoom;
        }

        private void PlaceRoom(Room _roomBeingPlaced, Room _connectedRoom) {
            List<DoorCandidate> connectedRoomDoorPositions = _connectedRoom.GetDoorsInWorldPosition();
            List<DoorCandidate> roomBeingPlacedDoorPositions = _roomBeingPlaced.GetDoorsInWorldPosition(); // right now, the room being placed is centered on (0,0)
            List<Vector2> roomBeingPlacedPositions = _roomBeingPlaced.GetWorldPositions();


            for(int i = 0; i < roomBeingPlacedDoorPositions.Count; i++) {
                for(int j = 0; j < connectedRoomDoorPositions.Count; j++) {
                    Vector2 centerCandidate = connectedRoomDoorPositions[j].doorPosition - roomBeingPlacedDoorPositions[i].doorPosition;

                    if(roomBeingPlacedDoorPositions[i].doorDirection == Vector2.right && connectedRoomDoorPositions[j].doorDirection != Vector2.left ||
                        roomBeingPlacedDoorPositions[i].doorDirection == Vector2.down && connectedRoomDoorPositions[j].doorDirection != Vector2.up ||
                        roomBeingPlacedDoorPositions[i].doorDirection == Vector2.left && connectedRoomDoorPositions[j].doorDirection != Vector2.right ||
                        roomBeingPlacedDoorPositions[i].doorDirection == Vector2.up && connectedRoomDoorPositions[j].doorDirection != Vector2.down) {
                        continue;
                    }

                    // Debug.Log($"Center Candidate: {centerCandidate}");
                    // Debug.Log($"Door Position on Room Being Placed: {centerCandidate + roomBeingPlacedDoorPositions[i]} - Door Position on Room being connected: {connectedRoomDoorPositions[j]}");

                    bool canRoomBePlaced = true;
                    foreach(Vector2 position in roomBeingPlacedPositions) {
                        int xPositionToCheck = (int)(centerCandidate.x + position.x);
                        int yPositionToCheck = (int)(centerCandidate.y + position.y);
                        
                        // Checking if room is not out of bounds
                        if(xPositionToCheck < 0 || xPositionToCheck >= km_stageWidth || yPositionToCheck < 0 || yPositionToCheck >= km_stageHeight) {
                            canRoomBePlaced = false;
                            break;
                        }

                        // Checking if there is a tile on this position that is not a wall
                        if(m_abstractedDungeonTiles[xPositionToCheck, yPositionToCheck] != EDungeonTile.WALL) {
                            canRoomBePlaced = false;
                            break;
                        }
                    }

                    if(canRoomBePlaced) {
                        _roomBeingPlaced.CenterPosition = centerCandidate;
                        ConsolidateRoom(_roomBeingPlaced);
                        m_abstractedDungeonTiles[(int)connectedRoomDoorPositions[j].doorPosition.x, (int)connectedRoomDoorPositions[j].doorPosition.y] = EDungeonTile.DOOR;
                        _roomBeingPlaced.doorCandidates.RemoveAt(i);
                        _connectedRoom.doorCandidates.RemoveAt(j);
                        m_rooms.Add(_roomBeingPlaced);

                        if(_roomBeingPlaced.HasHallway) {
                            _roomBeingPlaced.RestoreUnusedDoors();
                        }

                        return;
                    }
                }
            }
        }

        private void ConsolidateRoom(Room _roomToConsolidate) {
            List<Vector2> positionsToConsolidate = _roomToConsolidate.GetWorldPositions();
            List<DoorCandidate> doorPositions = _roomToConsolidate.GetDoorsInWorldPosition();

            foreach (Vector2 position in positionsToConsolidate) {
                if (position.x < 0 || position.x >= km_stageWidth || position.y < 0 || position.y >= km_stageHeight) {
                    continue;
                }

                m_abstractedDungeonTiles[(int)position.x, (int)position.y] = EDungeonTile.FLOOR;
            }

            foreach (DoorCandidate door in doorPositions) {
                Vector2 position = door.doorPosition;

                if (position.x < 0 || position.x >= km_stageWidth || position.y < 0 || position.y >= km_stageHeight) {
                    continue;
                }

                m_abstractedDungeonTiles[(int)position.x, (int)position.y] = EDungeonTile.DOOR_CANDIDATE;
            }
        }
        #endregion

        #region Cleanup and Tile Generation
        private void CleanupDungeon() {
            for(int x = 0; x < km_stageWidth; x++) {
                for (int y = 0; y < km_stageHeight; y++) {
                    if (m_abstractedDungeonTiles[x, y] == EDungeonTile.DOOR_CANDIDATE ||
                        x == 0 || y == 0 || x == km_stageWidth - 1 || y == km_stageHeight - 1) {
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
        #endregion

        #region Helper Functions
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
        #endregion
    }
}
