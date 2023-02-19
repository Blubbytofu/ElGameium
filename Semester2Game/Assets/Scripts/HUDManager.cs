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

        public void UpdateWeapon(string weaponName, int currentAmmo, int maxAmmo)
        {
            weaponNameText.text = weaponName;
            ammoText.text = "" + currentAmmo + " / " + maxAmmo;
        }
    }
}
