using Fusion;
using UnityEngine;

namespace Game
{
    public class MonsterSpawner : SimulationBehaviour
    {
        public GameObject MonsterPrefab;
        private bool monsterSpawned = false;

        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer || monsterSpawned) return;

            Debug.Log("Spawning Monster");
            Runner.Spawn(MonsterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            monsterSpawned = true;
        }
    }
}
