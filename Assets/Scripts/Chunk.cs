using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshGenerator ChunkMesh;
    Vector2Int GridPosition;
    private void Start()
    {
        ChunkMesh = this.gameObject.AddComponent<MeshGenerator>();
    }
}
