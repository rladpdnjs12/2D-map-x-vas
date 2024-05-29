using Fusion;
using UnityEngine;
using DigitalRuby.PyroParticles;

namespace ARScripts
{
    public class MonsterAttack : NetworkBehaviour
    {
        [Header("Attack Settings")]
        public float attackRange = 10f;
        public float attackDamage = 2f;
        public float attackCooldown = 3f;       //몬스터 공격 속도
        private float lastAttackTime;

        [Header("References")]
        [SerializeField] private Animator monsterAnimator;
        [SerializeField] private Transform attackPoint;
        public AudioSource attackSound;
        public GameObject meteorPrefab;
        public Transform meteorSpawnPoint;

        private Transform targetPlayer;
        private MonsterSync monsterSync;

        private void Start()
        {
            if (monsterAnimator == null)
                monsterAnimator = GetComponentInChildren<Animator>();

            if (attackPoint == null)
                attackPoint = transform;

            if (meteorSpawnPoint == null)
                meteorSpawnPoint = transform;

            monsterSync = GetComponent<MonsterSync>();
        }

        private void Update()
        {
            if (monsterSync != null && monsterSync.isTakingDamage)
            {
                return; // Skip attack if monster is taking damage
            }

            UpdateTargetPlayer();

            if (targetPlayer == null)
            {
                Debug.LogWarning("No target player found.");
                TriggerIdle();
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);
            Debug.Log($"Distance to player: {distanceToPlayer}");

            if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                if (Object.HasStateAuthority)
                {
                    if (!IsCurrentAnimationState("Idle01")) // 현재 상태가 Idle01이 아니면 공격 트리거 설정 안함
                    {
                        Debug.Log("Cannot attack, current animation state is not Idle01.");
                        return;
                    }

                    Debug.Log("Monster has state authority and is attacking.");
                    monsterAnimator.SetTrigger("Attack");
                    lastAttackTime = Time.time;
                    RPC_SpawnMeteor(targetPlayer.position);
                }
                else
                {
                    Debug.Log("Monster does not have state authority.");
                }
            }
        }

        private void UpdateTargetPlayer()
        {
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            if (playerObjects.Length == 0)
            {
                targetPlayer = null;
                return;
            }

            float closestDistance = Mathf.Infinity;
            Transform closestPlayer = null;

            foreach (GameObject playerObject in playerObjects)
            {
                float distance = Vector3.Distance(transform.position, playerObject.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = playerObject.transform;
                }
            }

            targetPlayer = closestPlayer;
            if (targetPlayer != null)
            {
                Debug.Log("Target player assigned.");
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_SpawnMeteor(Vector3 targetPosition)
        {
            if (meteorPrefab != null && meteorSpawnPoint != null)
            {
                Debug.Log("Spawning meteor...");
                NetworkObject meteorNetworkObject = Runner.Spawn(meteorPrefab,
                                                                 position: meteorSpawnPoint.position,
                                                                 rotation: meteorSpawnPoint.rotation,
                                                                 inputAuthority: Object.InputAuthority);
                if (meteorNetworkObject != null)
                {
                    GameObject meteor = meteorNetworkObject.gameObject;
                    Meteor meteorScript = meteor.GetComponent<Meteor>();
                    if (meteorScript != null)
                    {
                        meteorScript.Launch(targetPosition, attackDamage);
                        Debug.Log("Meteor launched towards target.");
                    }
                    else
                    {
                        Debug.LogError("Meteor script not found on spawned object.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to spawn meteor.");
                }
            }
            else
            {
                Debug.LogError("Meteor prefab or spawn point not set.");
            }

            if (attackSound != null)
            {
                attackSound.Play();
            }
        }

        public void TriggerIdle()
        {
            if (monsterAnimator != null)
            {
                monsterAnimator.SetTrigger("Idle01");
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (attackPoint == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        public bool IsPlayerInRange()
        {
            return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= attackRange;
        }

        private bool IsCurrentAnimationState(string stateName)
        {
            return monsterAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }
    }
}
