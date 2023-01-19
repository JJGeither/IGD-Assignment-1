using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform followTarget;

    public float smoothingValue = 0.125f;
    public Vector3 offset;
    public Quaternion rotation;

    //input variables
    public KeyCode lookLeft;
    public KeyCode lookRight;

    // Update is called once per frame
    void LateUpdate()
    {
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * 2, Vector3.up) * offset;
        transform.position = followTarget.position + offset;
        transform.LookAt(followTarget.position);
    }
}
