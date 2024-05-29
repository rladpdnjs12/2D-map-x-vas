using UnityEngine;
using Fusion;
using System.Collections;
using Fire;
using UnityEngine.SceneManagement;

namespace Enemy
{
    public class Monster : NetworkBehaviour
    {
        [Networked] public float health { get; set; } = 100f;
        [Networked] public NetworkBool isDead { get; set; } = false; // ���Ͱ� �׾����� �����ϴ� ��Ʈ��ũ ����
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void TakeDamageRpc(float damage, PlayerRef attacker)
        {
            if (Object.HasStateAuthority && !isDead) // ���Ͱ� ���� ���� �ʾҴ��� Ȯ��
            {
                health -= damage;
                Debug.Log($"���� �� ü��: {health}");
                if (health <= 0)
                {
                    isDead = true; // isDead ������ true�� ����
                    DieRpc();
                }
                else
                {
                    ShowHitEffectRpc();
                }
            }
        }

        public void TakeDamage(float damage, PlayerRef attacker)
        {
            TakeDamageRpc(damage, attacker);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void ShowHitEffectRpc()
        {
            animator.SetTrigger("Hit");
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void DieRpc()
        {
            if (isDead) // ���Ͱ� �׾����� Ȯ��
            {
                Debug.Log("���Ͱ� �׾����ϴ�.");
                animator.SetTrigger("Dead");
                StartCoroutine(WaitForDeathAnimation());
            }
        }

        private IEnumerator WaitForDeathAnimation()
        {
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            if (Object != null && Runner != null)
            {
                Runner.Despawn(Object);
                // Despawn �Ŀ� �ٷ� �� ��ȯ�� �õ����� ����, �ణ�� ���� �ð��� �Ӵϴ�.
                yield return new WaitForSeconds(0.1f);
                Debug.Log("��� �÷��̾ ���� �� ��ȯ�� �õ� ���Դϴ�.");
                ChangeSceneForAllPlayersRpc(); // ��� Ŭ���̾�Ʈ�� �� ��ȯ ȣ��
            }
            else
            {
                Debug.LogWarning("Object �Ǵ� Runner�� Despawn ���� null�Դϴ�.");
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void ChangeSceneForAllPlayersRpc()
        {
            Debug.Log("���� 'Location-basedGame'�� ��ȯ�մϴ�.");
            SceneManager.LoadScene("Location-basedGame");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Bullet"))
            {
                var bullet = collision.GetComponent<Fire.Bullet>();
                if (bullet != null && bullet.Object != null && bullet.Runner != null)
                {
                    TakeDamage(bullet.damage, bullet.Object.InputAuthority);
                    Runner.Despawn(bullet.Object);
                }
                else
                {
                    Debug.LogWarning("Bullet ������Ʈ �Ǵ� �� Object/Runner�� null�Դϴ�.");
                }
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (health <= 0 && !isDead && Object.HasStateAuthority)
            {
                isDead = true; // ���Ͱ� �׾����� Ȯ��
                DieRpc();
            }

            if (isDead && Object.HasStateAuthority)
            {
                ChangeSceneForAllPlayersRpc(); // ���� ������ ���� ��ü�� �� ��ȯ ȣ��
            }
        }
    }
}