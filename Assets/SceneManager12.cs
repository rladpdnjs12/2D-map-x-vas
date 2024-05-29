using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;
using UnityEngine.SceneManagement;

public class SceneManager12 : NetworkBehaviour, INetworkSceneManager
{
    public bool IsBusy => throw new System.NotImplementedException();

    public Scene MainRunnerScene => throw new System.NotImplementedException();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void loadscene()
    {
        // Reloading scene 2.
        if (Runner.IsSceneAuthority)
        {
            Runner.UnloadScene(SceneRef.FromIndex(1));
            Runner.LoadScene(SceneRef.FromIndex(0), LoadSceneMode.Additive);
        }
    }

    public void Initialize(NetworkRunner runner)
    {
        throw new System.NotImplementedException();
    }

    public void Shutdown()
    {
        throw new System.NotImplementedException();
    }

    public bool IsRunnerScene(Scene scene)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetPhysicsScene2D(out PhysicsScene2D scene2D)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetPhysicsScene3D(out PhysicsScene scene3D)
    {
        throw new System.NotImplementedException();
    }

    public void MakeDontDestroyOnLoad(GameObject obj)
    {
        throw new System.NotImplementedException();
    }

    public bool MoveGameObjectToScene(GameObject gameObject, SceneRef sceneRef)
    {
        throw new System.NotImplementedException();
    }

    public NetworkSceneAsyncOp LoadScene(SceneRef sceneRef, NetworkLoadSceneParameters parameters)
    {
        throw new System.NotImplementedException();
    }

    public NetworkSceneAsyncOp UnloadScene(SceneRef sceneRef)
    {
        throw new System.NotImplementedException();
    }

    public SceneRef GetSceneRef(GameObject gameObject)
    {
        throw new System.NotImplementedException();
    }

    public SceneRef GetSceneRef(string sceneNameOrPath)
    {
        throw new System.NotImplementedException();
    }

    public bool OnSceneInfoChanged(NetworkSceneInfo sceneInfo, NetworkSceneInfoChangeSource changeSource)
    {
        throw new System.NotImplementedException();
    }
}
