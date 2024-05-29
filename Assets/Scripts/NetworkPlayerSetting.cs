using Fusion;
using Unity.XR.CoreUtils;
using UnityEngine;

public class NetworkPlayerSetting : NetworkBehaviour
{
    //public Camera Camera;
    private Transform _arCameraTransform;

    public GameObject bulletPrefab; //�Ѿ� ������
    public GameObject casingPrefab; //ź�� ������
    public GameObject muzzleFlashPrefab;    //�߻� ����Ʈ
    public GameObject impactEffect;     //�� �ǰ� ����Ʈ

    // ���� ����
    [Tooltip("�Ѿ��� ������� �ִ� �ð�")]
    [SerializeField] private float _maxLifetime = 3.0f;
    [Tooltip("�Ѿ��� �̵� �ӵ�")]
    [SerializeField] private float _speed = 200.0f;
    [Tooltip("�Ѿ��� �浹 ���̾�")]
    [SerializeField] private LayerMask _asteroidLayer;

    // �Ѿ� ���� �ð��� ī��Ʈ �ٿ��մϴ�. ���� ������ ����Ǹ� ��Ʈ��ũȭ�˴ϴ� (���� �������� ����).
    [Networked] private TickTimer _currentLifetime { get; set; }

    public Transform casingExitLocation; // ź�� ���� ��ġ
    public float casingEjectForce = 10f; // ź�� ���� ��
    public float casingDestroyTimer = 2f; // ź�� �ı� Ÿ�̸�
    //�߰�
    private static Transform commonOrigin;

    private void Start()
    {
        //---�߰�--------------------------------------------------------
        Camera arCamera = Camera.main;
        if (arCamera != null)
        {
            arCamera.nearClipPlane = 0.01f;  // ���� Ŭ���� �÷��� ����
            arCamera.farClipPlane = 1000f;   // ���Ÿ� Ŭ���� �÷��� ����
        }
        //---�߰�--------------------------------------------------------
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
        // �� �κ��� ���� ���� ���� �÷��̾��� ������Ʈ�� �ڽ��� ī�޶� ���� �̵��ϰ� �˴ϴ�.
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

    // �Ѿ� �߻縦 �õ��մϴ�.
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
        // ���� ��ü�� ���������� Ȯ��
        if (!Object.HasStateAuthority)
        {
            Debug.LogWarning("Not authorized to send RPC from this instance.");
            return;
        }

        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = transform.rotation;
        TryShootRpc(spawnPosition, spawnRotation);
    }

    // ź�Ǹ� �����ϰ� �����ϴ� �޼���
    private void CasingRelease()
    {
        if (casingPrefab == null || casingExitLocation == null)
        {
            Debug.LogWarning("Casing prefab or casing exit location is not set!");
            return;
        }

        // ź�Ǹ� �����ϰ� �����մϴ�.
        GameObject casing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation);
        Rigidbody casingRigidbody = casing.GetComponent<Rigidbody>();

        // ź�ǿ� ���� ���Ͽ� ź�Ǹ� �о���ϴ�.
        if (casingRigidbody != null)
        {
            Vector3 ejectDirection = casingExitLocation.TransformDirection(Vector3.forward);
            casingRigidbody.AddForce(ejectDirection * casingEjectForce, ForceMode.Impulse);
        }

        // ���� �ð��� ���� �Ŀ� ź�Ǹ� �ı��մϴ�.
        Destroy(casing, casingDestroyTimer);
    }
}