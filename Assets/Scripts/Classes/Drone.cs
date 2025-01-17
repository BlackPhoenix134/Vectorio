using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    // Drone type
    public enum DroneType
    {
        Builder,
        Resource,
        Fixer
    }
    public DroneType type;

    // Stage of the current drone
    public enum Stage
    {
        ReadyToDeploy,
        ExitingPort,
        MovingToTarget,
        ReturningToPort,
        EnteringPort
    }
    public Stage stage;

    // Drone variables
    [HideInInspector] public Droneport home;
    [HideInInspector] public BaseEntity target;
    [HideInInspector] public float droneSpeed;
    public SpriteRenderer droneIcon;

    // Nearby targets
    [HideInInspector] public List<BaseEntity> nearbyTargets;

    // NOTE ABOUT VIRTUAL METHODS
    //
    // For optimization purposes, even though the Drone class
    // controls a majority of the logic for drones, DroneManager.cs
    // is the true driver class. Some methods on specific drones
    // will NOT be called unless a condition is first passed on 
    // the drone manager. Please be weary of this if you're adding
    // new drones, and try to use the DroneManager singleton to
    // optimize logic as much as possible.

    // Adjust size
    public void Start()
    {
        transform.localScale = new Vector3(0.4f, 0.4f, 0f);
    }

    // Specifies if the drone should add a certain target
    public virtual void AddTarget(BaseTile tile) 
    { 
        nearbyTargets.Add(tile); 
    }

    // Specifies if the drone should do anything while setting target
    public virtual void SetTarget(BaseTile tile)
    {
        target = tile;
    }

    // Specifies what the drone should do when it is deployed
    public virtual void ExitPort()
    {
        stage = Stage.ExitingPort;
        RotateToTarget();
    }

    // Specifies what the drone should do as it exits the port
    public virtual void ExitingPort()
    {
        if (transform.localScale.x <= 0.7f)
            transform.localScale += new Vector3(0.002f, 0.002f, 0f);
        if (home.OpenDoors()) StartRoute();
    }

    // Specifies what the drone should do when it starts it's route
    public virtual void StartRoute()
    {
        droneSpeed = Research.drone_movement_speed;
        droneIcon.sortingOrder = 3;
        stage = Stage.MovingToTarget;
    }

    // Specifies how the target should move (dont override for straight line)
    public virtual void Move()
    {
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, droneSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, target.transform.position) < 0.1f) TargetReached();
        }
        else if (home != null) target = home;
        else Destroy();
    }

    // Specifies what the drone should do when it reaches it's target
    public virtual void TargetReached() 
    {
        if (stage == Stage.MovingToTarget)
        {
            stage = Stage.ReturningToPort;
            target = home;
            RotateToTarget();
        }
        else EnterPort();
    }

    // Specifies what the drone should do when it reaches home
    public virtual void EnterPort()
    {
        droneIcon.sortingOrder = 1;
        stage = Stage.EnteringPort;
    }

    // Specifies what the drone should do as it enters the port
    public virtual void EnteringPort()
    {
        if (transform.localScale.x >= 0.4f)
            transform.localScale -= new Vector3(0.002f, 0.002f, 0f);
        if (home.CloseDoors()) FinishRoute();
    }

    // Specifies what the drone should do when it finishes 
    public virtual void FinishRoute()
    {
        transform.localScale = new Vector3(0.4f, 0.4f, 0f);
        stage = Stage.ReadyToDeploy;
    }

    // Rotate towards target
    public void RotateToTarget()
    {
        Vector2 lookDirection = new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
        transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f);
    }

    // Destroy drone method
    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
    
