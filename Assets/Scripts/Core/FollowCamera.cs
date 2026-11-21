using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5f, -9f);
    public float followSmoothness = 10f;
    public float lookSmoothness = 12f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothness * Time.deltaTime);

        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position + Vector3.up * 1.5f);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, lookSmoothness * Time.deltaTime);
    }
}