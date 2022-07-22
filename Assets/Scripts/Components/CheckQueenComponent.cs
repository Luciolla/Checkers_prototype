using UnityEngine;

namespace Assets.Scripts.Components
{
    public class CheckQueenComponent : MonoBehaviour
    {
        [Tooltip("Цветовая сторона игрового объекта"), SerializeField]
        private ColorType _color;
       
        public static CheckQueenComponent instance;
        private BoxCollider _collider;

        private void Start()
        {
            instance = this;
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != null)
            {
                var chip = other.GetComponent<ChipComponent>();
                if (chip.GetColor == _color)
                {
                    WinCheckComponent.instance.GetCheck = true;
                }
            }
        }
    }
}