using UnityEngine;
using UnityEngine.UI;

public class NPCInteractionUI : MonoBehaviour
{
    public static NPCInteractionUI Instance { get; private set; }

    [SerializeField] private Vector3 Offset = new Vector3(0f, 3.5f, 0f);

    private Text NPCNameText;

    void Awake()
    {
        Instance = this;
        NPCNameText = transform.Find("NPCName").GetComponent<Text>();

        gameObject.SetActive(false);
    }

    public void Show(Transform npcTransform, string npcName)
    {
        transform.position = npcTransform.position + Offset;
        NPCNameText.text = npcName;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
