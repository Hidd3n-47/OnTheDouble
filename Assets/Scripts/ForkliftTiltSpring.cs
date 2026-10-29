using UnityEngine;

public class ForkliftTiltSpring : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform body;
    [SerializeField] private Forklift forklift;

    [Header("Tilt Settings")]
    [SerializeField] private float maxForwardTiltForward  = 6f;
    [SerializeField] private float maxForwardTiltBackward = 12f;
    [SerializeField] private float maxTurnTilt            = 8f;

    [Header("Spring Settings")]
    [SerializeField] private float springStrength = 35f;
    [SerializeField] private float springDamping  = 5f;
    [SerializeField] private float tiltMass       = 1f;

    [Header("Boost Settings")]
    [SerializeField] private float boostSpringMultiplier  = 1.8f;
    [SerializeField] private float boostDampingMultiplier = 0.6f;
    [SerializeField] private float boostTiltMultiplier    = 1.3f;

    [Header("Input Smoothing")]
    [SerializeField] private float inputSmoothSpeed = 3f;

    private Vector2 currentTilt;
    private Vector2 tiltVelocity;
    private Vector2 smoothedInput;

    void Update()
    {
        float rawForward = Mathf.Clamp(forklift.Engine.Power, -1f, 1f);
        float rawTurn    = Mathf.Clamp(forklift.Wheels.Power, -1f, 1f);
        bool boosting    = forklift.IsBoosting;

        float spring   = boosting ? springStrength * boostSpringMultiplier  : springStrength;
        float damping  = boosting ? springDamping  * boostDampingMultiplier : springDamping;
        float tiltMult = boosting ? boostTiltMultiplier : 1f;

        // Smoothen input so that it's not as harsh when full lock turning.
        smoothedInput.x = Mathf.Lerp(smoothedInput.x, rawForward, Time.deltaTime * inputSmoothSpeed);
        smoothedInput.y = Mathf.Lerp(smoothedInput.y, rawTurn, Time.deltaTime * inputSmoothSpeed);

        float forwardLimit = smoothedInput.x >= 0 ? maxForwardTiltForward : maxForwardTiltBackward;

        float targetPitch = -(smoothedInput.x * forwardLimit);
        float targetRoll  = -(smoothedInput.y * maxTurnTilt  * tiltMult) * (forklift.Engine.Power < 0.0f ? 1.0f : -1.0f);

        Vector2 target = new Vector2(targetPitch, targetRoll);

        // Dampening.
        Vector2 force        = (target - currentTilt) * spring;
        Vector2 acceleration = force / tiltMass;

        tiltVelocity += acceleration * Time.deltaTime;
        tiltVelocity *= Mathf.Exp(-damping * Time.deltaTime);

        currentTilt += tiltVelocity * Time.deltaTime;

        currentTilt.x = Mathf.Clamp(currentTilt.x, -maxForwardTiltBackward, maxForwardTiltForward);
        currentTilt.y = Mathf.Clamp(currentTilt.y, -maxTurnTilt, maxTurnTilt);


        currentTilt.x *= currentTilt.x < 0.0f ? tiltMult : 1.0f;

        body.localRotation = Quaternion.Euler(currentTilt.x, 0f, currentTilt.y);
    }
}
