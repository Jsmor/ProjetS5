using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; // ton prefab du joueur

    void Start()
    {
        // Trouver l'objet "Spawner" dans la scène
        GameObject spawner = GameObject.Find("Spawner");

        if (spawner == null)
        {
            Debug.LogWarning("Aucun objet nommé 'Spawner' trouvé dans la scène !");
            return;
        }

        // Instancier ou déplacer le joueur
        if (playerPrefab != null)
        {
            // Si le joueur n'existe pas encore, on le crée
            Instantiate(playerPrefab, spawner.transform.position, spawner.transform.rotation);
        }
        else
        {
            // Si tu veux simplement déplacer un joueur déjà existant
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.SetPositionAndRotation(spawner.transform.position, spawner.transform.rotation);
            }
            else
            {
                Debug.LogWarning("Aucun joueur trouvé dans la scène !");
            }
        }
    }
}
