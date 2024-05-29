using Fusion;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class MonsterMovement : NetworkBehaviour
    {
        public float Speed = 2f;
        private Vector2 moveDirection;
        private Bounds tilemapBounds;

        private void Start()
        {
            tilemapBounds = GetTilemapBounds();
            SetRandomDirection();
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            Vector3 newPosition = transform.position + (Vector3)moveDirection * Speed * Runner.DeltaTime;
            if (!tilemapBounds.Contains(newPosition))
            {
                SetRandomDirection();
            }
            else
            {
                transform.position = newPosition;
            }
        }

        private void SetRandomDirection()
        {
            moveDirection = Random.insideUnitCircle.normalized;
        }

        private Bounds GetTilemapBounds()
        {
            var tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
            return tilemap.localBounds;
        }
    }
}
