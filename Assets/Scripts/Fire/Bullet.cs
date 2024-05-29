using UnityEngine;
using Fusion;
using Enemy;

namespace Fire
{
    public class Bullet : NetworkBehaviour
    {
        public float damage = 10f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Bullet collided with something.");
            if (other.CompareTag("Monster"))
            {
                var monster = other.GetComponent<Enemy.Monster>();
                if (monster != null)
                {
                    Debug.Log("Bullet hit the monster!");
                    monster.TakeDamage(damage, Object.InputAuthority);
                    if (Object != null && Runner != null)
                    {
                        Runner.Despawn(Object);
                    }
                }
            }
        }
    }
}
