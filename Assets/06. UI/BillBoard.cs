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
        transform.rotation = CamTransform.rotation;
    }
}
