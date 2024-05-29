using UnityEngine;
using Fusion;

namespace Fire
{
    public class FireSpread : NetworkBehaviour
    {
        public GameObject firePrefab; // 불의 프리팹
        public float initialRadius = 1f; // 초기 원의 반지름
        public int numberOfFires = 5; // 생성할 불의 개수
        public float interval = 2f; // 불을 생성하는 간격 (초)
        private float timer; // 타이머

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