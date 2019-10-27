using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.TurnBased {
    public class Door : MonoBehaviour, IInteractAndNotMove {
        public Sprite tileAfterInteracted;
        private Dungeon.DungeonTile m_dungeonTile;
        private BoxCollider2D m_boxColliderReference;

        private void Start() {
            m_dungeonTile = GetComponent<Dungeon.DungeonTile>();
            m_boxColliderReference = GetComponent<BoxCollider2D>();
        }

        public void Interact() {
            Debug.Log($"Interacting with Door...");
            m_dungeonTile.totallyVisibleSprite = tileAfterInteracted;
            m_dungeonTile.partiallyVisibleSprite = tileAfterInteracted;
            m_dungeonTile.nonVisibleSprite = tileAfterInteracted;
            m_dungeonTile.BlockVision = false;
            m_boxColliderReference.enabled = false;
        }
    }
}
