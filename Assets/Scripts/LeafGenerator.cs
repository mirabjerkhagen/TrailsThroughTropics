using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafGenerator : MonoBehaviour
{
    public Material LeafMaterial; // Assign a material with a leaf texture or solid color
    public Vector2 SizeRange = new Vector2(0.5f, 1.5f); // Min and max leaf size
    public Color ColorMin = new Color(0.3f, 0.7f, 0.3f); // Darker green
    public Color ColorMax = new Color(0.6f, 1.0f, 0.6f); // Lighter green

    public Mesh CreateLeafMesh()
    {
        Mesh mesh = new Mesh();

        // Define vertices for a simple quad leaf
        Vector3[] vertices = {
            new Vector3(0, 0, 0),   // Bottom left
            new Vector3(0, 0, 1),   // Top left
            new Vector3(1, 0, 1),   // Top right
            new Vector3(1, 0, 0)    // Bottom right
        };

        // Scale vertices to make a leaf shape (narrower at one end)
        vertices[1].x -= 0.3f; // Narrow the top left
        vertices[2].x += 0.3f; // Narrow the top right

        // Define triangles (two triangles to form a quad)
        int[] triangles = {
            0, 1, 2,
            0, 2, 3
        };

        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Add UVs (optional, for texturing)
        Vector2[] uvs = {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };
        mesh.uv = uvs;

        // Recalculate normals for proper lighting
        mesh.RecalculateNormals();

        return mesh;
    }

    public void CreateLeafInstance(Vector3 position)
    {
        // Create a GameObject for the leaf
        GameObject leaf = new GameObject("Leaf");

        // Add a MeshFilter and MeshRenderer
        MeshFilter mf = leaf.AddComponent<MeshFilter>();
        MeshRenderer mr = leaf.AddComponent<MeshRenderer>();

        // Assign the generated mesh
        mf.mesh = CreateLeafMesh();

        // Assign the material
        mr.material = LeafMaterial;

        // Randomize size and rotation
        float size = Random.Range(SizeRange.x, SizeRange.y);
        leaf.transform.localScale = new Vector3(size, size, size);
        leaf.transform.position = position;
        leaf.transform.Rotate(0, Random.Range(0, 360), 0);

        // Randomize color
        mr.material.color = Color.Lerp(ColorMin, ColorMax, Random.Range(0f, 1f));
    }
}
