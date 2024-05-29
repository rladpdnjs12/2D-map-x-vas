using Fusion;
using UnityEngine;

public class PlayerTest : NetworkBehaviour
{
    private Transform _arCameraTransform;

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            // 위치 초기화 코드 추가
            Vector3 spawnPosition = new Vector3(0, 0, 0); // 예시 위치
            Quaternion spawnRotation = Quaternion.identity;
            _arCameraTransform = Camera.main.transform;
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 이 부분을 통해 오직 로컬 플레이어의 오브젝트만 자신의 카메라를 따라 이동하게 됩니다.
        if (HasStateAuthority && _arCameraTransform != null)
        {
            transform.SetPositionAndRotation(_arCameraTransform.position, _arCameraTransform.rotation);
        }
    }
}
