using Fusion;
using UnityEngine;


namespace ARScripts
{
    public class Bullet : NetworkBehaviour
    {
        private float _speed; // �Ѿ� �̵� �ӵ�
        private LayerMask _asteroidLayer; // �Ѿ� �浹 ���̾�
        private GameObject _impactEffect; // �Ѿ� ���� ȿ��
        private float _damage = 10f;
        private NetworkShoot _networkShoot;

        [Networked]
        private TickTimer _currentLifetime { get; set; } // �Ѿ� ���� �ð�


        // �Ѿ��� �ʱ�ȭ�մϴ�.
        public void Init(float speed, LayerMask asteroidLayer, GameObject impactEffect, TickTimer currentLifetime, NetworkShoot networkShoot)
        {
            _speed = speed;
            _asteroidLayer = asteroidLayer;
            _impactEffect = impactEffect;
            _currentLifetime = currentLifetime;
            _networkShoot = networkShoot; // NetworkShoot �ν��Ͻ� �Ҵ�
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                // �Ѿ��� �̵��� ó���մϴ�.
                MoveBullet();

                // �Ѿ��� ������ Ȯ���ϰ�, ���� ������ �ʰ��Ǿ��ٸ� �����մϴ�.
                if (_currentLifetime.Expired(Runner))
                {
                    Runner.Despawn(Object);
                }
            }
        }

        // �Ѿ��� �̵���ŵ�ϴ�.
        private void MoveBullet()
        {
            transform.Translate(Vector3.forward * _speed * Runner.DeltaTime, Space.Self);
        }

        private void OnTriggerEnter(Collider other)
        {
            //�浹�� ��ü�� �����̸� �ǰ� ȿ��
            if ((_asteroidLayer.value & (1 << other.gameObject.layer)) != 0)
            {
                if ((_asteroidLayer.value & (1 << other.gameObject.layer)) != 0)
                {
                    if (_impactEffect != null)
                    {
                        Instantiate(_impactEffect, transform.position, Quaternion.identity);
                    }

                    // �Ѿ��� �����մϴ�.
                    Runner.Despawn(Object);

                    // NetworkShoot�� RPC_Shoot ȣ��
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
