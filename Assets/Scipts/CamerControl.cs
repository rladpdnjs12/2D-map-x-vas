using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerControl : MonoBehaviour
{
    public float dragSpeed = 2; // ī�޶� �̵� �ӵ�
    public float rotationSpeed = 5; // ī�޶� ȸ�� �ӵ�
    public float doubleClickTime = 0.2f; // ���� �� ����
    public float fixedRotationX = 60f; // ������ ī�޶� x�� ȸ����

    private Vector3 dragOrigin;
    private bool isDragging;
    private float lastClickTime;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // �ȵ���̵� ����Ʈ������ ȭ���� �� �հ������� ��ġ�ϰ� �������� ��
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // ī�޶� �̵�
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(-touchDeltaPosition.x * dragSpeed * Time.deltaTime, 0, -touchDeltaPosition.y * dragSpeed * Time.deltaTime, Space.World);
        }

        // �ȵ���̵� ����Ʈ������ ȭ���� �� �� ��ġ���� ��
        if (Input.touchCount == 2)
        {
            // ù ��° �հ����� ��ġ�� �� ��° �հ����� ��ġ�� ����
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // ���� ��ġ ��ġ�� ���� ��ġ ��ġ�� ���̸� ����
            Vector2 prevTouchDeltaPos = touch1.position - touch1.deltaPosition;
            Vector2 touchDeltaPos = touch2.position - touch2.deltaPosition;

            // ���� ��ġ�� ���� ��ġ ������ ���̸� ����
            float prevTouchDeltaMag = (prevTouchDeltaPos - touchDeltaPos).magnitude;
            float touchDeltaMag = (touch1.position - touch2.position).magnitude;

            // �� ��ġ ������ �Ÿ� ��ȭ�� ���� ȸ�� ������ ���
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            float rotationY = deltaMagnitudeDiff * rotationSpeed * Time.deltaTime;

            // ī�޶� ȸ��
            transform.Rotate(Vector3.up, rotationY, Space.World);
            transform.rotation = Quaternion.Euler(fixedRotationX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z); // x�� ȸ�� ����
        }

        // �ȵ���̵� ����Ʈ������ ȭ���� �� �� ��ġ�Ͽ� ���� �� ����
        if (Input.touchCount == 1 && Input.GetTouch(0).tapCount == 2)
        {
            if (Time.time - lastClickTime < doubleClickTime)
            {
                // ���� Ŭ�� �� ī�޶� ��ġ�� ȸ���� �ʱ� ���·� �ǵ���
                transform.localPosition = new Vector3(0, 130, -50); // �ʱ� ��ġ�� ����
                transform.localRotation = Quaternion.Euler(60, 0, 0); // �ʱ� ȸ������ ����
            }
            lastClickTime = Time.time;
        }
    }
}
