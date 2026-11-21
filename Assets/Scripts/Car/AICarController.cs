using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AICarController : MonoBehaviour
{
    [Header("AI Settings")]
    public List<Transform> waypoints = new List<Transform>();
    public float moveSpeed = 24f;
    public float turnSpeed = 6f;
    public float waypointReachDistance = 4f;

    private Rigidbody rb;
    private int currentIndex = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        Transform targetWp = waypoints[currentIndex];
        if (targetWp == null) return;

        Vector3 toTarget = targetWp.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude <= waypointReachDistance)
        {
            currentIndex = (currentIndex + 1) % waypoints.Count;
            targetWp = waypoints[currentIndex];
            if (targetWp == null) return;
            toTarget = targetWp.position - transform.position;
            toTarget.y = 0f;
        }

        if (toTarget.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRot = Quaternion.LookRotation(toTarget.normalized);
            Quaternion smoothRot = Quaternion.Slerp(rb.rotation, desiredRot, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothRot);
        }

        float currentForwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        if (currentForwardSpeed < moveSpeed)
        {
            rb.AddForce(transform.forward * (moveSpeed * 2f), ForceMode.Acceleration);
        }

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
}