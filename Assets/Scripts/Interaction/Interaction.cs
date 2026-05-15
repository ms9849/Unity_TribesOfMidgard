using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    [SerializeField]
    private InteractionSO InteractionData;

    private Canvas InteractionUI;
    private Text InteractorText;
    private Text InteractionText;
    private Image InteractionSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("--- Interaction Start 실행됨 ---");

        InteractionUI = GetComponentInChildren<Canvas>();
        InteractionUI.transform.position = InteractionUI.transform.parent.position + Vector3.up * 2f; 

        InteractorText = InteractionUI.transform.Find("InteractorName").GetComponent<Text>();
        InteractionText = InteractionUI.transform.Find("InteractName").GetComponent<Text>();
        InteractionSprite = InteractionUI.transform.Find("_Type").GetComponent<Image>();

        InteractorText.text = InteractionData.InteractorName;
        InteractionText.text = InteractionData.InteractName;
        InteractionSprite.sprite = InteractionData.SpriteInfo;

        Debug.Log(InteractionData.InteractorName);
        Debug.Log(InteractionData.InteractName);
        Debug.Log(InteractionData.SpriteInfo);

    }

    // Update is called once per frame
    void Update()
    {
    }
}
