using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public delegate void OnCreateTrainingExampleConnection();
    public static event OnCreateTrainingExampleConnection OnTEConnection;

    public delegate void OnCreateIMLConfigurationConnection();
    public static event OnCreateIMLConfigurationConnection OnIMLConfigConnection;

    public delegate void TrainMLS(string id);
    public static event TrainMLS OnTrainMLS;

    public delegate void ToggleRun(string id);
    public static event ToggleRun OnToggleRun;

    public delegate void OnToggleTrain(string id);
    public static event OnToggleTrain OnTrain;

    public delegate void OnTrainOneExample(string id);
    public static event OnTrainOneExample OnTrainOne;

    public delegate void OnDeleteNode(string id);
    public static event OnDeleteNode DeleteNode;

    }