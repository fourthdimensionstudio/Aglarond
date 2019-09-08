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
        [Header("Tilemaps for Generation")]
        public Tilemap carvedRooms;
        public Tile solidRock;
        public Tile roomTile;
        public Tile mazeTile;
        public Tile possibleConnector;

        [Header("Definitive Tilemap")]
        public Tilemap groundTilemap;
        public Tilemap collisionsTilemap;
        public Tile groundTile;
        public Tile wallTile;

        //---------------------------------------- Configuration
        // TODO Make a ScriptableObject with the config variables
        private const int km_stageWidth = 30;
        private const int km_stageHeight = 30;

        private const int km_attemptsToPlaceRoom = 200;
        private const float km_extraConnectorChance = .0f;
        private const int km_roomExtraSize = 2;
        private const int km_minRoomSize = 5;
        private const int km_maxRoomSize = 9;
        private const int km_amountAlloweedToExceedTilemap = 0;
        // -----------------------------------------------------

        private List<Room> m_rooms;
        private List<Maze> m_mazes;
        private List<RegionUnit> m_roomsAndMazes;
        private List<Connector> m_connectors;
        private List<Region> m_regions;

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

        private void Start() {
            m_rooms = new List<Room>();
            m_mazes = new List<Maze>();
            m_roomsAndMazes = new List<RegionUnit>();
            m_connectors = new List<Connector>();
            m_regions = new List<Region>();

            GenerateDungeon();
        }

        private void GenerateDungeon() {
            // I'm happy with this
            CreateRooms();
            // I'm happy with this
            CreatePathways();


            // I'm NOT happy with this
            CreateConnections();

            // I'm happy with this
            CleanDeadEnds();
            // I'm happy with this
            ValidateDungeon();
            // I'm happy with this
            GenerateDefinitiveTilemap();
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
                m_roomsAndMazes.Add(roomToAdd);

                // Creating a new region for this room
                Region newRegion = new Region(++m_currentRegion);
                newRegion.AddRegionUnitToRegion(roomToAdd);
                m_regions.Add(newRegion);
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

        #region MAZE GENERATION
        private void CreatePathways() {
            int tilemapWidth = carvedRooms.size.x;
            int tilemapHeight = carvedRooms.size.y;
            Vector3Int startingPoint;

            for(int x = 0; x < tilemapWidth; x++) {
                for (int y = 0; y < tilemapHeight; y++) {
                    startingPoint = new Vector3Int(x, y, 0);
                    if (carvedRooms.GetTile(startingPoint) == null && !DoesSingleTileHasAdjacents(startingPoint)) {
                        StartMazeOnPoint(startingPoint);
                    }
                }
            }
        }

        private void StartMazeOnPoint(Vector3Int _point) {
            Maze mazeBeingCreated = new Maze(_point);

            int tilemapWidth = carvedRooms.size.x;
            int tilemapHeight = carvedRooms.size.y;

            Stack<Vector3Int> visitedPositions = new Stack<Vector3Int>();
            Vector3Int currentPoint = _point;
            visitedPositions.Push(currentPoint);
            carvedRooms.SetTile(currentPoint, mazeTile);

            bool reachedEnd = false;
            int currentTries = 0;

            while (!reachedEnd && currentTries < 1999) {
                currentTries++;

                List<Vector3Int> possibleMoves = new List<Vector3Int> {
                    Vector3Int.left,
                    Vector3Int.up,
                    Vector3Int.right,
                    Vector3Int.down
                };

                while (possibleMoves.Count > 0) {
                    Vector3Int chosenMove = possibleMoves.RandomOrDefault();

                    if (DoesTileHasRoomAdjacents(currentPoint, currentPoint + chosenMove)) {
                        possibleMoves.Remove(chosenMove);
                    } else {
                        Vector3Int futurePoint = currentPoint + chosenMove;
                        if (futurePoint.x >= (tilemapWidth + km_amountAlloweedToExceedTilemap) ||
                            futurePoint.y >= (tilemapHeight + km_amountAlloweedToExceedTilemap) ||
                            futurePoint.x < (0 - km_amountAlloweedToExceedTilemap) ||
                            futurePoint.y < (0 - km_amountAlloweedToExceedTilemap)) {
                            // Debug.Log("REJECTED!!");
                            possibleMoves.Remove(chosenMove);
                        } else {
                            currentPoint += chosenMove;
                            visitedPositions.Push(currentPoint);
                            carvedRooms.SetTile(currentPoint, mazeTile);
                            mazeBeingCreated.mazePoints.Add(currentPoint);
                            break;
                        }
                    }
                }

                if (possibleMoves.Count == 0) {
                    if (visitedPositions.Count == 0) {
                        // Debug.Log($"Left Organically");
                        reachedEnd = true;
                    } else {
                        currentPoint = visitedPositions.Pop();
                    }
                }
            }

            m_mazes.Add(mazeBeingCreated);
            m_roomsAndMazes.Add(mazeBeingCreated);

            // Creating new region for the maze
            Region newRegion = new Region(++m_currentRegion);
            newRegion.AddRegionUnitToRegion(mazeBeingCreated);
            m_regions.Add(newRegion);
        }

        /// <summary>
        /// Check all tiles around a tile for adjacents.
        /// </summary>
        /// <param name="_tilePosition">Point to check for adjacents</param>
        /// <returns>True if the tile has 1 or more neighbours</returns>
        private bool DoesSingleTileHasAdjacents(Vector3Int _tilePosition) {
            int totalAdjacents = 0;

            foreach (Vector3Int movement in m_allMovesWithDiagonals) {
                if (carvedRooms.GetTile(_tilePosition + movement) != null) {
                    totalAdjacents++;
                }
            }

            // Debug.Log($"[Does Single Tile Has Adjacents] {totalAdjacents} total adjacents");
            return (totalAdjacents > 0);
        }

        /// <summary>
        /// Check all room tiles around a tile for adjacents, ignoring the origin point
        /// </summary>
        /// <param name="_currentPoint">Current point that the algorithm is on</param>
        /// <param name="_futurePoint">Point that the algorithm is trying to go</param>
        /// <returns>Returns true if the tile has 1 or more room neighbours</returns>
        private bool DoesTileHasRoomAdjacents(Vector3Int _currentPoint, Vector3Int _futurePoint) {
            int totalAdjacents = 0;

            // Checking all adjacents for the tile
            foreach (Vector3Int movement in m_allMovesWithDiagonals) {
                if(carvedRooms.GetTile(_futurePoint + movement) != null &&
                    _futurePoint + movement != _currentPoint) {
                    totalAdjacents++;
                }
            }

            // Then we check on diagonals for mazeTiles and subtract them from the adjacents
            foreach(Vector3Int diagonalMove in m_diagonalMoves) {
                if(carvedRooms.GetTile(_futurePoint + diagonalMove) == mazeTile) {
                    totalAdjacents--;
                }
            }

            // Debug.Log($"[Does Tile Has Room Adjacents] {totalAdjacents} total adjacents");
            return (totalAdjacents > 0);
        }
        #endregion

        #region CREATING CONNECTIONS
        // TODO some very poorly optimized code below, beware!
        /*
         * Rules for creating the connections:
         *      1. Tile is Null
         *      2. Tile is adjacent to two regions of different colors
         */
        private void CreateConnections() {
            Debug.Log($"{m_regions.Count} regions to check");

            for (int x = 0; x < carvedRooms.size.x; x++) {
                for(int y = 0; y < carvedRooms.size.y; y++) {
                    Vector3Int positionToInvestigate = new Vector3Int(x, y, 0);
                    if(carvedRooms.GetTile(positionToInvestigate) == null) {
                        // If it is null, it might be a possible connections.
                        // For it to be a connection it has to be adjacent to two different regions
                        VerifyConnectivity(positionToInvestigate);
                    }
                }
            }

            // DEBUGGING Printing all possible Connectors...
            /*
            Debug.Log($"We have {m_connectors.Count} connectors");
            foreach (Connector connector in m_connectors) {
                carvedRooms.SetTile(connector.connectorPosition, possibleConnector);
            }
            */

            // Now it's time to unify the region
            // What we have here:
            //     1. Connectors that can unify 2 regions
            //     2. Regions that contains all RegionUnits pertaining to its Region
            m_connectors.Shuffle();
            List<Connector> possibleConnectors = m_connectors;
            List<Connector> consolidatedConnectors = new List<Connector>();
            List<Connector> removedConnectors = new List<Connector>();

            do {
                Connector chosenConnector = possibleConnectors[0];
                possibleConnectors.RemoveAt(0);
                consolidatedConnectors.Add(chosenConnector);

                // Removing from the possible connectors all connectors that unify the same regions
                for (int i = 0; i < possibleConnectors.Count; i++) {
                    if (possibleConnectors[i].DoesConnectorUnifyTheseRegions(chosenConnector.connectedRegions)) {
                        removedConnectors.Add(possibleConnectors[i]);
                        possibleConnectors.RemoveAt(i);
                        i--;
                    }
                }

                carvedRooms.SetTile(chosenConnector.connectorPosition, possibleConnector);
                chosenConnector.UnifyRegions();

                // Removing all Invalidated Regions from m_regions
                for(int i = 0; i < m_regions.Count; i++) {
                    if(m_regions[i].regionNumber == -1) {
                        m_regions.RemoveAt(i);
                        i--;
                    }
                }
            } while (possibleConnectors.Count > 0);
        }

        // TODO Beware this is the main source of inefficiency of the entire generation system
        // An important thing to know here is that when this functions starts every region has either 1 room or 1 maze, no unification happened yet.
        private void VerifyConnectivity(Vector3Int _position) {
            Connector connectorBeingCreated = new Connector(_position);
            List<int> adjacentRegions = new List<int>();

            foreach(Vector3Int move in m_cardinalMoves) {
                Vector3Int positionToBeVerified = _position + move;
                TileBase adjacentTile = carvedRooms.GetTile(positionToBeVerified);

                if(adjacentTile != null) {
                    foreach(RegionUnit regionUnit in m_roomsAndMazes) {
                        if(regionUnit.IsPositionWithinRegionUnit(positionToBeVerified)) {
                            foreach(Region region in m_regions) {
                                if(region.IsThisUnitOnThisRegion(regionUnit)) {
                                    if(!adjacentRegions.Contains(region.regionNumber)) {
                                        connectorBeingCreated.AddConnectedRegion(region);
                                        adjacentRegions.Add(region.regionNumber);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if(adjacentRegions.Count > 1) {
                m_connectors.Add(connectorBeingCreated);
            }
        }

        private bool HaveDiagonalConnectivity(Vector3Int _position) {
            foreach(Vector3Int move in m_diagonalMoves) {
                Vector3Int positionToCheck = _position + move;
                if(carvedRooms.GetTile(positionToCheck) != null) {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region CLEANUP
        private void CleanDeadEnds() {
            List<Vector3Int> tilesToClean = new List<Vector3Int>();

            for(int x = 0; x < carvedRooms.size.x; x++) {
                for(int y = 0; y < carvedRooms.size.y; y++) {
                    Vector3Int positionBeingVerified = new Vector3Int(x, y, 0);
                    if(GetAmountOfEmptyNeighbors(positionBeingVerified) == 3) {
                        tilesToClean.Add(positionBeingVerified);
                    }
                }
            }

            // Debug.Log($"Tiles to Clean: {tilesToClean.Count}");
            foreach (Vector3Int positionToClean in tilesToClean) {
                CleanTilePosition(positionToClean);
            }
        }

        private void CleanTilePosition(Vector3Int _position) {
            carvedRooms.SetTile(_position, null);

            Vector3Int neighbourToClean = GetNeighbourThatNeedsToBeCleaned(_position);
            if(neighbourToClean != Vector3Int.zero) {
                CleanTilePosition(neighbourToClean);
            }
        }

        private Vector3Int GetNeighbourThatNeedsToBeCleaned(Vector3Int _position) {
            Vector3Int neighbour = Vector3Int.zero;

            foreach(Vector3Int move in m_cardinalMoves) {
                if(carvedRooms.GetTile(_position + move) != null) {
                    if(GetAmountOfEmptyNeighbors(_position + move) == 3) {
                        neighbour = _position + move;
                    }
                }
            }

            return neighbour;
        }

        private int GetAmountOfEmptyNeighbors(Vector3Int _position) {
            int emptyNeighbors = 0;

            foreach(Vector3Int move in m_cardinalMoves) {
                if(carvedRooms.GetTile(_position + move) == null) {
                    emptyNeighbors++;
                }
            }

            return emptyNeighbors;
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
            for(int x = -1; x < carvedRooms.size.x + 1; x++) {
                for(int y = -1; y < carvedRooms.size.y + 1; y++) {
                    if(carvedRooms.GetTile(new Vector3Int(x, y, 0)) == null) {
                        collisionsTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                    } else {
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                    }
                }
            }

            carvedRooms.ClearAllTiles();
            Destroy(carvedRooms.gameObject);
        }
        #endregion
    }
}
