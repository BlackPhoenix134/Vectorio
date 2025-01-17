using UnityEngine;
//using Mirror;
using System.Collections.Generic;

// This script is ported from Automa.
// https://github.com/Vitzual/Automa

public class InstantiationHandler : MonoBehaviour
{
    // Grid variable
    [HideInInspector] public Grid tileGrid;

    // Building variables
    public static InstantiationHandler active;
    public GhostTile ghostTile;
    public LayerMask enemyLayer;
    public int metadata = -1;

    // Debug variables
    public GameObject debugCircle;
    public List<GameObject> activeCircles;

    public void Awake()
    {
        // Get reference to active instance
        active = this;

        // Sets static variables on start
        tileGrid = new Grid();
        tileGrid.cells = new Dictionary<Vector2Int, Cell>();
    }

    // Creates an entity
    public void CreateEnemy(Entity entity, Vector2 position, Quaternion rotation)
    {
        // Check if entity is null
        if (entity == null) return;

        // Check if the entity is an enemy
        RpcInstantiateEnemy(entity, position, rotation);
    }

    // Creates a building
    public void CreateBuilding(Building building, Vector2 position, Quaternion rotation, bool isGhost = true)
    {
        // Check if entity is null
        if (building == null) return;

        // Check if resource should be used
        if (!Resource.active.CheckResources(building)) return;

        // Check to make sure the tiles are not being used
        if (!CheckTiles(building, position)) return;

        // Instantiate the object like usual
        if (Gamemode.active.useDroneConstruction && isGhost) RpcInstatiateGhost(building, position, rotation);
        else RpcInstantiateBuilding(building, position, rotation);
    }

    //[ClientRpc]
    private void RpcInstantiateEnemy(Entity entity, Vector2 position, Quaternion rotation)
    {
        // Use enemy handler thing
        RaycastHit2D[] hits = Physics2D.RaycastAll(position, Vector2.zero, Mathf.Infinity, enemyLayer);
        foreach (RaycastHit2D hit in hits)
            if (hit.collider != null) return;
        EnemyHandler.active.CreateEntity(entity, position, rotation);
    }

    //[ClientRpc]
    private void RpcInstantiateBuilding(Building building, Vector2 position, Quaternion rotation)
    {
        // Get game objected from scriptable manager
        GameObject obj = ScriptableManager.RequestObjectByName(building.name);
        if (obj == null) return;

        // Create the tile
        BaseTile lastBuilding = Instantiate(obj, position, rotation).GetComponent<BaseTile>();
        lastBuilding.name = building.name;

        // Set the tiles on the grid class
        SetCells(building, position, lastBuilding);

        // Update resource values promptly
        if (!Gamemode.active.useDroneConstruction)
            Resource.active.ApplyResources(building);

        // Call buildings setup method and metadata method if metadata is applied
        if (metadata != -1) lastBuilding.ApplyMetadata(metadata);
        lastBuilding.Setup();
    }

    //[ClientRpc]
    private void RpcInstatiateGhost(Building building, Vector2 position, Quaternion rotation)
    {
        // Create the tile
        GhostTile holder = Instantiate(ghostTile, position, rotation).GetComponent<GhostTile>();
        holder.name = building.name;

        // Setup the ghost tile
        holder.SetBuilding(building);
        DroneManager.active.AddGhost(holder);

        // Set the tiles on the grid class
        SetCells(building, position, holder);
    }

    public void SetCells(Building building, Vector2 position, BaseTile obj)
    {
        // Set the tiles on the grid class
        if (building.cells.Length > 0)
        {
            foreach (Building.Cell cell in building.cells)
                tileGrid.SetCell(Vector2Int.RoundToInt(new Vector2(position.x + cell.x, position.y + cell.y)), true, building, obj);
        }
    }

    // Destroys a buildingg
    //[ClientRpc]
    public void RpcDestroyBuilding(Vector3 position)
    {
        tileGrid.DestroyCell(GetCellCoords(position));
    }

    // Checks to make sure tile(s) isn't occupied
    //[Server]
    public bool CheckTiles(Building building, Vector3 position)
    {
        if (building.cells.Length > 0)
        {
            foreach (Building.Cell cell in building.cells)
                if (!CheckTile(Vector2Int.RoundToInt(new Vector2(position.x + cell.x, position.y + cell.y)), building)) return false;
        }
        else if (!CheckTile(Vector2Int.RoundToInt(new Vector2(position.x, position.y)), building)) return false;
        return true;
    }

    // Check a specific tile
    public bool CheckTile(Vector2Int coords, Building building)
    {
        // Check if tile is in bounds
        if (coords.y > Border.north || coords.y < Border.south ||
            coords.x > Border.east || coords.x < Border.west)
            return false;

        // Check if tile already occupied
        if (tileGrid.RetrieveCell(coords) != null)
            return false;

        // Check if tile is restricted to a specific node
        if (building.restrictPlacement && !WorldGenerator.active.CheckNode(coords, building.placedOn))
            return false;

        // If all checks passed, return true
        return true;
    }

    // Returns closest building to position given
    public BaseTile GetClosestBuilding(Vector2Int position)
    {
        BaseTile nearest = null;
        float distance = float.PositiveInfinity;

        foreach (KeyValuePair<Vector2Int, Cell> cell in tileGrid.cells)
        {
            float holder = Vector2Int.Distance(position, cell.Key);
            if (holder < distance)
            {
                distance = holder;
                nearest = cell.Value.obj;
            }
        }

        return nearest;
    }

    // Attempts to return a building
    public BaseTile TryGetBuilding(Vector2 position)
    {
        Cell cell = tileGrid.RetrieveCell(GetCellCoords(position));
        if (cell != null)
        {
            BaseTile building = cell.obj.GetComponent<BaseTile>();
            return building;
        }
        return null;
    }

    // Get cell coords function
    public Vector2Int GetCellCoords(Vector2 position)
    {
        // Create adjustment variables
        float xAdjustment = 2.5f;
        float yAdjustment = 2.5f;

        // Calculate adjustment amount
        if (position.x >= 0) xAdjustment = -xAdjustment;
        if (position.y >= 0) yAdjustment = -yAdjustment;

        // Get cell coordinate
        position = new Vector2(position.x - xAdjustment, position.y - yAdjustment);
        Vector2Int cellCoords = new Vector2Int((int)position.x / 5 * 5, (int)position.y / 5 *5);
        return cellCoords;
    }

    // Spawn a debug circle
    public void SpawnDebugCircle(Vector2 position)
    {
        activeCircles.Add(Instantiate(debugCircle, position, Quaternion.identity));
    }

    // Remove all debug circles
    public void RemoveDebugCircles()
    {
        foreach (GameObject circle in activeCircles)
            Recycler.AddRecyclable(circle.transform);
        activeCircles = new List<GameObject>();
    }
}
