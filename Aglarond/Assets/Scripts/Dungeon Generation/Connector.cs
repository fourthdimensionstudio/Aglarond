using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Dungeon {
    public class Connector {
        public Vector3Int connectorPosition;
        public List<Region> connectedRegions;

        public Connector(Vector3Int _connectorPosition) {
            connectorPosition = _connectorPosition;
            connectedRegions = new List<Region>();
        }

        public void AddConnectedRegion(Region _region) {
            connectedRegions.Add(_region);
        }

        public void UnifyRegions() {
            int lowestRegion = int.MaxValue;

            // Finding Lowest region number
            foreach(Region region in connectedRegions) {
                if(region.regionNumber < lowestRegion) {
                    lowestRegion = region.regionNumber;
                }
            }

            // Assigning Lowest Region Number
            foreach(Region region in connectedRegions) {
                region.regionNumber = lowestRegion;
            }

            // Now we choose the first region and add all the region units to it while invalidating all other regions
            for(int i = 1; i < connectedRegions.Count; i++) {
                connectedRegions[0].IncorporateRegion(connectedRegions[i]);
            }
        }

        public bool DoesConnectorUnifyTheseRegions(List<Region> _regions) {
            int matches = 0;
            foreach(Region region in _regions) {
                if(connectedRegions.Contains(region)) {
                    matches++;
                }
            }

            return matches == connectedRegions.Count;
        }
    }
}
