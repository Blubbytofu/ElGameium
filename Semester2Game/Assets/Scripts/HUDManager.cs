using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PlayerObject
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fpsCounter;
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI armorText;
        [SerializeField] private TextMeshProUGUI oxygenText;
        public GameObject oxygenIndicator;
        [SerializeField] private GameObject altIndicator;

        private void Start()
        {
            StartCoroutine(TrackFrames());
        }

        private IEnumerator TrackFrames()
        {
            while (true)
            {
                int frames = (int)(1 / Time.unscaledDeltaTime);
                fpsCounter.text = "FPS " + frames;
                yield return new WaitForSeconds(1f);
            }
        }

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

        public void UpdateOxygen(int newOxygen)
        {
            oxygenText.text = newOxygen.ToString();
        }

        public void ToggleOxygenMonitor(bool state)
        {
            oxygenIndicator.SetActive(state);
        }
    }
}
