using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBush : MonoBehaviour
{
    public int MinLeaves = 20;
    public int MaxLeaves = 50;
    public Vector3 BushSize = new Vector3(1f, 1f, 1f); // Size of the bush
    public Color MinLeafColor = new Color(0.2f, 0.8f, 0.2f); // Dark green
    public Color MaxLeafColor = new Color(0.4f, 1f, 0.4f); // Light green

    void Start()
    {
        GenerateBush();
    }

    public void GenerateBush()
    {
        int leafCount = Random.Range(MinLeaves, MaxLeaves);
        for (int i = 0; i < leafCount; i++)
        {
            CreateLeaf();
        }
    }

    void CreateLeaf()
    {
        // Create a new GameObject for the leaf
        GameObject leaf = new GameObject("Leaf");
        leaf.transform.parent = transform;

        // Create a MeshFilter and MeshRenderer for the leaf
        MeshFilter meshFilter = leaf.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = leaf.AddComponent<MeshRenderer>();

        // Generate a simple triangular leaf mesh
        Mesh leafMesh = new Mesh();
        leafMesh.vertices = new Vector3[]
        {
            new Vector3(0f, 0f, 0f),   // Bottom left
            new Vector3(0.1f, 0f, 0.2f), // Bottom right
            new Vector3(0.05f, 0.3f, 0.1f) // Top
        };
        leafMesh.triangles = new int[] { 0, 1, 2 };
        leafMesh.RecalculateNormals();

        meshFilter.mesh = leafMesh;

        // Assign a random green color to the leaf
        Material leafMaterial = new Material(Shader.Find("Standard"));
        leafMaterial.color = Color.Lerp(MinLeafColor, MaxLeafColor, Random.Range(0f, 1f));
        meshRenderer.material = leafMaterial;

        // Randomize the position, rotation, and scale of the leaf
        leaf.transform.localPosition = new Vector3(
            Random.Range(-BushSize.x / 2f, BushSize.x / 2f),
            Random.Range(0f, BushSize.y),
            Random.Range(-BushSize.z / 2f, BushSize.z / 2f)
        );
        leaf.transform.localRotation = Quaternion.Euler(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );
        leaf.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
    }
}
