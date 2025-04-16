using UnityEngine;
using System.Collections.Generic;
using System.Drawing;


public class TileData
{
    public Vector2Int tileCoord;
    public Vector2Int localCoord;
    public GameObject tile;

}


public class PooledChunk
{
    public Vector2Int chunkCoord;
    public GameObject chunkRoot;
    public List<TileData> tiles = new();
    private int chunkSize;

    public PooledChunk(Vector2Int coord, GameObject root, int size,GameObject tilePrefab)
    {
        chunkCoord = coord;
        chunkRoot = root;
        chunkSize = size;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 worldPos = new Vector2(Vector2Int.zero.x * size + x, Vector2Int.zero.y * size + y);
                var tileObj = Object.Instantiate(tilePrefab, worldPos, Quaternion.identity, root.transform);
                tileObj.name = $"Tile_{worldPos.x}_{worldPos.y}";
                var tile = new TileData();
                tile.tile = tileObj;
                tile.localCoord = new Vector2Int((int)worldPos.x, (int)worldPos.y);
                tile.tileCoord = new Vector2Int((int)worldPos.x, (int)worldPos.y);
                tiles.Add(tile);

            }
        }

    }


    public void Reset(Vector2Int coord)
    {
        chunkRoot.name = $"Chunk_{coord.x}_{coord.y}";

        chunkRoot.transform.position = new Vector3(coord.x*chunkSize,coord.y * chunkSize, 0);

        foreach (var tile in tiles)
        {
            tile.tileCoord = new Vector2Int(coord.x * chunkSize + tile.localCoord.x, coord.y * chunkSize + tile.localCoord.y);
        }
      




    }

}
