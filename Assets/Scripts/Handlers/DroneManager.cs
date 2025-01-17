﻿// This script handles all active drones each frame
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    // Builder mode
    public enum BuildPriority
    {
        closest,
        furthest,
        cheapest,
        expensive,
        droneport,
        energizer,
        defense,
        resource,
        power,
        cooling
    }
    public BuildPriority buildPriority;
    public bool ignorePriority = false;

    // Priority buildings
    public Building droneport;
    public Building energizer;

    // Get active instance
    public static DroneManager active;
    public void Awake() { active = this; }

    // List of all drone types
    public List<Drone> droneTypes;

    // Target lists
    public List<GhostTile> ghostTiles;
    public List<ResourceTile> resourceTiles;
    public List<BaseTile> damagedTiles;

    // Available drones 
    public List<Drone> builderDrones;
    public List<Drone> resourceDrones;
    public List<Drone> fixerDrones;

    // Drones actively moving
    public List<Drone> activeDrones;

    // Add a ghost
    public void AddGhost(GhostTile ghost)
    {
        ghostTiles.Add(ghost);
        ignorePriority = false;
    }

    // Add a drone
    public void AddDrone(Drone drone)
    {
        if (drone.type == Drone.DroneType.Builder) 
            builderDrones.Add(drone);
        else if (drone.type == Drone.DroneType.Resource)
            resourceDrones.Add(drone);
        else if (drone.type == Drone.DroneType.Fixer)
            fixerDrones.Add(drone);
    }

    // Move drones
    public void Update()
    {
        UpdateConstructionDrones();
        UpdateActiveDrones();
    }

    // Update active drones
    public void UpdateActiveDrones()
    {
        for(int i = 0; i < activeDrones.Count; i++)
        {
            switch(activeDrones[i].stage)
            {
                // Reset drone
                case Drone.Stage.ReadyToDeploy:
                    active.AddDrone(activeDrones[i]);
                    activeDrones.RemoveAt(i);
                    break;

                // Open plates
                case Drone.Stage.ExitingPort:
                    activeDrones[i].ExitingPort();
                    break;

                // Move to target
                case Drone.Stage.MovingToTarget:
                    activeDrones[i].Move();
                    break;

                // Target reached
                case Drone.Stage.ReturningToPort:
                    activeDrones[i].Move();
                    break;

                // Entering port
                case Drone.Stage.EnteringPort:
                    activeDrones[i].EnteringPort();
                    break;
            }
        }
    }

    // Check construction drones
    public void UpdateConstructionDrones()
    {
        // Local variables 
        Drone drone = null;
        GhostTile ghostTile = null;
        float valueOne = 0;
        float valueTwo = Mathf.Infinity;

        // Check to make sure enough build drones and ghost tiles exist
        if (ghostTiles.Count > 0)
        {
            if (builderDrones.Count > 0)
            {
                // Loop through all ghost tiles
                for (int a = 0; a < ghostTiles.Count; a++)
                {
                    // Check if null
                    if (ghostTiles[a] != null)
                    {
                        // Check resources
                        if (Resource.active.CheckResources(ghostTiles[a].building))
                        {
                            // Check if priority should be ignored
                            if (ignorePriority)
                            {
                                // Assign closest drone found
                                drone = FindClosestDrone(ghostTiles[a].transform.position);
                                if (drone != null) SetBuilderTarget(drone, ghostTiles[a]);
                                return;
                            }
                            else
                            {
                                // Switch build priority
                                switch (buildPriority)
                                {
                                    // CLOSEST PRIORITY
                                    case BuildPriority.closest:

                                        // Assign closest drone found
                                        drone = FindClosestDrone(ghostTiles[a].transform.position);
                                        if (drone != null) SetBuilderTarget(drone, ghostTiles[a]);
                                        return;

                                    // CHEAPEST PRIORITY
                                    case BuildPriority.cheapest:

                                        // Calculate total cost
                                        foreach (Building.Resources resource in ghostTiles[a].building.resources)
                                            if (resource.add) { valueOne -= resource.amount; }
                                            else { valueOne += resource.amount; }

                                        // Compare to current cheapest
                                        if (valueOne < valueTwo)
                                        {
                                            valueTwo = valueOne;
                                            ghostTile = ghostTiles[a];
                                        }

                                        break;

                                    // EXPENSIVE PRIORITY
                                    case BuildPriority.expensive:

                                        // Calculate total cost
                                        foreach (Building.Resources resource in ghostTiles[a].building.resources)
                                            if (resource.add) { valueTwo -= resource.amount; }
                                            else { valueTwo += resource.amount; }

                                        // Compare to current cheapest
                                        if (valueTwo > valueOne)
                                        {
                                            valueOne = valueTwo;
                                            ghostTile = ghostTiles[a];
                                        }

                                        break;

                                    // DRONEPORT PRIORITY
                                    case BuildPriority.droneport:

                                        // See if building is a droneport (yay magic strings)
                                        if (ghostTiles[a].building == droneport)
                                        {
                                            drone = FindClosestDrone(ghostTiles[a].transform.position);
                                            if (drone != null) SetBuilderTarget(drone, ghostTiles[a]);
                                            return;
                                        }

                                        // If no building found, set ghost tile for closest
                                        if (ghostTile != null) ghostTile = ghostTiles[a];

                                        break;

                                    // ENERGIZER PRIORITY
                                    case BuildPriority.energizer:

                                        // See if building is a energizer
                                        if (ghostTiles[a].building == energizer)
                                        {
                                            drone = FindClosestDrone(ghostTiles[a].transform.position);
                                            if (drone != null) SetBuilderTarget(drone, ghostTiles[a]);
                                            return;
                                        }

                                        // If no building found, set ghost tile for closest
                                        if (ghostTile != null) ghostTile = ghostTiles[a];

                                        break;

                                    // DEFENSE PRIORITY
                                    case BuildPriority.defense:

                                        // See if building is a defense
                                        if (ghostTiles[a].building.inventoryHeader == Entity.InvHeader.Defense)
                                        {
                                            drone = FindClosestDrone(ghostTiles[a].transform.position);
                                            if (drone != null) SetBuilderTarget(drone, ghostTiles[a]);
                                            return;
                                        }

                                        // If no building found, set ghost tile for closest
                                        if (ghostTile != null) ghostTile = ghostTiles[a];

                                        break;

                                    // RESOURCE PRIORITY
                                    case BuildPriority.resource:

                                        // See if building is a defense
                                        if (!ghostTiles[a].building.obj.GetComponent<ResourceTile>())
                                        {
                                            drone = FindClosestDrone(ghostTiles[a].transform.position);
                                            if (drone != null) SetBuilderTarget(drone, ghostTiles[a]);
                                            return;
                                        }

                                        // If no building found, set ghost tile for closest
                                        if (ghostTile != null) ghostTile = ghostTiles[a];

                                        break;

                                    // POWER PRIORITY
                                    case BuildPriority.power:

                                        // See if building produces power
                                        foreach (Building.Resources resource in ghostTiles[a].building.resources)
                                        {
                                            if (resource.resource == Resource.CurrencyType.Power && !resource.add)
                                            {
                                                drone = FindClosestDrone(ghostTiles[a].transform.position);
                                                if (drone != null) SetBuilderTarget(drone, ghostTiles[a]);
                                                return;
                                            }
                                        }

                                        // If no building found, set ghost tile for closest
                                        if (ghostTile != null) ghostTile = ghostTiles[a];

                                        break;

                                    // COOLING PRIORITY
                                    case BuildPriority.cooling:

                                        // See if building produces power
                                        foreach (Building.Resources resource in ghostTiles[a].building.resources)
                                        {
                                            if (resource.resource == Resource.CurrencyType.Heat && !resource.add)
                                            {
                                                drone = FindClosestDrone(ghostTiles[a].transform.position);
                                                if (drone != null) SetBuilderTarget(drone, ghostTiles[a]);
                                                return;
                                            }
                                        }

                                        // If no building found, set ghost tile for closest
                                        if (ghostTile != null) ghostTile = ghostTiles[a];

                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        ghostTiles.RemoveAt(a);
                        a--;
                    }
                }

                // After loop, check if index was assigned
                if (ghostTile != null)
                {
                    drone = FindClosestDrone(ghostTile.transform.position);
                    if (drone != null) SetBuilderTarget(drone, ghostTile);

                    if (buildPriority != BuildPriority.cheapest ||
                        buildPriority != BuildPriority.expensive) ignorePriority = true;
                }
            }
        }
    }
    
    // Find closest drone port
    public Drone FindClosestDrone(Vector2 position)
    {
        // Find closest
        Drone drone = null;
        float closest = Mathf.Infinity;

        // Loop through all drones and use closest one
        for (int i = 0; i < builderDrones.Count; i++)
        {
            float distance = Vector2.Distance(builderDrones[i].transform.position, position);
            if (distance < closest)
            {
                drone = builderDrones[i];
                closest = distance;
            }
        }

        // Return drone
        return drone;
    }

    // Sets a target
    public void SetBuilderTarget(Drone drone, GhostTile ghostTile)
    {
        // Set target
        drone.SetTarget(ghostTile);
        drone.ExitPort();

        // Take resources
        Resource.active.ApplyResources(ghostTile.building);

        // Update lists
        activeDrones.Add(drone);
        builderDrones.Remove(drone);
        ghostTiles.Remove(ghostTile);
    }

    // Return a drone
    public Drone GetDrone(Drone.DroneType type)
    {
        foreach (Drone drone in droneTypes)
            if (drone.type == type)
                return drone;
        return null;
    }

    // Attempts to set targets for all nearby ports
    public void UpdateNearbyPorts(BaseTile tile, Vector2 position)
    {
        // Loop through all nearby drone ports
        int adjustment = Research.drone_tile_coverage * 5;
        int xTile = (int)position.x;
        int yTile = (int)position.y;

        // Loop through all tiles and try to find drones
        for (int x = xTile - adjustment; x <= xTile + adjustment; x += 5)
        {
            for (int y = yTile - adjustment; y <= yTile + adjustment; y += 5)
            {
                BaseTile holder = InstantiationHandler.active.TryGetBuilding(new Vector2(x, y));
                if (holder != null)
                {
                    Droneport droneport = holder.GetComponent<Droneport>();
                    if (droneport != null) droneport.AddTarget(tile);
                }
            }
        }
    }
}
