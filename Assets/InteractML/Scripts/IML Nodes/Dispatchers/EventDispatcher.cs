using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractML
{
    public class EventDispatcher
    {
        public delegate bool TrainMLS(string nodeID);
        public static TrainMLS TrainMLSCallback;

        public delegate bool ToggleRunningModel(string nodeID);
        public static ToggleRunningModel ToggleRun;


    }
}