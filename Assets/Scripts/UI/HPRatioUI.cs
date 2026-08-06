using TMPro;
using UnityEngine;

public class HPRatioUI : MonoBehaviour
{
    [SerializeField]
    Health TargetHealth;
    [SerializeField]
    GameObject TargetObject;
    [SerializeField]
    TextMeshProUGUI TargetText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        float CurrentHP = TargetHealth.CurrentHp;
        float MaxHP = TargetHealth.MaxHp;

        TargetText.text = "( " + CurrentHP + " / " + MaxHP + " )";   
    }

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
    }
}
