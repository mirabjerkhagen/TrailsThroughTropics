using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour
{
    public List<Transform> Adjacents; // List of adjacent cells
    public Vector3 Position;          // Grid position of this cell
    public int weight;                // Weight used for sorting and pathfinding
    public int AdjacentsOpened;       // Tracks how many adjacents are opened

    // Called when the script is first added or on initialization
    void Awake()
    {
        Adjacents = new List<Transform>();
        AdjacentsOpened = 0;
    }

    void Start()
    {
        // Additional initialization if needed
    }

    // Optional: A helper method to reset the cell
    public void ResetCell()
    {
        Adjacents.Clear();
        weight = 0;
        AdjacentsOpened = 0;
    }

    // Optional: Debug visualization
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
