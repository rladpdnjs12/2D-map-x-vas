using Fusion;
using UnityEngine;


namespace ARScripts
{
    public class Bullet : NetworkBehaviour
    {
        private float _speed; // 총알 이동 속도
        private LayerMask _asteroidLayer; // 총알 충돌 레이어
        private GameObject _impactEffect; // 총알 폭발 효과
        private float _damage = 10f;
        private NetworkShoot _networkShoot;

        [Networked]
        private TickTimer _currentLifetime { get; set; } // 총알 생존 시간


        // 총알을 초기화합니다.
        public void Init(float speed, LayerMask asteroidLayer, GameObject impactEffect, TickTimer currentLifetime, NetworkShoot networkShoot)
        {
            _speed = speed;
            _asteroidLayer = asteroidLayer;
            _impactEffect = impactEffect;
            _currentLifetime = currentLifetime;
            _networkShoot = networkShoot; // NetworkShoot 인스턴스 할당
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

                    // NetworkShoot의 RPC_Shoot 호출
                    if (_networkShoot != null)
                    {
                        Vector3 shootPosition = _networkShoot.fpsCamera.transform.position;
                        Vector3 shootDirection = _networkShoot.fpsCamera.transform.forward;
                        _networkShoot.RPC_Shoot(shootPosition, shootDirection);
                    }
                }
            }
        }
    }
}
