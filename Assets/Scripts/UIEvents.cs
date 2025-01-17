using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvents : MonoBehaviour
{
    public static UIEvents active;

    // Start is called before the first frame update
    public void Awake()
    {
        active = this;
    }

    // On add resource
    public event Action<Resource.CurrencyType, int> onAddResource;
    public void AddResource(Resource.CurrencyType currency, int amount)
    {
        if (onAddResource != null)
            onAddResource(currency, amount);
    }

    // Invoked when a bullet is fired
    public event Action onBuildingMenuPressed;
    public void MenuOpened()
    {
        if (onBuildingMenuPressed != null)
            onBuildingMenuPressed();
    }

    // Invoked when a building is clicked
    public event Action<Entity> onEntityPressed;
    public void EntityPressed(Entity entity)
    {
        if (onEntityPressed != null)
            onEntityPressed(entity);
    }

    // Invoked when a building is clicked
    public event Action<Building> onBuildingPressed;
    public void BuildingPressed(Building building)
    {
        if (onBuildingPressed != null)
            onBuildingPressed(building);
    }

    public event Action<int> onHotbarPressed;
    public void HotbarPressed(int index)
    {
        if (onHotbarPressed != null)
            onHotbarPressed(index);
    }

    public event Action onDisableHotbar;
    public void DisableHotbar()
    {
        if (onDisableHotbar != null)
            onDisableHotbar();
    }

    public event Action onQuitGame;
    public void QuitGame()
    {
        if (onQuitGame != null)
            onQuitGame();
    }
}
