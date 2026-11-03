using Unity.VisualScripting;
using UnityEngine;


// Not technically tested, but compiles...
public class ContinuousEffect : Effect
{
    bool m_playing = false;

    public override void Play()
    {
        m_playing = true;
    }

    void Update()
    {
        if(m_playing)
        {
            foreach(IEffectPacket payload in m_payloads)
            {
                if(payload.Done)
                {
                    payload.Play();
                }
            }
        }
    }

    virtual public void Stop ()
    {
        m_playing = false;
        foreach(IEffectPacket payload in m_payloads)
        {
            payload.Stop();
        }
    }
}
