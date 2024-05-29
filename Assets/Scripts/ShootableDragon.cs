using UnityEngine;
using System.Collections;

public class ShootableDragon : MonoBehaviour
{

	//The box's current health point total
	public float currentHealth = 3;
	Animator anim;


    void Start()
    {
        gameObject.SetActive(true);
		anim = GetComponent<Animator>();
    }

    public void Damage(float damageAmount)
	{
		//subtract damage amount when Damage function is called
		currentHealth -= damageAmount;

		//Check if health has fallen below zero
		if (currentHealth <= 0)
		{
			//if health has fallen below zero, deactivate it 
			
			anim.SetTrigger("Die");
			//Invoke("Die", 20f);
			
		}
	}
	public float GetCurrentHealth()
	{
		return currentHealth;
	}

	public void Die()
	{
        gameObject.SetActive(false);
    }
}

