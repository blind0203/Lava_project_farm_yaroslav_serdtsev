using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FieldManager : Singleton<FieldManager> {
    [HideInInspector] public List<Cell> CellList { get; private set; }
    [HideInInspector] public Cell SelectedCell { get; private set; }
    [HideInInspector] public Vector2Int FieldSize => _cellCount;
    [SerializeField] private Vector2Int _cellCount;
    [SerializeField] private List<Vector3> _verts;
    [SerializeField] private List<int> _tris;
    [SerializeField, Range(0.0001f, 1f)] private float _noiseScale;
    [SerializeField] private Transform _underCube;
    [SerializeField] private Material _groundMaterial;
    private Texture2D _tileTexture;
    private Vector2 _noiseOffset;
    private void Start() {
        GenerateField();
    }

    public void ChangeSize(Vector2Int size) {
        _cellCount = size;
    }

    public void GenerateField() {
        _noiseOffset = new Vector2(Random.Range(0f, 10f), Random.Range(0f, 10f));
        _verts = new List<Vector3>();
        _tris = new List<int>();
        CellList = new List<Cell>();
        _cellCount += Vector2Int.one;

        _underCube.position = new Vector3((float)_cellCount.x / 2 - .5f, -2.501f, (float)_cellCount.y / 2 - .5f);
        _underCube.localScale = new Vector3(_cellCount.x - 1, 5, _cellCount.y - 1);

        for (int y = 0; y < _cellCount.y; y++) {
            for (int x = 0; x < _cellCount.x; x++) {
                float yMod = (x == 0 || y == 0 || x == _cellCount.x - 1 || y == _cellCount.y - 1) ? 0 : 1;
                _verts.Add(new Vector3(x , yMod * GetNoisyHeight(x, y), y));
                CellList.Add(new Cell(null, new Vector2Int(x, y), false, _cellCount.x * y + x));
            }
        }

        int i = 0;

        while (i < _verts.Count - _cellCount.x - 1) {
            _tris.Add(i);
            _tris.Add(i + _cellCount.x);
            _tris.Add(i + 1);

            _tris.Add(i + 1);
            _tris.Add(i + _cellCount.x);
            _tris.Add(i + _cellCount.x + 1);

            i++;

            if ((i + 1) % _cellCount.x == 0) {
                i++;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = _verts.ToArray();
        mesh.triangles = _tris.ToArray();
        mesh.Optimize();
        
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshCollider>();
        NavMeshSurface nms = GetComponent<NavMeshSurface>();
        nms.center = new Vector3(_cellCount.x / 2, .5f, _cellCount.y / 2);
        nms.size = new Vector3(_cellCount.x + 2, 3, _cellCount.y + 2);
        nms.BuildNavMesh();

        CreateDigTilesTexture();       
    }

    private void CreateDigTilesTexture() {
        _tileTexture = new Texture2D(_cellCount.x, _cellCount.y, TextureFormat.R8, false);
        _tileTexture.wrapMode = TextureWrapMode.Clamp;
        _tileTexture.filterMode = FilterMode.Point;
        for (int x = 0; x < _cellCount.x; x++) {
            for (int y = 0; y < _cellCount.y; y++) {
                _tileTexture.SetPixel(x, y, Color.clear);
            }
        }

        _tileTexture.Apply();

        _groundMaterial.SetTexture("_DigTexture", _tileTexture);
        _groundMaterial.SetVector("_FieldSize", new Vector4(_cellCount.x, _cellCount.y, 0, 0));
    }

    public void DigTexture(Cell cell) {
        _tileTexture.SetPixel(cell.CellCoords.x, cell.CellCoords.y, Color.white);
        _tileTexture.Apply();
    }

    public void UndigTexture(Cell cell) {
        _tileTexture.SetPixel(cell.CellCoords.x, cell.CellCoords.y, Color.grey);
        _tileTexture.Apply();
    }

    public float GetNoisyHeight(float x, float y) {
        return Mathf.PerlinNoise(x * _noiseScale + _noiseOffset.x, y * _noiseScale + _noiseOffset.y);
    }

    public void SelectCell(int x, int y) {
        SelectedCell = CellList[_cellCount.x * y + x];
    }

    public void DeselectCell() {
        SelectedCell = null;
    }

    public Vector3 GetCellCenter(Cell cell) {
        Vector3[] verts = new Vector3[4] {
            _verts[cell.BaseVertexID],
            _verts[cell.BaseVertexID + 1],
            _verts[cell.BaseVertexID + _cellCount.x],
            _verts[cell.BaseVertexID + _cellCount.x + 1],
        };

        return (verts[0] + verts[1] + verts[2] + verts[3]) / 4;
    }
}

public class Cell {
    public PlantComponent PlantInCell;

    public Vector2Int CellCoords;

    public bool IsBusy = false;

    public int BaseVertexID = 0;

    public Cell(PlantComponent plantInCell, Vector2Int cellCoords, bool isBusy, int baseVertexId) {
        PlantInCell = plantInCell;
        CellCoords = cellCoords;
        IsBusy = isBusy;
        BaseVertexID = baseVertexId;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FieldManager))]
public class FieldManagerInspector : Editor {
    override public void OnInspectorGUI() {
        FieldManager colliderCreator = (FieldManager)target;

        if (GUILayout.Button("Generate Field")) {
            colliderCreator.GenerateField();
        }

        DrawDefaultInspector();
    }
}
#endif
