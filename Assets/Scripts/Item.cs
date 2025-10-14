using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon;
    public string description = "Description de l'objet";

    // Nouveau champ : prefab 3D de l'objet à instancier quand on drop
    public GameObject prefab;
}
