using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    // Holds a dictionary of all cells
    // Int represents coords of the tile 
    public Dictionary<Vector2Int, Cell> cells;

    // Grid values
    public int gridSize;
    public int cellSize;

    public BaseTile RetrieveTile(Vector2Int coords)
    {
        if (cells.TryGetValue(coords, out Cell cell))
        {
            return cell.obj;
        }
        else return null;
    }

    public Cell RetrieveCell(Vector2Int coords)
    {
        if (cells.TryGetValue(coords, out Cell cell))
        {
            return cell;
        }
        else return null;
    }

    public void SetCell(Vector2Int coords, bool occupy, Entity entity = null, BaseTile obj = null)
    {
        if (cells.TryGetValue(coords, out Cell cell))
        {
            cell.occupied = true;
            cell.entity = entity;
            cell.obj = obj;
        }
        else cells.Add(coords, new Cell(occupy, entity, obj));
        if (obj != null) obj.cells.Add(coords);
    }

    public void RemoveCell(Vector2Int coords)
    {
        if (cells.ContainsKey(coords)) cells.Remove(coords);
    }

    public void DestroyCell(Vector2Int coords)
    {
        if (cells.TryGetValue(coords, out Cell cell))
        {
            BaseTile building = cell.obj.GetComponent<BaseTile>();
            if (building != null)
                building.DestroyEntity();
        }
    }
}