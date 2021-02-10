using System;
using UnityEngine;

namespace InteractML
{
    public struct InputSetUpVRSettings
    {
        public bool isEnabled;
        public IMLInputDevices device;
        /*public int deleteLastButtonNo;
        public IMLTriggerTypes deleteLastButtonTT;*/
        public int deleteAllButtonNo;
        public IMLTriggerTypes deleteAllButtonTT;
        public int recordOneButtonNo;
        public IMLTriggerTypes recordOneButtonTT;
        public int toggleRecordButtonNo;
        public IMLTriggerTypes toggleRecordButtonTT;
        public int trainButtonNo;
        public IMLTriggerTypes trainButtonTT;
        public int toggleRunButtonNo;
        public IMLTriggerTypes toggleRunButtonTT;
        public IMLSides trainingSide;
        public IMLSides mlsSide;
    }
}
