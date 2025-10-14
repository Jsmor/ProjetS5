using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera tpsCamera;   // caméra 3ème personne
    public Camera fpsCamera;   // caméra 1ère personne

    private bool isFPS = false;

    void Start()
    {
        // Assure-toi que TPS est active par défaut
        tpsCamera.enabled = true;
        fpsCamera.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            isFPS = !isFPS;
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        tpsCamera.enabled = !isFPS;
        fpsCamera.enabled = isFPS;
    }
}
