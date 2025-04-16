using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class SectorData
{
    public string sectorName;
    public Color sectorColor = Color.white;
    [Range(0.1f, 100f)] public float percentage; // 新增百分比字段
}

public class WheelCreator : MonoBehaviour
{
    [Header("基础设置")]
    public GameObject sectorPrefab;
    public GameObject textPrefab;
    public TextMeshProUGUI resultText;
    public Transform root;
    public float radius = 300f;
    public float textOffset = 50f;
    


    [Header("旋转设置")]
    public float spinDuration = 3f;       // 旋转持续时间
    public int minRotations = 3;         // 最小旋转圈数
    public int maxRotations = 5;         // 最大旋转圈数
    public AnimationCurve spinCurve;     // 旋转速度曲线



    [Header("设置页面")]
    public Button settingBtn;
    public GameObject settingPanel;
    public GameObject dataPrefab;
    public Button backBtn;
    public Button saveBtn;
    public Button addBtn;
    public Button clearBtn;

    public Toggle putBackTog;
    public Toggle probabilityTog;
    public Transform contentRoot;

    [Header("测试数据")]
    public List<SectorData> wheelData = new List<SectorData>
    {
        new SectorData { sectorName = "一等奖", sectorColor = Color.red, percentage = 10f },
        new SectorData { sectorName = "二等奖", sectorColor = Color.green, percentage = 30f },
        new SectorData { sectorName = "三等奖", sectorColor = Color.blue, percentage = 60f },
    };


    private bool isSpinning = false;
    private float[] sectorAngles;        // 记录每个扇区的角度范围
    private float[] sectorPercentages;   // 记录每个扇区的百分比



    private List<WheelDataItem> wheelDataItems = new List<WheelDataItem>();



    void Start()
    {

        settingBtn.onClick.AddListener(OnSettingBtnClick);
        backBtn.onClick.AddListener(OnBackBtnClick);
        saveBtn.onClick.AddListener(OnSaveBtnClick);
        addBtn.onClick.AddListener(OnAddBtnClick);
        clearBtn.onClick.AddListener(OnClearBtnClick);


        resultText.text = "点击按钮开始旋转";

        // 初始化默认曲线
        if (spinCurve == null || spinCurve.keys.Length == 0)
        {
            spinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }



        CreateWheel(wheelData);
    }


    #region 创建转盘




    public void CreateWheel(List<SectorData> sectors)
    {
        // 清空现有转盘
        foreach (Transform child in root)
        {
            Destroy(child.gameObject);
        }

        if (sectors == null || sectors.Count == 0)
        {
            Debug.LogError("Sector data is empty!");
            return;
        }


        // 计算总权重
        float totalWeight = 0f;
        foreach (var sector in sectors)
        {
            totalWeight += sector.percentage;
        }


        if (totalWeight <= Mathf.Epsilon)
        {
            Debug.LogError("Total percentage must be greater than 0!");
            return;
        }

        // 初始化角度数组
        sectorAngles = new float[sectors.Count];
        sectorPercentages = new float[sectors.Count];
        float accumulatedAngle = 0f; // 累积旋转角度

        for (int i = 0; i < sectors.Count; i++)
        {
            var sectorData = sectors[i];
            float fillAmount = sectorData.percentage / totalWeight;
            float sectorAngle = fillAmount * 360f;


            // 记录扇区角度和百分比
            sectorAngles[i] = sectorAngle;
            sectorPercentages[i] = sectorData.percentage;

            // 创建扇区
            GameObject sector = Instantiate(sectorPrefab, root);
            sector.name = $"Sector_{i}";

            // 设置扇形属性
            Image img = sector.GetComponent<Image>();
            img.color = sectorData.sectorColor;
            img.fillAmount = fillAmount;
            sector.transform.localRotation = Quaternion.Euler(0, 0, -accumulatedAngle);

            // 创建文字
            SetupSectorText(
                sectorData.sectorName,
                accumulatedAngle,
                sectorAngle,
                i
            );

            accumulatedAngle += sectorAngle;
        }
    }

