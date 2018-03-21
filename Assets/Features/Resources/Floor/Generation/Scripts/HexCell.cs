using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour{

    //Coordinate storage
    [SerializeField]
    public HexCoordinates coordinates;

    //type

    public int type;
    public FEATURE feature;

    public float waterDensity;
    public float woodDensity;
    public float stoneDensity;

    public bool walkable;

    public int height;

    public MeshFilter meshFilter;

    private MeshCollider meshCollider;

    public Mesh mesh;

    private List<int> triangles;
    private List<Vector3> vertices;
    private List<Color> colors;

    public List<House_Blueprint> blueprints;
    public List<Structure> structures;

    public void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    //quicker access to coordinates

    public int X
    {
        get
        {
            return coordinates.x;
        }
    }

    public int Y
    {
        get
        {
            return coordinates.y;
        }
    }

    public int Z
    {
        get
        {
            return coordinates.z;
        }
    }

    public Color getColor()
    {
        switch (type)
        {
            case 0:
                return Color.blue;
            case 1:
                return Color.green;
            case 2:
                return Color.yellow;
            case 3:
                return Color.white;
            case 4:
                return Color.grey;
            default:
                return Color.black;
        }
    }

    public void renderMesh()
    {
        mesh = new Mesh();

        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();

        Vector3 center = new Vector3(0, 0, 0);

        for (int i = 0; i < 6; i++)
        {
            createTriangle(
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[i + 1],
                getColor()
            );
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    void createTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color)
    {
        vertices.Add(p1);
        vertices.Add(p2);
        vertices.Add(p3);

        triangles.Add(triangles.Count);
        triangles.Add(triangles.Count);
        triangles.Add(triangles.Count);

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
}
