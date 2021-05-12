using UnityEngine;
using UnityEngine.UI;

namespace MalbersAnimations
{
    public class UIFollowTransform : MonoBehaviour
    {
        Camera main;
        public Transform WorldTransform;
        public Color FadeOut;
        public Color FadeIn;
        public float time = 0.3f;
        Graphic graphic;


        Graphic Graph
        {
            get
            {
                if (graphic == null)
                {
                    graphic = GetComponent<Graphic>();
                }
                return graphic;
            }
        }

        private void OnEnable()
        {
            Aling();
        }

        public void SetTransform(Transform newTarget)
        {
            WorldTransform = newTarget;
        }

        private void Start()
        {
            Aling();
        }

        void Awake()
        {
            main = Camera.main;
            graphic = GetComponent<Graphic>();
        }

        void Update()
        {
            Aling();
        }

        public void Aling()
        {
            if (!main || !WorldTransform) return;
            transform.position = main.WorldToScreenPoint(WorldTransform.position);
        }


        public virtual void Fade_In_Out(bool value)
        {
            Graph.CrossFadeColor(value ? FadeIn : FadeOut, time, false, true);
        }


        public virtual void Fade_In(float time)
        {
            graphic.CrossFadeColor(FadeIn, time, false, true);
        }

        public virtual void Fade_Out(float time)
        {
            graphic.CrossFadeColor(FadeOut, time, false, true);
        }
    }
}