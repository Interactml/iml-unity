using UnityEngine;

namespace MalbersAnimations.Utilities
{
    public class Messages : MonoBehaviour
    {
        public MesssageItem[] messages;                                     //Store messages to send it when Enter the animation State
        public bool UseSendMessage = false;
        public Component component;
        
        
        public virtual void SendMessage(Component component)
        {
            foreach (var m in messages)
            {
                if (m.message == string.Empty || !m.Active) break;          //If the messaje is empty or disabled break.... ignore it

                if (UseSendMessage)
                {
                    DeliverMessage(m, component.transform.root);
                }
                else
                {
                    IAnimatorListener liste = component.GetComponentInParent<IAnimatorListener>();
                    if (liste != null)
                    DeliverListener(m, liste);
                }
            }
        }


        void DeliverMessage(MesssageItem m, Component component)
        {
            switch (m.typeM)
            {
                case TypeMessage.Bool:
                    component.SendMessage(m.message, m.boolValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Int:
                    component.SendMessage(m.message, m.intValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Float:
                    component.SendMessage(m.message, m.floatValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.String:
                    component.SendMessage(m.message, m.stringValue, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.Void:
                    component.SendMessage(m.message, SendMessageOptions.DontRequireReceiver);
                    break;
                case TypeMessage.IntVar:
                    component.SendMessage(m.message, (int)m.intVarValue, SendMessageOptions.DontRequireReceiver);
                    break;
                default:
                    break;
            }
        }

        void DeliverListener(MesssageItem m, IAnimatorListener listener)
        {
            switch (m.typeM)
            {
                case TypeMessage.Bool:
                    listener.OnAnimatorBehaviourMessage(m.message, m.boolValue);
                    break;
                case TypeMessage.Int:
                    listener.OnAnimatorBehaviourMessage(m.message, m.intValue);
                    break;
                case TypeMessage.Float:
                    listener.OnAnimatorBehaviourMessage(m.message, m.floatValue);
                    break;
                case TypeMessage.String:
                    listener.OnAnimatorBehaviourMessage(m.message, m.stringValue);
                    break;
                case TypeMessage.Void:
                    listener.OnAnimatorBehaviourMessage(m.message, null);
                    break;
                case TypeMessage.IntVar:
                    listener.OnAnimatorBehaviourMessage(m.message, (int)m.intVarValue);
                    break;
                default:
                    break;
            }
        }
    }
}