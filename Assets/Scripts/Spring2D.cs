using System;
using UnityEngine;

[Serializable]
public class Spring2D
{
    public float springStrength;
    public float springDampener;
    public float maxSpringDistance;
    public float tolerence;

    public float velocity;
    public float position;
    public float force;

    public void AddForce(float force)
    {
        this.force += force;
        //rb.AddForce(rb.transform.right * force);
    }

    public void PullSpring(float distance)
    {
        position += distance;
        velocity = 0.0f;
        force = 0.0f;

        position = Mathf.Clamp(position, -maxSpringDistance, maxSpringDistance);
    }

    public void SetSpringPosition(float position)
    {
        this.position = position;
        velocity = 0.0f;
        force = 0.0f;
    }

    public void FixedUpdate()
    {
        float springCompression = -position / maxSpringDistance;

        float dampeningForce = velocity * springDampener;
        float springForce    = springStrength * springCompression;

        float netForce = springForce - dampeningForce;
        //rb.AddForce(rb.transform.right * netForce);
        //joint.attachedRigidbody.AddForceX(netForce)
        force += netForce;

        velocity += force;

        position += velocity * Time.fixedDeltaTime;
        velocity = 0.0f;
        force = 0.0f;

        if (Mathf.Abs(position) < tolerence)
        {
            velocity = 0;
            force = 0.0f;
            position = 0;
        }
    }
}

//using System;
//using UnityEngine;

//[Serializable]
//public class Spring2D
//{
//    public float restingDistance;
//    public float springStrength;
//    public float springDampener;
//    public float maxSpringDistance;
//    public float tolerence;

//    public float velocity;
//    public float position;
//    public float force;

//    public void AddForce(float force)
//    {
//        this.force += force;
//        //rb.AddForce(rb.transform.right * force);
//    }

//    public void PullSpring(float distance)
//    {
//        //position = distance;
//    }

//    public void FixedUpdate()
//    {
//        float springCompression = (restingDistance - position) / maxSpringDistance;

//        float dampeningForce = velocity * springDampener;
//        float springForce = springStrength * springCompression;

//        float netForce = springForce - dampeningForce;
//        Debug.Log(netForce);
//        //rb.AddForce(rb.transform.right * netForce);
//        //joint.attachedRigidbody.AddForceX(netForce)
//        force += netForce;

//        velocity += force;

//        position += velocity * Time.fixedDeltaTime;

//        if (Mathf.Abs(position - restingDistance) < tolerence)
//        {
//            force = 0.0f;
//        }
//    }
//}