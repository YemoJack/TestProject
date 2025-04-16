using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{

    private ChunkPool chunkPool;

    public GameObject tilePrefab;
    public Transform player;


    public int maxChunksExistSize = 6;                  // 最多生成的 地图块 的边长数
    public int chunkSize = 4;                           // 地图块 的边长尺寸
    public int viewDistance = 2;                        // 加载半径（以 chunk 为单位）

    private Dictionary<Vector2Int, Chunk> loadedChunks = new();
    private Vector2Int lastPlayerChunk;


    private Transform mapParent;

    public float updateInterval = 0.5f;                 // 更新间隔
    private float lastUpdateTime = 0f;

    void Start()
    {
        MapSpriteData.LoadSprites();

        // 创建地图根对象
        mapParent = new GameObject("Map").transform;

        chunkPool = new ChunkPool(tilePrefab, maxChunksExistSize, chunkSize, player); // 例如最多缓存 25 个 chunk
        lastPlayerChunk = GetChunkCoord(player.position);
        LoadChunksAround(lastPlayerChunk);
    }

    void Update()
    {
        lastUpdateTime += Time.deltaTime;

        if(lastUpdateTime >= updateInterval)
        {
            Vector2Int currentChunk = GetChunkCoord(player.position);
            if (currentChunk != lastPlayerChunk)
            {
                lastPlayerChunk = currentChunk;
                LoadChunksAround(currentChunk);
            }
            lastUpdateTime = 0f;
        }
    }

    Vector2Int GetChunkCoord(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / chunkSize),
            Mathf.FloorToInt(pos.y / chunkSize)
        );
    }
    void LoadChunksAround(Vector2Int center)
    {
        HashSet<Vector2Int> newChunks = new();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int coord = center + new Vector2Int(x, y);
                newChunks.Add(coord);
            }
        }


        // 卸载不再需要的 chunk
        List<Vector2Int> toRemove = new();
        foreach (var chunk in loadedChunks)
        {
            if (!newChunks.Contains(chunk.Key))
            {
                chunk.Value.Destroy(chunkPool);

                toRemove.Add(chunk.Key);
            }
        }

        foreach (var coord in toRemove)
        {
            loadedChunks.Remove(coord);
        }

        foreach (var chunkPos in newChunks)
        {
            if (!loadedChunks.ContainsKey(chunkPos))
            {
                var chunk = new Chunk(chunkPos, chunkPool, mapParent);

                loadedChunks.Add(chunkPos, chunk);
            }
        }



    }
   
}
