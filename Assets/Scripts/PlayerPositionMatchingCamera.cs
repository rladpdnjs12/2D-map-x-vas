using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionMatchingCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // AR ī�޶��� ��ġ�� ������
        Vector3 cameraPosition = Camera.main.transform.position;

        // �÷��̾� ������Ʈ�� ��ġ�� AR ī�޶��� ��ġ�� �����ϰ� ����
        transform.position = cameraPosition;
    }
}
