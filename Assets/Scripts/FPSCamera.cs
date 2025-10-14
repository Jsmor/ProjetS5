using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public float sensitivity = 2f;
    public Transform playerBody; // la capsule du joueur

    private float xRotation = 0f;

    void Start()
    {
        // Cacher et verrouiller le curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Mouvement souris
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Rotation verticale (haut/bas) uniquement sur la caméra
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // limiter pour éviter de se casser le cou
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotation horizontale (gauche/droite) sur le joueur entier
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
