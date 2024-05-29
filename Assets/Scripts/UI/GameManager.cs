using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameManager : MonoBehaviour
    {
        public void StartGame()
        {
            // 씬 로드를 통해 게임을 시작합니다. "GameScene"은 실제 게임 씬의 이름입니다.
            SceneManager.LoadScene("GameScene");
        }

        public void QuitGame()
        {
            // 애플리케이션 종료
            Application.Quit();

            // 에디터에서는 이 코드를 사용해 에디터 실행을 중지합니다.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
