using UnityEngine;

[RequireComponent(typeof(MonsterController))]
[RequireComponent(typeof(Health))]
public class Monster : MonoBehaviour
{
    public MonsterController monsterController { get; private set; }
    public Health monsterHealth { get; private set; }
    public IMonsterAttack monsterAttack { get; private set; }
    public Animator monsterAnimator { get; private set; }
    MonsterStateMachine monsterFSM;

    void Awake()
    {
        monsterController = GetComponent<MonsterController>();
        monsterHealth = GetComponent<Health>();
        monsterAttack = GetComponent<IMonsterAttack>();
        monsterAnimator = GetComponentInChildren<Animator>();
        monsterFSM = new MonsterStateMachine(this);
    }

    void Start()
    {
        monsterHealth.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        if (monsterHealth != null)
            monsterHealth.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        monsterFSM.ChangeState(MonsterStateID.Dead);
    }

    void Update()
    {
        monsterFSM.Update();
    }

    void FixedUpdate()
    {
        monsterFSM.FixedUpdate();
    }
}
