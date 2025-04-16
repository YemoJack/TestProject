using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

/// <summary>
/// 地图块池
/// </summary>
public class ChunkPool
{
    private List<PooledChunk> pool = new();                 // 预备地图块池子 （失活状态）

    private List<PooledChunk> chunks = new();               // 正在使用地图块池子（激活状态）


    private GameObject tilePrefab;                          // 地图块中的Tile预制体
    private Transform poolRoot;                             // 地图块池子（失活状态）根节点
    private int maxPoolChunks;                              // 最大地图块池子数量
    private Transform player;                               // 玩家位置

    public ChunkPool(GameObject tilePrefab, int maxChunkSize,int size, Transform playerRef)
    {
        InitChunkPool(tilePrefab, maxChunkSize, size, playerRef);
    }


    /// <summary>
    /// 初始化地图块池子
    /// </summary>
    /// <param name="tilePrefab">地图块中的Tile预制体</param>
    /// <param name="maxChunkSize">地图块池子（失活状态）根节点</param>
    /// <param name="size">最大地图块池子数量</param>
    /// <param name="playerRef">玩家位置</param>
    public void InitChunkPool(GameObject tilePrefab, int maxChunkSize, int size, Transform playerRef)
    {
        this.tilePrefab = tilePrefab;
        this.maxPoolChunks = maxChunkSize * maxChunkSize;
        this.player = playerRef;

        poolRoot = new GameObject("ChunkPool").transform;
        Object.DontDestroyOnLoad(poolRoot.gameObject);



        pool.Clear();
        chunks.Clear();

        // 创建地图块池子中的地图块
        for (int i = 0; i < maxPoolChunks; i++)
        {
            var root = new GameObject($"Chunk_{i}");
            root.transform.parent = poolRoot;
            var newChunk = new PooledChunk(Vector2Int.zero, root, size, tilePrefab);
            root.gameObject.SetActive(false);
            pool.Add(newChunk);
        }

    }


    /// <summary>
    /// 获取地图块
    /// </summary>
    /// <param name="coord">地图块的位置</param>
    /// <param name="parent">地图块的父对象</param>
    /// <returns></returns>
    public PooledChunk GetChunk(Vector2Int coord,Transform parent)
    {

        if(pool.Count == 0)
        {
            Debug.LogError("Pool is empty");
            return null;
        }


        PooledChunk chunk = pool.FirstOrDefault(c => true);
        //把chunk从pool中移除 并放入chunks中 表示chunk被使用
        pool.Remove(chunk);
        chunks.Add(chunk);
                

        chunk.chunkCoord = coord;
        chunk.Reset(coord);
        chunk.chunkRoot.SetActive(true);
        chunk.chunkRoot.transform.SetParent(parent);

        //Debug.Log("Get pool.Count :" + pool.Count);
        return chunk;
    }

    public void ReturnChunk(PooledChunk chunk)
    {
        chunk.chunkRoot.SetActive(false);
        chunk.chunkRoot.transform.SetParent(poolRoot);
        //把chunk从chunks中移除 并放入pool中 表示chunk被回收
        chunks.Remove(chunk);
        pool.Add(chunk);

        //Debug.Log("Return pool.Count :" + pool.Count);
    }



    public void ClearToPool()
    {

        foreach(var chunk in chunks)
        {
            chunk.chunkRoot.SetActive(false);
            chunk.chunkRoot.transform.SetParent(poolRoot);
            chunks.Remove(chunk);
            pool.Add(chunk);
        }
        chunks.Clear();
    }


    public void ClearPool()
    {
        foreach(var chunk in pool)
        {
            GameObject.Destroy(chunk.chunkRoot.gameObject);
        }
        pool.Clear();
    }


}
