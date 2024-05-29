using Fusion;
using UnityEngine;

public class Casing : NetworkBehaviour
{
    private float ejectForce = 10f;  // 탄피 배출 힘
    private float destroyTimer = 2f; // 탄피 파괴 타이머

    public override void FixedUpdateNetwork()
    {
        // 탄피의 이동을 처리합니다.
        MoveCasing();
    }

    // 탄피를 이동시킵니다.
    private void MoveCasing()
    {
        // 탄피는 그냥 앞으로 이동하도록 설정합니다.
        transform.Translate(Vector3.forward * ejectForce * Runner.DeltaTime, Space.Self);
    }
    private void Start()
    {
        // 탄피를 생성한 후, 설정된 타이머 이후에 파괴합니다.
        Destroy(gameObject, destroyTimer);
    }
}
