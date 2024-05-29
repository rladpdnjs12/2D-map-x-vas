using Fusion;
using UnityEngine;

public class Casing : NetworkBehaviour
{
    private float ejectForce = 10f;  // ź�� ���� ��
    private float destroyTimer = 2f; // ź�� �ı� Ÿ�̸�

    public override void FixedUpdateNetwork()
    {
        // ź���� �̵��� ó���մϴ�.
        MoveCasing();
    }

    // ź�Ǹ� �̵���ŵ�ϴ�.
    private void MoveCasing()
    {
        // ź�Ǵ� �׳� ������ �̵��ϵ��� �����մϴ�.
        transform.Translate(Vector3.forward * ejectForce * Runner.DeltaTime, Space.Self);
    }
    private void Start()
    {
        // ź�Ǹ� ������ ��, ������ Ÿ�̸� ���Ŀ� �ı��մϴ�.
        Destroy(gameObject, destroyTimer);
    }
}
