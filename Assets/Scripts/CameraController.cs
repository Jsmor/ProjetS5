using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;   // la capsule
    public float sensitivity = 2f;
    public float distance = 5f;
    public float height = 2f;

    private float rotationY = 0f;
    private float rotationX = 0f;

    void Start()
    {
        // Cacher et verrouiller le curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Récupère mouvement souris
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -20f, 60f); // limite haut/bas

        // Calcul rotation caméra
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

        // Position caméra derrière le joueur
        Vector3 targetPos = player.position - rotation * Vector3.forward * distance + Vector3.up * height;

        transform.position = targetPos;
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}