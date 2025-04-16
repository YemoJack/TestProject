using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

/// <summary>
/// ��ͼ���
/// </summary>
public class ChunkPool
{
    private List<PooledChunk> pool = new();                 // Ԥ����ͼ����� ��ʧ��״̬��

    private List<PooledChunk> chunks = new();               // ����ʹ�õ�ͼ����ӣ�����״̬��


    private GameObject tilePrefab;                          // ��ͼ���е�TileԤ����
    private Transform poolRoot;                             // ��ͼ����ӣ�ʧ��״̬�����ڵ�
    private int maxPoolChunks;                              // ����ͼ���������
    private Transform player;                               // ���λ��

    public ChunkPool(GameObject tilePrefab, int maxChunkSize,int size, Transform playerRef)
    {
        InitChunkPool(tilePrefab, maxChunkSize, size, playerRef);
    }


    /// <summary>
    /// ��ʼ����ͼ�����
    /// </summary>
    /// <param name="tilePrefab">��ͼ���е�TileԤ����</param>
    /// <param name="maxChunkSize">��ͼ����ӣ�ʧ��״̬�����ڵ�</param>
    /// <param name="size">����ͼ���������</param>
    /// <param name="playerRef">���λ��</param>
    public void InitChunkPool(GameObject tilePrefab, int maxChunkSize, int size, Transform playerRef)
    {
        this.tilePrefab = tilePrefab;
        this.maxPoolChunks = maxChunkSize * maxChunkSize;
        this.player = playerRef;

        poolRoot = new GameObject("ChunkPool").transform;
        Object.DontDestroyOnLoad(poolRoot.gameObject);



        pool.Clear();
        chunks.Clear();

        // ������ͼ������еĵ�ͼ��
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
    /// ��ȡ��ͼ��
    /// </summary>
    /// <param name="coord">��ͼ���λ��</param>
    /// <param name="parent">��ͼ��ĸ�����</param>
    /// <returns></returns>
    public PooledChunk GetChunk(Vector2Int coord,Transform parent)
    {

        if(pool.Count == 0)
        {
            Debug.LogError("Pool is empty");
            return null;
        }


        PooledChunk chunk = pool.FirstOrDefault(c => true);
        //��chunk��pool���Ƴ� ������chunks�� ��ʾchunk��ʹ��
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
        //��chunk��chunks���Ƴ� ������pool�� ��ʾchunk������
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
