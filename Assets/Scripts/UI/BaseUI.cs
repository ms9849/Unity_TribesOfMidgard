using UnityEngine;

// 모든 UI가 상속받는 기반 클래스. isActive로 활성/비활성을 토글합니다.
public abstract class BaseUI : MonoBehaviour
{
    [Header("Base UI")]
    [SerializeField] private bool isActive = true;

    public bool IsActive => isActive;

    protected virtual void Awake()
    {
        ApplyActiveState();
    }

    public virtual void SetActive(bool active)
    {
        isActive = active;
        ApplyActiveState();
    }

    public void ToggleActive()
    {
        SetActive(!isActive);
    }

    private void ApplyActiveState()
    {
        gameObject.SetActive(isActive);
    }
}
