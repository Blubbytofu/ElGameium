using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

namespace PlayerObject
{
    public class Weapon : MonoBehaviour
    {
        [Header("References-----------------------------------------------------------------------------")]
        [SerializeField] private Camera playerCam;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private GameObject muzzleFlash, bulletImpact;
        [SerializeField] private LayerMask environmentMask, enemyMask;
        [SerializeField] private Animator weaponAnimator;
        [SerializeField] PlayerInventory playerInventory;
        [SerializeField] private WeaponManager weaponManager;
        private RaycastHit rayHit;

        [Header("Weapon Identity-----------------------------------------------------------------------------")]
        public bool owned;
        [field: SerializeField] public string weaponName { get; private set; }
        [SerializeField] private int weaponIndex;
        [SerializeField] private int damage;
        [SerializeField] private float timeBetweenShooting;
        [SerializeField] private float range;
        //bullets shot per instance of shooting
        [SerializeField] private int bulletsPerShot;
        //bullets subtracted from ammo pool
        [SerializeField] private int bulletsSubtracted;
        [SerializeField] private bool singleFire;
        private bool shooting;
        private bool readyToShoot;
        [SerializeField] private bool isHitScan;
        [SerializeField] private GameObject projectile;
        [SerializeField] private float projectileZVel;
        [SerializeField] private float projectileYVel;

        [Header("Spread-----------------------------------------------------------------------------")]
        public float initialSpread;
        public float maxSpread;
        [SerializeField] private float currentSpread;
        [SerializeField] private float deltaSpread;
        private float lastShotTime;
        [SerializeField] private float spreadDecreaseDelay;
        [SerializeField] private float spreadDecreaseRate;
        private Vector3 totalSpread;

        [Header("Recoil-----------------------------------------------------------------------------")]
        [SerializeField] private float lowerRecoilXMagnitude;
        [SerializeField] private float upperRecoilXMagnitude;
        [SerializeField] private float lowerRecoilYMagnitude;
        [SerializeField] private float upperRecoilYMagnitude;

        [Header("Magazine-----------------------------------------------------------------------------")]
        [SerializeField] private int maxMagSize;
        [field: SerializeField] public bool hasMag { get; private set; }
        [field: SerializeField] public int currentMagSize { get; private set; }
        //part 1 is before updating ammo
        [SerializeField] private float reloadTimePart1;
        //part 2 is interval between updating hud and shooting
        [SerializeField] private float reloadTimePart2;
        //total reload time is part1 + part2
        public bool isReloading { get; private set; }
        [SerializeField] private bool shotgunReload;
        private bool queuedStop;

        [Header("Burst Fire-----------------------------------------------------------------------------")]
        [SerializeField] private int maxBulletsPerBurst;
        public bool isBurstFire;
        [SerializeField] private float burstDelay;

        public int currentBurst { get; private set; }

        [Header("Heat Based-----------------------------------------------------------------------------")]
        [SerializeField] private bool isHeatBased;
        private bool takenOverheatDamage;
        private bool alreadyRecharging;
        [SerializeField] private float heatStartRegenDelay;
        [SerializeField] private int regenPerTick;
        [SerializeField] private float fullEmptyDelay;
        [SerializeField] private float normalRegenDelay;
        [SerializeField] private int overheatDamage;

        [Header("Special Properties-----------------------------------------------------------------------------")]
        //is delay between fire input and actual shot
        [SerializeField] private float fireInputDelay;
        public bool isParentWeapon;
        public Weapon childWeapon;
        [SerializeField] private bool isChildWeapon;
        [SerializeField] private Weapon parentWeapon;
        public bool activeSecondary;

        private void Awake()
        {
            readyToShoot = true;
            if (hasMag)
            {
                currentMagSize = maxMagSize;
            }
        }

        public void StartEquip()
        {
            readyToShoot = false;
            isReloading = false;
        }

        public void FinishEquip()
        {
            readyToShoot = true;
        }

