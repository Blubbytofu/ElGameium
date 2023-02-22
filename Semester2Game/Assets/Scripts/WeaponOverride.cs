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
        private bool shooting;

        [SerializeField] private bool zoomIn;
        [SerializeField] private bool altFireMode;
        [SerializeField] private bool altProjectile;

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
            if (zoomIn)
            {
                if (shooting)
                {
                    //pCamera.fieldOfView = 70;
                }
                else
                {
                    //pCamera.fieldOfView = 90;
                }
            }

        }
    }
}
