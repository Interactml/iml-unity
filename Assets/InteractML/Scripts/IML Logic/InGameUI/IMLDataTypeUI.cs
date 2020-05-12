using UnityEngine;
using TMPro;

namespace InteractML
{
    /// <summary>
    /// Holds information about data types for the in-game UI
    /// </summary>
    public class IMLDataTypeUI: MonoBehaviour
    {
        public RectTransform rectTransform;
        public IMLSpecifications.DataTypes DataType;
        public TextMeshProUGUI Label;
        public IMLInputField[] InputFields;

        /// <summary>
        /// Setter for a specific input field
        /// </summary>
        /// <param name="which"></param>
        /// <param name="value"></param>
        public void SetInputField (int which, string value)
        {
            var inputField = InputFields[which];
            inputField.SetInputField(value);
        }

        /// <summary>
        /// Protects or unprotects data against a Set
        /// </summary>
        /// <param name="option"></param>
        public void ProtectAllData (bool option)
        {
            if (InputFields != null)
            {
                foreach (var item in InputFields)
                {
                    item.isProtected = option;
                }
            }
        }
    }    
}
