using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 v3;

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + v3;
    }
}
