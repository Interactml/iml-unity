using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using InteractML.Addons;

namespace InteractML.VR
{
    public class RadialMenu : MonoBehaviour, IAddonIML
    {
        [HideInInspector]
        public IMLComponent graph;

        public GameObject inner;
        public GameObject outer;

        public Transform innerSelectionTransform = null;
        public Transform outerSelectionTransform = null;
        public Transform cursorTransform = null;

        public RadialSection exit = null;
        public RadialSectionNode mlsInteraction = null;
        public RadialSection empty = null;
        public RadialSection trainingMenu = null;

        public RadialSection back = null;
        public RadialSectionNode trainingNode1 = null;
        public RadialSectionNode trainingNode2 = null;
        public RadialSectionNode trainingNode3 = null;
        public RadialSectionNode more = null;
        public RadialSectionNode trainingNode4 = null;
        public RadialSectionNode trainingNode5 = null;
        public RadialSectionNode trainingNode6 = null;

        
        public TMP_Text trainingicon1 = null;
        public TMP_Text trainingicon2 = null;
        public TMP_Text trainingicon3 = null;
        public TMP_Text trainingicon4 = null;
        public TMP_Text trainingicon5 = null;
        public TMP_Text trainingicon6 = null;
        private TMP_Text[] renderers;


        private string trainingSelected;
        private string mlsSelected;

        private Vector2 touchPosition = Vector2.zero;
        private List<RadialSection> firstSelections = null;
        private List<RadialSection> secondSelections = null;
        private List<RadialSectionNode> trainingSelections = null;
        private RadialSection highlightedSelection = null;

        private readonly float firstDegreeIncrement = 90.0f;
        private readonly float secondDegreeIncrement = 45.0f;

        private bool secondOn = false;
        private bool on = false;
        private bool training = false;

        public TextMeshProUGUI status;

        private int count = 0;

        private int counter = 60;

        /// <summary>
        /// Initialized? (Used by IMLAddon logic)
        /// </summary>
        [System.NonSerialized]
        private bool m_IsInit;

        private void OnEnable()
        {
#if UNITY_EDITOR
            // Subscribe to the editor manager so that our update loop gets called
            // Subscription also calls initialize
            IMLEditorManager.SubscribeIMLAddon(this);
#else
            // In case class isn't init
            if (!m_IsInit) Initialize();
#endif
        }

        private void Start()
        {
#if UNITY_EDITOR
            // In case the addon didn't subscribe...
            // Subscribe to the editor manager so that our update loop gets called
            // Subscription also calls initialize
            if (!IMLEditorManager.IsRegistered(this))
                IMLEditorManager.SubscribeIMLAddon(this);
#else
            // In case class isn't init
            if (!m_IsInit) Initialize();
#endif
        }

        private void OnDestroy()
        {
            UnsubscribeToEvents();
        }

        private void CreateAndSetUpSections()
        {
            firstSelections = new List<RadialSection>()
        {
            exit,
            mlsInteraction,
            empty,
            trainingMenu
        };
            secondSelections = new List<RadialSection>()
        {
            back,
            trainingNode1,
            trainingNode2,
            trainingNode3,
            more,
            trainingNode4,
            trainingNode5,
            trainingNode6
        };
            trainingSelections = new List<RadialSectionNode>() {
                trainingNode1,
            trainingNode2,
            trainingNode3,
            more,
            trainingNode4,
            trainingNode5,
            trainingNode6
            };
            mlsInteraction.nodeType = RadialSectionNode.nodeTypes.mlsNode;
            int i = 0;
            foreach (RadialSectionNode section in trainingSelections)
            {
                section.nodeType = RadialSectionNode.nodeTypes.trainingNode;
                section.no = i;
                i++;
            }
            
          
            renderers = new TMP_Text[6]{
            trainingicon1,
            trainingicon2,
            trainingicon3,
            trainingicon4,
            trainingicon5,
            trainingicon6
            };

        }


