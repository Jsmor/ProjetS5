using UnityEngine;

public class PlayerSprint : MonoBehaviour
{
    [Header("Références")]
    public PlayerMovement playerMovement;  // script existant de déplacement
    public Camera tpsCamera;               // caméra TPS
    public Camera fpsCamera;               // caméra FPS

    [Header("Vitesse")]
    public float sprintMultiplier = 1.5f;  // facteur de vitesse en sprint

    [Header("FOV")]
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float fovSmoothSpeed = 5f;      // interpolation fluide du FOV

    private bool isSprinting = false;

    void Update()
    {
        // Détection du sprint
        if (Input.GetKey(KeyCode.LeftShift))
            isSprinting = true;
        else
            isSprinting = false;

        // Appliquer la vitesse
        if (playerMovement != null)
        {
            playerMovement.currentSpeed = playerMovement.walkSpeed * (isSprinting ? sprintMultiplier : 1f);
        }

        // Appliquer le FOV selon la caméra active
        Camera activeCamera = (tpsCamera.enabled) ? tpsCamera : fpsCamera;
        float targetFOV = isSprinting ? sprintFOV : normalFOV;
        activeCamera.fieldOfView = Mathf.Lerp(activeCamera.fieldOfView, targetFOV, fovSmoothSpeed * Time.deltaTime);
    }
}
