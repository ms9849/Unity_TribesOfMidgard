using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/*
체력 UI 스크립트.
타겟 설정은.. 몬스터 내부에서?
*/

public class HPUI : BaseUI
{
    [SerializeField] private Image fillImage;

    public GameObject TargetObject;
    public Health TargetHealth { get; protected set; }
    public float HPRatio { get; protected set; }
    
    [SerializeField]
    bool isWorldUI = true;
    [SerializeField]
    public Vector3 TransformOffset;
    void Start()
    {
        if (TargetObject == null)
            SetTarget(GetComponentInParent<Health>()?.gameObject);
        else
            SetTarget(TargetObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(true == isWorldUI)
            UpdatePosition();
        CalcTargetHPRatio();
    }

    void UpdatePosition()
    {
        if (TargetObject == null)
            return;

        transform.position = TargetObject.transform.position + TransformOffset;
    }

    //타겟을 설정하거나 변경하는 함수.
    public void SetTarget(GameObject target)
    {
        TargetObject = target;
        if (null == TargetObject ||
            false == target.TryGetComponent<Health>(out Health healthCom))
        {
            TargetHealth = null;
            return;
        }

        TargetHealth = healthCom;
        CalcTargetHPRatio();
    }

    void CalcTargetHPRatio()
    {
        if (TargetHealth == null)
            return;

        HPRatio = TargetHealth.MaxHp > 0f ? TargetHealth.CurrentHp / TargetHealth.MaxHp : 0f;

        if (fillImage != null)
            fillImage.fillAmount = HPRatio;
    }
}
