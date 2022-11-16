using TMPro;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{
    [SerializeField] private TMP_Text _carrotsCountText, _xpCountText;
    private int _carrotsCount, _xpCount;

    private void Start() {
        _carrotsCountText.text = _carrotsCount.ToString();
        _xpCountText.text = _xpCount.ToString();
    }

    public void AddCarrots(int resourceCount) {
        _carrotsCount += resourceCount;
        _carrotsCountText.text = _carrotsCount.ToString();
    }

    public void AddXP(int xp) {
        _xpCount += xp;
        _xpCountText.text = _xpCount.ToString();
    }
}
