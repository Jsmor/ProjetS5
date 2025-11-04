using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Références")]
    public Rigidbody rb;
    public Animator animator;
    public Camera tpsCamera;
    public Camera fpsCamera;
    public Transform fpsCameraPivot;

    [Header("TPS Camera Rig")]
    public Transform tpsCameraRig;
    public Transform tpsNormalPos;
    public Transform tpsAimPos;

    [Header("Vitesse")]
    public float walkSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float aimSpeedMultiplier = 0.5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Saut")]
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    public float groundCheckOffset = 0.9f;
    public float groundCheckRadius = 10.00f;

    [Header("FOV")]
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float dashFOV = 90f;
    public float aimFOV = 40f;
    public float fovSmoothSpeed = 5f;

    [Header("FPS Settings")]
    public float mouseSensitivity = 2f;

    // États internes
    private bool isSprinting = false;
    private bool isDashing = false;
    private bool isFPS = false;
    private bool isFiring = false;
    private bool isAiming = false;
    private bool isGrounded = false;
    private bool isJumping = false;

    // Mouvement
    private float moveX, moveZ;
    private Vector3 moveDir;

    // Dash
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    // FPS rotation
    private float xRotation = 0f;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (tpsCamera != null) tpsCamera.enabled = true;
        if (fpsCamera != null) fpsCamera.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCameraSwitch();
        HandleCameraRotation();
        HandleAim();
        HandleSprint();        // calcule isSprinting
        HandleDashInput();
        HandleFiringState();

        HandleMovement();      // met à jour moveDir, moveX, moveZ
        HandleJump();          // check ground + appui espace
        UpdateTPSCameraRig();
        UpdateFOV();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleDashMovement();
    }

    #region Camera & Input
    void HandleCameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            isFPS = !isFPS;
            if (tpsCamera != null) tpsCamera.enabled = !isFPS;
            if (fpsCamera != null) fpsCamera.enabled = isFPS;
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
        tpsCameraRig.position = Vector3.Lerp(tpsCameraRig.position, targetPos.position, Time.deltaTime * 10f);
        tpsCameraRig.rotation = Quaternion.Lerp(tpsCameraRig.rotation, targetPos.rotation, Time.deltaTime * 10f);
    }

    void UpdateFOV()
    {
        Camera activeCamera = isFPS ? fpsCamera : tpsCamera;
        if (activeCamera == null) return;

        float targetFOV = normalFOV;
        if (isDashing) targetFOV = dashFOV;
        else if (isAiming) targetFOV = aimFOV;
        else if (isSprinting) targetFOV = sprintFOV;
        else targetFOV = normalFOV;

        activeCamera.fieldOfView = Mathf.Lerp(activeCamera.fieldOfView, targetFOV, fovSmoothSpeed * Time.deltaTime);
    }
    #endregion

    #region Movement
    void HandleMovement()
    {
        if (isDashing) return; // empêche le mouvement normal pendant un dash

        // ✅ utilise les variables globales moveX et moveZ
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        // Calcul de la direction selon la vue (FPS ou TPS)
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

            // On ignore la composante verticale pour éviter les déplacements en diagonale dans les airs
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
        }

        // ✅ Direction finale du déplacement (normalisée)
        moveDir = (forward * moveZ + right * moveX).normalized;

        // Calcul de la vitesse
        float speed = walkSpeed;
        if (isSprinting) speed *= sprintMultiplier;
        if (isAiming) speed *= aimSpeedMultiplier;

        // Application du mouvement au Rigidbody
        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

        // Rotation du personnage uniquement si on se déplace
        if (!isFPS && moveDir != Vector3.zero)
            transform.forward = moveDir;
    }
    #endregion


    #region Sprint
    void HandleSprint()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && !isDashing && !isFiring && !isAiming;
    }
    #endregion

    #region Dash
    void HandleDashInput()
    {
        dashCooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f) isDashing = false;
        }

        if (Input.GetKeyDown(KeyCode.E) && dashCooldownTimer <= 0f && !isDashing)
        {
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

            Vector3 desired = (forward * moveZ + right * moveX).normalized;
            dashDirection = desired.magnitude == 0f ? forward : desired;

            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;
        }
    }

    void HandleDashMovement()
    {
        if (!isDashing) return;
        rb.linearVelocity = dashDirection * dashSpeed;
    }
    #endregion

    #region Firing & Aiming
    void HandleFiringState()
    {
        isFiring = Input.GetButton("Fire1");
    }

    void HandleAim()
    {
        isAiming = Input.GetButton("Fire2");
    }
    #endregion

    #region Jump
    void HandleJump()
    {
        // Vérifie si le joueur est au sol (CheckSphere)
        Vector3 checkPos = transform.position + Vector3.down * groundCheckOffset;
        isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundLayer);

        // Debug visuel
        Debug.DrawLine(checkPos, checkPos + Vector3.up * 0.2f, isGrounded ? Color.green : Color.red);

        // Si le joueur est au sol et appuie sur espace → saut
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Reset de la vitesse verticale
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Applique une impulsion vers le haut
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Active le flag de saut
            isJumping = true;
            animator.SetTrigger("Jump"); // 🔥 Joue l’animation de saut immédiatement
        }

        // Si on est en l’air et que la vitesse verticale < 0 → on tombe
        if (!isGrounded && rb.linearVelocity.y < -0.1f)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }

        // Si on touche le sol → reset du saut
        if (isGrounded && isJumping && rb.linearVelocity.y <= 0f)
        {
            isJumping = false;
        }
    }
    #endregion


    #region Animator
    void UpdateAnimator()
    {
        if (animator == null) return;

        // Lissage des valeurs pour éviter les saccades
        float currentH = animator.GetFloat("Horizontal");
        float currentV = animator.GetFloat("Vertical");

        animator.SetFloat("Horizontal", Mathf.Lerp(currentH, moveX, Time.deltaTime * 10f));
        animator.SetFloat("Vertical", Mathf.Lerp(currentV, moveZ, Time.deltaTime * 10f));

        animator.SetBool("isSprinting", isSprinting);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", !isGrounded && rb.linearVelocity.y < -0.1f);
    }
    #endregion
}
