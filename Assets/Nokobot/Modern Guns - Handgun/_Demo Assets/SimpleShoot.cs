using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;
    [Tooltip("Bullet Lifetime")][SerializeField] private float bulletLifetime = 3f;

    //RayCast 추가
    public Camera fpsCamera;         // 플레이어가 조준하는 위치를 결정
    public float range = 100f;       // 광선이 얼마나 투사되는지 결정, 무기의 범위로 볼 수 있다.
    public float damage = 10f;       //적에게 주는 피해량


    public float impactForce = 60f;     // Raycast가 Rigidbody 요소가 부착된 객체와 교차하는 경우 정의되는 양만큼 힘을 가한다.
    public AudioSource gunsound;        // 총기 사운드(권총)

    //public ParticleSystem muzzleFlash;  // 총구 이펙트 파티클
    public GameObject impactEffect;     // 적 피격 파티클 (혈흔, 총알이 박힌 자국 등)


    void Awake()
    {
        // Awake 함수에서 get_main 호출
        fpsCamera = Camera.main;
    }

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //input
        if (Input.GetButtonDown("Fire1") || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //Calls animation on the gun that has the relevant animation events that will fire
            gunAnimator.SetTrigger("Fire");
        }
    }


    //This function creates the bullet behavior
    public bool Shoot()
    {
        RaycastHit hit; // 레이에서 반환된 정보를 보유하는 변수
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {   // 콜라이더를 가진 오브젝트와 충돌 여부를 체크합니다. 충돌이 되면 True를 반환하고, RaycastHit로 충돌정보를 넘겨줌.

            // 총알이 충돌하는 위치에 디버그 라인 그리기
            Debug.DrawLine(fpsCamera.transform.position, hit.point, Color.red, 1f);
            Debug.Log("RayCast hit!");

            ShootableDragon enemy = hit.transform.GetComponent<ShootableDragon>();  // 충돌을 감지한 충돌체에서 EnemyCube에 대한 컴포넌트 정보를 가져옵니다.

            if (enemy != null)  // 적이 만약 존재한다면
            {
                Debug.Log("Bullet Hit Enemy!"); // 디버그 로그 추가
                Debug.Log("Hit point: " + hit.point);

                Instantiate(impactEffect, hit.point, Quaternion.identity);  // 총을 쏜 그 위치에 피격 파티클 생성
                enemy.Damage(damage);   // enemy에게 피해량을 입힌다.

                Debug.Log("Enemy Current Health: " + enemy.GetCurrentHealth());
            }

            if (hit.rigidbody != null)  // RayCast로 적중한 게임 오브젝트에 Rigidbody 구성요소가 있다면
            {
                Debug.Log("Bullet Hit Enemy without ligidbody!"); // 디버그 로그 추가
                hit.rigidbody.AddForce(-hit.normal * impactForce); // AddForce(방향 * 힘 값)
            }

        }

        if (muzzleFlashPrefab)
        {
            //Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            //Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }

        // 총알 프리팹이 없으면 취소
        if (!bulletPrefab)
        {
            return false; // 총알 발사 실패
        }

        // 총알을 생성하고 총구 방향으로 힘을 가합니다.
        GameObject bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation) as GameObject;
        bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        // 총알 파괴 타이머 설정
        Destroy(bullet, bulletLifetime);

        return true; // 총알 발사 완료

    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }
}
