using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MazeRenderer : MonoBehaviour
{
    [SerializeField][Range(1, 100)] private int row = 10;
    public int Row => row;
    [SerializeField][Range(1, 100)] private int column = 10;
    public int Column => column;
    [SerializeField] private Vector3 cellSize = new Vector3(2.5f, 1.0f, 0.1f);
    [SerializeField] private Transform wallPrefab = null;
    [SerializeField] private Transform floorPrefab = null;

    public void Draw(Cell[,] maze)
    {
        var wallParent = new GameObject("Walls").transform;
        wallParent.parent = transform;

        var floorParent = new GameObject("Floor").transform;
        floorParent.parent = transform;

        var wallPool = new ObjectPool(row * column, wallPrefab, cellSize, wallParent);
        var wallBoundX = wallPool.bounds.x;

        var floorPool = new ObjectPool(row * column, floorPrefab, new Vector3(cellSize.x, cellSize.x, cellSize.x) * 1.02f, floorParent);

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                var cell = maze[i, j];

                var pos = (new Vector3(j, 0, i) * wallBoundX) + GetOffset(wallBoundX);

                floorPool.GetObj(pos, Quaternion.identity);

                if (cell.up)
                {
                    wallPool.GetObj(pos + new Vector3(0, 0, wallBoundX / 2), Quaternion.identity);
                }
                if (cell.left)
                {
                    wallPool.GetObj(pos + new Vector3(-wallBoundX / 2, 0, 0), Quaternion.Euler(0, 90, 0));
                }
                if (i == 0)
                {
                    if (cell.down)
                    {
                        wallPool.GetObj(pos + new Vector3(0, 0, -wallBoundX / 2), Quaternion.Euler(0, 0, 0));
                    }
                }
                if (j == column - 1)
                {
                    if (cell.right)
                    {
                        wallPool.GetObj(pos + new Vector3(wallBoundX / 2, 0, 0), Quaternion.Euler(0, 90, 0));
                    }
                }
            }
        }
    }

    public Vector3 GetOffset(float prefabwidth)
    {
        Vector3 mazeSize = new Vector3(column - 1, 0, row - 1) * prefabwidth;

        Vector3 offset = new Vector3(-mazeSize.x / 2, 0, -mazeSize.z / 2);

        return offset;
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(MazeRenderer))]

public class MazeRendererEditor : Editor
{
    private int buttonCount = -1;
    private Cell[,] maze;
    private bool isGenerated = false;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var renderer = target as MazeRenderer;

        maze = MazeGenerator.GenerateCells(renderer.Row, renderer.Column);

        if (GUI.changed)
        {
            ReloadGenerator(renderer);
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate"))
        {
            buttonCount++;

            if (buttonCount >= 0)
            {
                ClearGenerator(renderer);

                var newMaze = MazeGenerator.GenerateCells(renderer.Row, renderer.Column);

                renderer.Draw(newMaze);

            }
        }
        if (GUILayout.Button("Clear"))
        {
            ClearGenerator(renderer);

            buttonCount = 0;
        }

        EditorGUILayout.EndHorizontal();

    }

    private void ReloadGenerator(MazeRenderer renderer)
    {
        ClearGenerator(renderer);

        renderer.Draw(maze);

    }
    private void ClearGenerator(MazeRenderer component)
    {
        for (int i = component.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(component.transform.GetChild(i).gameObject);
        }
    }
}

#endif