    private void SetupSectorText(string text, float startAngle, float sectorAngle, int index)
    {
        GameObject textObj = Instantiate(textPrefab, root);
        textObj.name = $"Sector_Text_{index}";

        TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();
        textComponent.text = text;

        // 计算中间角度
        float middleAngle = startAngle + sectorAngle / 2f;

        // 计算文字位置
        Vector2 textPos = new Vector2(
            Mathf.Sin(middleAngle * Mathf.Deg2Rad) * textOffset,
            Mathf.Cos(middleAngle * Mathf.Deg2Rad) * textOffset
        );

        // 设置位置和旋转
        RectTransform textRect = textComponent.rectTransform;
        textRect.anchoredPosition = textPos;
        textRect.localRotation = Quaternion.Euler(0, 0, -middleAngle + 90f);

        // 文字自适应设置
        textComponent.enableAutoSizing = true;
        textComponent.fontSizeMin = 8;
        textComponent.fontSizeMax = 36;
    }



    // 外部调用的开始旋转方法
    public void StartSpin()
    {

        if (!isSpinning)
        {
            //重置转盘旋转
            root.rotation = Quaternion.identity;

            StartCoroutine(SpinWheelAndGetResult());
        }
    }

    private IEnumerator SpinWheelAndGetResult()
    {
        isSpinning = true;

        // 1. 随机选择结果（基于百分比概率）
        int randomAngle = Random.Range(1,360);
        
        // 2. 计算目标角度（考虑多圈旋转）
        float rotations = Random.Range(minRotations, maxRotations + 1);
        float targetAngle = 360f * rotations + randomAngle;

        // 3. 执行旋转动画
        float startRotation = root.eulerAngles.z;
        float endRotation = startRotation + targetAngle;

        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = spinCurve.Evaluate(elapsed / spinDuration);
            float currentAngle = Mathf.Lerp(startRotation, endRotation, t);
            root.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        // 4. 精确停在目标位置
        root.rotation = Quaternion.Euler(0, 0, endRotation);


        // 5. 计算并输出结果
        int resultIndex = GetSectorIndexFromAngle(randomAngle);
        string resultName = wheelData[resultIndex].sectorName;

        resultText.text = $" 结果: {resultName}";
        isSpinning = false;
    }


    // 根据角度计算对应的扇区索引
    private int GetSectorIndexFromAngle(float angle)
    {
        // 确保角度在0-360范围内
        angle = angle % 360;
        if (angle < 0) angle += 360;

        float accumulatedAngle = 0f;
        for (int i = 0; i < sectorAngles.Length; i++)
        {
            if (angle >= accumulatedAngle && angle < accumulatedAngle + sectorAngles[i])
            {
                return i;
            }
            accumulatedAngle += sectorAngles[i];
        }

        return sectorAngles.Length - 1; // 默认返回最后一个扇区
    }


    #endregion

    #region 设置页面



    public void OnSettingBtnClick()
    {
        settingPanel.SetActive(true);
    }


    public void OnAddBtnClick()
    {
        GameObject dataItem = Instantiate(dataPrefab, contentRoot);
        wheelDataItems.Add(dataItem.GetComponent<WheelDataItem>());

    }

    public void OnClearBtnClick()
    {
        if( wheelDataItems.Count > 0)
        {
            for(int i = wheelDataItems.Count - 1; i >= 0; i--)
            {
                Destroy(wheelDataItems[i].gameObject);
            }

            wheelDataItems.Clear();
        }

    }

    public void OnSaveBtnClick()
    {
        if( wheelDataItems.Count > 0)
        {
            int totalWeight = 0;
            //检查概率是否合理
            foreach (WheelDataItem dataItem in wheelDataItems)
            {
                totalWeight += dataItem.percentage;

            }
            if( totalWeight*100 < 9990 || totalWeight*100 > 10010)
            {
                Debug.LogError("概率总和不等于100");
                int singleWeight = 100 / wheelDataItems.Count;

                foreach (WheelDataItem dataItem in wheelDataItems)
                {
                    dataItem.percentage = singleWeight;
                }

            }


            wheelData.Clear();
            foreach (WheelDataItem dataItem in wheelDataItems)
            {
                SectorData sectorData = new SectorData();
                sectorData.sectorName = dataItem.sectorName;
                sectorData.percentage = dataItem.percentage;
                sectorData.sectorColor = dataItem.sectorColor;

                wheelData.Add(sectorData);
            }
        }
    }

    public void OnBackBtnClick()
    {
        settingPanel.SetActive(false);


        if(wheelData.Count > 0)
        {
            CreateWheel(wheelData);
        }
        
    }

    #endregion


}