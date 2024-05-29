using UnityEngine;
public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        // 에디터에서 실행 중일 경우 UnityEditor를 사용하여 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서 실행 중일 경우 응용 프로그램 종료
        Application.Quit();
#endif
    }
}
