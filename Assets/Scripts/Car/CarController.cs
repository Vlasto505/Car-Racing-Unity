using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Car Stats (neskôr prepojíme na Upgrades)")]
    public float maxSpeed = 30f;
    public float acceleration = 25f;
    public float turnSpeed = 120f;
    public float brakeForce = 35f;

    public float CurrentSpeed { get; private set; }

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Zatiaľ klasický Input (W/S alebo šípky hore/dole, A/D alebo vľavo/vpravo)
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleSteering();
    }

    private void HandleMovement()
    {
        // Aktuálna rýchlosť dopredu
        CurrentSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if (moveInput > 0f && CurrentSpeed < maxSpeed)
        {
            rb.AddForce(transform.forward * (moveInput * acceleration), ForceMode.Acceleration);
        }
        else if (moveInput < 0f)
        {
            // Brzdenie alebo cúvanie
            float force = CurrentSpeed > 0.5f ? brakeForce : acceleration * 0.5f;
            rb.AddForce(transform.forward * (moveInput * force), ForceMode.Acceleration);
        }

        // Obmedzenie maximálnej rýchlosti
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void HandleSteering()
    {
        // Zatáčame len vtedy, keď sa auto hýbe
        if (Mathf.Abs(CurrentSpeed) > 0.5f)
        {
            float direction = CurrentSpeed >= 0 ? 1f : -1f;
            float turn = turnInput * turnSpeed * direction * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}