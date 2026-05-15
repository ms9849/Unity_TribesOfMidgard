using Unity.VisualScripting;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Transform CamTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CamTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 TargetPosition = new Vector3(CamTransform.position.x, transform.position.y , CamTransform.position.z);
        transform.LookAt(TargetPosition);
    }
}
