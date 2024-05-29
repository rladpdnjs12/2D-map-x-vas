using Fusion;
using Unity.XR.CoreUtils;
using UnityEngine;

public class Monster : NetworkBehaviour
{
    public override void Spawned()
    {
        base.Spawned();

        // ARLocation 오브젝트 찾기
        GameObject arLocation = GameObject.Find("ARLocation");

        if (arLocation != null)
        {
            // 이 몬스터를 ARLocation의 자식으로 설정
            transform.SetParent(arLocation.transform);
        }
        else
        {
            Debug.LogError("ARLocation 오브젝트를 찾을 수 없습니다.");
        }
    }

}

