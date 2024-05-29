using UnityEngine;
public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        // �����Ϳ��� ���� ���� ��� UnityEditor�� ����Ͽ� ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ����� ���ӿ��� ���� ���� ��� ���� ���α׷� ����
        Application.Quit();
#endif
    }
}
