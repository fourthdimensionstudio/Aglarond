using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Have a reference to Turn Based System Manager?

namespace FourthDimension.TurnBased.Actor {
    public enum EActorType {
        Player,
        Enemy,
        // Moving Hazard?
        // Static Actor?
    }

    public abstract class BaseActor : MonoBehaviour {

        protected Vector2 m_currentPosition;
        public Vector2 CurrentPosition {
            get {
                return this.m_currentPosition;
            }

            set {
                this.m_currentPosition = value;
            }
        }

        protected EActorType m_actorType;
        public EActorType ActorType {
            get {
                return this.m_actorType;
            }

            set {
                this.m_actorType = value;
            }
        }

        public virtual void InitializeActor(EActorType _actorType) {
            m_currentPosition = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            m_actorType = _actorType;
        }

        public abstract bool HasTakenTurn();

        // TODO HasTakenEnforcedTurn?
        // TODO PlayAudioClip?
    }
}
