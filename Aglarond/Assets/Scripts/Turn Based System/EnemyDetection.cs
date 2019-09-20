using UnityEngine;

namespace FourthDimension.TurnBased {
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyDetection : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D _collision) {
            Actor.DynamicActorComponent actor = _collision.GetComponent<Actor.DynamicActorComponent>();

            if(actor != null && actor.ActorType != Actor.EActorType.Player) {
                TurnBasedSystemManager.instance.AddDynamicActorToScene(actor);
            }
        }

        private void OnTriggerExit2D(Collider2D _collision) {
            Actor.DynamicActorComponent actor = _collision.GetComponent<Actor.DynamicActorComponent>();

            if(actor != null && actor.ActorType != Actor.EActorType.Player) {
                TurnBasedSystemManager.instance.RemoveDynamicActorFromScene(actor);
            }
        }
    }
}
