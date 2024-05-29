using Fusion;
using UnityEngine;

public class NetworkBullet : NetworkBehaviour
{
    // 설정 값들
    [Tooltip("총알이 살아있을 최대 시간")]
    [SerializeField] private float _maxLifetime = 3.0f;
    [Tooltip("총알의 이동 속도")]
    [SerializeField] private float _speed = 200.0f;
    [Tooltip("총알의 충돌 레이어")]
    [SerializeField] private LayerMask _asteroidLayer;

    [Tooltip("총알이 폭발할 때 생성되는 효과")]
    [SerializeField] private GameObject _impactEffect;

    // 총알 생존 시간을 카운트 다운합니다. 상태 권한이 변경되면 네트워크화됩니다 (연결 끊김으로 인한).
    [Networked] private TickTimer _currentLifetime { get; set; }


    public override void Spawned()
    {
        if (Object.HasStateAuthority == false) return;

        // 네트워크 매개 변수는 호스트에 의해 초기화됩니다. 이것들은 변수가 [Networked]이기 때문에 클라이언트로 전파됩니다.
        _currentLifetime = TickTimer.CreateFromSeconds(Runner, _maxLifetime);
    }

    public override void FixedUpdateNetwork()
    {
        // 총알이 몬스터에 충돌하지 않았으면 전진합니다.
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

    // 총알이 수명을 초과했는지 확인하고, 초과했을 경우 제거합니다.
    private void CheckLifetime()
    {
        if (_currentLifetime.Expired(Runner) == false) return;

        Runner.Despawn(Object);
    }


    // 다음 틱에서 총알이 몬스터에 충돌할 것인지 확인합니다.
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

                // 적중한 몬스터가 있으면 총알 폭발 효과 생성
                Instantiate(_impactEffect, hit.point, Quaternion.identity);

                // 몬스터 체력바에서 체력 감소 코드 추가

                return true;
            }
        }
        return false;
    }
}

