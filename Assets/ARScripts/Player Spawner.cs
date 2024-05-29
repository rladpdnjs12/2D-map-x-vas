using Fusion;
using Unity.XR.CoreUtils;
using UnityEngine;
using Niantic.Lightship.AR.LocationAR;
using Niantic.Lightship.AR.PersistentAnchors;

namespace ARScripts
{
    public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
    {
        public GameObject PlayerPrefab;
        private bool _isTracked = false;
        private bool _isMonsterActive = false;

        private void Start()
        {
            // ARLocationManager 상태 변경 이벤트 구독
            ARLocationManager locationManager = FindObjectOfType<ARLocationManager>();
            if (locationManager != null)
            {
                locationManager.locationTrackingStateChanged += OnLocationTrackingStateChanged;
            }
            else
            {
                Debug.LogError("ARLocationManager not found!");
            }
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            ARLocationManager locationManager = FindObjectOfType<ARLocationManager>();
            if (locationManager != null)
            {
                locationManager.locationTrackingStateChanged -= OnLocationTrackingStateChanged;
            }
        }

        private void OnApplicationQuit()
        {
            // 앱이 종료될 때 트래킹 상태 해제
            _isTracked = false;
        }

        private void OnLocationTrackingStateChanged(ARLocationTrackedEventArgs args)
        {
            if (args.Tracking)
            {
                _isTracked = true;
                ActivateARLocation(args.ARLocation);
            }
            else
            {
                _isTracked = false;
                _isMonsterActive = false;
                DeactivateARLocation(args.ARLocation);
            }
        }

        private void ActivateARLocation(ARLocation location)
        {
            if (location != null)
            {
                location.gameObject.SetActive(true);
                foreach (Transform child in location.transform)
                {
                    if (child.CompareTag("Monster"))
                    {
                        child.gameObject.SetActive(true);
                        _isMonsterActive = true; // 몬스터 활성화 상태로 설정
                    }
                }
            }
        }

        private void DeactivateARLocation(ARLocation location)
        {
            if (location != null)
            {
                location.gameObject.SetActive(false);
                foreach (Transform child in location.transform)
                {
                    child.gameObject.SetActive(false);
                    _isMonsterActive = false;
                }
            }
        }


        //일반적으로 플레이어를 스폰하는 방법
        public void PlayerJoined(PlayerRef player)
        {
            if (PlayerPrefab == null)
            {
                Debug.LogError("PlayerPrefab이 설정되지 않았습니다!");
                return;
            }

            if (player == Runner.LocalPlayer)
            {
                // 트래킹 상태일 때만 플레이어를 스폰
                if (_isTracked && _isMonsterActive)
                {
                    Runner.Spawn(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("Player cannot be spawned because the AR location is not tracked.");
                }
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (player == Runner.LocalPlayer)
            {
                var playerObject = Runner.GetPlayerObject(player);
                if (playerObject != null)
                {
                    Runner.Despawn(playerObject);
                }

                // 플레이어가 나갔을 때 트래킹 상태 해제
                _isTracked = false;
            }
        }
    }
}
