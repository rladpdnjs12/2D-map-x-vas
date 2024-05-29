using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private float _speed; // �Ѿ� �̵� �ӵ�
    private LayerMask _asteroidLayer; // �Ѿ� �浹 ���̾�
    private GameObject _impactEffect; // �Ѿ� ���� ȿ��
    private float _damage = 10f;
    [Networked]
    private TickTimer _currentLifetime { get; set; } // �Ѿ� ���� �ð�
    

    // �Ѿ��� �ʱ�ȭ�մϴ�.
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

                 // �浹�� ��ü�� MonsterSync ������Ʈ�� ������ �ִ��� Ȯ���մϴ�.
                 MonsterSync monster = other.GetComponent<MonsterSync>();
                 if (monster != null)
                 {
                     // RPC�� ����Ͽ� ������ ü���� ���ҽ�ŵ�ϴ�.
                     monster.TakeDamageRpc(_damage);
                 }
             }
         }  
     }
}
