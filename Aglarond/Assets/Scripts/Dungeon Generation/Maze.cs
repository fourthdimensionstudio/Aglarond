using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Dungeon {
    public class Maze  {
        public Vector3Int startingPoint;
        public int region;
        public List<Vector3Int> mazePoints;

        public Maze(Vector3Int _startingPoint, int _region) {
            startingPoint = _startingPoint;
            region = _region;
            mazePoints = new List<Vector3Int>();
            mazePoints.Add(_startingPoint);
        }
    }
}
