using UnityEngine;
using Fusion;

namespace Fire
{
    public class MonsterAttack : NetworkBehaviour
    {
        public Transform target; // Ÿ���� ��ġ
        public GameObject projectilePrefab; // �߻�� ������Ÿ���� ������
        public float attackInterval = 2.0f; // ���� ���� (��)
        private float attackTimer; // ���� ���ݱ����� Ÿ�̸�

        void Start()
        {
            attackTimer = attackInterval; // Ÿ�̸� �ʱ�ȭ
        }

        void Update()
        {
            if (target != null)
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    Attack(); // ���� ����
                    attackTimer = attackInterval; // Ÿ�̸� �缳��
                }
            }
        }
        void Attack()
        {
            if (projectilePrefab && target)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Vector3 direction = (target.position - transform.position).normalized;
                projectile.transform.up = direction; // ������Ÿ���� 'up' ���Ͱ� Ÿ���� ���ϵ��� ����
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb)
                {
                    rb.velocity = direction * 10f; // �ӵ� ����
                }
            }
        }
    }
}
