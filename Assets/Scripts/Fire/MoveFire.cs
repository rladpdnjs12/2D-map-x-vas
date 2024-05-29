using UnityEngine;
using Fusion;

namespace Fire
{
    public class MoveFire : NetworkBehaviour
    {
        public float speed = 1.0f; // ���� �־����� �ӵ�

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                Vector3 moveDirection = transform.position.normalized;
                transform.position += moveDirection * speed * Runner.DeltaTime;
            }
        }
    }
}