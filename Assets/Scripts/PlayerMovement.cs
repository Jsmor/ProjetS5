using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public Rigidbody rb;
    public Transform cameraTransform; // référence à la caméra

    private bool isGrounded;

    //Sprint
    public float walkSpeed = 5f;
    [HideInInspector]
    public float currentSpeed;


    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Récupère les axes clavier
        float moveX = Input.GetAxis("Horizontal"); // A/D ou Q/D
        float moveZ = Input.GetAxis("Vertical");   // W/S ou Z/S

        // Direction par rapport à la caméra
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // On ignore la pente de la caméra (on veut bouger sur le sol uniquement)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calcul direction finale
        Vector3 moveDir = (forward * moveZ + right * moveX).normalized;

        // Applique au Rigidbody
        Vector3 newVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);
        rb.linearVelocity = newVelocity;

        // Saut
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        /*
        // (Optionnel) Faire tourner la capsule dans la direction du mouvement
        if (moveDir != Vector3.zero)
        {
            transform.forward = moveDir;
        }
        */
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }
}
