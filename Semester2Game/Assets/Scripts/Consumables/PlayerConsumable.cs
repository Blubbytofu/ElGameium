using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerObject;

public class PlayerConsumable : MonoBehaviour, IConsumable
{
    [Header("Health-----------------------------------------------------------------------------")]
    [SerializeField] private bool changeHealth;
    [SerializeField] private int healthAmount;

    [Header("Armor-----------------------------------------------------------------------------")]
    [SerializeField] private bool changeArmor;
    [SerializeField] private int armorAmount;

    [Header("Ammo-----------------------------------------------------------------------------")]
    [SerializeField] private bool changeAmmo;
    [SerializeField] private int ammoIndex;
    [SerializeField] private int ammoAmount;

    [Header("Unlock Weapon-----------------------------------------------------------------------------")]
    [SerializeField] private bool unlockWeapon;
    [SerializeField] private int unlockIndex;

    public void Consume(PlayerInventory playerInventory, WeaponManager weaponManager)
    {
        if (changeHealth)
        {
            if (playerInventory.health < playerInventory.maxHealth)
            {
                playerInventory.ChangeHealth(healthAmount);
                Destroy(gameObject);
            }
        }

        if (changeArmor)
        {
            if (playerInventory.armor < playerInventory.maxArmor)
            {
                playerInventory.ChangeArmor(armorAmount);
                Destroy(gameObject);
            }
        }

        if (changeAmmo)
        {
            if (weaponManager.GetCurrentAmmo(ammoIndex) < weaponManager.GetMaxAmmo(ammoIndex))
            {
                weaponManager.AddAmmo(ammoIndex, ammoAmount);
                Destroy(gameObject);
            }
        }

        if (unlockWeapon)
        {
            weaponManager.ForceUnlockWeapon(unlockIndex);
            Destroy(gameObject);
        }
    }
}
