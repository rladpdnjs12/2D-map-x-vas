using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSync : NetworkBehaviour
{
    [Networked]
    private Vector3 _position { get; set; }
    [Networked]
    private Quaternion _rotation { get; set; }

    [Networked]
    public float Health { get; set; } = 30;

    private Animator _animator;

    public override void Spawned()
    {
        base.Spawned();

        // ARLocation 오브젝트 찾기
        GameObject arLocation = GameObject.Find("ARLocation");

        if (arLocation != null)
        {
            // 이 몬스터를 ARLocation의 자식으로 설정
            transform.SetParent(arLocation.transform);
        }
        else
        {
            Debug.LogError("ARLocation 오브젝트를 찾을 수 없습니다.");
        }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        if(_animator == null)
        {
            if (_animator == null)
            {
                Debug.LogError("Animator component is missing from the GameObject.");
            }
        }
    }

    private void Update()
    {
        if (Object == null)
        {
            Debug.LogError("NetworkObject is not initialized.");
            return;
        }

        if (Object.HasInputAuthority)
        {
            _position = transform.position;
            _rotation = transform.rotation;
        }
        else
        {
            transform.position = _position;
            transform.rotation = _rotation;
        }
    }

    // 체력을 감소시키는 메서드, 다른 클라이언트에서 호출할 수 있도록 RPC로 구현
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void TakeDamageRpc(float damage)
    {
        if (Health > 0)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        // 몬스터가 죽었을 때의 로직, 예를 들면 파괴 애니메이션 실행, 게임 오브젝트 삭제 등
        Debug.Log("Monster died.");
        gameObject.SetActive(false);
        StartCoroutine(DeactivateAfterAnimation());
        //Runner.Despawn(Object);
    }

    private IEnumerator DeactivateAfterAnimation()
    {
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
    }

    // 이벤트를 시각화하기 위한 간단한 GUI 표시
    private void OnGUI()
    {
        //GUI.Label(new Rect(10, 10, 100, 20), "Health: " + Health);

        if (Object != null && Object.IsSpawnable)
        {
            GUI.Label(new Rect(10, 10, 100, 20), "Health: " + Health);
        }
        else
        {
            GUI.Label(new Rect(10, 10, 150, 20), "Monster not spawned yet.");
        }
    }

}
