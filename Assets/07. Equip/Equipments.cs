using UnityEngine;

public class Equpments : MonoBehaviour
{
    [SerializeField]
    EQUIP_TYPE EquipType;
    [SerializeField]
    GameObject Mesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Mesh != null)
            Instantiate(Mesh, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
