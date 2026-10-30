using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Engine")]
    [Tooltip("The curve of acceleration based off how long the accelerate has been held down.")]
    [SerializeField] private float engineAccelerationCurve = 1.0f;
    [SerializeField] private float engineDecelerationCurve = -0.5f;
    [Tooltip("The curve of maximum power multiplier based off the acceleration input.")]
    [SerializeField] private AnimationCurve engineMaxPowerMultiplierCurve;
    [SerializeField] private AnimationCurve engineFrictionCurve;
    [SerializeField] private AnimationCurve enginePowerMaxCurve;
    [Tooltip("A spring used as a timer to get max speed - length is duration to reach max speed.")]
    [SerializeField] private Spring2D engineAccelerationTimer;
    [SerializeField] private float engineAccelerateTimerBaseModifer = 2.0f;
    [SerializeField] private Spring2D engineDecelerationTimer;
    [SerializeField] private float engineDecelerateTimerBaseModifier = 2.0f;
    [Tooltip("A spring used to gradually reduce max speed down as acceleration button isn't pressed.")]
    [SerializeField] private Spring2D engineMaxPowerSpring;

    [Header("Turning")]
    [Tooltip("The amount of power the wheels have based off the engine power ratio. (i.e. max speed = 1)")]
    [SerializeField] private float wheelsPowerCurve = 10.0f;
    [SerializeField] private float wheelsPowerMaxCurve = 170.0f;
    [Tooltip("The amount of friction the wheels experience based off engine power ratio. (i.e. max speed = 1)")]
    [SerializeField] private float wheelsFrictionCurve = 125.0f;

    public Transform visualModel;
    public float maxLeanAngle = 30f;
    public float leanSpeed = 5f;

    private float currentLean = 0f;

    [SerializeField] private AnimationCurve horizontalInputScaling;

    [SerializeField] private Forklift    forklift;
    [SerializeField] private OptionsMenu optionsMenu;

    [SerializeField] private Transform   cauldron;

    private Inventory inventory;

    public Transform GetCauldronTransform()
    {
        return cauldron;
    }

    public void AddResource(ResourceType resource)
    {
        inventory.AddResource(resource);
    }

    public bool IsBoosting => forklift.IsBoosting;
    public float EnginePower => forklift.Engine.Power;
    public float EnginePowerRatio => Mathf.Abs(forklift.Engine.Power) / enginePowerMaxCurve.Evaluate(engineMaxPowerSpring.position);
    public float EngineWheelRatio => Mathf.Abs(forklift.Wheels.Power) / wheelsPowerMaxCurve;

    public Transform camera;

    public GameObject ParticleLeft;
    public GameObject ParticleRight;
    public float leanThreashold = 20.0f;

    private Vector3 offset;

    private void Start()
    {
        offset = camera.position - transform.position;

        inventory = GetComponent<Inventory>();
    }


    private void Update()
    {
        camera.position = transform.position + offset;

        ParticleLeft.SetActive(currentLean >= leanThreashold);
        ParticleRight.SetActive(currentLean <= -leanThreashold);
    }
    private void FixedUpdate()
    {

        
        // Input.
        float acceleration    = Gamepad.current?.rightTrigger.ReadValue() ?? 0.0f;
        float brake           = Gamepad.current?.leftTrigger.ReadValue()  ?? 0.0f;
        forklift.AcceleratingThisFrame = acceleration != 0.0f;
        forklift.BrakingThisFrame      = brake != 0.0f;

        float horizontalInput = (float)Math.Round(Gamepad.current?.leftStick.value.x ?? 0, 2);
        float targetLean = horizontalInput * maxLeanAngle;
        currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);
        visualModel.localRotation = Quaternion.Euler(0f, 0f, -currentLean);

        // Engine Power.
        float engineInput = acceleration - brake;

        bool accelerating = !Mathf.Approximately(acceleration, 0.0f);
        bool braking      = !Mathf.Approximately(brake       , 0.0f);

        float enginePowerAddition = 0.0f;

        // Start off by seeing how long the acceleration is pressed for.
        // Updating this spring means we can reach a higher speed.
        if (accelerating)
        {
            forklift.Engine.PreventResistanceForOneFrame = true;

            engineAccelerationTimer.PullSpring(Time.fixedDeltaTime * engineAccelerateTimerBaseModifer);

            enginePowerAddition = engineAccelerationCurve;
        }
        else
        {
            engineAccelerationTimer.FixedUpdate();
        }

        if (braking)
        {
            forklift.Engine.PreventResistanceForOneFrame = true;

            engineDecelerationTimer.PullSpring(Time.fixedDeltaTime * engineDecelerateTimerBaseModifier);

            enginePowerAddition = engineDecelerationCurve;
        }
        else
        {
            engineDecelerationTimer.FixedUpdate();
        }

        // Get the maximum possible linear velocity of the vehicle based of how far the trigger is pressed.
        float maxPowerFromCurve = engineMaxPowerMultiplierCurve.Evaluate(engineInput);
        if (Mathf.Abs(engineMaxPowerSpring.position) < Mathf.Abs(maxPowerFromCurve))
        {
            // Don't fully actuate the spring but extended a small amount if we are more actuated than previously.
            engineMaxPowerSpring.PullSpring(Time.fixedDeltaTime);
        }
        else
        {
            engineMaxPowerSpring.FixedUpdate();
        }

        forklift.Engine.Power += enginePowerAddition;
        forklift.Engine.Power = Mathf.Max(forklift.Engine.Power, 0.0f);

        if (engineInput > 0.0f)
        {
            forklift.Engine.Power = Math.Clamp(forklift.Engine.Power,
                0.0f,
                enginePowerMaxCurve.Evaluate(engineAccelerationTimer.position) * engineMaxPowerSpring.position);
        }
        else if (engineInput < 0.0f)
        {
            forklift.Engine.Power = Math.Max(forklift.Engine.Power,
                0.0f);
        }

        forklift.Engine.Resistance = engineFrictionCurve.Evaluate(EnginePowerRatio);

        float horizontalScale = horizontalInputScaling.Evaluate(Mathf.Abs(horizontalInput));

        // Turning Power.
        if (horizontalInput > 0.0f)
        {
            float counterSteerMultiplier = forklift.Wheels.Power < 0.0f ? 2.0f : 1.0f;
            forklift.Wheels.Power += wheelsPowerCurve * counterSteerMultiplier * horizontalScale;
            //todo this max should be inside the forklift get and set property.
            forklift.Wheels.Power = Math.Min(forklift.Wheels.Power, wheelsPowerMaxCurve * horizontalScale);
        }
        else if (horizontalInput < 0.0f)
        {
            float counterSteerMultiplier = forklift.Wheels.Power > 0.0f ?  2.0f : 1.0f;

            forklift.Wheels.Power -= wheelsPowerCurve * counterSteerMultiplier * horizontalScale;
            //todo this max should be inside the forklift get and set property.
            forklift.Wheels.Power = Math.Max(forklift.Wheels.Power, -wheelsPowerMaxCurve * horizontalScale);
        }
        else
        {
            forklift.Wheels.Resistance = wheelsFrictionCurve;
            forklift.Wheels.Resistance *= 5.0f;


        }
    }
}
