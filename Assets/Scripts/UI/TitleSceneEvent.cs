using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneEvent : MonoBehaviour
{
    [SerializeField] private string MainSceneName = "MainScene";

    
    public void Start()
    {
        SoundManager.Instance.PlayBGM("BGM_TitleMenu", 0);
    }

    public void OnClickStartButton()
    {
        SceneLoader.Instance.LoadNextScene(MainSceneName);
        SoundManager.Instance.StopBGM(0);
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
