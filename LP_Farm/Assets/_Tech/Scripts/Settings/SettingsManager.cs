using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider _sliderX, _sliderY;
    [SerializeField] private TMP_Text _sizeText;

    private void Start() {
        _sliderX.onValueChanged.AddListener((float value) => ChangeSize(value));
        _sliderY.onValueChanged.AddListener((float value) => ChangeSize(value));
    }

    public void ChangeSize(float value) {
        FieldManager.Instance.ChangeSize(new Vector2Int((int)_sliderX.value, (int)_sliderY.value));
        _sizeText.text = _sliderX.value + "x" + _sliderY.value;
        FieldManager.Instance.GenerateField();
    }
}
