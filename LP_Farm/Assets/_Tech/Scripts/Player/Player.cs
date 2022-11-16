using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    [SerializeField]private DecalProjector _projector;
    [SerializeField] private NavMeshAgent _playerAgent;
    [SerializeField] private PlantSpawnWindowComponent _plantSpawnWindow;

    private Tween _opacityTween, _camMoveTween;
    private const string OPACITY = "_Opacity";
    private FieldManager _fieldManager;

    private Camera _camera;
    private Vector3 _cameraOffset;
    private float _tapTime = .5f;
    private float _tapStart;
    private Vector2 _tapStartPos;
    private Vector2 _cameraPix;

    private void Start() {
        _playerAgent.updateRotation = false;
        _fieldManager = FieldManager.Instance;
        _projector.material.SetFloat(OPACITY, 0f);
        _camera = Camera.main;
        _cameraOffset = _camera.transform.position;
        _cameraPix = new Vector2(_camera.pixelWidth, _camera.pixelHeight);
    }

    private void Update() {
        HandleTapInput();
    }

    private void HandleTapInput() {
        if (_camMoveTween.IsActive()) {
            return;
        }

        if (Input.touchCount > 0) {

            foreach (Touch t in Input.touches) {
                int id = t.fingerId;
                if (EventSystem.current.IsPointerOverGameObject(id)) {
                    return;
                }
            }

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) {
                _tapStart = Time.time;
                _tapStartPos = touch.position;
            }

            if (touch.phase == TouchPhase.Moved) {
                Vector2 delta = touch.deltaPosition;
               
                Vector3 desieredPosition = _camera.transform.position + Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0) * new Vector3(-delta.x / _cameraPix.y, 0, -delta.y / _cameraPix.y) * _camera.transform.position.y;

                desieredPosition.x = Mathf.Clamp(desieredPosition.x, _cameraOffset.x, _fieldManager.FieldSize.x + _cameraOffset.x);
                desieredPosition.z = Mathf.Clamp(desieredPosition.z, _cameraOffset.z, _fieldManager.FieldSize.y + _cameraOffset.z);

                _camera.transform.position = desieredPosition;
            }

            if (touch.phase == TouchPhase.Ended) {
                if (_tapTime + _tapStart >= Time.time && _tapStartPos == touch.position) {
                    OnFieldClick();
                } else {
                    _plantSpawnWindow.HideWindow();
                }
            }
        }
    }

    public void OnFieldClick() {
        Ray r = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(r, out RaycastHit hit, 50f, 1 << 3)) {

            _fieldManager.SelectCell((int)hit.point.x, (int)hit.point.z);
            OnCellSelect((int)hit.point.x, (int)hit.point.z);

            if (_fieldManager.SelectedCell.IsBusy == false) {
                _plantSpawnWindow.ShowWindow(_fieldManager.GetCellCenter(_fieldManager.SelectedCell));
            } else {
                _plantSpawnWindow.HideWindow();
            }
        } else {
            _fieldManager.DeselectCell();
            _plantSpawnWindow.HideWindow();
        }
    }

    private void OnCellSelect(int x, int y) {
        _projector.transform.position = new Vector3(x + .5f, 1.5f, y + .5f);
        
        if (_opacityTween.IsActive()) {
            _opacityTween.Kill();
        }
        _opacityTween = _projector.material.DOFloat(1f, OPACITY, .5f).From(0).SetEase(Ease.OutCubic);
    }

    public void PlantThePlant(PlantSO plantSO) {
        if (_fieldManager.SelectedCell.IsBusy) {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(PlantCycle(plantSO));
    }

    private IEnumerator PlantCycle(PlantSO plantSO) {
        Cell cell = _fieldManager.SelectedCell;
        Vector3 targetPos = _fieldManager.GetCellCenter(cell);
        Vector3 targetOffset = (_playerAgent.transform.position - targetPos);
        targetOffset.y = 0;
        targetOffset.Normalize();
        _playerAgent.SetDestination(targetPos + targetOffset);
        Vector3 fwd = (targetPos - _playerAgent.transform.position);
        while (FlatSqrDistance(_playerAgent.transform.position, targetPos + targetOffset) > .5f) {
            fwd = (targetPos - _playerAgent.transform.position);
            fwd.y = 0;
            fwd.Normalize();
            _playerAgent.transform.forward = Vector3.Slerp(_playerAgent.transform.forward, fwd, 10 * Time.deltaTime);
            yield return null;
        }

        _playerAgent.transform.DOLookAt(_playerAgent.transform.position + fwd, 0.25f, AxisConstraint.Y, Vector3.up);

        _camMoveTween = _camera.transform.DOMove(new Vector3(targetPos.x, 0, targetPos.z) + _cameraOffset, .25f).SetEase(Ease.InOutCubic).OnComplete(() => {
            _fieldManager.DigTexture(cell);
            PlantComponent plantBase = Instantiate(plantSO.Prefab, null);
            plantBase.transform.position = targetPos;
            cell.PlantInCell = plantBase;
            cell.IsBusy = true;
            plantBase.Grow(plantSO, cell);
        });

    }

    private float FlatSqrDistance(Vector3 pos0, Vector3 pos1) {
        Vector3 v = pos0 - pos1;
        v.y = 0;
        return v.sqrMagnitude;
    }
}
