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
        [SerializeField] private GameObject altIndicator;

        public void UpdateWeapon(string weaponName, int currentAmmo, int maxAmmo)
        {
            weaponNameText.text = weaponName;
            ammoText.text = "" + currentAmmo + " / " + maxAmmo;
        }
        public void IndicateAltFire(bool state)
        {
            altIndicator.SetActive(state);
        }

        public void UpdateHealth(int newHealth)
        {
            healthText.text = newHealth.ToString();
        }

        public void UpdateArmor(int newArmor)
        {
            armorText.text = newArmor.ToString();
        }
    }
}
