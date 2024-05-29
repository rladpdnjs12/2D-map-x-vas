using UnityEngine;
using Fusion;

namespace Fire
{
    public class MonsterAttack : NetworkBehaviour
    {
        public Transform target; // 타겟의 위치
        public GameObject projectilePrefab; // 발사될 프로젝타일의 프리팹
        public float attackInterval = 2.0f; // 공격 간격 (초)
        private float attackTimer; // 다음 공격까지의 타이머

        void Start()
        {
            attackTimer = attackInterval; // 타이머 초기화
        }

        void Update()
        {
            if (target != null)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    Attack(); // 공격 실행
                    attackTimer = attackInterval; // 타이머 재설정
                }
            }
        }
        void Attack()
        {
            if (projectilePrefab && target)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Vector3 direction = (target.position - transform.position).normalized;
                projectile.transform.up = direction; // 프로젝타일의 'up' 벡터가 타겟을 향하도록 설정
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    rb.velocity = direction * 10f; // 속도 설정
                }
            }
        }
    }
}
