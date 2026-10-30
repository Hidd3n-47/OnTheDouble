using System;
using System.Collections.Generic;
using UnityEngine;

class ModSpeed
{
    ModSpeed(MovementControl m)
    {
        //m.Pow
    }

    int value;
    float timer = 4f;
}

public class Forklift : MonoBehaviour
{
    public Action<Collision> OnCollideFront;
    public Action<Collision> OnCollideBack;
    public Action<Collision> OnCollideRight;
    public Action<Collision> OnCollideLeft;

    public BoxCollider ColliderFront;
    public BoxCollider ColliderBack;
    public BoxCollider ColliderRight;
    public BoxCollider ColliderLeft;

    public Force Engine = new();
    public Force Wheels = new();

    public List<Force> ExternalForces = new();

    List<ModSpeed> m_mods;

    public bool IsBoosting = false;

    public bool AcceleratingThisFrame { get; set; }
    public bool BrakingThisFrame      { get; set; }
    public bool BoostingThisFrame     { get; set; }

    void Awake()
    {
        Engine.Type = ForceType.Translation;
        Engine.Transform = transform;

        Wheels.Type = ForceType.Rotation;
        Wheels.Transform = transform;
        Wheels.Direction = Vector3.up;
    }

    private void Update()
    {
        UpdateExternalForces();
        UpdateWheels();
        UpdateEngine();
    }

    void OnCollisionEnter(Collision collision)
    {
        //todo refactor.
        //if (collision.GetContact(0).thisCollider == ColliderFront) { OnCollideFront?.Invoke(collision);}
        //if (collision.GetContact(0).thisCollider == ColliderBack) { OnCollideBack?.Invoke(collision);}
        //if (collision.GetContact(0).thisCollider == ColliderRight) { OnCollideRight?.Invoke(collision);}
        //if (collision.GetContact(0).thisCollider == ColliderLeft) { OnCollideLeft?.Invoke(collision); }
    }

    void UpdateWheels()
    {
        Wheels.Refresh();
    }

    void UpdateEngine()
    {
        Engine.Direction = transform.forward;
        Engine.Refresh();
    }

    void UpdateExternalForces()
    {
        ExternalForces.ForEach(f => f.Refresh());
        ExternalForces.RemoveAll(f => f.Power == 0.0f);
    }

    internal void ModSpeed(float v)
    {
        throw new NotImplementedException();
    }
}
