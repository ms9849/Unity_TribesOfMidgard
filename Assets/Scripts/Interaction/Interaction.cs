using TMPro;
using Unity.VisualScripting;
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
    private TextMeshProUGUI NotifyText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InteractionUI = GetComponentInChildren<Canvas>();
        InteractionUI.transform.position = InteractionUI.transform.parent.position + Vector3.up * 2f; 

        NotifyText = InteractionUI.transform.Find("NotifyText").GetComponent<TextMeshProUGUI>();
        NotifyText.enabled = false;

        InteractorText = InteractionUI.transform.Find("InteractorName").GetComponent<Text>();
        InteractionText = InteractionUI.transform.Find("InteractName").GetComponent<Text>();
        InteractionSprite = InteractionUI.transform.Find("_Type").GetComponent<Image>();

        InteractorText.text = InteractionData.InteractorName;
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

            CheckIACondition(player);
            InteractionUI.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player.CurrentInteractionObject == this)
            {
                player.CurrentInteractionObject = null;
                player.isInteractKeyPressed = false;
            }

            InteractionUI.gameObject.SetActive(false);
        }
    }

    /*
    나무, 돌 채집이라면 상호작용이 가능한지 상태 체크하고 들어가야함.
    */
    void CheckIACondition(PlayerController playerController)
    {
        switch(InteractionData.InteractionType)
        {
            case INTERACTION_TYPE.WOOD:
                if(playerController.IsAxeEquipped())
                {
                    InteractionText.color = new Color32(235,222,186,255);
                    NotifyText.enabled = false;
                }
                else
                {
                    InteractionText.color = new Color32(255,0,0,255);
                    NotifyText.enabled = true;
                    NotifyText.text = "필요 도구: 도끼";
                }
                break;
            case INTERACTION_TYPE.STONE:
                if(playerController.IsPickaxeEquipped())
                {
                    InteractionText.color = new Color32(235,222,186,255);
                    NotifyText.enabled = false;
                }
                else
                {
                        InteractionText.color = new Color32(255,0,0,255);
                    NotifyText.enabled = true;
                    NotifyText.text = "필요 도구: 곡괭이";
                }
                break;
            default:
                break;
        }
    }
}
