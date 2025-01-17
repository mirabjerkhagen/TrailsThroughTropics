using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateBigIsland : MonoBehaviour
{   
    // Properties of the terrain
    public int width = 15;
    public int depth = 10;
    public float height= 2f; 

    //Scales the noise
    public float scale = 5f; 

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector3[] holes; 
     private Vector3[] normals; 
    private Vector3 normal; 
    private Vector2[] uv; 

    void Start()
    {
        GenerateTerrainMesh();
       //CreateHoles();
       // CreatePiles();
        CreateSlope(); 
        UpdateMesh();
    }

    // Generate Mesh that handles vertices and triangels
    void GenerateTerrainMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        
    }
    
    //Triangle method 
     Vector3 CalculateNormal(Vector3 v0, Vector3 v1, Vector3 v2) {

        Vector3 normal = new Vector3();
        Vector3 edge1 = v1-v0;
        Vector3 edge2 = v2-v1; 

        normal = Vector3.Cross(edge1,edge2); 
        return normal; 

    }

//Create the basic shape 
    void CreateShape()
{
    // Create vertices, normals, and UV arrays
    vertices = new Vector3[(width + 1) * (depth + 1)];
    normals = new Vector3[vertices.Length];
    uv = new Vector2[vertices.Length]; // Ensure UV array matches the vertex count
    float rand = Random.Range(1.5f,2.0f);

    // Generate vertices and UVs
    for (int z = 0; z <= depth; z++)
    {
        for (int x = 0; x <= width; x++)
        {
            int index = z * (width + 1) + x;

            // Use Perlin noise to determine height
            float y = Mathf.PerlinNoise(x * scale / width *0.5f, z * scale / depth*0.5f) * height*rand;
            //float y = 0.0f; 
           //float y = Mathf.PerlinNoise(x * scale/width*0.5f, z *scale/depth* 0.1f); // Subtle noise
            vertices[index] = new Vector3(x, y, z);

            // Generate UV coordinates (normalize to [0,1])
            uv[index] = new Vector2((float)x / width, (float)z / depth);
        }
    }

    // Create triangles
    triangles = new int[width * depth * 6];
    int vert = 0;
    int tris = 0;

    for (int z = 0; z < depth; z++)
    {
        for (int x = 0; x < width; x++)
        {
            // Triangle 1
            triangles[tris + 0] = vert + 0;
            triangles[tris + 1] = vert + width + 1;
            triangles[tris + 2] = vert + 1;

            // Triangle 2
            triangles[tris + 3] = vert + 1;
            triangles[tris + 4] = vert + width + 1;
            triangles[tris + 5] = vert + width + 2;

            vert++;
            tris += 6;
        }
        vert++; // Skip to the next row
    }

    // Calculate normals using the triangle method
    for (int i = 0; i < triangles.Length; i += 3)
    {
        // Get the indices of the triangle vertices
        int v0 = triangles[i];
        int v1 = triangles[i + 1];
        int v2 = triangles[i + 2];

        // Calculate the normal for the triangle
        Vector3 normal = CalculateNormal(vertices[v0], vertices[v1], vertices[v2]);

        // Add the normal to each vertex of the triangle
        normals[v0] += normal;
        normals[v1] += normal;
        normals[v2] += normal;
    }

    // Normalize the accumulated normals
    for (int i = 0; i < normals.Length; i++)
    {
        normals[i] = normals[i].normalized;
    }
}

     void CreateHoles() {
    int numberOfHoles = 30; 
    float holeDepth = 0.1f; 

    for (int i = 0; i < numberOfHoles; i++)
    {
        // Find a random position on the grid
        int holeX = Random.Range(0, width + 1); 
        int holeZ = Random.Range(0, depth + 1); 

        // Calculate the vertex index
        int vertexIndex = holeZ * (width + 1) + holeX;

        // Modify the vertex height
        vertices[vertexIndex].y -= holeDepth;
    }
}

  void CreatePiles()
{
    int numberOfPiles = 60; 
    float pileRadius = 6f; // Radius of the stone
    float pileHeight = 0.8f; // Maximum height of the stone

    for (int i = 0; i < numberOfPiles; i++)
    {
        // Find a random position on the grid
        int pileX = Random.Range(0, width -1); 
        int pileZ = Random.Range(0, depth -1); 

        // Modify the center vertex (with less drastic height)
        int centerVertexIndex = pileZ * (width + 1) + pileX;
        vertices[centerVertexIndex].y += pileHeight * 0.75f; // Reduce sharpness at the top

        // Modify vertices around the center for smooth, rounded shape
        for (int angle = 0; angle < 360; angle += 5) // Adjust every 10 degrees for smoother circle
        {
            float rad = angle * Mathf.Deg2Rad;
            int offsetX = Mathf.RoundToInt(Mathf.Cos(rad) * pileRadius);
            int offsetZ = Mathf.RoundToInt(Mathf.Sin(rad) * pileRadius);

            int nearbyX = pileX + offsetX;
            int nearbyZ = pileZ + offsetZ;

            // Ensure the indices are within bounds
            if (nearbyX >= 0 && nearbyX <= width && nearbyZ >= 0 && nearbyZ <= depth)
            {
                int nearbyVertexIndex = nearbyZ * (width + 1) + nearbyX;

                // Apply a quadratic falloff for smoother transitions
                float distance = Mathf.Sqrt(offsetX * offsetX + offsetZ * offsetZ);
                float falloff = Mathf.Pow(1 - (distance / pileRadius), 2); // Quadratic falloff
                vertices[nearbyVertexIndex].y += pileHeight * falloff;
            }
        }
    }
}


void CreateSlope()
{
    // Maximum distance from the center to any edge (used for normalization)
    float maxDistance = Mathf.Min(width / 2f, depth / 2f);

    for (int z = 0; z <= depth; z++)
    {
        for (int x = 0; x <= width; x++)
        {
            int index = z * (width + 1) + x;

            // Calculate the distance to the closest edge
            float distToLeft = x;                  // Distance to the left edge (x = 0)
            float distToRight = width - x;         // Distance to the right edge (x = width)
            float distToTop = z;                   // Distance to the top edge (z = 0)
            float distToBottom = depth - z;        // Distance to the bottom edge (z = depth)
            float minDistToEdge = Mathf.Min(distToLeft, distToRight, distToTop, distToBottom);

            // Normalize the distance (closer to edges means smaller values)
            float normalizedDistance = minDistToEdge / maxDistance;

            // Smooth falloff using a curve
            float falloff = Mathf.SmoothStep(0.0f, 1.0f, normalizedDistance)*3.0f; // Smooth transition

            // Adjust the vertex height based on the falloff
            vertices[index].y *= falloff;

            // Gradually transition vertices near edges to underwater
            if (normalizedDistance < 0.2f) // Adjust 0.2f for the smoothness of underwater edges
            {
                vertices[index].y = Mathf.Lerp(vertices[index].y, -1.0f, 1.0f - normalizedDistance / 0.2f);
            }
        }
    }
}





    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals; 
        mesh.uv = uv; 
        //mesh.RecalculateNormals(); // Smooth shading
        mesh.RecalculateBounds(); // Ensure correct mesh bounds
    }
}

