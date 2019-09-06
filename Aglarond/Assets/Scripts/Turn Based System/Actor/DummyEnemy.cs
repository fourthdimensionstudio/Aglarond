using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.TurnBased.Actor {
    public class DummyEnemy : DynamicActorComponent {
        private void Awake() {
            InitializeActor(EActorType.Enemy);
        }

        public override bool HasTakenTurn() {
            return true;
        }
    }
}
