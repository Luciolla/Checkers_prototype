using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Components
{
    public class ChipComponent : TargetClickComponent
    {
        [SerializeField][Range(1, 5)] private float _moveTime = 2;
        [SerializeField][Range(1, 5)] private float _moveSpeed = 1;
        [SerializeField][Range(0, 5)] private float _moveHeight = 1;

        public event Action OnChipMove;

        private CapsuleCollider _collider;

        private bool _cantEatColor = true;

        protected override void Start()
        {
            base.Start();

            _collider = GetComponent<CapsuleCollider>();
            _collider.isTrigger = true;

            PairChipWithCell();

            OnFocusEventHandler += ChangeMaterial;
        }

        private void PairChipWithCell()
        {
            if (Pair != null)
            {
                Pair.Pair = null;
            }
            if (Physics.Raycast(transform.position, Vector3.down, out var hit, 5f))
            {
                var cell = hit.collider.gameObject.GetComponent<CellComponent>();
                if (cell != null)
                {
                    Pair = cell;
                    cell.Pair = this;
                }
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent((CellComponent)Pair, false);
        }

        public void OnChipSelect()
        {
            if (IsSelected)
            {
                IsSelected = false;
                RemoveAdditionalMaterial(2);
                ShowAvailableMoves();
            }
            else
            {
                IsSelected = true;
                AddAdditionalMaterial(_isSelectMaterial, 2);
                ShowAvailableMoves();
            }
        }

        public void MoveToNewCell(CellComponent cell)
        {
            OnChipSelect();
            var newPosition = cell.transform.position;
            StartCoroutine(MoveChip(newPosition));
        }

        private IEnumerator MoveChip(Vector3 newPosition)
        {
            var lerpTime = 0f;
            var startPos = transform.position;
            newPosition = new Vector3(newPosition.x, startPos.y + _moveHeight, newPosition.z);
            while (lerpTime < _moveTime / 2)
            {
                transform.position = Vector3.Lerp(startPos, newPosition, lerpTime / _moveTime);
                lerpTime += _moveSpeed * Time.deltaTime;
                yield return null;
            }

            TryEatEnemyChip();
            lerpTime = 0f;
            newPosition = new Vector3(newPosition.x, startPos.y, newPosition.z);
            startPos = transform.position;
            while (lerpTime < _moveTime / 2)
            {
                transform.position = Vector3.Lerp(startPos, newPosition, lerpTime / _moveTime);
                lerpTime += _moveSpeed * Time.deltaTime;
                yield return null;
            }

            transform.position = newPosition;
            PairChipWithCell();
            OnChipMove?.Invoke();
        }

        private bool TryEatEnemyChip()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out var hitChip, 5))
            {
                var chip = hitChip.collider.GetComponent<ChipComponent>();
                if (chip != null)
                {
                    chip.DestroyChip();
                    if (chip.GetColor == ColorType.White) WinCheckComponent.instance.WhiteHP -= 1;
                    else WinCheckComponent.instance.BlackHP -= 1;
                    return true;
                }
            }
            return false;
        }

        public void DestroyChip()
        {
            Pair.Pair = null;
            Pair = null;
            gameObject.SetActive(false);
        }

        public void SetEatMaterial()
        {
            if (_cantEatColor)
            {
                _cantEatColor = false;
                AddAdditionalMaterial(_canEatMaterial, 3);
            }
            else
            {
                _cantEatColor = true;
                RemoveAdditionalMaterial(3);
            }
        }

        private void ShowAvailableMoves()
        {
            if (Pair is CellComponent cell)
            {
                NeighborType a = NeighborType.TopLeft;
                NeighborType b = NeighborType.TopRight;

                if (GetColor == ColorType.Black)
                {
                    a = NeighborType.BottomLeft;
                    b = NeighborType.BottomRight;
                }


                if (cell.TryGetNeighbor(a, out var leftCell))
                {
                    if (leftCell.IsEmpty)
                    {
                        leftCell.SetMaterial();
                    }
                    else
                    {
                        if (leftCell.Pair.GetColor != GetColor &&
                            leftCell.TryGetNeighbor(a, out var leftOverEnemy) &&
                            leftOverEnemy.IsEmpty)
                        {
                            (leftCell.Pair as ChipComponent)?.SetEatMaterial();
                            leftOverEnemy.SetMaterial();
                        }
                    }
                }
                if (cell.TryGetNeighbor(b, out var rightCell))
                {
                    if (rightCell.IsEmpty)
                    {
                        rightCell.SetMaterial();
                    }
                    else
                    {
                        if (rightCell.Pair.GetColor != GetColor &&
                            rightCell.TryGetNeighbor(b, out var rightOverEnemy) &&
                            rightOverEnemy.IsEmpty)
                        {
                            (rightCell.Pair as ChipComponent)?.SetEatMaterial();
                            rightOverEnemy.SetMaterial();
                        }
                    }
                }
            }
        }
    }
}