using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour
    {
        [SerializeField]
        private float _maxMana;
        [SerializeField]
        private float manaRecover;

        private float _currentMana;

        #region Properties
        public float MaxMana
        {
            get => _maxMana;
        }

        public float CurrentMana
        {
            get => _currentMana;
        }
        #endregion

        private void Awake()
        {
            _currentMana = _maxMana;
        }

        private void Update()
        {
            if (_currentMana < _maxMana)
            {
                _currentMana += manaRecover * Time.deltaTime;

                if (_currentMana > _maxMana)
                {
                    _currentMana = _maxMana;
                }
            }
        }

        public bool UseMana(float manaToUse)
        {
            if (manaToUse > _currentMana)
            {
                return false;
            }

            _currentMana -= manaToUse;
            return true;
        }
    }
}