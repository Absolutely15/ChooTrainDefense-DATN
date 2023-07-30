using UnityEngine;

public class AutoRotateDeltaTime : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    private Transform trans;

    private void Start()
    {
        trans = transform;
    }

    private void Update()
    {
        trans.Rotate(rotationAxis * (rotationSpeed * Time.deltaTime), Space.Self);
    }
}