        private void Update()
        {
            if (on)
            {
                Vector2 direction = Vector2.zero + touchPosition;
                float rotation = GetDegree(direction);

                SetCursorPosition();
                //SetSelectionRotation(rotation);
                SetSelectedEvent(rotation);
                if(count >= counter)
                {
                    if (training)
                    {
                        if (trainingSelected != null)
                        {
                            // Listens and draws information about the currently selected training examples node
                            IMLEventDispatcher.listenText?.Invoke(trainingSelected);
                            status.text = IMLEventDispatcher.getText?.Invoke(trainingSelected);
                        }

                    }
                    else
                    {
                        // Listens and draws information about the currently selected MLS node
                        IMLEventDispatcher.listenText?.Invoke(mlsSelected);
                        status.text = IMLEventDispatcher.getText?.Invoke(mlsSelected);
                    }
                    count = 0;
                }
                count++;
            }

            
            
        }

        private float GetDegree(Vector2 direction)
        {
            float value = Mathf.Atan2(direction.x, direction.y);
            value *= Mathf.Rad2Deg;

            if (value < 0)
                value += 360f;

            return value;
        }

        private void SetCursorPosition()
        {
            cursorTransform.localPosition = touchPosition;
        }

        public void SetTouchPosition(Vector2 newValue)
        {
            touchPosition = newValue;
            // Update highlighted section if needed
            MoveHiglightSection(); 
        }

        private void SetSelectionRotation(float newRotation)
        {
            float snapRoation = SnapRotation(newRotation);
            if (!secondOn)
                innerSelectionTransform.localEulerAngles = new Vector3(0, 0, -snapRoation);
            else
                outerSelectionTransform.localEulerAngles = new Vector3(0, 0, -snapRoation);
        }

        private float SnapRotation(float rotation)
        {
            float snap = 0f;
            if (!secondOn)
                snap = GetNearestIncrement(rotation) * firstDegreeIncrement;
            else
                snap = GetNearestIncrement(rotation) * secondDegreeIncrement;
            return snap;
        }

        private int GetNearestIncrement(float rotation)
        {
            int increment = 0;
            if (!secondOn)
                increment = Mathf.RoundToInt(rotation / firstDegreeIncrement);
            else
                increment = Mathf.RoundToInt(rotation / secondDegreeIncrement);
            return increment;
        }

        private void SetSelectedEvent(float currentRotation)
        {
            int index = GetNearestIncrement(currentRotation);
            if ((!secondOn && index == 4) || (secondOn && index == 8))
            {
                index = 0;
            }
            if (secondOn)
            {
                highlightedSelection = secondSelections[index];
            }
            else
            {
                highlightedSelection = firstSelections[index];
            }

        }

        /// <summary>
        /// Moves the highlight section of the radialMenu 
        /// </summary>
        public void MoveHiglightSection()
        {
            Vector2 direction = Vector2.zero + touchPosition;
            float rotation = GetDegree(direction);
            SetSelectionRotation(rotation);
        }

        /// <summary>
        /// Selects highlighted section
        /// </summary>
        public void ActivateHighlightedSection()
        {
            MoveHiglightSection();
            if (highlightedSelection != null)
            {
                RadialSectionNode section = highlightedSelection as RadialSectionNode;
                if (trainingSelections.Contains(section))
                {
                    int i = section.no;
                    if (graph.TrainingExamplesNodesList.Count > i)
                    {
                        trainingSelected = graph.TrainingExamplesNodesList[i].id;
                    }
                }
                highlightedSelection.onPress?.Invoke();

            } 
            
        }

        private void SubscribeToEvents()
        {
            UnsubscribeToEvents(); // unsubscribing first to avoid duplicates
            IMLEventDispatcher.selectGraph += MenuOpen;
            
        }

        IEnumerator SlowSubscribe()
        {
            yield return new WaitForSeconds(0.5f);
            OnactivateFirstEvents();
        }

        private void OnactivateFirstEvents()
        {
            // direty code need to figure out why this is happening

            if(graph == null)
            {
                exitMenu();
            } else
            {
                exit.onPress += exitMenu;
                trainingMenu.onPress += secondMenu;
                mlsInteraction.onPress += MLSNodeSelected;
            }
            
        }
        
        private void OnactivateSecondEvents()
        {
            // direty code need to figure out why this is happening

            if(graph == null)
            {
                exitMenu();
            } else
            {
                int noOfExamples = graph.TrainingExamplesNodesList.Count;

                for (int i = 0; i < noOfExamples; i++)
                {
                    trainingSelections[i].onPress += TrainingNodeSelected;
                }

                back.onPress += backMenu;
            }
            
        }

