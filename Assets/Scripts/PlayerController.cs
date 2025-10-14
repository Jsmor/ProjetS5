using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Références")]
    public Rigidbody rb;
    public Camera tpsCamera;
    public Camera fpsCamera;
    public Transform fpsCameraPivot;
    public GameObject weapon;

    [Header("TPS Camera Rig")]
    public Transform tpsCameraRig;    // empty qui contient TPS_Camera
    public Transform tpsNormalPos;    // position TPS centrée
    public Transform tpsAimPos;       // position TPS épaule droite

    [Header("Vitesse")]
    public float walkSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float aimSpeedMultiplier = 0.5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("FOV")]
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float dashFOV = 90f;
    public float aimFOV = 40f;
    public float fovSmoothSpeed = 5f;

    [Header("FPS Settings")]
    public float mouseSensitivity = 2f;

    [Header("Comportement pendant le tir (TPS)")]
    public bool stopMovementWhileFiring = false;
    public float fireRotationSpeed = 12f;

    // états
    private bool isSprinting = false;
    private bool isDashing = false;
    private bool isFPS = false;
    private bool isFiring = false;
    private bool isAiming = false;

    // dash
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    // FPS rotation
    private float xRotation = 0f;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        tpsCamera.enabled = true;
        fpsCamera.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCameraSwitch();
        HandleAim();
        HandleSprint();
        HandleDashInput();
        HandleFiringState();

        HandleCameraRotation();
        UpdateTPSCameraRig();
        UpdateFOV();
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

    //=====================================================================================
    //Ce code ne fonctionne pas
    //=====================================================================================
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
    //=====================================================================================
    //Ce code ne fonctionne pas
    //=====================================================================================
    
    void UpdateFOV()
    {
        Camera activeCamera = isFPS ? fpsCamera : tpsCamera;

        float targetFOV;
        if (isDashing) targetFOV = dashFOV;
        else if (isAiming) targetFOV = aimFOV;
        else if (isSprinting) targetFOV = sprintFOV;
        else targetFOV = normalFOV;

        activeCamera.fieldOfView = Mathf.Lerp(
            activeCamera.fieldOfView,
            targetFOV,
            fovSmoothSpeed * Time.deltaTime
        );
    }
    #endregion

    #region Movement & rotation
    void HandleMovement()
    {
        if (isDashing) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

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

        Vector3 moveDir = (forward * moveZ + right * moveX).normalized;
        float speed = walkSpeed;

        if (isSprinting) speed *= sprintMultiplier;
        if (isAiming) speed *= aimSpeedMultiplier;

        if (!isFPS && isFiring)
        {
            rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

            Vector3 lookDir = tpsCamera.transform.forward;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, fireRotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

            if (!isFPS && moveDir != Vector3.zero)
            {
                transform.forward = moveDir;
            }
        }
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
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

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

            Vector3 moveDir = new Vector3(moveX, 0, moveZ);
            if (moveDir.magnitude == 0f)
                dashDirection = forward;
            else
                dashDirection = (forward * moveZ + right * moveX).normalized;

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
        isAiming = Input.GetButton("Fire2"); // clic droit
    }
    #endregion
}