using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractML
{
    public interface IIMLConfiguration
    {
        /*enum CR_LearningChoice { Classification, Regression, DTW };*/
        // Method to train machine learning algorithm. To be called by TrainMLS delegate  
        bool Train();
        //Method to toggle running of model to be called by ToggleRunningModel delegate
        bool ToggleRun();

    }
}