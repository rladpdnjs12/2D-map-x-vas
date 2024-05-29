using Fusion;
using UnityEngine;

namespace ARScripts
{
    public class NetworkShoot : NetworkBehaviour
    {
        [Header("Prefab References")]
        public GameObject bulletPrefab;
        public GameObject casingPrefab;
        public GameObject muzzleFlashPrefab;

        [Header("Location References")]
        [SerializeField] private Animator gunAnimator;
        [SerializeField] private Transform barrelLocation;
        [SerializeField] private Transform casingExitLocation;

        [Header("Settings")]
        [Tooltip("Specify time to destroy the casing object")][SerializeField] private float destroyTimer = 2f;
        [Tooltip("Bullet Speed")][SerializeField] private float shotPower = 500f;
        [Tooltip("Casing Ejection Speed")][SerializeField] private float ejectPower = 150f;
        [Tooltip("Bullet Lifetime")][SerializeField] private float bulletLifetime = 3f;

        [Header("RayCast Settings")]
        public Camera fpsCamera;
        public float range = 100f;
        public float damage = 10f;
        public float impactForce = 60f;
        public AudioSource gunsound;
        public GameObject impactEffect;

        private void Awake()
        {
            fpsCamera = Camera.main;
        }

        private void Start()
        {
            if (barrelLocation == null)
                barrelLocation = transform;

            if (gunAnimator == null)
                gunAnimator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire1") || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                if (Object.HasStateAuthority)
                {
                    gunAnimator.SetTrigger("Fire");

                    Vector3 shootPosition = fpsCamera.transform.position;
                    Vector3 shootDirection = fpsCamera.transform.forward;
                    RPC_Shoot(shootPosition, shootDirection);
                }
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_Shoot(Vector3 shootPosition, Vector3 shootDirection)
        {
            RaycastHit hit;
            if (Physics.Raycast(shootPosition, shootDirection, out hit, range))
            {
                Debug.DrawLine(shootPosition, hit.point, Color.red, 1f);
                Debug.Log("RayCast hit!");

                MonsterSync enemy = hit.transform.GetComponent<MonsterSync>();
                if (enemy != null)
                {
                    Debug.Log("Bullet Hit Enemy!");
                    Debug.Log("Hit point: " + hit.point);

                    //Instantiate(impactEffect, hit.point, Quaternion.identity);
                    Quaternion impactRotation = Quaternion.LookRotation(hit.normal); //¼öÁ¤----------------------
                    Instantiate(impactEffect, hit.point, impactRotation);

                    enemy.TakeDamageRpc(damage);

                    Debug.Log("Enemy Current Health: " + enemy.GetCurrentHealth());
                }

                if (hit.rigidbody != null)
                {
                    Debug.Log("Bullet Hit Enemy with Rigidbody!");
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }
            }

            if (muzzleFlashPrefab)
            {
                GameObject tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
                Destroy(tempFlash, destroyTimer);
            }

            if (!bulletPrefab)
            {
                return;
            }

            GameObject bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
            Destroy(bullet, bulletLifetime);
        }


        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_CasingRelease()
        {
            if (!casingExitLocation || !casingPrefab)
            {
                return;
            }

            GameObject tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation);
            Rigidbody casingRigidbody = tempCasing.GetComponent<Rigidbody>();

            if (casingRigidbody != null)
            {
                casingRigidbody.AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower),
                    casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f, 1f);
                casingRigidbody.AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);
            }

            Destroy(tempCasing, destroyTimer);
        }
    }
}