using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourthDimension.TurnBased {
    [CreateAssetMenu(fileName = "DynamicActorStats", menuName = "TurnBased/Dynamic Actor Stats")]
    public class DynamicActorStats : ScriptableObject {
        public int maxHealth;
        public int baseDamage;
    }
}
