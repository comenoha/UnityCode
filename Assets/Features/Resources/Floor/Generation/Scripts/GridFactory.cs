using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridFactory : MonoBehaviour
{

    public int width = 10;
    public int height = 10;

    int x_length;
    int y_length;

    public static HexCell[] cells;

    public HexCell cellPrefab;

    public GameObject treePrefab;
    public GameObject cactusPrefab;

    Mesh mesh;

    MeshRenderer meshRenderer;

    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;

    //Initializer

    void Start()
    {
        cells = new HexCell[width * height];

        //x_length = (width / 2);
        //y_length = - (int) (((width * 2) - 1) * (.75f));

        prepareMesh();
        createGrid();

        generateWorld(WORLD.island);

        //refreshGrid();

        renderMesh();
    }

    /////////////////////////
    //World Type Generation//
    /////////////////////////

    //world generation controller

    void generateWorld(int worldType)
    {
        switch (worldType)
        {
            case 0:
                generateIsland();
                break;
        }
    }

    //Island generator

    void generateIsland()
    {
        HexCell middleOfIsland = cells[((width * height / 2) + (width / 2))];

        generateBiome(middleOfIsland, BIOME.forest, SIZE.xxLarge);
        generateBiome(middleOfIsland, BIOME.taiga, SIZE.large);
        generateBiome(middleOfIsland, BIOME.desert, SIZE.small);

        generateFeatures();
    }

    /////////////////////////////
    //HexGrid Creator Functions//
    /////////////////////////////

    //loops through number of cells and creates each cell

    void createGrid()
    {
        for (int x = 0, i = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //int rand = Random.Range(0, 5);
				createCell(x, z, 0, i++, BIOME.ocean);
            }
        }
    }

    //refreshes grid

    void refreshGrid()
    {
        for (int x = 0, i = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                createCell(x, z, cells[i].height, i, cells[i].type);
                i += 1;
            }
        }
    }

    //creates cell from coordinates; also sets HexCoordinates

    void createCell(int x, int z, int height, int i, int type)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerRadius * 2;
        position.y = height;
        position.z = z * HexMetrics.outerRadius * 1.5f;
        
        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.transform.SetParent(this.gameObject.transform);
        cell.gameObject.tag = "HexCell";
        cell.coordinates = HexCoordinates.formOffsetCoordinates(x, z);
        cell.type = type;
        cell.walkable = false;
        cell.feature = FEATURE.NONE;

        //Debug.Log("The real coordinates are: (" + x + ", " + z + "), but the offset coordinates are (" + cell.coordinates.x + ", " + cell.coordinates.y + ", " + cell.coordinates.z + ")");
    }

    //Get cell from grid using X and Y coordinates

    public static HexCell getCell(int x, int y)
    {
        for(int i = 0; i < cells.Length; i++)
        {
            if(cells[i].coordinates.x == x && cells[i].coordinates.y == y)
            {
                return cells[i];
            }
        }
        return null;
    }

    public static HexCell getRandomWalkableCell()
    {
        HexCell tempCell = cells[Random.Range(0, cells.Length)];

        if (tempCell.walkable) return tempCell;
        else return getRandomWalkableCell();
    }

    //////////////////////////
    //Mesh Creator Functions//
    //////////////////////////

    //initial preperation for Mesh

    void prepareMesh()
    {
        mesh = new Mesh();

        if(gameObject.GetComponent<MeshRenderer>() == null) gameObject.AddComponent<MeshRenderer>();
        if(gameObject.GetComponent<MeshFilter>() == null) gameObject.AddComponent<MeshFilter>();

        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }

    //clears all mesh properties

    void clearMesh()
    {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
    }

    //renders mesh

    void renderMesh()
    {
        clearMesh();

        for(int i = 0; i < cells.Length; i++)
        {
            createMesh(cells[i]);
        }

        //mesh.vertices = vertices.ToArray();
        //mesh.triangles = triangles.ToArray();
        //mesh.colors = colors.ToArray();

        //meshFilter.mesh = mesh;
    }

    //creates mesh for a single cell

    void createMesh(HexCell cell)
    {
        /*Vector3 center = cell.transform.localPosition;
        for (int i = 0; i < 6; i++)
        {
            createTriangle(
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[i + 1],
                cell.getColor()
            );
        }*/

        cell.renderMesh();
    }

    //creates triangle using HexMetrics points and a cell

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

    ////////////////////////////
    //World Creation Functions//
    ////////////////////////////

    //create a biome of specified type

	void generateBiome(HexCell centerCell, int type, float size)
    {
        centerCell.type = type;
		expandFromCell(centerCell, 0, size);
    }

    //recursive expansion from center point using size of biome

	void expandFromCell(HexCell centerCell, int n, float size)
    {
        HexCell[] adjCells = getAdjacent(centerCell);

        foreach(HexCell cell in adjCells)
        {
            if (cell != null && cell.type != centerCell.type)
            {
                cell.type = centerCell.type;
                if(cell.type != BIOME.ocean)
                {
                    cell.walkable = true;
                }
                if ((n < 10 && size == SIZE.small) || (n < 20 && size == SIZE.medium) || (n < 40 && size == SIZE.large) || (n < 75 && size == SIZE.xLarge) || (n < 100 && size == SIZE.xxLarge))
                {
                    if (Random.Range(0, 100) > (size * n - 10) || n < 10)
                    {
                        n += 1;
                        expandFromCell(cell, n, size);
                    }
                }
            }
        }
    }

    /////////////////////
    //Height Generation//
    /////////////////////

    //Recursive height generator

    void generateMountain(HexCell centerCell, float size, int n) {
        HexCell[] adjCells = getAdjacent(centerCell);

        foreach(HexCell cell in adjCells)
        {
            if (cell.type == 0) break;
            if (n < 10 - size && Random.Range(0, 100) > n * size * 5)
            {
                cell.height += 2;
                n += 1;
                generateMountain(cell, size, n);
            }
        }
    }

    ////////////////////
    //Nature Functions//
    ////////////////////

    void generateFeatures()
    {
        foreach (HexCell cell in cells)
        {
            if (cell.type == BIOME.forest)
            {
                if (randBetween(0, 100) < 10)
                {
                    generateOnTile(cell, treePrefab, 4);
                    cell.feature = FEATURE.FOREST;
                }
            }else if (cell.type == BIOME.desert)
            {
                if (randBetween(0, 100) < 10)
                {
                    generateOnTile(cell, cactusPrefab, 2);
                    cell.feature = FEATURE.CACTUS;
                }
            }
        }
    }

    //Takes tile, instantiates terrain objects randomly

    void generateOnTile(HexCell cell, GameObject prefab, int maxNum)
    {
        int a = randBetween(0, maxNum);

        for(int x = 0; x <= a; x++)
        {
            Vector3 treePos = new Vector3(Random.Range(cell.transform.position.x - HexMetrics.innerRadius, cell.transform.position.x + HexMetrics.innerRadius), cell.transform.position.y + treePrefab.transform.localScale.y / 2, Random.Range(cell.transform.position.z - HexMetrics.innerRadius, cell.transform.position.z + HexMetrics.innerRadius));

            GameObject tree = Instantiate(prefab, treePos, Quaternion.identity);

            Vector3 scale = tree.gameObject.transform.localScale;
            scale = new Vector3(scale.x * Random.Range(0.7f, 1.1f), scale.y * Random.Range(0.7f, 3f), scale.z * Random.Range(0.7f, 1.2f));

            tree.transform.localScale = scale;
            tree.transform.SetParent(cell.transform);
        }
    }

    void setResourceValues()
    {
        foreach(HexCell cell in cells)
        {
            if(cell.type == BIOME.forest)
            {
                cell.waterDensity = Random.Range(0, 10);
            }
        }
    }

    //////////////////////////
    //Miscelaneous Functions//
    //////////////////////////

    int randBetween(int x, int y)
    {
        return Random.Range(x, y + 1);
    }

    //takes centerCell and returns array of adjacent cells

    HexCell[] getAdjacent(HexCell centerCell)
    {
        HexCell[] adj = {
            getCell(centerCell.X, centerCell.Y + 1), // Bottom Left
            getCell(centerCell.X, centerCell.Y - 1), // Top Right
            getCell(centerCell.X - 1, centerCell.Y), // Top Left
            getCell(centerCell.X + 1, centerCell.Y), // Bottom Right
            getCell(centerCell.X - 1, centerCell.Y + 1), // Middle Left
            getCell(centerCell.X + 1, centerCell.Y - 1), // Middle Right
        };

        return adj;
    }
}
