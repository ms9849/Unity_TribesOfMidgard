using UnityEngine;

public class YggDrasilEvent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameObject YggDrasilHP;
    
    void Start()
    {
        YggDrasilHP = transform.Find("YggDrasilHPUI").gameObject;

        YggDrasilHP.SetActive(false);
        
        GameTimeManager.Instance.OnDayStart += SetUIInActive; 
        GameTimeManager.Instance.OnNightStart += SetUIActive;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnDestroy() 
    {
        GameTimeManager.Instance.OnDayStart -= SetUIInActive; 
        GameTimeManager.Instance.OnNightStart -= SetUIActive;
    }

    void SetUIActive()
    {
        YggDrasilHP.SetActive(true);
    }
    void SetUIInActive()
    {
        YggDrasilHP.SetActive(false);
    }
}
