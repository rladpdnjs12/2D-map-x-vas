using Fusion;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;

public class MonsterSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject MonsterPrefab; // 몬스터 프리팹

    // 몬스터가 스폰될 위치
    private Vector3 spawnPosition = new Vector3(-0.8415996f, -1.28f, 16.15f);
    private Quaternion spawnRotation = Quaternion.Euler(0, -153, 0);

    // 플레이어가 들어올 때 몬스터를 스폰하는 방법
    public void PlayerJoined(PlayerRef player)
    {
        // 첫 번째 플레이어인지 확인하고, LocalPlayer인지 확인
        if (IsFirstPlayer() && player == Runner.LocalPlayer)
        {
            // 몬스터를 스폰
            Runner.Spawn(MonsterPrefab, spawnPosition, spawnRotation);
        }
    }

    private bool IsFirstPlayer()
    {
        // 현재 활성 플레이어 수를 확인
        int activePlayers = Runner.ActivePlayers.Count();
        // 첫 번째 플레이어인지 확인
        return activePlayers == 1;
    }
}