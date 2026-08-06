using UnityEngine;
using UnityEngine.EventSystems;

public class UISoundTrigger : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySFX("ButtonAct", 2, 0.3f);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySFX("ButtonAct", 2, 0.3f);
    }
}
