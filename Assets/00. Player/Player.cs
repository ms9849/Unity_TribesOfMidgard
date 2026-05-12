using UnityEngine;

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
    }

    void Update()
    {
        playerFSM.Update();
    }
}
