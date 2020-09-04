using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.Anim
{
    public class AFUnityAnimator : AFBaseAnimator
    {
        Animator mAnimator;

        private void Awake()
        {
            mAnimator = GetComponent<Animator>();
        }

        public override float normalizedTime
        {
            get
            {
                return mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            }
        }

        public override float GetAnimDuration(string name)
        {
            RuntimeAnimatorController ac = mAnimator.runtimeAnimatorController;    //Get Animator controller
            for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
            {
                if (ac.animationClips[i].name == name)        //If it has the same name as your clip
                {
                    return ac.animationClips[i].length;
                }
            }
            return -1;
        }

        public override float playSpeed
        {
            get
            {
                return mAnimator.speed;
            }

            set
            {
                mAnimator.speed = value;
            }
        }

        public override bool status
        {
            get
            {
                return mAnimator.enabled;
            }

            set
            {
                mAnimator.enabled = value;
            }
        }

        public override void Play(string animationName, bool loop = false, bool force = false)
        {
            Play(Animator.StringToHash(animationName), loop, force);
        }

        public override void Play(int animationId, bool loop = false, bool force = false)
        {
            if (force)
            {
                mAnimator.Play(animationId, 0, 0);
            }
            else
            {
                mAnimator.Play(animationId);
            }
        }
    }
}