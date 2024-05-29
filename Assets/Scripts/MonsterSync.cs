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

        // ARLocation ������Ʈ ã��
        GameObject arLocation = GameObject.Find("ARLocation");

        if (arLocation != null)
        {
            // �� ���͸� ARLocation�� �ڽ����� ����
            transform.SetParent(arLocation.transform);
        }
        else
        {
            Debug.LogError("ARLocation ������Ʈ�� ã�� �� �����ϴ�.");
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

    // ü���� ���ҽ�Ű�� �޼���, �ٸ� Ŭ���̾�Ʈ���� ȣ���� �� �ֵ��� RPC�� ����
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
        // ���Ͱ� �׾��� ���� ����, ���� ��� �ı� �ִϸ��̼� ����, ���� ������Ʈ ���� ��
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

    // �̺�Ʈ�� �ð�ȭ�ϱ� ���� ������ GUI ǥ��
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
