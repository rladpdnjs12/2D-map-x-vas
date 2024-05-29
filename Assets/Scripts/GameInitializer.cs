using Fusion;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    public GameObject playerPrefab;
    public GameObject monsterSpawnerPrefab;

    private NetworkRunner _runner;

    private async void Start()
    {
        _runner = Instantiate(networkRunnerPrefab);
        _runner.ProvideInput = true;  // ProvideInput을 명시적으로 설정

        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TestRoom",
            SceneManager = _runner.GetComponent<NetworkSceneManagerDefault>()
        };

        var result = await _runner.StartGame(startGameArgs);
        if (result.Ok)
        {
            Debug.Log("Game started successfully");
            if (_runner.GameMode == GameMode.Host)
            {
                Instantiate(monsterSpawnerPrefab);
            }
        }
        else
        {
            Debug.LogError($"Failed to start game: {result.ErrorMessage}");
        }
    }

    public void OnPlayerJoined(PlayerRef player)
    {
        if (player == _runner.LocalPlayer)
        {
            _runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
        }
    }
}
