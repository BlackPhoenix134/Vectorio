using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    public static Events active;

    // Start is called before the first frame update
    public void Awake()
    {
        active = this;
    }

    // Fires the hub laser
    public event Action fireHubLaser;
    public void FireHubLaser()
    {
        if (fireHubLaser != null)
            fireHubLaser();
    }

    // Invoked when a bullet is fired
    public event Action<DefaultBullet, BaseEntity> onBulletFired;
    public void BulletFired(DefaultBullet bullet, BaseEntity target)
    {
        if (onBulletFired != null)
            onBulletFired(bullet, target);
    }

    // Invoked when a building with a rotating piece is placed
    public event Action<Rotator> onRotatorPlaced;
    public void RotatorPlaced(Rotator rotator)
    {
        if (onRotatorPlaced != null)
            onRotatorPlaced(rotator);
    }

    // Invoked when a building with a collector script is placed
    public event Action<Collector> onCollectorPlaced;
    public void CollectorPlaced(Collector collector)
    {
        if (onCollectorPlaced != null)
            onCollectorPlaced(collector);
    }

    // Invoked when a building with a storage script is placed
    public event Action<Storage> onStoragePlaced;
    public void StoragePlaced(Storage storage)
    {
        if (onStoragePlaced != null)
            onStoragePlaced(storage);
    }

    // Invoked when a building with a turret script is placed
    public event Action<DefaultTurret> onTurretRegistered;
    public void RegisterTurret(DefaultTurret turret)
    {
        if (onTurretRegistered != null)
            onTurretRegistered(turret);
    }

    // Invoked when a building is placed
    public event Action onLeftMousePressed;
    public void LeftMousePressed()
    {
        if (onLeftMousePressed != null)
            onLeftMousePressed();
    }

    // Invoked when a building is placed
    public event Action onRightMousePressed;
    public void RightMousePressed()
    {
        if (onRightMousePressed != null)
            onRightMousePressed();
    }

    // Invoked when a building is placed
    public event Action onRightMouseReleased;
    public void RightMouseReleased()
    {
        if (onRightMouseReleased != null)
            onRightMouseReleased();
    }

    // Invoked when a survival save is loaded
    public event Action<SurvivalData> onSurvivalLoaded;
    public void SurvivalLoaded(SurvivalData data)
    {
        if (onSurvivalLoaded != null)
            onSurvivalLoaded(data);
    }

    // Invoked when a hotbar is set
    public event Action<Tile, int> onHotbarSet;
    public void HotbarSet(Tile tile, int slot)
    {
        if (onHotbarSet != null)
            onHotbarSet(tile, slot);
    }

    // Invoked when input from the keyboard is detected
    public event Action<int> onNumberInput;
    public void NumberInput(int number)
    {
        if (onNumberInput != null)
            onNumberInput(number);
    }

    // Initializes variants
    public event Action<Variant> onVariantLoaded;
    public void VariantLoaded(Variant variant)
    {
        if (onVariantLoaded != null)
            onVariantLoaded(variant);
    }
}