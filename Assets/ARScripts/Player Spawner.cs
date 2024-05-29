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
            // ARLocationManager ���� ���� �̺�Ʈ ����
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
            // �̺�Ʈ ���� ����
            ARLocationManager locationManager = FindObjectOfType<ARLocationManager>();
            if (locationManager != null)
            {
                locationManager.locationTrackingStateChanged -= OnLocationTrackingStateChanged;
            }
        }

        private void OnApplicationQuit()
        {
            // ���� ����� �� Ʈ��ŷ ���� ����
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
                        _isMonsterActive = true; // ���� Ȱ��ȭ ���·� ����
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


        //�Ϲ������� �÷��̾ �����ϴ� ���
        public void PlayerJoined(PlayerRef player)
        {
            if (PlayerPrefab == null)
            {
                Debug.LogError("PlayerPrefab�� �������� �ʾҽ��ϴ�!");
                return;
            }

            if (player == Runner.LocalPlayer)
            {
                // Ʈ��ŷ ������ ���� �÷��̾ ����
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

                // �÷��̾ ������ �� Ʈ��ŷ ���� ����
                _isTracked = false;
            }
        }
    }
}
