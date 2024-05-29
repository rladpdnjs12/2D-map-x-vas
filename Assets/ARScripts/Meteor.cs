using Fusion;
using UnityEngine;

namespace ARScripts
{
    public class Meteor : NetworkBehaviour
    {
        public float speed = 1f;
        public float damage = 20f;
        private Vector3 targetPosition;
        private bool hasImpacted = false;
        private float destroyDelay = 0.1f;

        public void Launch(Vector3 target, float damageAmount)
        {
            targetPosition = target;
            damage = damageAmount;
            Destroy(gameObject, 1f);
        }

        private void Update()
        {
            if (targetPosition != Vector3.zero)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    OnImpact();
                }
            }
        }

        private void OnCollisionEnter(Collision col)
        {
            if (!hasImpacted) // 이미 충돌된 경우 다시 충돌 처리하지 않도록
            {
                OnImpact();
            }
        }

        private void OnImpact()
        {
            hasImpacted = true; // 충돌 상태를 true로 설정
            Collider[] hitPlayers = Physics.OverlapSphere(transform.position, 1f); // 충돌 범위 설정

            foreach (Collider hitPlayer in hitPlayers)
            {
                if (hitPlayer.CompareTag("Player"))
                {
                    NetworkPlayerSetting playerSetting = hitPlayer.GetComponent<NetworkPlayerSetting>();
                    if (playerSetting != null)
                    {
                        playerSetting.TakeDamage(damage);
                        Debug.Log("Player hit by meteor!");
                    }
                }
            }
            Destroy(gameObject, destroyDelay);
        }
    }
}
