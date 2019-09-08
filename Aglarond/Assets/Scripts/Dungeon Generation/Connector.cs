using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.Dungeon {
    public class Connector {
        public Vector3Int connectorPosition;
        public List<Region> connectedRegions;
        public List<int> connectedRegionsNumbers;

        public Connector(Vector3Int _connectorPosition) {
            connectorPosition = _connectorPosition;
            connectedRegions = new List<Region>();
            connectedRegionsNumbers = new List<int>();
        }

        public void AddConnectedRegion(Region _region) {
            connectedRegions.Add(_region);
            connectedRegionsNumbers.Add(_region.region);
        }

        public void ConnectAllRegions() {
            int lowestRegion = int.MaxValue;

            // Getting Lowest Region Value
            foreach(Region region in connectedRegions) {
                if(region.region < lowestRegion) {
                    lowestRegion = region.region;
                }
            }

            // Assigning Them
            foreach(Region region in connectedRegions) {
                region.region = lowestRegion;
            }
        }

        public bool AreRegionsConnected() {
            List<int> regions = new List<int>();

            foreach(Region region in connectedRegions) {
                if(!regions.Contains(region.region)) {
                    regions.Add(region.region);
                }
            }

            return regions.Count == 1;
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
