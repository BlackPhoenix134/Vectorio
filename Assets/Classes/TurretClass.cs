﻿using System.Collections.Generic;
using UnityEngine;

public abstract class TurretClass : TileClass
{
    // Bullet Handler
    private BulletHandler bulletHandler;

    // Weapon variables
    public float fireRate;
    public float bulletSpread;
    public float bulletAmount;
    public float rotationSpeed;
    public int range;
    public Transform[] FirePoints;
    public Rigidbody2D Gun;
    public GameObject Bullet;
    public HashSet<GameObject> nearbyEnemies = new HashSet<GameObject>();

    // Global variables
    protected float nextFire = 0;
    protected float timePassed = 0;
    protected bool hasTarget = false;
    protected GameObject target = null;
    protected float enemyAngle;
    protected float gunRotation;

    // Let's see if this shit works amiright my dude
    private void Start()
    {
        bulletHandler = GameObject.Find("Bullet Handler").GetComponent<BulletHandler>();
    }

    protected GameObject FindNearestEnemy()
    {
        var colliders = Physics2D.OverlapCircleAll(
            this.gameObject.transform.position, 
            range + Research.bonus_range, 
            1 << LayerMask.NameToLayer("Enemy"));
        GameObject result = null;
        float closest = float.PositiveInfinity;

        foreach (Collider2D collider in colliders)
        {
            float distance = (collider.transform.position - this.transform.position).sqrMagnitude;
            if (distance < closest) {
                result = collider.gameObject;
                closest = distance;
            }
        }
        return result;
    }

    protected GameObject FindNearestPlayer()
    {
        var colliders = Physics2D.OverlapCircleAll(
            this.gameObject.transform.position,
            range + Research.bonus_range,
            1 << LayerMask.NameToLayer("Building"));
        GameObject result = null;
        float closest = float.PositiveInfinity;

        foreach (Collider2D collider in colliders)
        {
            float distance = (collider.transform.position - this.transform.position).sqrMagnitude;
            if (distance < closest)
            {
                result = collider.gameObject;
                closest = distance;
            }
        }
        return result;
    }

    protected void RotateTowardsPlayer()
    {
        if (!hasTarget)
        {
            target = FindNearestPlayer();
        }

        // If a target exists, shoot at it
        if (target != null)
        {
            // Flag hasTarget
            hasTarget = true;

            // Rotate turret towards target
            Vector2 TargetPosition = new Vector2(target.transform.position.x, target.transform.position.y);
            Vector2 lookDirection = (TargetPosition - Gun.position);
            enemyAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
            enemyAngle = AlignRotation(enemyAngle);
            gunRotation = AlignRotation(Gun.rotation);

            // Smooth rotation when targetting enemies
            if (((gunRotation >= enemyAngle && !(gunRotation >= 270 && enemyAngle <= 90)) || (gunRotation <= 90 && enemyAngle >= 270))
            && !((gunRotation - enemyAngle) <= 0.3 && (gunRotation - enemyAngle) >= -0.3))
            {
                if (Gun.rotation - rotationSpeed < enemyAngle)
                {
                    Gun.rotation = enemyAngle;
                }
                else
                {
                    Gun.rotation -= rotationSpeed;
                }
            }
            else if (!((gunRotation - enemyAngle) <= 0.3 && (gunRotation - enemyAngle) >= -0.3))
            {
                if (Gun.rotation + rotationSpeed > enemyAngle)
                {
                    Gun.rotation = enemyAngle;
                }
                else
                {
                    Gun.rotation += rotationSpeed;
                }
            }
        }
    }

    protected void RotateTowardNearestEnemy() 
    {
        if (!hasTarget) {
            target = FindNearestEnemy();
        }

        // If a target exists, shoot at it
        if (target != null)
        {
            // Flag hasTarget
            hasTarget = true;

            // Rotate turret towards target
            Vector2 TargetPosition = new Vector2(target.transform.position.x, target.transform.position.y);
            Vector2 lookDirection = (TargetPosition - Gun.position);
            enemyAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
            enemyAngle = AlignRotation(enemyAngle);
            gunRotation = AlignRotation(Gun.rotation);

            // Smooth rotation when targetting enemies
            if (((gunRotation >= enemyAngle && !(gunRotation >= 270 && enemyAngle <= 90)) || (gunRotation <= 90 && enemyAngle >= 270))
            && !((gunRotation - enemyAngle) <= 0.3 && (gunRotation - enemyAngle) >= -0.3))
            {
                if (Gun.rotation - rotationSpeed < enemyAngle) 
                {
                    Gun.rotation = enemyAngle;
                } else 
                {
                    Gun.rotation -= rotationSpeed;
                }
            }
            else if (!((gunRotation - enemyAngle) <= 0.3 && (gunRotation - enemyAngle) >= -0.3))
            {
                if (Gun.rotation + rotationSpeed > enemyAngle) 
                {
                    Gun.rotation = enemyAngle;
                } else 
                {
                    Gun.rotation += rotationSpeed;
                }
            }
        }
    }

    // Creates bullet object
    protected void Shoot(GameObject prefab, Transform pos, float multiplier = 1)
    {
        if (nextFire > 0)
        {
            nextFire -= Time.deltaTime;
            return;
        }
        else
        {
            for (int i = 0; i < bulletAmount; i += 1)
            {
                pos.position = new Vector3(pos.position.x, pos.position.y, 0);
                Debug.Log(pos.localRotation);
                GameObject bullet = Instantiate(prefab, pos.position, pos.rotation);
                bullet.transform.rotation = FirePoints[0].rotation;
                bullet.transform.Rotate(0f, 0f, Random.Range(-bulletSpread, bulletSpread));

                // Set optional damage multiplier
                bullet.GetComponent<BulletClass>().MultiplyDamage(multiplier);

                // Register the bullet
                float speed = bullet.GetComponent<BulletClass>().GetSpeed();
                bulletHandler.RegisterBullet(bullet.transform, Random.Range(speed-10,speed+10));

                // This is like, physics and stuff (until it's not hehe)
                // bullet.GetComponent<Rigidbody2D>().AddForce(BulletSpread(pos.up, Random.Range(bulletSpread, -bulletSpread)) * bulletForce * Random.Range(1f, 1.5f), ForceMode2D.Impulse);
            }
            if (fireRate - Research.bonus_firerate <= 0.03f) nextFire = 0.03f;
            else nextFire = fireRate - Research.bonus_firerate;
        }
    }

    // Calculate bullet spread
    private static Vector2 BulletSpread(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    protected static float AlignRotation(float r) {
        float temp = r;
        while (temp < 0f) 
        {
            temp += 360f;
        }
        while (temp > 360f)
        {
            temp -= 360f;
        }
        return temp;
    }
}
