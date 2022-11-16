using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantComponent : MonoBehaviour
{
    [SerializeField] private float _scaleModifier = 1;
    [SerializeField] private Transform _model;
    private PlantSO _plant;
    private Cell _parentCell;
    private PlantStatusWindowComponent _statusWindow;

    public void Grow(PlantSO plant, Cell cell) {
        _plant = plant;
        _parentCell = cell;
        _model.eulerAngles += new Vector3(0, Random.Range(0f, 360f), 0);
        _statusWindow = GetComponentInChildren<PlantStatusWindowComponent>();
        StartCoroutine(GrowCycle());
    }

    private IEnumerator GrowCycle() {
        _model.localScale = Vector3.zero;
        float t = _plant.SecondsToGrow;
        while (t > 0) {
            t -= Time.deltaTime;
            float progress = (1 - t / _plant.SecondsToGrow);
            _model.localScale = Vector3.one * progress * _scaleModifier;
            _statusWindow.SetProgress(progress);
            yield return null;
        }
        GlobalParticlesManager.Instance.PlayParticle(transform.position);
        ResourcesManager.Instance.AddXP(_plant.XP);
        _statusWindow.OnFullProgress(_plant.IsCollectable);
    }

    public void Collect() {
        _model.DOScale(.1f, .15f).SetRelative(true).SetEase(Ease.InCubic).OnComplete(() => {
            _parentCell.IsBusy = false;
            if (_plant.ResourceCount > 0) {
                ResourcesManager.Instance.AddCarrots(_plant.ResourceCount);
            }
            FieldManager.Instance.UndigTexture(_parentCell);
            Destroy(gameObject);
        });
    }
}
