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
            if (!hasImpacted) // �̹� �浹�� ��� �ٽ� �浹 ó������ �ʵ���
            {
                OnImpact();
            }
        }

        private void OnImpact()
        {
            hasImpacted = true; // �浹 ���¸� true�� ����
            Collider[] hitPlayers = Physics.OverlapSphere(transform.position, 1f); // �浹 ���� ����

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
