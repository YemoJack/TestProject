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
    [Range(0.1f, 100f)] public float percentage; // �����ٷֱ��ֶ�
}

public class WheelCreator : MonoBehaviour
{
    [Header("��������")]
    public GameObject sectorPrefab;
    public GameObject textPrefab;
    public TextMeshProUGUI resultText;
    public Transform root;
    public float radius = 300f;
    public float textOffset = 50f;
    


    [Header("��ת����")]
    public float spinDuration = 3f;       // ��ת����ʱ��
    public int minRotations = 3;         // ��С��תȦ��
    public int maxRotations = 5;         // �����תȦ��
    public AnimationCurve spinCurve;     // ��ת�ٶ�����



    [Header("����ҳ��")]
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

    [Header("��������")]
    public List<SectorData> wheelData = new List<SectorData>
    {
        new SectorData { sectorName = "һ�Ƚ�", sectorColor = Color.red, percentage = 10f },
        new SectorData { sectorName = "���Ƚ�", sectorColor = Color.green, percentage = 30f },
        new SectorData { sectorName = "���Ƚ�", sectorColor = Color.blue, percentage = 60f },
    };


    private bool isSpinning = false;
    private float[] sectorAngles;        // ��¼ÿ�������ĽǶȷ�Χ
    private float[] sectorPercentages;   // ��¼ÿ�������İٷֱ�



    private List<WheelDataItem> wheelDataItems = new List<WheelDataItem>();



    void Start()
    {

        settingBtn.onClick.AddListener(OnSettingBtnClick);
        backBtn.onClick.AddListener(OnBackBtnClick);
        saveBtn.onClick.AddListener(OnSaveBtnClick);
        addBtn.onClick.AddListener(OnAddBtnClick);
        clearBtn.onClick.AddListener(OnClearBtnClick);


        resultText.text = "�����ť��ʼ��ת";

        // ��ʼ��Ĭ������
        if (spinCurve == null || spinCurve.keys.Length == 0)
        {
            spinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }



        CreateWheel(wheelData);
    }


    #region ����ת��




    public void CreateWheel(List<SectorData> sectors)
    {
        // �������ת��
        foreach (Transform child in root)
        {
            Destroy(child.gameObject);
        }

        if (sectors == null || sectors.Count == 0)
        {
            Debug.LogError("Sector data is empty!");
            return;
        }


        // ������Ȩ��
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

        // ��ʼ���Ƕ�����
        sectorAngles = new float[sectors.Count];
        sectorPercentages = new float[sectors.Count];
        float accumulatedAngle = 0f; // �ۻ���ת�Ƕ�

        for (int i = 0; i < sectors.Count; i++)
        {
            var sectorData = sectors[i];
            float fillAmount = sectorData.percentage / totalWeight;
            float sectorAngle = fillAmount * 360f;


            // ��¼�����ǶȺͰٷֱ�
            sectorAngles[i] = sectorAngle;
            sectorPercentages[i] = sectorData.percentage;

            // ��������
            GameObject sector = Instantiate(sectorPrefab, root);
            sector.name = $"Sector_{i}";

            // ������������
            Image img = sector.GetComponent<Image>();
            img.color = sectorData.sectorColor;
            img.fillAmount = fillAmount;
            sector.transform.localRotation = Quaternion.Euler(0, 0, -accumulatedAngle);

            // ��������
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

        // �����м�Ƕ�
        float middleAngle = startAngle + sectorAngle / 2f;

        // ��������λ��
        Vector2 textPos = new Vector2(
            Mathf.Sin(middleAngle * Mathf.Deg2Rad) * textOffset,
            Mathf.Cos(middleAngle * Mathf.Deg2Rad) * textOffset
        );

        // ����λ�ú���ת
        RectTransform textRect = textComponent.rectTransform;
        textRect.anchoredPosition = textPos;
        textRect.localRotation = Quaternion.Euler(0, 0, -middleAngle + 90f);

        // ��������Ӧ����
        textComponent.enableAutoSizing = true;
        textComponent.fontSizeMin = 8;
        textComponent.fontSizeMax = 36;
    }



    // �ⲿ���õĿ�ʼ��ת����
    public void StartSpin()
    {

        if (!isSpinning)
        {
            //����ת����ת
            root.rotation = Quaternion.identity;

            StartCoroutine(SpinWheelAndGetResult());
        }
    }

    private IEnumerator SpinWheelAndGetResult()
    {
        isSpinning = true;

        // 1. ���ѡ���������ڰٷֱȸ��ʣ�
        int randomAngle = Random.Range(1,360);
        
        // 2. ����Ŀ��Ƕȣ����Ƕ�Ȧ��ת��
        float rotations = Random.Range(minRotations, maxRotations + 1);
        float targetAngle = 360f * rotations + randomAngle;

        // 3. ִ����ת����
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

        // 4. ��ȷͣ��Ŀ��λ��
        root.rotation = Quaternion.Euler(0, 0, endRotation);


        // 5. ���㲢������
        int resultIndex = GetSectorIndexFromAngle(randomAngle);
        string resultName = wheelData[resultIndex].sectorName;

        resultText.text = $" ���: {resultName}";
        isSpinning = false;
    }


    // ���ݽǶȼ����Ӧ����������
    private int GetSectorIndexFromAngle(float angle)
    {
        // ȷ���Ƕ���0-360��Χ��
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

        return sectorAngles.Length - 1; // Ĭ�Ϸ������һ������
    }


    #endregion

    #region ����ҳ��



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
            //�������Ƿ����
            foreach (WheelDataItem dataItem in wheelDataItems)
            {
                totalWeight += dataItem.percentage;

            }
            if( totalWeight*100 < 9990 || totalWeight*100 > 10010)
            {
                Debug.LogError("�����ܺͲ�����100");
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