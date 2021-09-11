using UnityEngine;

[CreateAssetMenu(fileName = "New Turret", menuName = "Building/Turret")]
public class Turret : Building
{
    // IAudible interface variables
    public AudioClip sound;

    // Base turret stat variables
    public float damage;
    public float range;
    public float rotationSpeed;
    public float fireRate;
    public int bulletPierces;
    public int bulletAmount;
    public float bulletSpeed;
    public float bulletSpread;

    // Base turret modifiers
    [HideInInspector] public float damageModifier;
    [HideInInspector] public float rangeModifier;
    [HideInInspector] public float rotationSpeedModifier;
    [HideInInspector] public float fireRateModifier;
    [HideInInspector] public int bulletPiercesModifier;
    [HideInInspector] public int bulletAmountModifier;
    [HideInInspector] public float bulletSpeedModifier;
    [HideInInspector] public float bulletSpreadModifier;

    // Set panel stats
    // This gets used to set the stats on the building menu panel
    public override void CreateStats(Panel panel)
    {
        panel.CreateStat(new Stat("Damage", damage, damageModifier, Sprites.GetSprite("Damage")));
        panel.CreateStat(new Stat("Range", range, damageModifier, Sprites.GetSprite("Range")));
        panel.CreateStat(new Stat("Firerate", fireRate, damageModifier, Sprites.GetSprite("Firerate")));
        panel.CreateStat(new Stat("Pierces", bulletPierces, damageModifier, Sprites.GetSprite("Pierces")));
        panel.CreateStat(new Stat("Bullets", bulletAmount, damageModifier, Sprites.GetSprite("Bullets")));
        panel.CreateStat(new Stat("Spread", bulletSpread, damageModifier, Sprites.GetSprite("Spread")));

        // Base method
        base.CreateStats(panel);
    }
}
