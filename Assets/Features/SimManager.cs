using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SimManager : MonoBehaviour {

    public Floop Floop_Prefab;
    public HexCell Cell_Prefab;

    public GameObject Tree_Prefab;
    public GameObject Cactus_Prefab;

    public Tent Tent_Prefab;

	void Start () {
        //Camera Manager details
        CameraMove cameraMove = gameObject.AddComponent<CameraMove>();
        cameraMove.height = 10;
        cameraMove.moveSpeed = 1;

        //Game Manager details
        GameObject gameManager = Instantiate(new GameObject(), new Vector3(0, 0, 0), Quaternion.identity);
        gameManager.name = "GameManager";

        //Destroys random game object that keeps coming up and is super annoying...
        Destroy(GameObject.Find("New Game Object"));

        //Grid Factory details
        GridFactory gridFactory = gameManager.AddComponent<GridFactory>();
        gridFactory.width = 60;
        gridFactory.height = 60;
        gridFactory.cellPrefab = Cell_Prefab;
        gridFactory.treePrefab = Tree_Prefab;
        gridFactory.cactusPrefab = Cactus_Prefab;

        //Floop Manager details
        FloopGlobalManager floopGlobalManager = gameManager.AddComponent<FloopGlobalManager>();
        floopGlobalManager.initialFloopSpawnRate = .0024f;
        floopGlobalManager.floopPrefab = Floop_Prefab;
        floopGlobalManager.tent = Tent_Prefab;

        //UI details
        GameObject UI = (GameObject) Instantiate(Resources.Load("UI/Prefabs/Canvas"));
        UI.AddComponent<UIManager>();

        gameManager.AddComponent<TimeManager>();

    }
}
