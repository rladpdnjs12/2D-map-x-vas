using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARScripts
{
    public class PlayerPositionMatchingCamera : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // AR 카메라의 위치를 가져옴
            Vector3 cameraPosition = Camera.main.transform.position;

            // 플레이어 오브젝트의 위치를 AR 카메라의 위치와 동일하게 설정
            transform.position = cameraPosition;
        }
    }
}
