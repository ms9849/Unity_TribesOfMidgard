using UnityEngine;

public enum PLAYER_WEAPON {
    NAKED,
    SWORD,
    BOW,
    END
};

public class Player : MonoBehaviour
{
    public Animator playerAnimator { get; private set; }
    public PlayerController playerController { get; private set; }
    PlayerStateMachine playerFSM;
   
    void Awake()
    {
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
        playerFSM.Update();
    }
}
