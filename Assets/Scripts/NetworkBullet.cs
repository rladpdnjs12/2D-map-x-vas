using Fusion;
using UnityEngine;

public class NetworkBullet : NetworkBehaviour
{
    // ���� ����
    [Tooltip("�Ѿ��� ������� �ִ� �ð�")]
    [SerializeField] private float _maxLifetime = 3.0f;
    [Tooltip("�Ѿ��� �̵� �ӵ�")]
    [SerializeField] private float _speed = 200.0f;
    [Tooltip("�Ѿ��� �浹 ���̾�")]
    [SerializeField] private LayerMask _asteroidLayer;

    [Tooltip("�Ѿ��� ������ �� �����Ǵ� ȿ��")]
    [SerializeField] private GameObject _impactEffect;

    // �Ѿ� ���� �ð��� ī��Ʈ �ٿ��մϴ�. ���� ������ ����Ǹ� ��Ʈ��ũȭ�˴ϴ� (���� �������� ����).
    [Networked] private TickTimer _currentLifetime { get; set; }


    public override void Spawned()
    {
        if (Object.HasStateAuthority == false) return;

        // ��Ʈ��ũ �Ű� ������ ȣ��Ʈ�� ���� �ʱ�ȭ�˴ϴ�. �̰͵��� ������ [Networked]�̱� ������ Ŭ���̾�Ʈ�� ���ĵ˴ϴ�.
        _currentLifetime = TickTimer.CreateFromSeconds(Runner, _maxLifetime);
    }

    public override void FixedUpdateNetwork()
    {
        // �Ѿ��� ���Ϳ� �浹���� �ʾ����� �����մϴ�.
        if (HasHitAsteroid() == false)
        {
            transform.Translate(transform.forward * _speed * Runner.DeltaTime, Space.World);
        }
        else
        {
            Runner.Despawn(Object);
            return;
        }

        CheckLifetime();
    }

    // �Ѿ��� ������ �ʰ��ߴ��� Ȯ���ϰ�, �ʰ����� ��� �����մϴ�.
    private void CheckLifetime()
    {
        if (_currentLifetime.Expired(Runner) == false) return;

        Runner.Despawn(Object);
    }


    // ���� ƽ���� �Ѿ��� ���Ϳ� �浹�� ������ Ȯ���մϴ�.
    public bool HasHitAsteroid()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _speed * Runner.DeltaTime * 1.25f, _asteroidLayer))
        {
            var asteroidBehaviour = hit.collider.GetComponent<SimpleShoot>();
            if (asteroidBehaviour)
            {
                var hasHit = asteroidBehaviour.Shoot();
                if (!hasHit) return false;

                // ������ ���Ͱ� ������ �Ѿ� ���� ȿ�� ����
                Instantiate(_impactEffect, hit.point, Quaternion.identity);

                // ���� ü�¹ٿ��� ü�� ���� �ڵ� �߰�

                return true;
            }
        }
        return false;
    }
}

