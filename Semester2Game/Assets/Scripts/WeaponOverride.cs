using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerObject
{
    public class WeaponOverride : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private Camera pCamera;

        [SerializeField] private bool singleFire;
        public bool shooting { get; private set; }

        [SerializeField] private bool zoom;
        [SerializeField] private int zoomAmount;

        [SerializeField] private bool dualWield;
        [SerializeField] private GameObject secondGun;

        [SerializeField] private bool altBurstMode;
        [SerializeField] private bool altProjectile;

        [SerializeField] private bool affectSpread;
        [SerializeField] private float normalInitialSpread;
        [SerializeField] private float normalMaxSpread;
        [SerializeField] private float newInitialSpread;
        [SerializeField] private float newMaxSpread;

        private void Update()
        {
            GetInput();
            ExecuteShoot();
        }

        private void GetInput()
        {
            if (singleFire)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    shooting = !shooting;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    shooting = true;
                }
                else
                {
                    shooting = false;
                }
            }

        }

        private void ExecuteShoot()
        {
            if (dualWield)
            {
                if (shooting)
                {
                    secondGun.SetActive(true);
                    weapon.activeSecondary = true;
                }
                else
                {
                    secondGun.SetActive(false);
                    weapon.activeSecondary = false;
                }
            }

            if (zoom)
            {
                if (shooting)
                {

                }
                else
                {

                }
            }

            if (altBurstMode)
            {
                if (shooting)
                {
                    weapon.isBurstFire = true;
                }
                else
                {
                    if (weapon.currentBurst < 1)
                    {
                        weapon.isBurstFire = false;
                    }
                }
            }

            if (affectSpread)
            {
                if (shooting)
                {
                    weapon.initialSpread = newInitialSpread;
                    weapon.maxSpread = newMaxSpread;
                }
                else
                {
                    weapon.initialSpread = normalInitialSpread;
                    weapon.maxSpread = normalMaxSpread;
                }
            }
        }
    }
}
