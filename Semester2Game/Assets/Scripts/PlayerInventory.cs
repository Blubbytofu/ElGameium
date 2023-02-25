using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerObject
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private HUDManager hudManager;
        [SerializeField] private PlayerCamera playerCamera;

        [field: SerializeField] public int maxHealth { get; private set; }
        [field: SerializeField] public int health { get; private set; }
        [field: SerializeField] public int maxArmor { get; private set; }
        [field: SerializeField] public int armor { get; private set; }
        [SerializeField] private int partsHealthToArmor;

        [field: SerializeField] public int maxOxygen { get; private set; }
        [field: SerializeField] public int oxygen { get; private set; }

        [SerializeField] private float oxygenDetectIntervals;
        private float lastOxygenTime;
        [SerializeField] private float hideOxygenDelay;
        [SerializeField] private int oxygenDecreaseAmount;
        [SerializeField] private int oxygenIncreaseAmount;
        [SerializeField] private int suffocationDamage;

        public bool isDead { get; private set; }

        private void Start()
        {
            hudManager.UpdateHealth(health);
            hudManager.UpdateArmor(armor);
        }

        private void Update()
        {
            if (!isDead)
            {
                ManageOxygen();
            }
        }

        private void ManageOxygen()
        {
            if (playerCamera.breathingWater)
            {
                hudManager.ToggleOxygenMonitor(true);
            }
            else
            {
                if (oxygen == maxOxygen)
                {
                    if (hudManager.oxygenIndicator.activeSelf)
                    {
                        Invoke(nameof(OffOxygenText), hideOxygenDelay);
                    }
                    lastOxygenTime = Time.time + hideOxygenDelay;
                }
            }

            if (Time.time > lastOxygenTime + oxygenDetectIntervals)
            {
                lastOxygenTime = Time.time;
                if (playerCamera.breathingWater)
                {
                    ChangeOxygen(-oxygenDecreaseAmount);
                }
                else
                {
                    ChangeOxygen(oxygenIncreaseAmount);
                }
            }
        }

        private void OffOxygenText()
        {
            if (!playerCamera.breathingWater)
            {
                hudManager.ToggleOxygenMonitor(false);
            }
        }

        public IEnumerator DamageOverTime(int ticks, float delay, int damage)
        {
            for (int i = 0; i < ticks; i++)
            {
                TakeDamage(damage);
                yield return new WaitForSeconds(delay);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isDead)
            {
                IConsumable consumable = other.gameObject.GetComponent<IConsumable>();
                if (consumable != null)
                {
                    consumable.Consume();
                }
            }
        }

        public void ChangeHealth(int healthChange)
        {
            if (health + healthChange >= maxHealth)
            {
                health = maxHealth;
            }
            else if (health + healthChange <= 0)
            {
                health = 0;
            }
            else
            {
                health += healthChange;
            }

            hudManager.UpdateHealth(health);
        }

        public void ChangeArmor(int armorChange)
        {
            if (armor + armorChange >= maxArmor)
            {
                armor = maxArmor;
            }
            else if (armor + armorChange <= 0)
            {
                ChangeHealth(armorChange + armor);
                armor = 0;
            }
            else
            {
                armor += armorChange;
            }

            hudManager.UpdateArmor(armor);
        }

        public void ChangeOxygen(int O2Change)
        {
            if (oxygen + O2Change >= maxOxygen)
            {
                oxygen = maxOxygen;
            }
            else if (oxygen + O2Change <= 0)
            {
                oxygen = 0;
                ChangeHealth(-suffocationDamage);
            }
            else
            {
                oxygen += O2Change;
            }

            hudManager.UpdateOxygen(oxygen);
        }

        public void TakeDamage(int damage)
        {
            if (!isDead)
            {
                int healthDamage;
                int armorDamage;

                healthDamage = damage / partsHealthToArmor;
                armorDamage = damage - healthDamage;

                ChangeHealth(-healthDamage);
                ChangeArmor(-armorDamage);

                if (health <= 0)
                {
                    isDead = true;
                }
            }
        }

        //detects the void
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Death"))
            {
                TakeDamage(9999);
            }
        }
    }
}
