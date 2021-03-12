﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    // Contains all active coins in scene
    [System.Serializable]
    public class ActiveBullets
    {
        // Constructor
        public ActiveBullets(Transform Object, float Speed, bool Tracker, int Piercing, int Damage)
        {
            this.Object = Object;
            this.Speed = Speed;
            this.Tracker = Tracker;
            this.Piercing = Piercing;
            this.Damage = Damage;
        }

        // Class variables
        public Transform Object { get; set; }
        public float Speed { get; set; }
        public bool Tracker { get; set; }
        public int Piercing { get; set; }
        public int Damage { get; set; }
        public List<Transform> IgnoreEnemies = new List<Transform>();

    }
    public List<ActiveBullets> Bullets;

    public LayerMask EnemyLayer;

    // Handles bullet movement and hit detection frame-by-frame
    public void Update()
    {
        for (int i = 0; i < Bullets.Count; i++)
            try
            {
                Bullets[i].Object.position += Bullets[i].Object.up * Bullets[i].Speed * Time.deltaTime;
                if (Bullets[i].Tracker)
                {
                    Bullets[i].Tracker = false;
                    RaycastHit2D hit = Physics2D.Raycast(Bullets[i].Object.position, Bullets[i].Object.up, 1.5f, EnemyLayer);
                    if (hit.collider != null && !Bullets[i].IgnoreEnemies.Contains(hit.collider.transform))
                        if (OnHit(i, hit.collider.transform)) { i--; continue; }
                }
                else
                {
                    Bullets[i].Tracker = true;
                    continue;
                }
            }
            catch
            {
                Bullets.RemoveAt(i);
                i--;
            }
    }

    // Registers a bullet to be handled by the updater in this script
    public void RegisterBullet(Transform bullet, float speed, int pierces, int damage)
    {
        Bullets.Add(new ActiveBullets(bullet, speed, true, pierces, damage));
    }

    // Called when a hit is detected in the updater 
    public bool OnHit(int bulletID, Transform other)
    {
        // Add the other transform to the ignore list for future collisions
        Bullets[bulletID].IgnoreEnemies.Add(other);

        // Get correct component
        switch (other.name)
        {
            case "Hive":
                other.GetComponent<SpawnerAI>().SpawnEnemy();
                break;
            case "Enemy Turret":
                other.GetComponent<EnemyTurretAI>().DamageTile(Bullets[bulletID].Damage);
                break;
            case "Enemy Wall":
                other.GetComponent<EnemyWallAI>().DamageTile(Bullets[bulletID].Damage);
                break;
            case "Enemy Mine":
                other.GetComponent<EnemyStaticAI>().DamageTile(Bullets[bulletID].Damage);
                break;
            default:
                Debug.Log("Applying damage");
                other.GetComponent<EnemyClass>().DamageEntity(Bullets[bulletID].Damage);
                break;
        }

        Bullets[bulletID].Piercing--;
        if (Bullets[bulletID].Piercing == 0)
        {
            BulletClass bds = Bullets[bulletID].Object.GetComponent<BulletClass>();
            if (other.name.Contains("Dark"))
                bds.SetHitEffect("DarkParticle");
            else if (other.name.Contains("Phantom"))
                bds.SetHitEffect("PhantomParticle");
            else
                bds.SetHitEffect("EnemyParticle");
            bds.collide();
            Bullets.RemoveAt(bulletID);
            return true;
        }
        return false;
    }
}
