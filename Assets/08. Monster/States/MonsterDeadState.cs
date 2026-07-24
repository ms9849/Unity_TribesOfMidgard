using UnityEngine;

public class MonsterDeadState : MonsterState
{
    public MonsterDeadState(MonsterStateMachine FSM) : base(FSM, MonsterStateID.Dead) { }

    public override void Enter()
    {
        Monster.monsterController.StopMoving();
        Monster.monsterController.Agent.enabled = false;

        // 사망 연출을 위한 유예 시간 후 제거
        Object.Destroy(Monster.gameObject, 2f);
    }
}
