using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class CameraComponent : MonoBehaviour
    {
        [SerializeField][Range(60, 600)] private float _rotationTime = 60;
        [SerializeField][Range(1, 100)] private float _rotationSpeed = 1;

        public static CameraComponent Instance;

        private void Start()
        {
            Instance = this;
        }

        public IEnumerator SetViewPoint()
        {
            var stepTime = 0.0f;
            var startRotation = transform.rotation;
            var endRotaion = transform.rotation * new Quaternion(0, 180, 0, 0);
            while (stepTime < 1f)
            {
                stepTime += _rotationSpeed/_rotationTime + Time.deltaTime;
                transform.rotation = Quaternion.Lerp(startRotation, endRotaion, Mathf.Pow(stepTime - Time.deltaTime, 2));
                yield return null;
            }

            transform.rotation = endRotaion;
        }
    }
}