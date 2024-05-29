using UnityEngine;
using UnityEngine.UI;
using Fusion;

namespace ARScripts
{
    public class MonsterHealthUI : NetworkBehaviour
    {
        public Text healthText;
        private MonsterSync monsterSync;

        private void Awake()
        {
            monsterSync = GetComponent<MonsterSync>();
        }

        private void Update()
        {
            if (monsterSync != null && healthText != null)
            {
                if (Object != null && Object.IsSpawnable)
                {
                    // ü���� 0 �����̸� 0���� ǥ��
                    float displayedHealth = Mathf.Max(monsterSync.currentHealth, 0);
                    healthText.text = "Health: " + displayedHealth.ToString();
                }
                else
                {
                    Debug.LogWarning("Monster is not spawned yet.");
                }
            }
        }
    }
}