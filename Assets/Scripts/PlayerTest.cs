using Fusion;
using UnityEngine;

public class PlayerTest : NetworkBehaviour
{
    private Transform _arCameraTransform;

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            // ��ġ �ʱ�ȭ �ڵ� �߰�
            Vector3 spawnPosition = new Vector3(0, 0, 0); // ���� ��ġ
            Quaternion spawnRotation = Quaternion.identity;
            _arCameraTransform = Camera.main.transform;
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        }
    }

    public override void FixedUpdateNetwork()
    {
        // �� �κ��� ���� ���� ���� �÷��̾��� ������Ʈ�� �ڽ��� ī�޶� ���� �̵��ϰ� �˴ϴ�.
        if (HasStateAuthority && _arCameraTransform != null)
        {
            transform.SetPositionAndRotation(_arCameraTransform.position, _arCameraTransform.rotation);
        }
    }
}
