using UnityEngine;

public class Equpments : MonoBehaviour
{
    [SerializeField]
    EQUIP_TYPE EquipType;
    [SerializeField]
    GameObject Mesh;
    // 원본 FBX 축변환만으로는 이 손 소켓 기준 방향이 안 맞는 모델을 위한 추가 보정 회전.
    [SerializeField]
    Vector3 MeshRotationOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Mesh != null)
        {
            GameObject Instance = Instantiate(Mesh, transform);
            Instance.transform.localRotation *= Quaternion.Euler(MeshRotationOffset);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
