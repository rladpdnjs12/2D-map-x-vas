using Fusion;
using UnityEngine;

public class SetPlayer : NetworkBehaviour
{
    public Camera Camera;

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            transform.position = Camera.main.transform.position; 
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Camera = Camera.main;
        }
    }
}
