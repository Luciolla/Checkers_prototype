using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Assets.Scripts.Components
{
    public enum NeighborType : byte
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class CellComponent : TargetClickComponent
    {
        private Dictionary<NeighborType, CellComponent> _neighbors;
        public bool IsEmpty => Pair == null;
        public bool IsEmptyToMove => !_canSelect && IsEmpty;
        private bool _canSelect = true;

        protected override void Start()
        {
            _neighbors = new Dictionary<NeighborType, CellComponent>();

            base.Start();

            AddNeighbor(NeighborType.TopLeft, new Vector3(-1, 0, 1));
            AddNeighbor(NeighborType.TopRight, new Vector3(1, 0, 1));
            AddNeighbor(NeighborType.BottomLeft, new Vector3(-1, 0, -1));
            AddNeighbor(NeighborType.BottomRight, new Vector3(1, 0, -1));
        }

        private void AddNeighbor(NeighborType neighborType, Vector3 direction)
        {
            if (Physics.Raycast(transform.position, direction, out var hit, 5))
            {
                var cell = hit.collider.GetComponent<CellComponent>();
                if (cell != null)
                {
                    _neighbors.Add(neighborType, cell);
                }
            }
        }

        public bool TryGetNeighbor(NeighborType type, out CellComponent component) => _neighbors.TryGetValue(type, out component);

        public override void OnPointerEnter(PointerEventData eventData)
        {
            CallBackEvent(this, true);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            CallBackEvent(this, false);
        }

        public void SetMaterial()
        {
            if (_canSelect)
            {
                _canSelect = false;
                AddAdditionalMaterial(_isSelectMaterial, 2);
            }
            else
            {
                _canSelect = true;
                RemoveAdditionalMaterial(1);
                RemoveAdditionalMaterial(2);
            }
        }
    }
}