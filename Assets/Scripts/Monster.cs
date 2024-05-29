using Fusion;
using Unity.XR.CoreUtils;
using UnityEngine;

public class Monster : NetworkBehaviour
{
    public override void Spawned()
    {
        base.Spawned();

        // ARLocation ������Ʈ ã��
        GameObject arLocation = GameObject.Find("ARLocation");

        if (arLocation != null)
        {
            // �� ���͸� ARLocation�� �ڽ����� ����
            transform.SetParent(arLocation.transform);
        }
        else
        {
            Debug.LogError("ARLocation ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

}

