using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator playerAnimator { get; private set; }
    public PlayerController playerController { get; private set; }
    public Inventory playerInventory { get; private set; }
    PlayerStateMachine playerFSM;

    /*TEST CODE */
    [Header("Test SO")]
    public ItemSO TestWood;

    /*TEST CODE */
    [Header("WEAPON SO")]
    public ItemSO TestWeapon;
    
    /* ***** */
    void Awake()
    {
        playerInventory = GetComponent<Inventory>();
        playerController = GetComponent<PlayerController>();
        playerAnimator = GetComponent<Animator>();
        playerFSM = new PlayerStateMachine(this);
    }

    void Start()
    {
        Debug.Log($"[플레이어 초기화] 인스턴스 이름: {gameObject.name} | 컨트롤러 주소: {playerController.GetHashCode()}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerInventory.AddItem(TestWood, 1);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            playerInventory.AddItem(TestWeapon, 1);
        }


        playerFSM.Update();
    }
}
