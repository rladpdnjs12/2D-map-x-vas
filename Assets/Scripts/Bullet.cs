using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private float _speed; // 총알 이동 속도
    private LayerMask _asteroidLayer; // 총알 충돌 레이어
    private GameObject _impactEffect; // 총알 폭발 효과
    private float _damage = 10f;
    [Networked]
    private TickTimer _currentLifetime { get; set; } // 총알 생존 시간
    

    // 총알을 초기화합니다.
    public void Init(float speed, LayerMask asteroidLayer, GameObject impactEffect, TickTimer currentLifetime)
     {
         _speed = speed;
         _asteroidLayer = asteroidLayer;
         _impactEffect = impactEffect;
         _currentLifetime = currentLifetime;
    }

     public override void FixedUpdateNetwork()
     {
         if (Object.HasStateAuthority)
         {
             // 총알의 이동을 처리합니다.
             MoveBullet();

             // 총알의 수명을 확인하고, 만약 수명이 초과되었다면 제거합니다.
             if (_currentLifetime.Expired(Runner))
             {
                 Runner.Despawn(Object);
             }
         }
     }

    // 총알을 이동시킵니다.
    private void MoveBullet()
    {
        transform.Translate(Vector3.forward * _speed * Runner.DeltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
     {
         //충돌한 객체가 몬스터이면 피격 효과
         if ((_asteroidLayer.value & (1 << other.gameObject.layer)) != 0)
         {
             if ((_asteroidLayer.value & (1 << other.gameObject.layer)) != 0)
             {
                 if (_impactEffect != null)
                 {
                     Instantiate(_impactEffect, transform.position, Quaternion.identity);
                 }

                 // 총알을 제거합니다.
                 Runner.Despawn(Object);

                 // 충돌한 객체가 MonsterSync 컴포넌트를 가지고 있는지 확인합니다.
                 MonsterSync monster = other.GetComponent<MonsterSync>();
                 if (monster != null)
                 {
                     // RPC를 사용하여 몬스터의 체력을 감소시킵니다.
                     monster.TakeDamageRpc(_damage);
                 }
             }
         }  
     }
}
