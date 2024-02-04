using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;


public enum Direction
{
    None, Up, Down, left, Right
}
public abstract class Cell
{
    protected int row;
    protected int col;
    public bool up { get; protected set; } = true;
    public bool down { get; protected set; } = true;
    public bool left { get; protected set; } = true;
    public bool right { get; protected set; } = true;
    public bool exit { get; protected set; } = false;
    protected bool visited = false;
    protected Direction direction = Direction.None;

    protected Cell(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
}

public class MazeGenerator : Cell
{
    public MazeGenerator(int row, int col) : base(row, col) { }
    public static Vector3 startingPosition { get; private set; }

    public static MazeGenerator[,] GenerateCells(int row, int col)
    {
        MazeGenerator[,] cells = new MazeGenerator[row, col];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                cells[i, j] = new MazeGenerator(i, j);
            }
        }

        return RecursiveBackTracker(cells);
    }

    private static MazeGenerator GetInitialCell(MazeGenerator[,] cells)
    {
        int col = cells.GetLength(1);

        int randCol = Random.Range(0, col);

        MazeGenerator cell = cells[0, randCol];

        cell.visited = true;

        startingPosition = new Vector3(randCol, 0, 0);

        return cell;
    }

    private static MazeGenerator[,] RecursiveBackTracker(MazeGenerator[,] cells)
    {
        Stack<MazeGenerator> visitedCells = new();

        List<MazeGenerator> cellList = new();

        MazeGenerator initialCell = GetInitialCell(cells);

        visitedCells.Push(initialCell);

        while (visitedCells.Count > 0)
        {
            MazeGenerator currentCell = visitedCells.Pop();

            if (!cellList.Contains(currentCell)) cellList.Add(currentCell);

            var unvisitedNeighbours = UnvisitedNeighbours(cells, currentCell);

            if (unvisitedNeighbours.Count > 0)
            {
                visitedCells.Push(currentCell);

                int randNeighbour = Random.Range(0, unvisitedNeighbours.Count);

                MazeGenerator chosenNeighbour = unvisitedNeighbours[randNeighbour];

                RemoveWall(currentCell, chosenNeighbour);

                chosenNeighbour.visited = true;

                visitedCells.Push(chosenNeighbour);
            }
        }
        
        return cells;
    }

    private static List<MazeGenerator> GetNeighbours(MazeGenerator[,] cells, MazeGenerator currentcell)
    {
        List<MazeGenerator> neighbours = new();

        int row = currentcell.row;
        int col = currentcell.col;

        MazeGenerator neigh = null;

        // Check and add valid neighbors
        if (row - 1 >= 0)
        {
            neigh = cells[row - 1, col];
            neigh.direction = Direction.Down;

            neighbours.Add(neigh);
        }

        if (row + 1 < cells.GetLength(0))
        {
            neigh = cells[row + 1, col];
            neigh.direction = Direction.Up;

            neighbours.Add(neigh);
        }

        if (col - 1 >= 0)
        {
            neigh = cells[row, col - 1];
            neigh.direction = Direction.left;

            neighbours.Add(neigh);

        }

        if (col + 1 < cells.GetLength(1))
        {
            neigh = cells[row, col + 1];
            neigh.direction = Direction.Right;

            neighbours.Add(neigh);
        }

        return neighbours;
    }

    private static List<MazeGenerator> UnvisitedNeighbours(MazeGenerator[,] cells, MazeGenerator currentcell)
    {
        List<MazeGenerator> neighbours = GetNeighbours(cells, currentcell);
        List<MazeGenerator> unvisitedNeighbours = new();

        foreach (MazeGenerator neighbour in neighbours)
        {
            if (neighbour.visited == false)
            {
                unvisitedNeighbours.Add(neighbour);
            }
        }

        return unvisitedNeighbours;

    }

    private static void RemoveWall(MazeGenerator currentCell, MazeGenerator currentNeigh)
    {
        if (currentNeigh.direction == Direction.Up)
        {
            currentNeigh.down = false;
            currentCell.up = false;
        }

        else if (currentNeigh.direction == Direction.Down)
        {
            currentNeigh.up = false;
            currentCell.down = false;
        }

        else if (currentNeigh.direction == Direction.left)
        {
            currentNeigh.right = false;
            currentCell.left = false;
        }
        else if (currentNeigh.direction == Direction.Right)
        {
            currentNeigh.left = false;
            currentCell.right = false;
        }

    }
}
