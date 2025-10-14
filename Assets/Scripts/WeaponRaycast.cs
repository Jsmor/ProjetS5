using UnityEngine;

public class WeaponRaycast : MonoBehaviour
{
    [Header("Références")]
    public Camera playerCamera;   // caméra du joueur
    public LineRenderer lineRenderer; // pour voir la trajectoire
    public float range = 50f;         // distance du tir
    public float fireRate = 0.2f;

    private float nextTimeToFire = 0f;
    private Weapon weapon;

    void Awake()
    {
        weapon = GetComponent<Weapon>();
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    void Update()
    {
        if (!weapon.CanShoot()) return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        Vector3 endPoint = ray.origin + ray.direction * range;

        if (Physics.Raycast(ray, out hit, range))
        {
            endPoint = hit.point;
            // plus tard : appliquer dégâts si hit
        }

        if (lineRenderer != null)
        {
            StartCoroutine(DrawLine(ray.origin, endPoint));
        }
    }

    System.Collections.IEnumerator DrawLine(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f);
        lineRenderer.enabled = false;
    }
}
