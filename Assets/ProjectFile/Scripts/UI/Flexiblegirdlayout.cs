using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class FlexibleGridLayout : MonoBehaviour
{
    public enum FitType { Uniform, FixedColumns, FixedRows }

    [Min(1)] public int columns = 4;
    [Min(1)] public int rows = 1;
    public Vector2 spacing = Vector2.zero;

    public bool keepAspect = false;
    public float aspectRatio = 1f; // width / height
    public bool basedOnWidth = true; // true면 width 기준, false면 height 기준

    private GridLayoutGroup grid;
    private RectTransform rt;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        UpdateGrid(); // Start에서 확실하게 RectTransform 크기 확보 후 실행
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Init();
        UpdateGrid();
    }
#endif

    private void Init()
    {
        if (grid == null)
            grid = GetComponent<GridLayoutGroup>();
        if (rt == null)
            rt = GetComponent<RectTransform>();
    }

    private void UpdateGrid()
    {
        if (grid == null || rt == null) return;

        float totalWidth = rt.rect.width;
        float totalHeight = rt.rect.height;

        float cellWidth, cellHeight;

        if (keepAspect)
        {
            if (basedOnWidth)
            {
                cellWidth = (totalWidth - spacing.x * (columns - 1) - grid.padding.left - grid.padding.right) / columns;
                cellHeight = cellWidth / aspectRatio;
            }
            else
            {
                cellHeight = (totalHeight - spacing.y * (rows - 1) - grid.padding.top - grid.padding.bottom) / rows;
                cellWidth = cellHeight * aspectRatio;
            }
        }
        else
        {
            cellWidth = (totalWidth - spacing.x * (columns - 1) - grid.padding.left - grid.padding.right) / columns;
            cellHeight = (totalHeight - spacing.y * (rows - 1) - grid.padding.top - grid.padding.bottom) / rows;
        }

        grid.cellSize = new Vector2(cellWidth, cellHeight);
        grid.spacing = spacing;
    }

    private void OnRectTransformDimensionsChange()
    {
        // 해상도 변경 or 부모 변경 시에도 대응
        UpdateGrid();
    }
}
