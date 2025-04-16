using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelDataItem : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TextMeshProUGUI ProbabilityText;
    public Slider probabilitySlider;

    public Image colorImage;
    public Slider RSlider;
    public Slider GSlider;
    public Slider BSlider;

    public Button removeBtn;


    public string sectorName;
    public Color sectorColor;
    public int percentage;


    private void Start()
    {
        nameInput.onEndEdit.AddListener(OnSectorNameChanged);
        probabilitySlider.onValueChanged.AddListener(OnPercentageChanged);


        sectorColor.a = 1;

        RSlider.onValueChanged.AddListener(OnRSliderChanged);
        GSlider.onValueChanged.AddListener(OnGSliderChanged);
        BSlider.onValueChanged.AddListener(OnBSliderChanged);

    }


    private void OnSectorNameChanged(string value)
    {
        sectorName = value;
    }

    private void OnPercentageChanged(float value)
    {
        percentage = (int)value;

        ProbabilityText.text = $"¸ÅÂÊÎª£º {percentage} %";

    }

    private void OnRSliderChanged(float value)
    {
        sectorColor.r = value * 0.01f;

        colorImage.color = sectorColor;

    }

    private void OnGSliderChanged(float value)
    {
        sectorColor.g = value*0.01f;

        colorImage.color = sectorColor;

    }

    private void OnBSliderChanged(float value)
    {
        sectorColor.b = value * 0.01f;

        colorImage.color = sectorColor;

    }


}
