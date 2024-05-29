using Fusion;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    //일반적으로 플레이어를 스폰하는 방법
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
