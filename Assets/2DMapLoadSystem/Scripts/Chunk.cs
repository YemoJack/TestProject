using UnityEngine;

public class Chunk
{
    private PooledChunk pooledChunk;
    private Vector2Int coord;

    public Chunk(Vector2Int coord,ChunkPool pool, Transform parent)
    {
        this.coord = coord;
        pooledChunk = pool.GetChunk(coord, parent);

        // TODO: 可加 Tile 刷新逻辑（如果 tile sprite 由数据决定）

        //Debug.Log("ChunkPool.GetChunk :" + pooledChunk.chunkCoord);

        //if(Mathf.Abs(coord.x)<4 && Mathf.Abs(coord.y) < 4)
        //{
        //    foreach (var tile in pooledChunk.tiles)
        //    {
        //        tile.tile.GetComponent<SpriteRenderer>().sprite = MapSpriteData.GetSprite(tile.tileCoord.x, tile.tileCoord.y);
        //    }
        //}

        //Debug.Log("ChunkPool.GetChunk :" + coord);
    }

    public void Destroy(ChunkPool pool)
    {
        //if (Mathf.Abs(coord.x) < 4 && Mathf.Abs(coord.y) < 4)
        //{
        //    foreach (var tile in pooledChunk.tiles)
        //    {
        //        tile.tile.GetComponent<SpriteRenderer>().sprite = null;
        //    }
        //}

        pool.ReturnChunk(pooledChunk);



    }
}


// 全地图图像，按 [x, y] 存储
public static class MapSpriteData
{
    public static Sprite[,] sprites = new Sprite[8, 8];

    public static void LoadSprites()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                string name = $"map_{x}_{y}";
                sprites[x, y] = Resources.Load<Sprite>(name);
            }
        }
    }

    public static Sprite GetSprite(int x, int y)
    {
        if (x < 0 || x >= 8 || y < 0 || y >= 8) return null;
        return sprites[7-y, x];
    }
}