        private void Update()
        {
            if (isChildWeapon && !parentWeapon.isActiveAndEnabled)
            {
                gameObject.SetActive(false);
            }

            if (isParentWeapon)
            {
                shooting = (singleFire ? Input.GetKeyDown(KeyCode.Mouse0) : Input.GetKey(KeyCode.Mouse0)) && (childWeapon.readyToShoot || !activeSecondary);
            }
            else
            {
                shooting = singleFire ? Input.GetKeyDown(KeyCode.Mouse0) : Input.GetKey(KeyCode.Mouse0);
            }

            if (readyToShoot && !isReloading)
            {
                ShootInput();
            }

            if (hasMag && Input.GetKeyDown(KeyCode.R) && currentMagSize < maxMagSize && weaponManager.GetCurrentAmmo(weaponIndex) > 0 && !isReloading && currentBurst < 1)
            {
                if (isChildWeapon && weaponManager.GetCurrentAmmo(weaponIndex) == 1)
                {
                    return;
                }

                isReloading = true;
                if (weaponAnimator != null)
                {
                    weaponAnimator.SetTrigger("_reload");
                }

                StartCoroutine(ReloadMag());
            }

            if (isReloading && !queuedStop && shooting)
            {
                queuedStop = true;
            }

            if (isHeatBased)
            {
                RechargeHeatAmmo();
            }
        }

        private void RechargeHeatAmmo()
        {
            if (!alreadyRecharging && Time.time > lastShotTime + heatStartRegenDelay && weaponManager.GetCurrentAmmo(weaponIndex) < weaponManager.GetMaxAmmo(weaponIndex))
            {
                alreadyRecharging = true;
                if (weaponManager.GetCurrentAmmo(weaponIndex) == 0)
                {
                    Invoke(nameof(ResetRecharge), fullEmptyDelay);
                }
                else
                {
                    Invoke(nameof(ResetRecharge), normalRegenDelay);
                }
            }

            if (weaponManager.GetCurrentAmmo(weaponIndex) == 0 && !takenOverheatDamage)
            {
                takenOverheatDamage = true;
                playerInventory.TakeDamage(overheatDamage);
            }
        }

        private void ResetRecharge()
        {
            weaponManager.AddAmmo(weaponIndex, regenPerTick);
            takenOverheatDamage = false;
            alreadyRecharging = false;
        }

        private void ShootInput()
        {
            if (shooting && (hasMag? currentMagSize >= bulletsSubtracted : weaponManager.GetCurrentAmmo(weaponIndex) >= bulletsSubtracted))
            {
                if (isBurstFire)
                {
                    if (hasMag? currentMagSize >= maxBulletsPerBurst : weaponManager.GetCurrentAmmo(weaponIndex) >= maxBulletsPerBurst)
                    {
                        currentBurst = maxBulletsPerBurst;
                    }
                    else
                    {
                        currentBurst = (hasMag? currentMagSize : weaponManager.GetCurrentAmmo(weaponIndex));
                    }
                }

                readyToShoot = false;
                Invoke(nameof(Shoot), fireInputDelay);
            }

            if (!shooting && Time.time > lastShotTime + spreadDecreaseDelay)
            {
                currentSpread = Mathf.Lerp(currentSpread, initialSpread, Time.deltaTime * spreadDecreaseRate);
            }
        }

