using UnityEngine;

namespace Match3
{
    public class Match3 : MonoBehaviour
    {
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 8;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 originPosition = new(4, 4);
        [SerializeField] private bool debug = true;

        private GridSystem2D<GridObject<Gem>> gridSystem2D;

        private void Start()
        {
            gridSystem2D = GridSystem2D<GridObject<Gem>>.VerticalGrid(width, height, cellSize, originPosition, debug);
        }
    }
}
