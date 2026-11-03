using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour, IModdable
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

    [SerializeField] private Outline     outline;
    [SerializeField] private LayerMask   collisionLayer;

    private Inventory inventory;

    [SerializeField] ResourceWheelUi leftResourceWheel;
    [SerializeField] ResourceWheelUi rightResourceWheel;
    [SerializeField] Collider pickupCollider;

    // This will need removing. I was testing functionality, but continuous 
    // sounds cannot be easily faded out without extra dependencies, so this isn't viable.
    [SerializeField] ContinuousEffect movementWhooshing;

    public float EngineMaxSpeedModifier { get; set; }
    public int ConfusedCount { get; set; }

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

    public ParticleSystem ParticleLeft;
    public ParticleSystem ParticleRight;
    public ParticleSystem ParticleBack;

    public float leanThreashold = 20.0f;


    bool leaningLeft = false;
    bool leaningRight = false;

    private Vector3 offset;

    List<IMod> m_mods = new List<IMod>();

    private void Start()
    {
        ConfusedCount = 0;
        EngineMaxSpeedModifier = 1;

        offset = camera.position - transform.position;

        inventory = GetComponent<Inventory>();

        rightResourceWheel.Initialize(inventory);
        leftResourceWheel.Initialize(inventory);
    }


    private void Update()
    {
        camera.position = transform.position + offset;

        outline.enabled = (Physics.SphereCast(new Ray(camera.position, camera.forward), 1.0f, (transform.position - camera.position).magnitude, collisionLayer));

        if(currentLean >= leanThreashold)
        {
            if (!leaningLeft)
            {
                ParticleLeft.Play();
            }
            leaningLeft = true;
        }
        else
        {
            leaningLeft = false;
            ParticleLeft.Stop();
        }

        if (currentLean <= -leanThreashold)
        {
            if (!leaningRight)
            {
                ParticleRight.Play();
            }
            leaningRight = true;
        }
        else
        {
            leaningRight = false;
            ParticleRight.Stop();
        }

        bool leftBumperDown  = (Gamepad.current?.leftShoulder.ReadValue()  ?? 0.0f) > 0.5f || Input.GetKey(KeyCode.Q);
        bool rightBumperDown = (Gamepad.current?.rightShoulder.ReadValue() ?? 0.0f) > 0.5f || Input.GetKey(KeyCode.E);

        if(ConfusedCount > 0)
        {
            bool temp = rightBumperDown;
            rightBumperDown = leftBumperDown;
            leftBumperDown = temp;
        }

        if (leftBumperDown)
        {
            leftResourceWheel.Activate();
            rightResourceWheel.Deactivate();
        }
        else if (rightBumperDown)
        {
            rightResourceWheel.Activate();
            leftResourceWheel.Deactivate();
        }
        else
        {
            leftResourceWheel.Deactivate();
            rightResourceWheel.Deactivate();
        }
        
        // Mostly this weird implementation is for shiggles. - M@
        m_mods.RemoveAll(mod => mod.Expired ? mod.Dispose : false);
    }
    private void FixedUpdate()
    {
        // Input.
        float acceleration    = Gamepad.current?.rightTrigger.ReadValue() ?? (Input.GetKey(KeyCode.W) ? 1.0f : 0.0f);
        float brake           = Gamepad.current?.leftTrigger.ReadValue()  ?? (Input.GetKey(KeyCode.S) ? 1.0f : 0.0f);
        forklift.AcceleratingThisFrame = acceleration != 0.0f;
        forklift.BrakingThisFrame      = brake != 0.0f;

        float keyboardHorizontal = (Input.GetKey(KeyCode.A) ? -1.0f : 0.0f) + (Input.GetKey(KeyCode.D) ? 1.0f : 0.0f);
        float horizontalInput = (float)Math.Round(Gamepad.current?.leftStick.value.x ?? keyboardHorizontal, 2);

        if(ConfusedCount > 0)
        {
            // Oh yeah. We're doing this :P
            horizontalInput *= -1;
        }

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
            movementWhooshing.Play();
            forklift.Engine.Power = Math.Clamp(forklift.Engine.Power,
                0.0f,
                enginePowerMaxCurve.Evaluate(engineAccelerationTimer.position) * engineMaxPowerSpring.position * EngineMaxSpeedModifier);
        }
        else if (engineInput < 0.0f)
        {
            movementWhooshing.Play();
            forklift.Engine.Power = Math.Max(forklift.Engine.Power,
                0.0f);
        }
        else
        {
            movementWhooshing.Stop();
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

    public virtual void AddMod(PotionAbility ability, float amount)
    { 
        switch (ability)
        {
            case PotionAbility.Speed:
            case PotionAbility.Slow:
                AddSpeedMod(amount);
                break;
            case PotionAbility.Magnetism:
                AddPickupRangeMod(amount);
                break;
            case PotionAbility.Butterfingers:
                AddButterFingersMod((int)amount);
                break;
            case PotionAbility.VisionBoost:
            case PotionAbility.Blinkers:
                AddVisionMod(amount);
                break;
            case PotionAbility.Confuse:
                AddConfuseMod();
                break;
            default:
                break;
        }
    }

    public void AddSpeedMod(float amount)
    {
        const float modifierTime = 4f;
        m_mods.Add(new ModSpeed(this, amount, modifierTime));
    }

    public void AddPickupRangeMod(float amount)
    {
        const float modifierTime = 8f;
        if (pickupCollider) m_mods.Add(new ModPickupRange(pickupCollider, amount, modifierTime));
    }

    public void AddVisionMod(float amount)
    {
        const float modifierTime = 15f;
        Camera cam = camera.GetComponent<Camera>();
        if(cam) m_mods.Add(new ModVision(camera.GetComponent<Camera>(), amount, modifierTime));
        
    }

    public void AddButterFingersMod(int amount)
    {
        if(inventory) m_mods.Add(new ModButterFingers(inventory, amount));
    }

    public void AddConfuseMod()
    {
        const float modifierTime = 6f;
        m_mods.Add(new ModConfuse(this, modifierTime));
    }
}
