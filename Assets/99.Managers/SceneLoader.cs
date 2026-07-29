using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    [SerializeField]
    private GameObject LoadingUIPrefab;
    private GameObject loadingUIInstance;
    private Image progressBarFill;
    private bool isLoading = false;
    private AsyncOperation Op = null;
    public float LoadingRatio { get; private set; }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        loadingUIInstance = Instantiate(LoadingUIPrefab, transform);
        foreach(Image image in loadingUIInstance.GetComponentsInChildren<Image>(true))
        {
            if(image.name == "ProgressBarFill")
            {
                progressBarFill = image;
                break;
            }
        }
        loadingUIInstance.SetActive(false);
    }

    void Update()
    {
        //로딩 중이라면,
        if(true == isLoading)
        {
            CalcLoadingRatio();
        }
    }

    public void LoadNextScene(string nextSceneName)
    {
        if(true == isLoading)
            return;

        isLoading = true;
        StartCoroutine(LoadSceneRoutine(nextSceneName));
    }

    private IEnumerator LoadSceneRoutine(string nextSceneName)
    {
        SetSceneObjectsActive(SceneManager.GetActiveScene(), false);
        loadingUIInstance.SetActive(true);
        yield return null; //로딩 UI가 최소 한 프레임 렌더링되도록 대기

        Op = SceneManager.LoadSceneAsync(nextSceneName);

        if(Op == null)
        {
            isLoading = false;
            yield break;
        }

        Op.completed += OnLoadCompleted;
    }

    private void SetSceneObjectsActive(Scene scene, bool active)
    {
        foreach(GameObject root in scene.GetRootGameObjects())
        {
            if(root.GetComponentInChildren<Camera>(true) != null)
                continue;

            root.SetActive(active);
        }
    }

    private void CalcLoadingRatio()
    {
        if(Op == null)
            return;

        LoadingRatio = Op.progress;
        progressBarFill.fillAmount = LoadingRatio;
    }

    private void OnLoadCompleted(AsyncOperation op)
    {
        Debug.Log("로딩 완료");
        isLoading = false;
        Op = null;
        loadingUIInstance.SetActive(false);
    }
}
