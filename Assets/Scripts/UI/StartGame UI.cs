using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneUI : MonoBehaviour
{
    [SerializeField] private string MainSceneName = "MainScene";


    public void OnClickStart()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnClickQuit()
    {
        //디버깅용 분기
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }


}