        private void Shoot()
        {
            if (weaponAnimator != null)
            {
                weaponAnimator.SetTrigger("_shoot");
            }

            //weaponManager.PlayAttackSound(weaponIndex);

            if (muzzleFlash != null)
            {
                //GameObject flash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
                //flash.GetComponent<MuzzleFlash>().attackPoint = attackPoint;
            }

            if (hasMag)
            {
                currentMagSize -= bulletsSubtracted;
            }
            else
            {
                weaponManager.SubtractAmmo(weaponIndex, bulletsSubtracted);
            }


            for (int shotBullets = 0; shotBullets < bulletsPerShot; shotBullets++)
            {
                if (isHitScan)
                {
                    float xSpread = Random.Range(-currentSpread, currentSpread);
                    float ySpread = Random.Range(-currentSpread, currentSpread);

                    totalSpread = playerCam.transform.right * xSpread + playerCam.transform.up * ySpread;

                    if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward + totalSpread, out rayHit, range, enemyMask) ||
                    Physics.Raycast(playerCam.transform.position, playerCam.transform.forward + totalSpread, out rayHit, range, environmentMask))
                    {
                        if (bulletImpact != null)
                        {
                            Instantiate(bulletImpact, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                        }

                        IDamageable damageable = rayHit.collider.gameObject.GetComponent<IDamageable>();
                        if (damageable != null)
                        {
                            damageable.RecieveDamage(damage);
                        }
                    }
                }
                else
                {
                    GameObject playerProjectile = Instantiate(projectile, attackPoint.position, Quaternion.identity);

                    Ray directionRay = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                    Vector3 projectileDestination;

                    if (Physics.Raycast(directionRay, out rayHit))
                    {
                        projectileDestination = rayHit.point;
                    }
                    else
                    {
                        projectileDestination = directionRay.GetPoint(100f);
                    }

                    Vector3 projectileDirection = projectileDestination - attackPoint.position;
                    playerProjectile.transform.forward = projectileDirection.normalized;

                    playerProjectile.GetComponent<Rigidbody>().AddForce(playerProjectile.transform.forward * projectileZVel, ForceMode.Impulse);
                    playerProjectile.GetComponent<Rigidbody>().AddForce(playerProjectile.transform.up * projectileYVel, ForceMode.Impulse);
                }


                float xRecoil = Random.Range(lowerRecoilXMagnitude, upperRecoilXMagnitude);
                float yRecoil = Random.Range(lowerRecoilYMagnitude, upperRecoilYMagnitude);
                playerCamera.SimulateRecoil(xRecoil, yRecoil);
            }

            if (currentSpread <= maxSpread)
            {
                currentSpread += deltaSpread;
            }

            lastShotTime = Time.time;

            if (isBurstFire)
            {
                currentBurst--;
                if (currentBurst > 0)
                {
                    Invoke(nameof(Shoot), burstDelay);
                }
                else
                {
                    StartCoroutine(ResetShot());
                }
            }
            else
            {
                StartCoroutine(ResetShot());
            }
        }

        private IEnumerator ResetShot()
        {
            yield return new WaitForSeconds(timeBetweenShooting);
            readyToShoot = true;
        }

        private IEnumerator ReloadMag()
        {
            int wishReload;
            int bulletsInStock;

            if (shotgunReload)
            {
                wishReload = 1;
            }
            else
            {
                wishReload = maxMagSize - currentMagSize;

                if (isParentWeapon)
                {
                    bulletsInStock = weaponManager.GetCurrentAmmo(weaponIndex);
                    //even stock
                    if (bulletsInStock % 2 == 0)
                    {
                        bulletsInStock = weaponManager.GetCurrentAmmo(weaponIndex) / 2;
                    }
                    else
                    {
                        bulletsInStock = weaponManager.GetCurrentAmmo(weaponIndex) / 2 + 1;
                    }
                }
                else if (isChildWeapon)
                {
                    bulletsInStock = weaponManager.GetCurrentAmmo(weaponIndex) / 2;
                }
                else
                {
                    bulletsInStock = weaponManager.GetCurrentAmmo(weaponIndex);
                }

                if (bulletsInStock < wishReload)
                {
                    wishReload = bulletsInStock;
                }
            }

            yield return new WaitForSeconds(reloadTimePart1);

            currentMagSize += wishReload;
            weaponManager.SubtractAmmo(weaponIndex, wishReload);

            yield return new WaitForSeconds(reloadTimePart2);

            if (shotgunReload)
            {
                if (queuedStop)
                {
                    queuedStop = false;
                    isReloading = false;
                }
                else
                {
                    if (currentMagSize < maxMagSize && weaponManager.GetCurrentAmmo(weaponIndex) > 0)
                    {
                        StartCoroutine(ReloadMag());
                    }
                    else
                    {
                        isReloading = false;
                    }
                }
            }
            else
            {
                isReloading = false;
            }
        }
    }
}