        private void OndeactivateFirst()
        {
            exit.onPress -= exitMenu;
            trainingMenu.onPress -= secondMenu;
            mlsInteraction.onPress -= MLSNodeSelected;
        }
        private void OnDeactivateSecond()
        {
            back.onPress -= backMenu;
            foreach (RadialSectionNode section in trainingSelections)
            {
                section.onPress -= TrainingNodeSelected;
            }
        }
        private void UnsubscribeToEvents()
        {
            IMLEventDispatcher.selectGraph -= MenuOpen;
        }

        private void exitMenu()
        {
            Debug.Log("Exit Radial Menu");
            on = false;
            IMLEventDispatcher.deselectGraph?.Invoke(graph);
            //graph = null;
            RemoveMenuSetUp();
            OndeactivateFirst();
            transform.Find("radial").gameObject.SetActive(false); 
        }

        private void secondMenu()
        {
            if (inner.activeSelf)
            {
                inner.SetActive(false);
            }
            if (!outer.activeSelf)
            {
                outer.SetActive(true);
            }
            training = true;
            IMLEventDispatcher.EnableTraining?.Invoke();
            IEnumerator coroutine = EnableOuter();
            StartCoroutine(coroutine);

        }
        private void backMenu()
        {
            training = false;
            if (outer.activeSelf)
            {
                outer.SetActive(false);
            }
            if (!inner.activeSelf)
            {
                inner.SetActive(true);
            }
            IMLEventDispatcher.DisableTraining?.Invoke();
            IEnumerator coroutine = EnableInner();
            StartCoroutine(coroutine);
        }

        IEnumerator EnableInner()
        {
            OnDeactivateSecond();
            // suspend execution for 5 seconds
            yield return new WaitForSeconds(1);
            secondOn = false;
            OnactivateFirstEvents();
        }

        IEnumerator EnableOuter()
        {
            OndeactivateFirst();
            yield return new WaitForSeconds(1.5f);
            secondOn = true;
            OnactivateSecondEvents();
        }

        private void MenuOpen(IMLComponent graphToControl)
        {
            Debug.Log("RadialMenu.MenuOpen");
            transform.Find("radial").gameObject.SetActive(true);
            on = true;
            CreateAndSetUpSections();
            graph = graphToControl;
            SetUpMenu();
        }

        private void SetUpMenu()
        {
            if (graph.MLSystemNodeList.Count > 0)
            {
                mlsSelected = graph.MLSystemNodeList[0].id;
            }
            for (int i = 0; i < trainingSelections.Count-1; i ++)
            {
                if(i < graph.TrainingExamplesNodesList.Count)
                {
                    renderers[i].text = "TTM\n " + i;
                } 
            }

            IEnumerator coroutine = SlowSubscribe();
            StartCoroutine(coroutine);
            
        } 
        
        private void RemoveMenuSetUp()
        {
            trainingSelected = null;
            mlsSelected = null;
            
        } 

        private void MLSNodeSelected()
        {
            training = false;
            IMLEventDispatcher.DisableTraining?.Invoke();
            IMLEventDispatcher.SetUniversalMLSID?.Invoke(mlsSelected);
        }

        private void TrainingNodeSelected()
        {
            IMLEventDispatcher.SetUniversalTrainingID?.Invoke(trainingSelected);
        }

#region IMLAddon

        public bool IsInit()
        {
            return m_IsInit;
        }

        public void Initialize()
        {
            // if it is subscribed already it is fine, it will not duplicate the subscription
            SubscribeToEvents();
            m_IsInit = true;
        }

        public void AddAddonToGameObject(GameObject GO)
        {
            // do nothing
        }

        public void SubscribeToIMLEventDispatcher()
        {
            // if it is subscribed already it is fine, it will not duplicate the subscription
            SubscribeToEvents();
        }

        public void EditorUpdateLogic()
        {
            // do nothing
        }

        public void EditorSceneOpened()
        {
            Initialize();
        }

        public void EditorEnteredPlayMode()
        {
            Initialize();
        }

        public void EditorEnteredEditMode()
        {
            Initialize();
        }

        public void EditorExitingPlayMode()
        {
            // do nothing
        }

        public void EditorExitingEditMode()
        {
            // do nothing
        }
#endregion

    }
}

