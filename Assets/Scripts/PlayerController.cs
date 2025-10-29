using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Références")]
    public Rigidbody rb;
    public Animator animator;
    public Camera tpsCamera;
    public Camera fpsCamera;
    public Transform fpsCameraPivot;
    public Transform tpsCameraRig;
    public Transform tpsNormalPos;
    public Transform tpsAimPos;

    [Header("Mouvement")]
    public float walkSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float aimSpeedMultiplier = 0.5f;

    [Header("Saut")]
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    public float groundCheckOffset = 0.9f;

    [Header("FOV")]
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float aimFOV = 40f;
    public float fovSmoothSpeed = 5f;

    [Header("Sensibilité FPS")]
    public float mouseSensitivity = 2f;

    // États internes
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isSprinting = false;
    private bool isAiming = false;
    private bool isFPS = false;
    private bool isFiring = false;

    // FPS rotation
    private float xRotation = 0f;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        tpsCamera.enabled = true;
        fpsCamera.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCameraSwitch();
        HandleCameraRotation();
        HandleAim();
        HandleSprint();
        HandleMovement();
        HandleJump();
        UpdateTPSCameraRig();
        UpdateFOV();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    #region Camera & Input
    void HandleCameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            isFPS = !isFPS;
            tpsCamera.enabled = !isFPS;
            fpsCamera.enabled = isFPS;
        }
    }

    void HandleCameraRotation()
    {
        if (!isFPS) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        if (fpsCameraPivot != null)
            fpsCameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void UpdateTPSCameraRig()
    {
        if (tpsCameraRig == null || tpsNormalPos == null || tpsAimPos == null) return;

        Transform targetPos = isAiming ? tpsAimPos : tpsNormalPos;
        tpsCameraRig.position = Vector3.Lerp(
            tpsCameraRig.position,
            targetPos.position,
            Time.deltaTime * 10f
        );
        tpsCameraRig.rotation = Quaternion.Lerp(
            tpsCameraRig.rotation,
            targetPos.rotation,
            Time.deltaTime * 10f
        );
    }

    void UpdateFOV()
    {
        Camera activeCamera = isFPS ? fpsCamera : tpsCamera;

        float targetFOV;
        if (isAiming) targetFOV = aimFOV;
        else if (isSprinting) targetFOV = sprintFOV;
        else targetFOV = normalFOV;

        activeCamera.fieldOfView = Mathf.Lerp(
            activeCamera.fieldOfView,
            targetFOV,
            fovSmoothSpeed * Time.deltaTime
        );
    }
    #endregion

    #region Movement
    float moveX, moveZ;
    Vector3 moveDir;

    void HandleMovement()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        Vector3 forward, right;
        if (isFPS)
        {
            forward = transform.forward;
            right = transform.right;
        }
        else
        {
            forward = tpsCamera.transform.forward;
            right = tpsCamera.transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
        }

        moveDir = (forward * moveZ + right * moveX).normalized;
    }

    void ApplyMovement()
    {
        if (isJumping) return;

        float speed = walkSpeed;
        if (isSprinting) speed *= sprintMultiplier;
        if (isAiming) speed *= aimSpeedMultiplier;

        Vector3 velocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);
        rb.linearVelocity = velocity;

        if (!isFPS && moveDir != Vector3.zero)
        {
            transform.forward = moveDir;
        }
    }

    void HandleSprint()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && isGrounded && moveZ > 0.1f;
    }
    #endregion

    #region Jump
    void HandleJump()
    {
        // Vérifie si le joueur est au sol
        Vector3 checkPos = transform.position + Vector3.down * groundCheckOffset;
        isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundLayer);

        // Affiche la sphère pour debug
        Debug.DrawLine(checkPos, checkPos + Vector3.up * 0.1f, isGrounded ? Color.green : Color.red);

        // Détection du saut
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }

        // Quand on retouche le sol, on reset le saut
        if (isGrounded && isJumping && rb.linearVelocity.y <= 0f)
        {
            isJumping = false;
        }
    }
    #endregion

    #region Aiming
    void HandleAim()
    {
        isAiming = Input.GetButton("Fire2"); // clic droit
    }
    #endregion

    #region Animator
    void UpdateAnimator()
    {
        if (animator == null) return;

        animator.SetFloat("Horizontal", moveX);
        animator.SetFloat("Vertical", moveZ);
        animator.SetBool("isSprinting", isSprinting);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", !isGrounded && rb.linearVelocity.y < -0.1f);
    }
    #endregion
}
