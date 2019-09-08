using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Dungeon {
    public abstract class Region {
        public int region;

        public Region(int _region) {
            region = _region;
        }
    }
}
