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
        [Networked] public NetworkBool isDead { get; set; } = false; // 몬스터가 죽었는지 추적하는 네트워크 변수
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void TakeDamageRpc(float damage, PlayerRef attacker)
        {
            if (Object.HasStateAuthority && !isDead) // 몬스터가 아직 죽지 않았는지 확인
            {
                health -= damage;
                Debug.Log($"피해 후 체력: {health}");
                if (health <= 0)
                {
                    isDead = true; // isDead 변수를 true로 설정
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
            if (isDead) // 몬스터가 죽었음을 확인
            {
                Debug.Log("몬스터가 죽었습니다.");
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
                // Despawn 후에 바로 씬 전환을 시도하지 말고, 약간의 지연 시간을 둡니다.
                yield return new WaitForSeconds(0.1f);
                Debug.Log("모든 플레이어를 위한 씬 전환을 시도 중입니다.");
                ChangeSceneForAllPlayersRpc(); // 모든 클라이언트에 씬 전환 호출
            }
            else
            {
                Debug.LogWarning("Object 또는 Runner가 Despawn 전에 null입니다.");
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void ChangeSceneForAllPlayersRpc()
        {
            Debug.Log("씬을 'Location-basedGame'로 전환합니다.");
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
                    Debug.LogWarning("Bullet 컴포넌트 또는 그 Object/Runner가 null입니다.");
                }
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (health <= 0 && !isDead && Object.HasStateAuthority)
            {
                isDead = true; // 몬스터가 죽었음을 확인
                DieRpc();
            }

            if (isDead && Object.HasStateAuthority)
            {
                ChangeSceneForAllPlayersRpc(); // 상태 권한을 가진 객체가 씬 전환 호출
            }
        }
    }
}