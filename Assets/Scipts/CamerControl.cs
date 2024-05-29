using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerControl : MonoBehaviour
{
    public float dragSpeed = 2; // 카메라 이동 속도
    public float rotationSpeed = 5; // 카메라 회전 속도
    public float doubleClickTime = 0.2f; // 더블 탭 간격
    public float fixedRotationX = 60f; // 고정된 카메라 x축 회전각

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
        // 안드로이드 스마트폰에서 화면을 한 손가락으로 터치하고 움직였을 때
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // 카메라 이동
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(-touchDeltaPosition.x * dragSpeed * Time.deltaTime, 0, -touchDeltaPosition.y * dragSpeed * Time.deltaTime, Space.World);
        }

        // 안드로이드 스마트폰에서 화면을 두 번 터치했을 때
        if (Input.touchCount == 2)
        {
            // 첫 번째 손가락의 위치와 두 번째 손가락의 위치를 구함
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // 이전 터치 위치와 현재 터치 위치의 차이를 구함
            Vector2 prevTouchDeltaPos = touch1.position - touch1.deltaPosition;
            Vector2 touchDeltaPos = touch2.position - touch2.deltaPosition;

            // 이전 터치와 현재 터치 사이의 차이를 구함
            float prevTouchDeltaMag = (prevTouchDeltaPos - touchDeltaPos).magnitude;
            float touchDeltaMag = (touch1.position - touch2.position).magnitude;

            // 두 터치 사이의 거리 변화에 따라 회전 각도를 계산
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            float rotationY = deltaMagnitudeDiff * rotationSpeed * Time.deltaTime;

            // 카메라 회전
            transform.Rotate(Vector3.up, rotationY, Space.World);
            transform.rotation = Quaternion.Euler(fixedRotationX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z); // x축 회전 고정
        }

        // 안드로이드 스마트폰에서 화면을 두 번 터치하여 더블 탭 감지
        if (Input.touchCount == 1 && Input.GetTouch(0).tapCount == 2)
        {
            if (Time.time - lastClickTime < doubleClickTime)
            {
                // 더블 클릭 시 카메라 위치와 회전을 초기 상태로 되돌림
                transform.localPosition = new Vector3(0, 130, -50); // 초기 위치로 설정
                transform.localRotation = Quaternion.Euler(60, 0, 0); // 초기 회전으로 설정
            }
            lastClickTime = Time.time;
        }
    }
}
