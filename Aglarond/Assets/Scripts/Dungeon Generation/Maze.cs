using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Dungeon {
    public class Maze : Region  {
        public Vector3Int startingPoint;
        public List<Vector3Int> mazePoints;

        public Maze(Vector3Int _startingPoint, int _region) : base(_region) {
            startingPoint = _startingPoint;
            mazePoints = new List<Vector3Int>();
            mazePoints.Add(_startingPoint);
        }

        public bool IsPositionWithinMaze(Vector3Int _position) {
            return mazePoints.Contains(_position);
        }
    }
}
