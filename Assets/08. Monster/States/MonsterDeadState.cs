using UnityEngine;

public class MonsterDeadState : MonsterState
{
    public MonsterDeadState(MonsterStateMachine FSM) : base(FSM, MonsterStateID.Dead) { }

    public override void Enter()
    {
        Monster.monsterController.StopMoving();
        Monster.monsterController.Agent.enabled = false;

        switch(Monster.monsterName)
        {
            case "Monster_FireGiant":
                SoundManager.Instance.PlaySFX("Death_Firegiant", 3, 0.2f);
                break;
            case  "Monster_Dog":
                SoundManager.Instance.PlaySFX("Death_dogling", 3, 0.2f);
                break;
            default:
                break;
        }

        // 디졸브 연출 후 제거. FX 컴포넌트가 없으면 유예 시간 후 즉시 제거.
        MonsterRenderFX RenderFX = Monster.GetComponent<MonsterRenderFX>();
        if (RenderFX != null)
            RenderFX.PlayDeathDissolve();
        else
            Object.Destroy(Monster.gameObject, 2f);
    }
}
