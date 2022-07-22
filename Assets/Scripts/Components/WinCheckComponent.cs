using UnityEngine;

namespace Assets.Scripts.Components
{
    public class WinCheckComponent : MonoBehaviour
    {
        public static WinCheckComponent instance;

        private bool IsChecked = false;

        private int _blackHP = 12;
        private int _whiteHP = 12;

        private void Start()
        {
            instance = this;
        }
        private void LateUpdate()
        {
            CheckEleminationWin();
            //Debug.Log("черных осталось = " + _blackHP + ". Белых осталось = " + _whiteHP);
        }

        public bool GetCheck
        {
            get { return IsChecked; }
            set { IsChecked = value; }
        }

        public int WhiteHP     {
            get { return _whiteHP; }
            set { _whiteHP = value; }
        }
        public int BlackHP
        {
            get { return _blackHP; }
            set { _blackHP = value; }
        }
        public bool CheckEleminationWin()
        {
            if (_blackHP == 0 || _whiteHP == 0)
            {
                return IsChecked = true;
            }
            else return false;
        }
    }
}