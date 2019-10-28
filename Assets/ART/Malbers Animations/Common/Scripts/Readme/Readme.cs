using System;
using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// Base on the New Unity Readme System
    /// </summary>
    ///  [CreateAssetMenu(menuName = "Malbers Animations/Read Me")]
    public class Readme : ScriptableObject
    {
        public Texture2D icon;
        public string title;
        public Section[] sections;

        [Serializable]
        public class Section
        {
            public string heading, text, linkText, url;
        }
    }
}