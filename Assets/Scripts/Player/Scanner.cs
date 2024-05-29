using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Scanner : MonoBehaviour
    {
        public float scanRange;
        public LayerMask targetLayer;
        public RaycastHit2D[] targets;
        public Transform nearestTarget;

        private void FixedUpdate()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, scanRange, targetLayer);
            nearestTarget = GetNearest(hits);
            if (nearestTarget != null)
            {
                //Debug.Log($"Nearest Target: {nearestTarget.name}");
            }
            else
            {
                //Debug.Log("No target found.");
            }
        }

        Transform GetNearest(Collider2D[] hits)
        {
            Transform result = null;
            float minDist = float.MaxValue;

            foreach (Collider2D hit in hits)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    result = hit.transform;
                }
            }

            return result;
        }
    }
}