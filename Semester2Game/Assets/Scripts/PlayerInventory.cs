using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerObject
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private HUDManager hudManager;

        [field: SerializeField] public int maxHealth { get; private set; }
        [field: SerializeField] public int health { get; private set; }
        [field: SerializeField] public int maxArmor { get; private set; }
        [field: SerializeField] public int armor { get; private set; }

        [SerializeField] private int partsHealthToArmor;

        public bool isDead { get; private set; }

        private void Start()
        {
            hudManager.UpdateHealth(health);
            hudManager.UpdateArmor(armor);
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
