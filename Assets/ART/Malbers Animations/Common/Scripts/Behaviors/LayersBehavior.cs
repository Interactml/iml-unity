using UnityEngine;
using System.Collections.Generic;
namespace MalbersAnimations
{
    [System.Serializable]
    public class LayersActivation
    {
        public string layer;
        public bool activate;
        public StateTransition transA;
        public bool deactivate;
        public StateTransition transD;

    }
    public class LayersBehavior : StateMachineBehaviour
    {
        public LayersActivation[] layers;
        AnimatorTransitionInfo transition;


        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (LayersActivation layer in layers)
            {
                int layer_index = animator.GetLayerIndex(layer.layer);

                transition = animator.GetAnimatorTransitionInfo(layerIndex);

                if (animator.IsInTransition(layerIndex))
                {
                    if (layer.activate)
                    {
                        if (layer.transA == StateTransition.First && stateInfo.normalizedTime <= 0.5f)
                        {
                            animator.SetLayerWeight(layer_index, transition.normalizedTime);
                        }
                        if (layer.transA == StateTransition.Last && stateInfo.normalizedTime >= 0.5f)
                        {
                            animator.SetLayerWeight(layer_index, transition.normalizedTime);
                        }
                    }

                    if (layer.deactivate)
                    {
                        if (layer.transD == StateTransition.First && stateInfo.normalizedTime <= 0.5f)
                        {
                            animator.SetLayerWeight(layer_index, 1 - transition.normalizedTime);
                        }
                        if (layer.transD == StateTransition.Last && stateInfo.normalizedTime >= 0.5f)
                        {
                            animator.SetLayerWeight(layer_index, 1 - transition.normalizedTime);
                        }
                    }
                }

                else 
                {
                    //Clean LayerWeight(1) when finish the first Transition
                    if (layer.activate && layer.transA == StateTransition.First)
                            animator.SetLayerWeight(layer_index, 1);

                    //Clean LayerWeight(0) when finish the first Transition
                    if (layer.deactivate && layer.transD == StateTransition.First)
                            animator.SetLayerWeight(layer_index, 0);
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (LayersActivation layer in layers)
            {
                int layer_index = animator.GetLayerIndex(layer.layer);

                //Clean LayerWeight(1) when finish the Last Transition
                if (layer.activate && layer.transA == StateTransition.Last)
                        animator.SetLayerWeight(layer_index, 1);

                //Clean LayerWeight(0) when finish the Last Transition
                if (layer.deactivate && layer.transD == StateTransition.Last)
                animator.SetLayerWeight(layer_index, 0);
            }
        }
    }
}
