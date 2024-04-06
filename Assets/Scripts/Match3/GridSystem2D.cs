using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Match3
{
    public class GridSystem2D<T>
    {
        private int width;
        private int height;
        private float cellSize;
        private Vector3 originPosition;
        private T[,] grid;

        CoordinateConverter coordinateConverter;

        // Event to notify when a grid value has changed.
        public event Action<int, int, T> OnGridValueChanged;

        // Factory method for having a Vertical Grid
        public static GridSystem2D<T> VerticalGrid(int width, int height, float cellSize, Vector3 originPosition,
            bool debug = false)
        {
            return new GridSystem2D<T>(width, height, cellSize, originPosition, new VerticalConverter(), debug);
        }

        // Factory method for having a Horizontal Grid
        public static GridSystem2D<T> HorizontalGrid(int width, int height, float cellSize, Vector3 originPosition,
            bool debug = false)
        {
            return new GridSystem2D<T>(width, height, cellSize, originPosition, new HorizontalConverter(), debug);
        }

        // Constructor
        public GridSystem2D(int width, int height, float cellSize, Vector3 originPosition,
            CoordinateConverter coordinateConverter, bool debug = false)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;
            // If no coordinate converter is provided, use the default one.
            this.coordinateConverter = coordinateConverter ?? new VerticalConverter();

            grid = new T[width, height];

            // Draw debug lines if debug is enabled.
            if (debug)
            {
                DrawDebugLines();
            }
        }

        // Set grid value given a world position
        public void SetValue(Vector3 worldPosition, T value)
        {
            Vector2Int gridPosition = GetGridPosition(worldPosition);
            SetValue(gridPosition.x, gridPosition.y, value);
        }

        // Set grid value given grid position
        public void SetValue(int x, int y, T value)
        {
            if (IsValidPosition(x, y))
            {
                grid[x, y] = value;
                OnGridValueChanged?.Invoke(x, y, value);
            }
        }

        // Get grid value given a world position
        public T GetValue(Vector3 worldPosition)
        {
            Vector2Int gridPosition = GetGridPosition(worldPosition);
            return GetValue(gridPosition.x, gridPosition.y);
        }

        // Get grid value given grid position
        public T GetValue(int x, int y)
        {
            return IsValidPosition(x, y) ? grid[x, y] : default(T);
        }

        // Helper function to get the world position of a cell.
        bool IsValidPosition(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;

        Vector2Int GetGridPosition(Vector3 worldPosition) =>
            coordinateConverter.WorldToGrid(worldPosition, cellSize, originPosition);

        Vector3 GetWorldPositionCenter(int x, int y) =>
            coordinateConverter.GridToWorldCenter(x, y, cellSize, originPosition);

        Vector3 GetWorldPosition(int x, int y) => coordinateConverter.GridToWorld(x, y, cellSize, originPosition);

        // TODO: All this debugging code could probably be extracted and moved to the Debugging object.
        private void DrawDebugLines()
        {
            const float duration = 100f;
            // TODO: Why not using an object that comes from the editor?
            GameObject parent = new GameObject("Debugging");

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CreateDebugText(parent, x + ", " + y, GetWorldPositionCenter(x, y), coordinateConverter.Forward);
                    // Draw an horizontal line from y to y+1
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, duration);
                    // Draw a vertical line from x to x+1
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, duration);
                }
            }

            // Draw the closing horizontal line
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, duration);
            // Draw the closing vertical line
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, duration);
        }

        /// <summary>
        /// Creates a debug text object and attach it to the parent GameObject.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="text"></param>
        /// <param name="localPosition"></param>
        /// <param name="direction"></param>
        /// <param name="fontSize"></param>
        /// <param name="color"></param>
        /// <param name="textAlignment"></param>
        /// <param name="sortingOrder"></param>
        /// <returns></returns>
        TextMeshPro CreateDebugText(GameObject parent, string text, Vector3 localPosition, Vector3 direction,
            int fontSize = 2, Color color = default, TextAlignmentOptions textAlignment = TextAlignmentOptions.Center,
            int sortingOrder = 0)
        {
            GameObject gameObject = new GameObject("DebugText_" + text, typeof(TextMeshPro));
            Transform transform = gameObject.transform;
            transform.SetParent(parent.transform);
            transform.localPosition = localPosition;
            transform.forward = direction;

            TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color == default ? Color.white : color;
            textMesh.alignment = textAlignment;
            // TODO: Which one is better?
            textMesh.sortingOrder = sortingOrder;
            //textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMesh;
        }


        // TODO: This could probably be extracted to a separate file.
        public abstract class CoordinateConverter
        {
            public abstract Vector3 GridToWorld(int x, int y, float cellSize, Vector3 originPosition);
            public abstract Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 originPosition);
            public abstract Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 originPosition);
            public abstract Vector3 Forward { get; }
        }

        /// <summary>
        /// Vertical converter, a vertical grid stands up in front of the camera at z=0.
        /// </summary>
        public class VerticalConverter : CoordinateConverter
        {
            public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 originPosition)
            {
                return new Vector3(x, y, 0) * cellSize + originPosition;
            }

            public override Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 originPosition)
            {
                return new Vector3(x + cellSize / 2, y + cellSize / 2, 0) * cellSize + originPosition;
            }

            public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 originPosition)
            {
                var x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
                var y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
                return new Vector2Int(x, y);
            }

            public override Vector3 Forward => Vector3.forward;
        }

        /// <summary>
        /// Horizontal converter, an horizontal grid is flattened on the "ground" at y=0.
        /// </summary>
        public class HorizontalConverter : CoordinateConverter
        {
            public override Vector3 GridToWorld(int x, int z, float cellSize, Vector3 originPosition)
            {
                return new Vector3(x, 0, z) * cellSize + originPosition;
            }

            public override Vector3 GridToWorldCenter(int x, int z, float cellSize, Vector3 originPosition)
            {
                return new Vector3(x + cellSize / 2, 0, z + cellSize / 2) * cellSize + originPosition;
            }

            public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 originPosition)
            {
                var x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
                var y = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
                return new Vector2Int(x, y);
            }

            public override Vector3 Forward => Vector3.right;
        }
    }
}
