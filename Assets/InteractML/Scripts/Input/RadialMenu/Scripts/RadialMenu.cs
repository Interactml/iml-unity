﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractML
{
    public class RadialMenu : MonoBehaviour
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

        private void Awake()
        {

            SubscribeToEvents();
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
            
            foreach (RadialSection section in firstSelections)
            {
                //section.iconRenderer.sprite = section.icon;
            }

            foreach (RadialSection section in secondSelections)
            {
                //section.iconRenderer.sprite = section.icon;
            }
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
                Debug.Log("highlightSelection" + highlightedSelection);
                Debug.Log(secondSelections.Count);
            }
            else
            {
                highlightedSelection = firstSelections[index];
                Debug.Log("highlightSelection" + highlightedSelection.icon);
                Debug.Log(firstSelections.Count);
            }

        }

        public void ActivateHighlightedSection()
        {
            Vector2 direction = Vector2.zero + touchPosition;
            float rotation = GetDegree(direction);
            SetSelectionRotation(rotation);
            if (highlightedSelection != null)
            {
                RadialSectionNode section = highlightedSelection as RadialSectionNode;
                if (trainingSelections.Contains(section))
                {
                    int i = section.no;
                    if (graph.TrainingExamplesNodesList.Count > i)
                    {
                        trainingSelected = graph.TrainingExamplesNodesList[i].id;
                        Debug.Log(trainingSelected + "id");
                    }
                }
                highlightedSelection.onPress?.Invoke();

            } else
            {
                Debug.Log(highlightedSelection + " null");
            }
            
        }

        private void SubscribeToEvents()
        {
            IMLEventDispatcher.selectGraph += MenuOpen;
            
        }

        IEnumerator SlowSubscribe()
        {
            yield return new WaitForSeconds(1);
            OnactivateEvents();
        }

        private void OnactivateEvents()
        {
            Debug.Log("activateEvents");
            int noOfExamples = graph.TrainingExamplesNodesList.Count;
            
            for(int i = 0; i < noOfExamples; i++)
            {
                trainingSelections[i].onPress += TrainingNodeSelected;
            }

            exit.onPress += exitMenu;
            trainingMenu.onPress += secondMenu;
            back.onPress += backMenu;
            mlsInteraction.onPress += MLSNodeSelected;
        }

        private void OndeactivateUnsubscribe()
        {
            exit.onPress -= exitMenu;
            trainingMenu.onPress -= secondMenu;
            back.onPress -= backMenu;
            mlsInteraction.onPress -= MLSNodeSelected;
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
            Debug.Log("exiting");
            on = false;
            IMLEventDispatcher.deselectGraph?.Invoke(graph);
            graph = null;
            RemoveMenuSetUp();
            OndeactivateUnsubscribe();
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
            IMLEventDispatcher.EnableTraining?.Invoke();
            IEnumerator coroutine = EnableOuter();
            StartCoroutine(coroutine);

        }
        private void backMenu()
        {
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
            // suspend execution for 5 seconds
            yield return new WaitForSeconds(1);
            secondOn = false;
        }

        IEnumerator EnableOuter()
        {
            yield return new WaitForSeconds(1);
            secondOn = true;
        }

        private void MenuOpen(IMLComponent graphToControl)
        {
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
            IMLEventDispatcher.SetUniversalMLSID?.Invoke(mlsSelected);
        }

        private void TrainingNodeSelected()
        {
            IMLEventDispatcher.SetUniversalTrainingID(trainingSelected);
        }
    }
}

