using UnityEngine;

// -- Generics --

public abstract class IMod
{
    public bool Expired => timer < Time.time;
    public abstract bool Dispose {get;}
    public IMod(float modTime) { timer = Time.time + modTime; }
    readonly float timer;
}

public interface IModdable
{
    public abstract void AddMod(PotionAbility modtype, float amount);
}


// -- Specific Mods --

public class ModSpeed : IMod
{
    public ModSpeed(PlayerController player, float modAmount, float modTime) : base(modTime)
    {
        this.player = player;
        player.EngineMaxSpeedModifier += modAmount;
        modifier = modAmount;
    }
    // Can't let the dtor handle popping...
    public override bool Dispose => (player.EngineMaxSpeedModifier -= modifier) < float.MaxValue;

    PlayerController player;
    float modifier;
}

public class ModPickupRange : IMod
{
    public ModPickupRange(Collider col, float modAmount, float modTime) : base(modTime)
    {
        this.col = col;
        col.transform.localScale += Vector3.one * modAmount;
        modifier = modAmount;
    }

    // Can't let the dtor handle popping...s
    public override bool Dispose => (col.transform.localScale -= Vector3.one * modifier) != Vector3.positiveInfinity;

    Collider col;
    float modifier;
}

public class ModVision : IMod
{
    public ModVision(Camera cam, float modAmount, float modTime) : base(modTime)
    {
        this.camera = cam;
        cam.fieldOfView += modAmount;
        modifier = modAmount;
    }

    // Can't let the dtor handle popping...s
    public override bool Dispose => (camera.fieldOfView -= modifier) != int.MaxValue;

    Camera camera;
    float modifier;
}

public class ModButterFingers : IMod
{
    public ModButterFingers(Inventory inv, int amount): base(0.5f) { inv.DropRandomIngredients(amount); }
    // Can't let the dtor handle popping...s
    public override bool Dispose => true;
}

public class ModConfuse : IMod
{
    public ModConfuse(PlayerController player, float modTime): base(modTime)
    {
         this.player = player; 
         ++player.ConfusedCount; 
    }
    // Can't let the dtor handle popping...s
    public override bool Dispose => ( --player.ConfusedCount ) != int.MaxValue;

    PlayerController player;
}