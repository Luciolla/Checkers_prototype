using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.Components;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ColorType _currentTurn;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private Text _finishScreen;

    private ChipComponent _selectedChip;

    private CellComponent[] _cells;
    private ChipComponent[] _chips;

    private void Awake()
    {
        _cells = FindObjectsOfType<CellComponent>();
        _chips = FindObjectsOfType<ChipComponent>();

        if (_chips?.Length > 0)
        {
            foreach (var chip in _chips)
            {
                chip.OnClickEventHandler += SelectComponent;
                chip.OnChipMove += SwitchEventSystemStatus;
            }
        }

        if (_cells?.Length > 0)
        {
            foreach (var cell in _cells)
            {
                cell.OnClickEventHandler += SelectComponent;
            }
        }
    }

    private void LateUpdate()
    {
        CheckWinCondition();
    }

    private void SelectComponent(TargetClickComponent component)
    {
        if (component is ChipComponent chip && chip.GetColor == _currentTurn)
        {
            if (_selectedChip == null)
            {
                _selectedChip = chip;
            }

            if (_selectedChip == chip)
            {
                chip.OnChipSelect();

                if (!chip.IsSelected)
                {
                    _selectedChip = null;
                }
            }
            else
            {
                _selectedChip.OnChipSelect();
                _selectedChip = chip;
                chip.OnChipSelect();
            }
        }
        else if (component is CellComponent cell && cell.IsEmptyToMove && _selectedChip != null)
        {
            SwitchEventSystemStatus();
            _selectedChip.MoveToNewCell(cell);
            _selectedChip = null;

            ApplyNextTurn();
        }
    }

    private void ApplyNextTurn()
    {
        _currentTurn = _currentTurn == ColorType.Black ? ColorType.White : ColorType.Black;
        StartCoroutine(CameraComponent.Instance.SetViewPoint());
    }

    private void SwitchEventSystemStatus()
    {
        _eventSystem.enabled = !_eventSystem.enabled;
    }

    private void CheckWinCondition()
    {
        if (WinCheckComponent.instance.GetCheck == true)
        {
            _finishScreen.GetComponent<Text>().enabled = true;
            UnityEditor.EditorApplication.isPaused = true;
        }
    }
}