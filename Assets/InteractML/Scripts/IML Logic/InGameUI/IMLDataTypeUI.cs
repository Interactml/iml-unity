using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InteractML
{
    /// <summary>
    /// Holds information about data types for the in-game UI
    /// </summary>
    public class IMLDataTypeUI: MonoBehaviour
    {
        public RectTransform UIPrefab;
        public IMLSpecifications.DataTypes DataType;
        public TextMeshProUGUI Label;
        public TMP_InputField[] InputField;
    }

}
