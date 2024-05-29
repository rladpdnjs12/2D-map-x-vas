using Fusion;
using UnityEngine;

namespace ARScripts
{
    public class PlayerHealth : NetworkBehaviour
    {
        [Networked] public float currentHealth { get; private set; } = 100f;

        public void TakeDamageRpc(float damage)
        {
            if (Object.HasStateAuthority)
            {
                currentHealth -= damage;
                Debug.Log($"Player health: {currentHealth}");

                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    Die();
                }
            }
        }

        private void Die()
        {
            // �÷��̾� ��� ó�� ���� (��: ���� ���� ȭ�� ǥ�� ��)
            Debug.Log("Player Died!");
            // �ʿ��� ��� �߰����� ��� ó�� ������ �߰��ϼ���.
        }
    }
}