using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARScripts
{
    public class MonsterSync : NetworkBehaviour
    {
        [Networked] public float currentHealth { get; set; } = 50f;
        [Networked] public string animationState { get; set; } = "Idle01";
        [Networked] public NetworkBool isAlive { get; set; } = true;
        private Animator anim;
        public bool isTakingDamage = false;

        private float transitionTime = 5f;

        private float generalTransitionTime = 1f;
        private float returnToIdleTransitionTime = 5f;

        public GameObject successPanel;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public override void Spawned()
        {
            base.Spawned();
            SetAnimationState(animationState); // 초기 애니메이션 상태 설정
            StartCoroutine(AnimationLoop());
            Debug.Log("Spawned: Initial Animation State - " + animationState);
        }

        public override void FixedUpdateNetwork()
        {
            // 애니메이션 상태 동기화
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName(animationState))
            {
                Debug.Log("Changing animation state to: " + animationState);
                anim.Play(animationState);
            }
        }

        private IEnumerator AnimationLoop()
        {
            string[] states = { "Idle01", "Basic Attack", "Claw Attack", "Defend" };
            int index = 0;
            while (isAlive)
            {
                SetAnimationStateRpc(states[index]);
                float clipLength = GetAnimationClipLength(states[index]);
                yield return new WaitForSeconds(clipLength);
                index = (index + 1) % states.Length;
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void TakeDamageRpc(float damageAmount)
        {
            if (Object.HasStateAuthority)
            {
                isTakingDamage = true;
                currentHealth -= damageAmount;
                Debug.Log("Current Health: " + currentHealth);
                SetAnimationTriggerRpc("GetHit");

                if (currentHealth <= 0 && isAlive)
                {
                    isAlive = false;
                    SetAnimationTriggerRpc("Die");
                    if (gameObject.activeSelf) // 게임 오브젝트가 활성화된 상태인지 확인
                    {
                        StartCoroutine(DieAfterAnimation());
                    }
                    StartCoroutine(ResetTakingDamageFlag());
                }
                else
                {
                    if (gameObject.activeSelf) // 게임 오브젝트가 활성화된 상태인지 확인
                    {
                        StartCoroutine(ReturnToIdleAfterGetHit());
                    }
                }
            }
            else
            {
                Debug.LogError("Local simulation is not allowed to send this RPC.");
            }
        }


        private IEnumerator ResetTakingDamageFlag()
        {
            yield return new WaitForSeconds(0.5f); // 0.5초 후에 플래그 리셋
            isTakingDamage = false;
        }

        public float GetCurrentHealth()
        {
            return currentHealth;
        }

        private IEnumerator DieAfterAnimation()
        {
            Debug.Log("DieAfterAnimation: Waiting for die animation to finish.");
            // Die 애니메이션의 길이를 가져와 대기
            float dieAnimationLength = GetAnimationClipLength("Die");
            yield return new WaitForSeconds(dieAnimationLength);
            Debug.Log("DieAfterAnimation: Die animation finished.");
            ShowSuccessPanelRpc();
            yield return new WaitForSeconds(0.1f); // 패널이 뜨는 것을 보장하기 위해 약간의 지연 추가
            DespawnNetworkObject();
        }

        private IEnumerator ReturnToIdleAfterGetHit()
        {
            Debug.Log("ReturnToIdleAfterGetHit: Waiting for GetHit animation to finish.");
            // GetHit 애니메이션의 길이를 가져와 대기
            float getHitAnimationLength = GetAnimationClipLength("GetHit");
            yield return new WaitForSeconds(getHitAnimationLength);
            yield return new WaitForSeconds(2f); // 2초 대기
            SetAnimationStateRpc("Idle01", returnToIdleTransitionTime);
        }

        public void Die()
        {
            StopAllCoroutines();
            StartCoroutine(DespawnAfterPanel());
        }

        private IEnumerator DespawnAfterPanel()
        {
            ShowSuccessPanelRpc();
            yield return new WaitForSeconds(2f); // 패널을 보이기 위해 2초 대기
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void SetAnimationTriggerRpc(string triggerName)
        {
            if (!IsCurrentAnimationState("Idle01") && triggerName == "Attack")
            {
                Debug.Log("Cannot set Attack trigger, current animation state is not Idle01.");
                return;
            }

            if (!IsCurrentAnimationState("Idle01") && triggerName == "GetHit")
            {
                Debug.Log("Cannot set GetHit trigger, current animation state is not Idle01.");
                return;
            }

            anim.SetTrigger(triggerName);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void SetAnimationStateRpc(string state, float transitionDuration = 0f)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName(state))
            {
                anim.CrossFade(state, transitionDuration > 0 ? transitionDuration : generalTransitionTime);
                animationState = state;
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void ShowSuccessPanelRpc()
        {
            if (successPanel != null)
            {
                successPanel.SetActive(true);
            }
        }

        public void PlayAnimation(string animationName)
        {
            if (Object.HasStateAuthority)
            {
                SetAnimationStateRpc(animationName);
            }
        }

        private void SetAnimationState(string state)
        {
            animationState = state;
        }

        private float GetAnimationClipLength(string clipName)
        {
            foreach (var clip in anim.runtimeAnimatorController.animationClips)
            {
                if (clip.name == clipName)
                {
                    return clip.length;
                }
            }
            Debug.LogWarning("GetAnimationClipLength: Clip not found - " + clipName);
            return 0f;
        }

        private void DespawnNetworkObject()
        {
            if (Object.HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }

        private bool CheckPlayerInAttackRange()
        {
            MonsterAttack monsterAttack = GetComponent<MonsterAttack>();
            return monsterAttack != null && monsterAttack.IsPlayerInRange();
        }

        private bool IsCurrentAnimationState(string stateName)
        {
            return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
        }
    }
}
