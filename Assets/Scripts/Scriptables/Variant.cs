using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Variants", menuName = "Variants/Normal")]
public class Variant : ScriptableObject
{
    // Variant info
    public new string name;
    [TextArea] public string desc;

    // Variant modifiers
    public float healthModifier;
    public float damageModifier;
    public float speedModifier;

    // Spawn on death
    [System.Serializable]
    public class EnemySpawn
    {
        public Enemy enemy;
        public int amount;
        public float radius;
    }
    public EnemySpawn[] spawns;

    // Materials and stuff
    public Material border;
    public Material fill;
    public Material trail;
    public ParticleSystem particle;
}