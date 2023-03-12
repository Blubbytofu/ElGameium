using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableCrate : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;

    private bool dead;
    [SerializeField] private GameObject deathSpawn;

    public void RecieveDamage(int damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            dead = true;
            Destroy(gameObject);
            Rigidbody[] rb = Instantiate(deathSpawn, transform.position, transform.rotation).GetComponentsInChildren<Rigidbody>();
            foreach(Rigidbody rigidBody in rb)
            {
                //rigidBody.AddForce((transform.position - rigidBody.transform.position) * 100, ForceMode.Impulse);
                rigidBody.AddTorque(Vector3.up * 10000, ForceMode.Impulse);
            }
        }
    }

    void Start()
    {
        health = maxHealth;
    }
}
