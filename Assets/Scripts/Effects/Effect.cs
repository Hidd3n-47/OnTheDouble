using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [Header("Unused in game.")]
    [Header("To specify different purposes on the same Object")]
    [SerializeField] string Description;
    
    [SerializeReference] protected IEffectPacket[] m_payloads = new IEffectPacket[0];

    virtual public void Play()
    {
        foreach (IEffectPacket payload in m_payloads)
        {
            payload.Play();
        }
    }


    // This has to be public to use in the Editor script.
    // Could do this using the serializedProperty to avoid this but... effort. -m@
    #if UNITY_EDITOR
    public void AddPayload(IEffectPacket payload)
    {
        int size = m_payloads.Length;
        Array.Resize(ref m_payloads, size + 1);
        m_payloads[size] = payload;
    }
    #endif
}

