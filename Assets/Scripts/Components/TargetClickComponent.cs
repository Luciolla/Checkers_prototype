using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Components
{
    public delegate void ClickEventHandler(TargetClickComponent component);
    public delegate void FocusEventHandler(CellComponent component, bool isSelect);
    public enum ColorType
    {
        White,
        Black
    }

    public abstract class TargetClickComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected Material _isPreSelectMaterial;
        [SerializeField] protected Material _isSelectMaterial;
        [SerializeField] protected Material _canEatMaterial;

        [Tooltip("Цветовая сторона игрового объекта"), SerializeField]
        private ColorType _color;
        public ColorType GetColor => _color;

        private MeshRenderer _mesh;
        protected Material[] _meshMaterials = new Material[4];

        public bool IsSelected { get; protected set; }
        public TargetClickComponent Pair { get; set; }

        protected virtual void Start()
        {
            _mesh = GetComponent<MeshRenderer>();
            _meshMaterials[0] = _mesh.material;
        }

        public event ClickEventHandler OnClickEventHandler;
        public event FocusEventHandler OnFocusEventHandler;
        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);

        public void AddAdditionalMaterial(Material material, int index = 1)
        {
            if (index < 1 || index > 3)
            {
                Debug.LogError("Попытка добавить лишний материал. Индекс может быть равен только 1 или 3");
                return;
            }
            _meshMaterials[index] = material;
            _mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }

        public void RemoveAdditionalMaterial(int index = 1)
        {
            if (index < 1 || index > 3)
            {
                Debug.LogError("Попытка удалить несуществующий материал. Индекс может быть равен только 1 или 3");
                return;
            }
            _meshMaterials[index] = null;
            _mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEventHandler?.Invoke(this);
        }

        protected void CallBackEvent(CellComponent target, bool isSelect)
        {
            OnFocusEventHandler?.Invoke(target, isSelect);
        }

        protected void ChangeMaterial(CellComponent component, bool isSelect) //основной метод смены цветов.. 
        {
            if (isSelect)
            {
                component.AddAdditionalMaterial(_isPreSelectMaterial, 1);
            }
            else
            {
                component.RemoveAdditionalMaterial(1);
            }
        }
    }
}