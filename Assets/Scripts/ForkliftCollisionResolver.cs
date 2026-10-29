using System;
using UnityEngine;

[Serializable]
public struct ForkliftCollisionInfo
{
    [SerializeField] public float radius;
    [SerializeField] public float distance;
    [SerializeField] public float minimumForceResolve;
    [SerializeField] public float maximumForceResolve;
    [SerializeField] public Transform pivot;
}

public class ForkliftCollisionResolver : MonoBehaviour
{
    private Forklift forklift;

    [SerializeField] private ForkliftCollisionInfo[] forkliftCollisionInfo;

    [SerializeField] private LayerMask collisionMask;

    private void Start()
    {
        forklift = GetComponent<Forklift>();
    }

    private void Update()
    {
        foreach (var info in forkliftCollisionInfo)
        {
            Vector3 right    = info.pivot.right;
            Vector3 forward  = info.pivot.forward;
            Vector3 position = info.pivot.position;

            bool hit = PreformSphereCast(right, forward, position, info.radius, info.distance, info.minimumForceResolve,
                info.maximumForceResolve);

            DebugRender(position, forward, right, info.radius, info.distance, hit);
        }
    }

    private bool PreformSphereCast(Vector3 right, Vector3 forward, Vector3 position, float radius, float distance, float minForceResolve, float maxForceResolve)
    {
        var ray = new Ray(position, forward);
        if (Physics.SphereCast(ray, radius, out _, distance, collisionMask))
        {
            forklift.ExternalForces.Add(new Force()
            {
                Type = ForceType.Translation,
                Transform = forklift.transform,
                Power = Mathf.Lerp(minForceResolve, maxForceResolve, 0.2f),
                Resistance = forklift.Engine.Resistance,
                Direction = -forward
            });

            forklift.Engine.Power = 0;

            return true;
        }

        return false;
    }

    private void DebugRender(Vector3 position, Vector3 forward, Vector3 right, float radius, float distance, bool hit)
    {
        Vector3 up = Vector3.up;
        Color debugLineColor = hit ? Color.green : Color.red;

        Vector3 forwardAddition = forward * (distance + radius);
        {
            Debug.DrawLine(position, position + forward * (distance + radius), debugLineColor);
        }
        {
            Vector3 start = position + up * radius;
            Debug.DrawLine(start, start + forwardAddition, debugLineColor);
        }
        {
            Vector3 start = position - up * radius;
            Debug.DrawLine(start, start + forwardAddition, debugLineColor);
        }
        {
            Vector3 start = position + right * radius;
            Debug.DrawLine(start, start + forwardAddition, debugLineColor);
        }
        {
            Vector3 start = position - right * radius;
            Debug.DrawLine(start, start + forwardAddition, debugLineColor);
        }
    }
}
