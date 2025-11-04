using UnityEngine;

public class SalleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] sallesPrefabs; // glisse les prefabs ici
    [SerializeField] private Transform spawnPoint; // où placer la salle
    public int indexTest = 1;

    void Start()
    {
        int index = Random.Range(0, sallesPrefabs.Length);
        index = indexTest;
        Instantiate(sallesPrefabs[index], spawnPoint.position, spawnPoint.rotation);
    }
}
