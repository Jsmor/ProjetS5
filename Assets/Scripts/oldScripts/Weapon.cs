using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName = "BasicWeapon";
    public float fireRate = 0.5f;
    public int damage = 10;

    [Header("Références")]
    public GameObject model; // <- ce champ doit apparaître dans l'Inspector

    private bool isEquipped = false;

    void Start()
    {
        SetEquipped(false); // commence déséquipée
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleEquip();
        }
    }

    public void SetEquipped(bool state)
    {
        isEquipped = state;

        if (model != null)
            model.SetActive(state);
    }

    public void ToggleEquip()
    {
        SetEquipped(!isEquipped);
    }

    public bool CanShoot()
    {
        return isEquipped;
    }
}
