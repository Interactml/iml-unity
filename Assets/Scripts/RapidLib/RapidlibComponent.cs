using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractML
{
    /// <summary>
    /// Allows to do IML operations in the Unity Editor
    /// </summary>
    public class RapidlibComponent : MonoBehaviour
    {
        #region Variables

        private EasyRapidlib m_EasyRapidlib;

        public bool AllowKeyboardShortcuts;

        public enum LearningType { Classification, Regression, DTW };

        //This is what you need to show in the inspector.
        public LearningType learningType;

        //public bool classification = false;
        [HideInInspector]
        public float startDelay = 0.0f;
        public float captureRate = 10.0f;
        [HideInInspector]
        public float recordTime = -1.0f;
        float timeToNextCapture = 0.0f;
        float timeToStopCapture = 0.0f;

        public Transform[] inputs;

        [HideInInspector]
        public double[] outputs;

        private bool run = false;
        public bool Running { get { return m_EasyRapidlib.Running; } }

        public bool collectData = false;
        public bool CollectingData { get { return collectData; } }

        public bool Training { get { return m_EasyRapidlib.Training; } }
        public bool Trained { get { return m_EasyRapidlib.Trained; } }

        // Feature extraction
        public enum FeaturesEnum { Position, Rotation, Velocity, Acceleration, DistanceToFirstInput, Scale }
        [SerializeField]
        private List<FeaturesEnum> m_Features;
        private int[] m_LengthsFeatureVector;

        private delegate void FeaturesDelegate(ref double[] inputForModel, Transform[] rawInputData);
        private FeaturesDelegate m_ExtractFeatures;
        private int m_PointerFeatureVector;

        // Used to calculate velocity and acceleration
        private Vector3 m_Velocity;
        private Vector3 m_Acceleration;
        private Vector3 m_LastFramePosition;
        private Vector3 m_LastFrameVelocity;

        // Used to calculate distance from all inputs to the first one (i.e. fingers to the hand palm)
        [SerializeField]
        private float[] m_DistancesToFirstInput;


        #endregion

        #region Unity Messages

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

        #region Public Methods

        public void StartCollectingData()
        {

            collectData = true;
            if (!Running)
            {
                //Debug.Log("starting recording in " + startDelay + " seconds");
                timeToNextCapture = Time.time + startDelay;
                if (recordTime > 0)
                {
                    timeToStopCapture = Time.time + startDelay + recordTime;
                }
                else
                {
                    timeToStopCapture = -1;
                }
            }

        }

        public void StopCollectingData()
        {
            if (learningType == LearningType.DTW)
            {
                if (!Running)
                {
                    //trainingSerieses.Add(new TrainingSeries());
                    //trainingSerieses.Last().examples = new List<TrainingExample>(trainingExamples);
                }
                //trainingExamples.Clear();
            }
            collectData = false;
        }

        public void ToggleCollectingData()
        {
            if (collectData)
            {
                StopCollectingData();
            }
            else
            {
                StartCollectingData();
            }
        }

        public void StartRunning()
        {
            run = true;
            if (learningType == LearningType.DTW)
            {
                StartCollectingData();
            }
            else
            {
                StopCollectingData();
            }
        }

        public void StopRunning()
        {
            if (learningType == LearningType.DTW)
            {
                StopCollectingData();
            }
            run = false;
        }

        public void ToggleRunning()
        {
            if (run)
            {
                StopRunning();
            }
            else
            {
                StartRunning();
            }
        }

        [ContextMenu("Clear Training Examples")]
        public void ClearTrainingExamples()
        {
             m_EasyRapidlib.TrainingExamples.Clear();
        }


        #endregion

        #region Private Methods

        private void Initialize()
        {
            // Make sure inputs array is not null (to avoid null references when opening for the first time the project)
            if (inputs == null)
                inputs = new Transform[0];

            // Set size of outputs array to 1
            outputs = new double[1];

            // Storing the this frame as the last frame to properly calculate velocity every frame      
            m_LastFramePosition = this.transform.position;

            // Initialize array of distance of all inputs to first one based on number of inputs
            if (inputs.Length > 0)
            {
                // We will need to calculate the distance from everything to the first one
                m_DistancesToFirstInput = new float[inputs.Length - 1];
            }

            // Initialise instance of easy rapidlib
            m_EasyRapidlib = new EasyRapidlib();

        }

        #endregion
    }

}

