using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile", menuName = "Buildings/Building")]
public class Building : Tile
{
    // Resource class
    [System.Serializable]
    public class Resources
    {
        public Resource.Currency resource;
        public int amount;
        public int modifier;
    }

    // Building base variables
    public int health;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int healthModifier;

    // Resources
    public Resources[] resources;

    // Materials
    public Material material;
    public ParticleSystem particle;

    public virtual void CreateStat()
    {
        foreach (Resources resource in resources) 
        {
            string name = nameof(resource.resource);
            UIEvents.active.CreateStat(new Stat(name, resource.amount, resource.modifier, Sprites.active.GetByName(name), true));
        }

        UIEvents.active.CreateStat(new Stat("Health", health, healthModifier, Sprites.active.health));
    }
}
