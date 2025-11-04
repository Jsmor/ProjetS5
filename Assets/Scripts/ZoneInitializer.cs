using UnityEngine;

public class ZoneInitializer : MonoBehaviour
{
    [SerializeField] private GameObject[] statusPrefabs; // Tes 3 prefabs de "status"
    [SerializeField] private Transform spawnPoint; // où placer la salle

    void Start()
    {
        // Trouver toutes les Zones dans la salle active
        GameObject[] zones = GameObject.FindGameObjectsWithTag("Zone");

        for (int i = 0; i < zones.Length; i++)
        {
            // On choisit le prefab correspondant à l'index (si tu en as 3)
            int prefabIndex = Random.Range(0, statusPrefabs.Length);

            // Instancier le prefab dans la Zone
            Instantiate(
                statusPrefabs[prefabIndex],
                spawnPoint.position,
                spawnPoint.rotation,
                spawnPoint // parenté pour rester attaché à la Zone
            );
        }
    }
}