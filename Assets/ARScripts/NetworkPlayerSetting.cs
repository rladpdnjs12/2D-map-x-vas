using Fusion;
using Niantic.Lightship.AR.LocationAR;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace ARScripts
{
    public class NetworkPlayerSetting : NetworkBehaviour
    {
        private Transform _arCameraTransform;
        private PlayerHealth playerHealth;

        [Networked] private TickTimer _currentLifetime { get; set; }

        private static Transform commonOrigin;

        private void Start()
        {
            Camera arCamera = Camera.main;
            if (arCamera != null)
            {
                arCamera.nearClipPlane = 0.01f;  // 근접 클리핑 플레인 설정
                arCamera.farClipPlane = 1000f;   // 원거리 클리핑 플레인 설정
            }

            playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = gameObject.AddComponent<PlayerHealth>();
            }
        }

        public override void Spawned()
        {
            _arCameraTransform = Camera.main.transform;

            if (commonOrigin == null)
            {
                GameObject arLocation = GameObject.Find("ARLocation");
                if (arLocation != null)
                {
                    commonOrigin = arLocation.transform;
                }
                else
                {
                    Debug.LogError("ARLocation not found!");
                    return;
                }
            }

            transform.SetParent(commonOrigin);

            if (HasStateAuthority)
            {
                Vector3 spawnPosition = new Vector3(0, 0, 0);
                Quaternion spawnRotation = Quaternion.identity;
                transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority && _arCameraTransform != null)
            {
                transform.SetPositionAndRotation(_arCameraTransform.position, _arCameraTransform.rotation);
            }
        }

        public void TakeDamage(float damage)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamageRpc(damage);
            }
        }
    }
}