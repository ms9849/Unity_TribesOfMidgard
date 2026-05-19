using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    [SerializeField]
    public InteractionSO InteractionData;

    private Canvas InteractionUI;
    private Text InteractorText;
    private Text InteractionText;
    private Image InteractionSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InteractionUI = GetComponentInChildren<Canvas>();
        InteractionUI.transform.position = InteractionUI.transform.parent.position + Vector3.up * 2f; 

        InteractorText = InteractionUI.transform.Find("InteractorName").GetComponent<Text>();
        InteractionText = InteractionUI.transform.Find("InteractName").GetComponent<Text>();
        InteractionSprite = InteractionUI.transform.Find("_Type").GetComponent<Image>();

        InteractorText.text = InteractionData.InteractorName;
        InteractionText.text = InteractionData.InteractName;
        InteractionSprite.sprite = InteractionData.SpriteInfo;

        InteractionUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player.CurrentInteractionObject != null)
            {
                player.CurrentInteractionObject.InteractionUI.gameObject.SetActive(false);
            }

            if (gameObject.activeSelf == true)
            {
                player.CurrentInteractionObject = this;
            }

            InteractionUI.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player.CurrentInteractionObject == this)
                player.CurrentInteractionObject = null;

            InteractionUI.gameObject.SetActive(false);
        }
    }

}
