using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LotteryTest : MonoBehaviour
{
    private Dictionary<string, int> prizeWeights = new Dictionary<string, int>()
    {
        { "һ�Ƚ�", 10 },
        { "���Ƚ�", 30 },
        { "���Ƚ�", 60 }
    };

    private List<string> prizePool = new List<string> { "A", "B", "C", "D", "E" };

    void Start()
    {
        // �趨������ӣ����Ի��ɹ̶���ֵ����һ���ԣ�
        int seed = System.DateTime.Now.Millisecond; 
        Random.InitState(seed);
        Debug.Log($"�������: {seed}");

        Debug.Log("=== �齱�߼����� ===");
        Debug.Log("ת�̳齱���: " + SpinWheel());
        Debug.Log("������ֳ齱: " + DrawRandomNumber(1, 100));
        Debug.Log("���Żس齱���: " + DrawNoReplacement());
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
        return "δ�н�";
    }

    int DrawRandomNumber(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    string DrawNoReplacement()
    {
        if (prizePool.Count == 0)
            return "���н����ѳ��꣡";

        int index = Random.Range(0, prizePool.Count);
        string prize = prizePool[index];
        prizePool.RemoveAt(index);
        return prize;
    }
}
