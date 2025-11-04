using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField] private GameObject[] statusPrefabs; // Tes 3 prefabs de status

    void Start()
    {
        // Récupère toutes les Zones enfants
        foreach (Transform zone in transform)
        {
            // 33 % de chance pour chaque prefab
            int randomIndex = Random.Range(0, statusPrefabs.Length);

            // Instancier le status choisi dans la Zone
            Instantiate(
                statusPrefabs[randomIndex],
                zone.position,
                zone.rotation,
                zone // le prefab devient enfant de la Zone
            );
        }
    }
}
