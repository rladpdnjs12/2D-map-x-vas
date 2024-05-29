using UnityEngine;
using Fusion;

namespace Fire
{
    public class FireSpread : NetworkBehaviour
    {
        public GameObject firePrefab; // ���� ������
        public float initialRadius = 1f; // �ʱ� ���� ������
        public int numberOfFires = 5; // ������ ���� ����
        public float interval = 2f; // ���� �����ϴ� ���� (��)
        private float timer; // Ÿ�̸�

        public override void Spawned()
        {
            timer = interval;
            CreateInitialCircle();
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;

            timer -= Runner.DeltaTime;
            if (timer <= 0)
            {
                CreateInitialCircle();
                timer = interval;
            }
        }

        void CreateInitialCircle()
        {
            for (int i = 0; i < numberOfFires; i++)
            {
                float angle = i * Mathf.PI * 2 / numberOfFires;
                Vector3 position = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * initialRadius;
                position += transform.position;
                Runner.Spawn(firePrefab, position, Quaternion.identity, null);
            }
        }
    }
}