using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFramework.Anim
{
    public class AFBaseAnimator : MonoBehaviour
    {
        public System.Action<string, object[]> onAnimTriggerEvent;
        public System.Action<string> onAnimCompleteCallback;

        public virtual float normalizedTime
        {
            get { return 0; }
        }

        public virtual float GetAnimDuration(string name)
        {
            return -1;
        }

        public virtual bool status { get; set; }

        public virtual float playSpeed { get; set; }

        public virtual void Play(string animationName, bool loop = false, bool force = false)
        {

        }

        public virtual void Play(int animationId, bool loop = false, bool force = false)
        {

        }

        public virtual void SetTimeScale(float timeScale) { }
    }
}