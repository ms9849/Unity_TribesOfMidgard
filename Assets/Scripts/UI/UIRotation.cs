using UnityEngine;

public class UIRotation : MonoBehaviour
{
    Transform ObjectTransform;
    [Header("Angle Per Second")]
    [SerializeField]
    float AnglePerSec;
    [Header("Rotation Axis By Vector3")]
    [SerializeField]
    Vector3 RotationAxis;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ObjectTransform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        ObjectTransform.Rotate(RotationAxis, AnglePerSec * Time.deltaTime);
    }
}
