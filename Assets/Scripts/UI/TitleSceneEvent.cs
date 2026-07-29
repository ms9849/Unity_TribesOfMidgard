using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneEvent : MonoBehaviour
{
    [SerializeField] private string MainSceneName = "MainScene";


    public void OnClickStartButton()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnClickExitButton()
    {
        //디버깅용 분기
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }


}
