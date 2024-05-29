using Fusion;
using Unity.XR.CoreUtils;
using UnityEngine;

public class NetworkPlayerSetting : NetworkBehaviour
{
    //public Camera Camera;
    private Transform _arCameraTransform;

    public GameObject bulletPrefab; //총알 프리팹
    public GameObject casingPrefab; //탄피 프리팹
    public GameObject muzzleFlashPrefab;    //발사 이펙트
    public GameObject impactEffect;     //적 피격 이펙트

    // 설정 값들
    [Tooltip("총알이 살아있을 최대 시간")]
    [SerializeField] private float _maxLifetime = 3.0f;
    [Tooltip("총알의 이동 속도")]
    [SerializeField] private float _speed = 200.0f;
    [Tooltip("총알의 충돌 레이어")]
    [SerializeField] private LayerMask _asteroidLayer;

    // 총알 생존 시간을 카운트 다운합니다. 상태 권한이 변경되면 네트워크화됩니다 (연결 끊김으로 인한).
    [Networked] private TickTimer _currentLifetime { get; set; }

    public Transform casingExitLocation; // 탄피 배출 위치
    public float casingEjectForce = 10f; // 탄피 배출 힘
    public float casingDestroyTimer = 2f; // 탄피 파괴 타이머
    //추가
    private static Transform commonOrigin;

    private void Start()
    {
        //---추가--------------------------------------------------------
        Camera arCamera = Camera.main;
        if (arCamera != null)
        {
            arCamera.nearClipPlane = 0.01f;  // 근접 클리핑 플레인 설정
            arCamera.farClipPlane = 1000f;   // 원거리 클리핑 플레인 설정
        }
        //---추가--------------------------------------------------------
    }
    public override void Spawned()
    {
        _arCameraTransform = Camera.main.transform;

        if (commonOrigin == null)
        {
            commonOrigin = GameObject.FindObjectOfType<XROrigin>()?.transform;
            if (commonOrigin == null)
            {
                Debug.LogError("Common XR Origin not found!");
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
        // 이 부분을 통해 오직 로컬 플레이어의 오브젝트만 자신의 카메라를 따라 이동하게 됩니다.
        if (HasStateAuthority && _arCameraTransform != null)
        {
            transform.SetPositionAndRotation(_arCameraTransform.position, _arCameraTransform.rotation);

            _currentLifetime = TickTimer.CreateFromSeconds(Runner, _maxLifetime);

            if (Input.GetButtonDown("Fire1"))
            {
                TryShoot();
            }
        }
    }

    // 총알 발사를 시도합니다.
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void TryShootRpc(Vector3 position, Quaternion rotation)
    {
        if (!HasStateAuthority) return;

        Fusion.NetworkObject bulletObject = Runner.Spawn(bulletPrefab, position, rotation, Object.InputAuthority);
        bulletObject.GetComponent<Bullet>().Init(_speed, _asteroidLayer, impactEffect, _currentLifetime);

        CasingRelease();
    }

    private void TryShoot()
    {
        // 현재 객체가 소유자인지 확인
        if (!Object.HasStateAuthority)
        {
            Debug.LogWarning("Not authorized to send RPC from this instance.");
            return;
        }

        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = transform.rotation;
        TryShootRpc(spawnPosition, spawnRotation);
    }

    // 탄피를 생성하고 배출하는 메서드
    private void CasingRelease()
    {
        if (casingPrefab == null || casingExitLocation == null)
        {
            Debug.LogWarning("Casing prefab or casing exit location is not set!");
            return;
        }

        // 탄피를 생성하고 설정합니다.
        GameObject casing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation);
        Rigidbody casingRigidbody = casing.GetComponent<Rigidbody>();

        // 탄피에 힘을 가하여 탄피를 밀어냅니다.
        if (casingRigidbody != null)
        {
            Vector3 ejectDirection = casingExitLocation.TransformDirection(Vector3.forward);
            casingRigidbody.AddForce(ejectDirection * casingEjectForce, ForceMode.Impulse);
        }

        // 일정 시간이 지난 후에 탄피를 파괴합니다.
        Destroy(casing, casingDestroyTimer);
    }
}