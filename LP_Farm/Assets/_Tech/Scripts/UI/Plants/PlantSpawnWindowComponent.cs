using DG.Tweening;
using UnityEngine;

public class PlantSpawnWindowComponent : MonoBehaviour
{
    [SerializeField] private Transform _window;

    private Tween _scaleTween;

    private void Start() {
        _window.localScale = Vector3.zero;
    }

    public void ShowWindow(Vector3 position) {
        transform.position = position;
        if (_scaleTween.IsActive()) {
            _scaleTween.Kill();
        }
        _scaleTween = _window.DOScale(1, .25f).From(0);
    }

    public void HideWindow() {
        if (_scaleTween.IsActive()) {
            _scaleTween.Kill();
        }
        _scaleTween = _window.DOScale(0, 0.25f);
    }
}
