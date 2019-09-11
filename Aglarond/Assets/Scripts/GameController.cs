using FourthDimension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    private FourthDimension.Dungeon.DungeonGeneration m_dungeonGeneration;
    private FourthDimension.TurnBased.Actor.Hero m_heroReference;

    private void Awake() {
        m_dungeonGeneration = FindObjectOfType<FourthDimension.Dungeon.DungeonGeneration>();
        m_heroReference = FindObjectOfType<FourthDimension.TurnBased.Actor.Hero>();
    }

    private void Start() {
        m_heroReference.transform.position = m_dungeonGeneration.GetStartingPosition();
        m_heroReference.InitializeHero();
    }
}
