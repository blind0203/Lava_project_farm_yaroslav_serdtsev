using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantStatusWindowComponent : MonoBehaviour
{
    [SerializeField] private Image _progressImage;
    [SerializeField] private GameObject _progressTab, _collectTab;
    [SerializeField] private Button _collectButton;

    private void Start() {
        _collectButton.onClick.AddListener(() => GetComponentInParent<PlantComponent>().Collect());
    }

    public void SetProgress(float progress) {
        _progressImage.fillAmount = progress;
    }

    public void OnFullProgress(bool isCollectable) {
        if (isCollectable) {
            _progressTab.SetActive(false);
            _collectTab.SetActive(true);
        } else {
            Destroy(gameObject);
        }
    }
}
