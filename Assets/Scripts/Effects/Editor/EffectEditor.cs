
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

[CustomEditor(typeof(Effect),true)]
public class EffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        const string particleButton = "Add Particle Payload";
        const string audioButton = "Add Audio Payload";
        const string animationButton = "Add Animation Payload";

        DrawDefaultInspector();
        Effect effect = (Effect)target;
        string anyPressed = null;

        if (GUILayout.Button(particleButton))
        {
            effect.AddPayload(new ParticlePacket());
            anyPressed = particleButton;
        }

        else if (GUILayout.Button(audioButton))
        {
            effect.AddPayload(new AudioPacket());
            anyPressed = audioButton;
        }

        else if (GUILayout.Button(animationButton))
        {
            effect.AddPayload(new AnimationPacket());
            anyPressed = animationButton;
        }

        if(anyPressed != null)
        {
            Undo.RecordObject(effect, "Add Particle Payload");
            EditorUtility.SetDirty(effect);
        }
        
    }
}