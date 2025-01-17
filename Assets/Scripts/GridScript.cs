using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridScript : MonoBehaviour
{
    public Transform CellPrefab;
    public Transform TreePrefab;
    public Transform Bush;
    public UnityEngine.Vector3 Size;
    public Transform[,] Grid;
    public List<Transform> Set;
    public List<List<Transform>> AdjSet;
    public GenerateTerrain terrainGenerator;
    //public ProceduralBush ProceduralBushScript;

    //List for fruit objects
    public Transform[] Fruits;  // Assign 3D fruit models in Unity Inspector
     public float FruitSpawnChance = 0.2f;  // 20% chance to spawn a fruit
    //public ProceduralBush ProceduralBushScript;
    
    //public Material transparent;
    //public ProceduralBush ProceduralBushScript;

    // Start is called before the first frame update
    // void Start()
    // {
    //     CreateGrid();
    //     SetRandomNumbers();
    //     SetAdjacents();
    //     SetStart(); 
    //     FindNext();
    // }

    void Start() {
        Debug.Log("GridScript Start executed");
        Debug.Log("Terrain generation complete. Proceeding to create grid...");
        CreateGrid();
        Debug.Log("Grid creation invoked.");
        
        SetRandomNumbers();
        SetAdjacents();
        SetStart(); 
        FindNext();
    }
    void CreateGrid() {
        if (CellPrefab == null) {
            Debug.LogError("CellPrefab is not assigned!");
            return;
        }

        float cellSize = 2.0f;
        float xOffset = Size.x / 2f; // Center offset for x
        float zOffset = Size.z / 2f; // Center offset for z


        Grid = new Transform[(int)Size.x, (int)Size.z];

        UnityEngine.Vector3[] terrainVertices = terrainGenerator.vertices;
        int terrainGridWidth = terrainGenerator.width + 1;

        
        for (int x = 0; x < Size.x; x++) {
            for (int z = 0; z < Size.z; z++) {
                int terrainIndex = z * terrainGridWidth + x;

                float x_displace = Size.x/2;
                float z_displace = Size.z/2;

                Transform newCell = Instantiate(CellPrefab, new UnityEngine.Vector3(x-x_displace, 1, z-z_displace), UnityEngine.Quaternion.identity);
                newCell.localScale = new UnityEngine.Vector3(cellSize, cellSize, cellSize);

                newCell.name = $"Cell ({x}, {z})";
                newCell.parent = transform;
                    

                var cellScript = newCell.GetComponent<CellScript>();
                if (cellScript == null) {
                    Debug.LogError($"Cell at ({x}, {z}) is missing CellScript!");
                    continue;
                }

                cellScript.Position = new UnityEngine.Vector3(x, 0, z);
                Grid[x, z] = newCell;
            }
        }
    }
//     void CreateGrid() {
//     if (CellPrefab == null) {
//         Debug.LogError("CellPrefab is not assigned!");
//         return;
//     }

//     Debug.Log("Starting grid creation...");

//     float cellSize = 1.5f;
//     float xOffset = Size.x / 2f; // Center offset for x
//     float zOffset = Size.z / 2f; // Center offset for z

//     Grid = new Transform[(int)Size.x, (int)Size.z];

//     UnityEngine.Vector3[] terrainVertices = terrainGenerator.vertices;
//     int terrainGridWidth = terrainGenerator.width + 1;

//     for (int x = 0; x < Size.x; x++) {
//         for (int z = 0; z < Size.z; z++) {
//             int terrainIndex = z * terrainGridWidth + x;

//             if (terrainIndex >= terrainVertices.Length) {
//                 Debug.LogError($"Terrain index out of bounds: {terrainIndex}");
//                 continue;
//             }

//             UnityEngine.Vector3 terrainVertex = terrainVertices[terrainIndex];
//             float centeredX = terrainVertex.x - xOffset;
//             float centeredZ = terrainVertex.z - zOffset;
//             float y = terrainVertex.y;

//             Debug.Log($"Creating cell at ({centeredX}, {y}, {centeredZ})");

//             Transform newCell = Instantiate(CellPrefab, new UnityEngine.Vector3(centeredX, y, centeredZ), UnityEngine.Quaternion.identity);
//             newCell.localScale = new UnityEngine.Vector3(cellSize, cellSize, cellSize);

//             newCell.name = $"Cell ({x}, {z})";
//             newCell.parent = transform;

//             var cellScript = newCell.GetComponent<CellScript>();
//             if (cellScript == null) {
//                 Debug.LogError($"Cell at ({x}, {z}) is missing CellScript!");
//                 continue;
//             }

//             cellScript.Position = new UnityEngine.Vector3(centeredX, y, centeredZ);
//             Grid[x, z] = newCell;
//         }
//     }

//     Debug.Log("Grid creation complete!");
// }

void PlaceFruit(UnityEngine.Vector3 position)
{
    // Randomly decide to place a fruit based on the spawn chance
    if (Random.Range(0f, 1f) <= FruitSpawnChance)
    {
        if (Fruits.Length > 0)
        {
            // Randomly select a fruit prefab
            Transform fruitPrefab = Fruits[Random.Range(0, Fruits.Length)];

            // Instantiate the fruit slightly above the ground
            UnityEngine.Vector3 fruitPosition = position + new UnityEngine.Vector3(0, 0.5f, 0);

            // Set the fruit's rotation to ensure it's upright
            UnityEngine.Quaternion uprightRotation = UnityEngine.Quaternion.Euler(-80, 0, 0); // No rotation, straight up

            // Instantiate the fruit with the upright rotation
            Instantiate(fruitPrefab, fruitPosition, uprightRotation, transform);
        }
        else
        {
            Debug.LogWarning("No fruit prefabs assigned!");
        }
    }
}



    void CenterCamera() {
        if (Grid != null && Grid.Length > 0) {
            // Calculate the center cell position
            int centerX = (int)(Size.x / 2);
            int centerZ = (int)(Size.z / 2);

            // Position of center cell
            UnityEngine.Vector3 centerPosition = Grid[centerX, centerZ].position;

            // Center camera
            Camera.main.transform.position = new UnityEngine.Vector3(centerPosition.x, 30, centerPosition.z); // Adjust `y` as needed
            Camera.main.transform.LookAt(centerPosition);
        } else {
            Debug.LogError("Grid is not initialized.");
        }
    }
    void SetRandomNumbers() {
        foreach (Transform child in transform) {
            if (child == null) continue;

            int weight = Mathf.Clamp(Random.Range(0, 10), 0, 9);
            var cellScript = child.GetComponent<CellScript>();
            if (cellScript != null) {
                cellScript.weight = weight;

                var textMesh = child.GetComponentInChildren<TextMeshPro>();
                if (textMesh != null) {
                    textMesh.text = weight.ToString();
                } else {
                    Debug.LogWarning($"No TextMeshPro found in {child.name}");
                }
            }
        }
    }

    void SetAdjacents() {
        for (int x = 0; x < Size.x; x++) {
            for (int z = 0; z < Size.z; z++) {
                Transform cell = Grid[x, z];
                if (cell == null) {
                    Debug.LogError($"Cell at ({x},{z}) is null!");
                    continue;
                }

                CellScript cScript = cell.GetComponent<CellScript>();
                if (cScript == null) {
                    Debug.LogError($"Cell at ({x},{z}) is missing CellScript!");
                    continue;
                }

                // Assign adjacents
                if (x - 1 >= 0) cScript.Adjacents.Add(Grid[x - 1, z]);
                if (x + 1 < Size.x) cScript.Adjacents.Add(Grid[x + 1, z]);
                if (z - 1 >= 0) cScript.Adjacents.Add(Grid[x, z - 1]);
                if (z + 1 < Size.z) cScript.Adjacents.Add(Grid[x, z + 1]);

                cScript.Adjacents.Sort(SortByLowestWeight);
            }
        }
    }


    int SortByLowestWeight(Transform inputA, Transform inputB){
        int a = inputA.GetComponent<CellScript>().weight; //a's weight
        int b = inputB.GetComponent<CellScript>().weight; //b's weight
        return a.CompareTo(b);
    }

    void SetStart() {
        Set = new List<Transform>();
        AdjSet = new List<List<Transform>>();
        for (int i = 0; i < 10; i++) {
            AdjSet.Add(new List<Transform>());
        }

        Transform startCell = Grid[0, 0];
        Renderer cellRenderer = startCell.GetComponentInChildren<Renderer>();
        if (cellRenderer == null) {
            Debug.LogWarning($"Cell at {startCell.name} is missing a Renderer on its child!");
            return;
        }

        //Start cell marked green
        //cellRenderer.material.color = Color.green;

        // Adding start cell to the set
        AddToSet(startCell);
        Debug.Log("Starting maze generation...");

        // Begin the algorithm
        Invoke("FindNext", 0);
    }

    void AddToSet(Transform toAdd){
        Set.Add(toAdd);

        foreach(Transform adj in toAdd.GetComponent<CellScript>().Adjacents){
            adj.GetComponent<CellScript>().AdjacentsOpened++;

            if(!Set.Contains(adj) && !AdjSet[adj.GetComponent<CellScript>().weight].Contains(adj)){
                AdjSet[adj.GetComponent<CellScript>().weight].Add(adj);
            }
        }
    }

    void FindNext() {
    Transform next;

    do {
        bool empty = true;
        int lowestList = 0;

        // Find the lowest weighted list with cells
        for (int i = 0; i < 10; i++) {
            lowestList = i;
            if (AdjSet[i].Count > 0) {
                empty = false;
                break;
            }
        }

        if (empty) {
            Debug.Log("Maze generation complete!");
            CancelInvoke("FindNext");

            // Mark end cell red
            //Set[Set.Count - 1].GetComponentInChildren<Renderer>().material.color = Color.red;

            // Mark remaining cells as walls
            foreach (Transform cell in Grid) {
                if (!Set.Contains(cell)) {
                    // Remove the original cell
                    Destroy(cell.gameObject);

                    // Randomly decide to place a tree on certain cells with a small chance (e.g., 10%)
                    if (Random.Range(0f, 1f) <= 0.08f) {
                        if (TreePrefab != null) {
                            Transform newTree = Instantiate(TreePrefab, cell.position, UnityEngine.Quaternion.identity, transform);
                            newTree.name = "Tree at " + cell.position;
                            // Optionally add random rotation or scale for variety
                            newTree.localScale = new UnityEngine.Vector3(1f, 1f, 1f); // Adjust the scale if necessary
                        } else {
                            Debug.LogWarning("Tree prefab is not assigned!");
                        }
                    }

                    // Instantiate a bush at the cell's position (for walls)
                    if (Bush != null) {
                        Transform newBush = Instantiate(Bush, cell.position, UnityEngine.Quaternion.identity, transform);
                        RandomizeBush(newBush);
                    } else {
                        Debug.LogWarning("Bush prefab is not assigned!");
                    }

                    // Check for fruit placement
                    PlaceFruit(cell.position);

                    // Remove the original cell GameObject
                    Destroy(cell.gameObject);

                } else {
                    // Remove path cells (this logic remains unchanged)
                    Destroy(cell.gameObject);
                }
            }
            return;
        }

        // Get next cell
        next = AdjSet[lowestList][0];
        AdjSet[lowestList].Remove(next);

        Debug.Log($"Processing cell: {next.name} with weight: {lowestList}");

    } while (next.GetComponent<CellScript>().AdjacentsOpened >= 2);

    // Mark next cell as part of the maze
    next.GetComponentInChildren<TextMeshPro>().GetComponent<Renderer>().enabled = false;
    next.GetComponentInChildren<Renderer>().enabled = false;
    AddToSet(next);

    Debug.Log($"Added cell {next.name} to the set. Total cells in set: {Set.Count}");

    // Continue algorithm
    Invoke("FindNext", 0);
}


    void RandomizeBush(Transform bush){
        if (bush == null)
        {
            Debug.LogWarning("Bush is null, cannot randomize.");
            return;
        }

        // Randomize rotation
        float randomYRotation = Random.Range(0f, 360f); // Full 360-degree rotation
        bush.rotation = UnityEngine.Quaternion.Euler(0, randomYRotation, 0); // Only rotate around Y-axis

        // Randomize scale
        float randomScale = Random.Range(0.8f, 1.2f); // Slight randomization of scale
        bush.localScale = new UnityEngine.Vector3(randomScale, randomScale, randomScale);

    }


    void Update() {
        if(Input.GetKeyDown(KeyCode.F1)){
            //Application.LoadLevel(0);
        }
    }

    
}
