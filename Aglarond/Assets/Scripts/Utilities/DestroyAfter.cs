/*
 * MIT License
 * Copyright (c) 2019 Fourth Dimension Studios
 * Code written by Guilherme de Oliveira
 */

using System.Collections;
using UnityEngine;

namespace FourthDimension {
    public class DestroyAfter : MonoBehaviour {
        public float timeToDestroy = 0.5f;

        void Start() {
            StartCoroutine(WaitUntilDestroy(timeToDestroy));
        }

        private IEnumerator WaitUntilDestroy(float time) {
            yield return new WaitForSecondsRealtime(time);
            Destroy(gameObject);
        }
    }
}