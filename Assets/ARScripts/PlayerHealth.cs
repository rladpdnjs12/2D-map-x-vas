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
            // 플레이어 사망 처리 로직 (예: 게임 오버 화면 표시 등)
            Debug.Log("Player Died!");
            // 필요한 경우 추가적인 사망 처리 로직을 추가하세요.
        }
    }
}