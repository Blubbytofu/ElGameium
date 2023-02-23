using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerObject
{
    public class WeaponManager : MonoBehaviour
    {
        //[SerializeField] private PlayerInventory playerInventory;
        //[SerializeField] private SettingsManager settingsManager;
        [SerializeField] private HUDManager hudManager;

        //public AudioSource weaponManagerAudioSource;
        //public AudioClip[] weaponAttackSounds;
        //public float[] weaponAttackVolumes;

        [SerializeField] private GameObject[] weapons;
        [SerializeField] private int[] weaponCurrentAmmo;
        [SerializeField] private int[] weaponMaxAmmo;
        private int weaponID;

        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private float centerHorizontalPosition;
        [SerializeField] private float swayAmplitude;
        [SerializeField] private float swayFrequency;
        [SerializeField] private float offsetMagnitude;
        [SerializeField] private float offsetSpeed;

        [SerializeField] private bool canScroll;

        private void Start()
        {
            canScroll = true;
        }

        private void Update()
        {
            Weapon weaponComp = weapons[weaponID].GetComponent<Weapon>();
            WeaponOverride weaponOverride = weapons[weaponID].GetComponent<WeaponOverride>();
            if (!weaponComp.isBurstFire || (weaponComp.isBurstFire && weaponComp.currentBurst < 1))
            {
                CycleAttack();
                WeaponNumber();
            }

            EquipWeapon();
            MoveSway();

            bool mag = weaponComp.hasMag;
            bool parent = weaponComp.isParentWeapon && weaponComp.activeSecondary;
            int secondMagSize = 0;
            if (parent)
            {
                secondMagSize = weaponComp.childWeapon.currentMagSize;
            }
            hudManager.UpdateWeapon(parent ? GetWeaponName(weaponID) + " x2": GetWeaponName(weaponID),  mag ? (parent ? GetCurrentMagSize(weaponID) + secondMagSize : GetCurrentMagSize(weaponID)) : GetCurrentAmmo(weaponID), mag ? GetCurrentAmmo(weaponID) : GetMaxAmmo(weaponID));

            if (weaponOverride != null)
            {
                hudManager.IndicateAltFire(weaponOverride.shooting);
            }
            else
            {
                hudManager.IndicateAltFire(false);
            }

            //exceptions examples
            //weaponCurrentAmmo[3] = weaponCurrentAmmo[2];
            //weaponMaxAmmo[3] = weaponMaxAmmo[2];
        }

        public void ForceUnlockWeapon(int index)
        {
            weaponID = index;
            EquipWeapon();
            weapons[index].GetComponent<Weapon>().owned = true;
        }

        private void MoveSway()
        {
            if (playerMovement.vInput != 0 || playerMovement.hInput != 0)
            {
                Vector3 pos = Vector3.zero;
                pos.y += swayAmplitude * Mathf.Sin((Time.time * swayFrequency));
                pos.x += swayAmplitude * 2 * Mathf.Cos((Time.time * swayFrequency / 2));

                transform.localPosition = new Vector3(centerHorizontalPosition + pos.x, pos.y, 0);
            }
            else
            {
                //transform.localPosition = Vector3.zero;
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * offsetSpeed);
            }

            if (playerMovement.hInput > 0)
            {
                centerHorizontalPosition = Mathf.Lerp(centerHorizontalPosition, -offsetMagnitude, Time.deltaTime * offsetSpeed);
            }
            else if (playerMovement.hInput < 0)
            {
                centerHorizontalPosition = Mathf.Lerp(centerHorizontalPosition, offsetMagnitude, Time.deltaTime * offsetSpeed);
            }
            else
            {
                centerHorizontalPosition = Mathf.Lerp(centerHorizontalPosition, 0, Time.deltaTime * offsetSpeed);
            }
        }

        private void WeaponNumber()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && weapons.Length > 0 && weapons[0].GetComponent<Weapon>().owned)
            {
                weaponID = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Length > 1 && weapons[1].GetComponent<Weapon>().owned)
            {
                weaponID = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Length > 2 && weapons[2].GetComponent<Weapon>().owned)
            {
                weaponID = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && weapons.Length > 3 && weapons[3].GetComponent<Weapon>().owned)
            {
                weaponID = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5) && weapons.Length > 4 && weapons[4].GetComponent<Weapon>().owned)
            {
                weaponID = 4;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6) && weapons.Length > 5 && weapons[5].GetComponent<Weapon>().owned)
            {
                weaponID = 5;
            }
            //and so on
        }

        private void CycleAttack()
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f && canScroll)
            {
                canScroll = false;
                CycleDown();
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f && canScroll)
            {
                canScroll = false;
                CycleUp();
            }
        }

        private void CycleDown()
        {
            if (weaponID > weapons.Length - 2)
            {
                weaponID = 0;
                canScroll = true;
            }
            else
            {
                weaponID++;
                if (!weapons[weaponID].GetComponent<Weapon>().owned)
                {
                    CycleDown();
                }
                else
                {
                    canScroll = true;
                }
            }
        }

        private void CycleUp()
        {
            if (weaponID < 1)
            {
                weaponID = weapons.Length - 1;
                if (!weapons[weaponID].GetComponent<Weapon>().owned)
                {
                    CycleUp();
                }
                else
                {
                    canScroll = true;
                }
            }
            else
            {
                weaponID--;
                if (!weapons[weaponID].GetComponent<Weapon>().owned)
                {
                    CycleUp();
                }
                else
                {
                    canScroll = true;
                }
            }
        }

        private void EquipWeapon()
        {
            int index = 0;
            foreach (GameObject weapon in weapons)
            {
                if (weaponID == index)
                {
                    weapon.SetActive(true);
                }
                else
                {
                    weapon.SetActive(false);
                }
                index++;
            }
        }

        /*
        public void PlayAttackSound(int index)
        {
            if (weaponAttackSounds[index] != null)
            {
                if (weaponID == 3)
                {
                    weaponManagerAudioSource.pitch = 0.7f;
                }
                else
                {
                    weaponManagerAudioSource.pitch = 1f;
                }
                weaponManagerAudioSource.PlayOneShot(weaponAttackSounds[index], weaponAttackVolumes[index]);
            }
        }
        */

        public void AddAmmo(int weaponIndex, int value)
        {
            int currentAmmoPool = weaponCurrentAmmo[weaponIndex];
            int maxAmmoPool = weaponMaxAmmo[weaponIndex];

            if (currentAmmoPool + value > maxAmmoPool)
            {
                weaponCurrentAmmo[weaponIndex] = maxAmmoPool;
            }
            else
            {
                weaponCurrentAmmo[weaponIndex] += value;
            }
        }

        public void SubtractAmmo(int weaponIndex, int value)
        {
            int currentAmmoPool = weaponCurrentAmmo[weaponIndex];
            if (currentAmmoPool - value < 0)
            {
                weaponCurrentAmmo[weaponIndex] = 0;
            }
            else
            {
                weaponCurrentAmmo[weaponIndex] -= value;
            }
        }

        public int GetCurrentAmmo(int weaponIndex)
        {
            return weaponCurrentAmmo[weaponIndex];
        }

        public int GetMaxAmmo(int weaponIndex)
        {
            return weaponMaxAmmo[weaponIndex];
        }

        public int GetCurrentMagSize(int weaponIndex)
        {
            return weapons[weaponIndex].GetComponent<Weapon>().currentMagSize;
        }

        public string GetWeaponName(int weaponIndex)
        {
            return weapons[weaponIndex].GetComponent<Weapon>().weaponName;
        }
    }
}
