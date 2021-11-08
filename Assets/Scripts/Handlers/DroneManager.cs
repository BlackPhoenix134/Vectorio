﻿// This script handles all active drones each frame
using System.Collections.Generic;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    // Get active instance
    public static DroneManager active;
    public void Awake() { active = this; }

    // Target lists
    public List<GhostTile> ghostTiles;
    public List<ResourceTile> resourceTiles;
    public List<BaseTile> damagedTiles;

    // Available drones 
    public List<Drone> hubDrones;
    public List<Drone> builderDrones;
    public List<Drone> resourceDrones;
    public List<Drone> fixerDrones;

    // Drones actively moving
    public List<Drone> activeDrones;

    // Add a drone
    public void AddDrone(Drone drone)
    {
        if (drone.type == Drone.DroneType.Builder)
        {
            if (drone.home.hubDrone) hubDrones.Add(drone);
            else builderDrones.Add(drone);
        }
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
        bool hubDronesAvailable = hubDrones.Count > 0;

        if (ghostTiles.Count > 0 && (builderDrones.Count > 0 || hubDronesAvailable))
        {
            for (int a = 0; a < ghostTiles.Count; a++)
            {
                if (InstantiationHandler.active.CheckResources(ghostTiles[a].building))
                {
                    // Check all active builder ports
                    for (int b = 0; b < builderDrones.Count; b++)
                    {
                        if (builderDrones[b].nearbyTargets.Contains(ghostTiles[a]))
                        {
                            // Drone assigned
                            Debug.Log("Drone assigned");

                            // Set target
                            builderDrones[b].SetTarget(ghostTiles[a]);
                            builderDrones[b].ExitPort();

                            // Take resources
                            Resource.active.ApplyResources(ghostTiles[a].building);

                            // Update lsits
                            activeDrones.Add(builderDrones[b]);
                            ghostTiles.RemoveAt(a);
                            builderDrones.RemoveAt(b);

                            // End loop
                            return;
                        }
                    }

                    // If no builder ports available, default to hub ports
                    if (hubDronesAvailable)
                    {
                        // Set target
                        hubDrones[0].SetTarget(ghostTiles[a]);
                        hubDrones[0].ExitPort();

                        // Take resources
                        Resource.active.ApplyResources(ghostTiles[a].building);

                        // Update lsits
                        activeDrones.Add(hubDrones[0]);
                        ghostTiles.RemoveAt(a);
                        hubDrones.RemoveAt(0);

                        // End loop
                        return;
                    }
                }
            }
        }
    }
}
