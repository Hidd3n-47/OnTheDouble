using UnityEngine;
using System.Runtime.Serialization;
using System;

[Serializable]
public abstract class IEffectPacket : ISerializationCallbackReceiver
{
    public virtual void OnAfterDeserialize(){}
    public virtual void OnBeforeSerialize(){}

    public abstract void Play();
    public abstract void Stop();
    public abstract bool Done {get;}
}

[Serializable]
public class ParticlePacket : IEffectPacket
{
    [Header("Particle Payload")]
    [SerializeField] ParticleSystem m_particles;

    public override bool Done => !m_particles.isPlaying;

    public override void Play()
    {
        m_particles.Play();
    }
    public override void Stop()
    {
        m_particles.Stop();
    }

}

[Serializable]
public class AudioPacket : IEffectPacket
{
    [Header("Audio Payload")]
    [SerializeField] AudioSource m_audio;

    public override bool Done => !m_audio.isPlaying;
    public override void Play()
    {
        m_audio.Play();
    }
    public override void Stop()
    {
        m_audio.Stop();
    }
}

[Serializable]
public class AnimationPacket : IEffectPacket
{
    [Header("Animation Payload")]
    [SerializeField] Animator m_animator;
    [SerializeField] string m_animationName;
    [SerializeField] int m_animLayer;

    public override bool Done 
    {
        get 
        {
            AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(m_animLayer);
            return stateInfo.IsName(m_animationName) && stateInfo.normalizedTime >= 1f && !m_animator.IsInTransition(m_animLayer);
        }
    } 
    public override void Play()
    {
        m_animator.Play(m_animationName, m_animLayer);
    }
    public override void Stop() {} // Do nothing?
}
