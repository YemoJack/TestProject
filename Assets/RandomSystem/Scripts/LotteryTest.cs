using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LotteryTest : MonoBehaviour
{
    private Dictionary<string, int> prizeWeights = new Dictionary<string, int>()
    {
        { "一等奖", 10 },
        { "二等奖", 30 },
        { "三等奖", 60 }
    };

    private List<string> prizePool = new List<string> { "A", "B", "C", "D", "E" };

    void Start()
    {
        // 设定随机种子（可以换成固定数值测试一致性）
        int seed = System.DateTime.Now.Millisecond; 
        Random.InitState(seed);
        Debug.Log($"随机种子: {seed}");

        Debug.Log("=== 抽奖逻辑测试 ===");
        Debug.Log("转盘抽奖结果: " + SpinWheel());
        Debug.Log("随机数字抽奖: " + DrawRandomNumber(1, 100));
        Debug.Log("不放回抽奖结果: " + DrawNoReplacement());
    }

    string SpinWheel()
    {
        int totalWeight = prizeWeights.Values.Sum();
        int randomValue = Random.Range(1, totalWeight + 1);
        int sum = 0;

        foreach (var prize in prizeWeights)
        {
            sum += prize.Value;
            if (randomValue <= sum)
                return prize.Key;
        }
        return "未中奖";
    }

    int DrawRandomNumber(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    string DrawNoReplacement()
    {
        if (prizePool.Count == 0)
            return "所有奖项已抽完！";

        int index = Random.Range(0, prizePool.Count);
        string prize = prizePool[index];
        prizePool.RemoveAt(index);
        return prize;
    }
}
