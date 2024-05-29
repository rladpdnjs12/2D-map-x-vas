using UnityEngine;
using System.Collections;

namespace ARScripts
{
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
            anim.SetTrigger("GetHit");

            //Check if health has fallen below zero
            if (currentHealth <= 0)
			{
				//if health has fallen below zero, deactivate it 

				anim.SetTrigger("Die");
				StartCoroutine(DieAfterAnimation());

			}
			else
			{
                StartCoroutine(ReturnToIdleAfterGetHit());
            }
		}

		public float GetCurrentHealth()
		{
			return currentHealth;
		}

		private IEnumerator DieAfterAnimation()
		{
			yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
			Die();
		}

		private IEnumerator ReturnToIdleAfterGetHit()
		{
			yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            anim.SetTrigger("Idle01");
        }

		public void Die()
		{
			gameObject.SetActive(false);
		}
	}
}