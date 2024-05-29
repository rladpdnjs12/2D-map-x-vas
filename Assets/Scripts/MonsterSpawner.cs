using Fusion;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;

public class MonsterSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject MonsterPrefab; // ���� ������

    // ���Ͱ� ������ ��ġ
    private Vector3 spawnPosition = new Vector3(-0.8415996f, -1.28f, 16.15f);
    private Quaternion spawnRotation = Quaternion.Euler(0, -153, 0);

    // �÷��̾ ���� �� ���͸� �����ϴ� ���
    public void PlayerJoined(PlayerRef player)
    {
        // ù ��° �÷��̾����� Ȯ���ϰ�, LocalPlayer���� Ȯ��
        if (IsFirstPlayer() && player == Runner.LocalPlayer)
        {
            // ���͸� ����
            Runner.Spawn(MonsterPrefab, spawnPosition, spawnRotation);
        }
    }

    private bool IsFirstPlayer()
    {
        // ���� Ȱ�� �÷��̾� ���� Ȯ��
        int activePlayers = Runner.ActivePlayers.Count();
        // ù ��° �÷��̾����� Ȯ��
        return activePlayers == 1;
    }
}