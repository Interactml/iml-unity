using TMPro;

namespace InteractML
{
    /// <summary>
    /// Container for the input field
    /// </summary>
    [System.Serializable]
    public class IMLInputField
    {
        public TMP_InputField localInputField;
        public bool isProtected;

        /// <summary>
        /// Constructor (default isProtected is false)
        /// </summary>
        public IMLInputField()
        {
            isProtected = false;
        }

        /// <summary>
        /// Sets a value to the input field
        /// </summary>
        /// <param name="value"></param>
        public void SetInputField(string value)
        {
            if (!isProtected)
            {
                localInputField.text = value;
            }
        }

        /// <summary>
        /// Gets the contained input field
        /// </summary>
        /// <returns></returns>
        public TMP_InputField GetInputField()
        {
            return localInputField;
        }
    }
}