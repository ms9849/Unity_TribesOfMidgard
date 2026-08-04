using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// GameTimeManager의 낮/밤 전환 이벤트에 맞춰 알림 텍스트를 띄웁니다.
public class NotifyUI : MonoBehaviour
{
    public static NotifyUI Instance { get; private set; }

    [SerializeField] private float DisplayDuration = 3f;

    private Text NotifyText;
    private Coroutine HideRoutine;

    void Awake()
    {
        Instance = this;
        NotifyText = GetComponentInChildren<Text>(true);
        NotifyText.gameObject.SetActive(false);
    }

    void Start()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnDayStart += HandleDayStart;
            GameTimeManager.Instance.OnNightStart += HandleNightStart;
        }
    }

    void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnDayStart -= HandleDayStart;
            GameTimeManager.Instance.OnNightStart -= HandleNightStart;
        }
    }

    private void HandleDayStart()
    {
        Show("새 아침이 밝았습니다!");
        SoundManager.Instance.PlayBGM("BGM_Day", 0);  
    }

    private void HandleNightStart()
    {
        Show("밤이 찾아옵니다..");
        SoundManager.Instance.PlayBGM("BGM_Night", 0);   
    }

    public void Show(string message)
    {
        NotifyText.text = message;
        NotifyText.gameObject.SetActive(true);

        if (HideRoutine != null)
            StopCoroutine(HideRoutine);
        HideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(DisplayDuration);
        NotifyText.gameObject.SetActive(false);
        HideRoutine = null;
    }
}
