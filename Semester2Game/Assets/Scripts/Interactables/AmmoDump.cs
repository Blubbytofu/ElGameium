using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;
using ExtensionMethods;

public class AmmoDump : MonoBehaviour, IInteractable
{
    public void Interact(GameObject source)
    {
        WeaponManager weaponManager = source.GetComponentInChildren<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.MaxAmmo();
        }
    }
}
