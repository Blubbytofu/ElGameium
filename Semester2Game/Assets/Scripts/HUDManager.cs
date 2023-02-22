using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PlayerObject
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI armorText;

        public void UpdateWeapon(string weaponName, int currentAmmo, int maxAmmo)
        {
            weaponNameText.text = weaponName;
            ammoText.text = "" + currentAmmo + " / " + maxAmmo;
        }

        public void UpdateHealth(int newHealth)
        {
            healthText.text = "+ " + newHealth;
        }

        public void UpdateArmor(int newArmor)
        {
            armorText.text = "|| " + newArmor;
        }
    }
}
