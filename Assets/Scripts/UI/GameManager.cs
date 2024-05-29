using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameManager : MonoBehaviour
    {
        public void StartGame()
        {
            // �� �ε带 ���� ������ �����մϴ�. "GameScene"�� ���� ���� ���� �̸��Դϴ�.
            SceneManager.LoadScene("GameScene");
        }

        public void QuitGame()
        {
            // ���ø����̼� ����
            Application.Quit();

            // �����Ϳ����� �� �ڵ带 ����� ������ ������ �����մϴ�.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
