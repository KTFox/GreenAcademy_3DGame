using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI damageText;

        /// <summary>
        /// Set text traitValue visual
        /// </summary>
        /// <param name="damageAmount"></param>
        public void UpdateText(float damageAmount)
        {
            damageText.text = damageAmount.ToString();
        }

        #region Animation events
        public void DestroyText()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
