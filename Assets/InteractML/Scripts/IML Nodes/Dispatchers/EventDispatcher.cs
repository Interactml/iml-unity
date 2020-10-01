using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractML
{
    public class EventDispatcher
    {
        public delegate bool TrainMLS(string nodeID);
        public static TrainMLS TrainMLSCallback;


    }
}