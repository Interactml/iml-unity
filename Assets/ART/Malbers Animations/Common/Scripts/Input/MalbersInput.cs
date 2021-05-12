using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;

namespace MalbersAnimations
{
    public class MalbersInput : MonoBehaviour , IInputSource
    {
        #region Variables
        // private iMalbersInputs mCharacter;
        public IMCharacter mCharacter;
        private ICharacterMove mCharacterMove;

        private IInputSystem Input_System;
        private Vector3 m_CamForward;
        private Vector3 m_Move;
        private Transform m_Cam;

        public List<InputRow> inputs = new List<InputRow>();                                        //Used to convert them to dictionary
        protected Dictionary<string, InputRow> DInputs = new Dictionary<string, InputRow>();        //Shame it cannot be Serialided :(

        public InputAxis Horizontal = new InputAxis("Horizontal", true, true);
        public InputAxis Vertical = new InputAxis("Vertical", true, true);
        //public InputAxis UpDown = new InputAxis("UpDown", true, AxisType.Raw);


        /// <summary>
        /// Send to the Character to Move using the interface ICharacterMove
        /// </summary>    
        public bool MoveCharacter   { set; get; }
     

        [SerializeField] private bool cameraBaseInput;
        [SerializeField] private bool alwaysForward;

        public bool showInputEvents = false;
        public UnityEvent OnInputEnabled = new UnityEvent();
        public UnityEvent OnInputDisabled = new UnityEvent();

        private bool injectingAxisExternally;
        public bool InjectingAxisExternally { get { return injectingAxisExternally; } set { injectingAxisExternally = true; } }

        private float h;        //Horizontal Right & Left   Axis X
        private float v;        //Vertical   Forward & Back Axis Z
        //private float u;      //Vertical   Forward & Back Axis Z

        public float HorizontalAxis { get { return h; } set { h = value; } }
        public float VerticalAxis { get { return v; } set { v = value; } }

        public string PlayerID = "Player0"; //This is use for Rewired Asset
        #endregion
       
        public bool CameraBaseInput
        {
            get { return cameraBaseInput; }
            set { cameraBaseInput = value; }
        }

        public bool AlwaysForward
        {
            get { return alwaysForward; }
            set { alwaysForward = value; }
        }

        void Awake()
        {

            Input_System = DefaultInput.GetInputSystem(PlayerID);                   //Get Which Input System is being used
            Horizontal.InputSystem = Vertical.InputSystem = Input_System;
            //UpDown.InputSystem = Input_System;

            foreach (var i in inputs)
                i.InputSystem = Input_System;                 //Update to all the Inputs the Input System


            List_to_Dictionary();

            InitializeCharacter();

            MoveCharacter = true;       //Set that the Character can be moved
        }

        void InitializeCharacter()
        {
            mCharacter = GetComponent<IMCharacter>();

            mCharacterMove = GetComponent<ICharacterMove>();

            if (mCharacter != null)
            {
                var keys = new Dictionary<string, BoolEvent>();

                foreach (var dinput in DInputs)
                {
                    keys.Add(dinput.Key, dinput.Value.OnInputChanged); //Use OnINPUT CHANGE INSTEAD OF ON INPUT PRESSED ... IS CALLED LESS TImes
                }
                mCharacter.InitializeInputs(keys);
            }
        }

        private void OnEnable()
        {
            OnInputEnabled.Invoke();
        }

        /// <summary>
        /// Send to the Character to Move using the interface ICharacterMove and the Move(Vector3) method
        /// </summary>  
        public virtual void EnableMovement(bool value)
        {
            MoveCharacter = value;
        }

        void OnDisable()
        {
            if (mCharacterMove != null)
                mCharacterMove.Move(Vector3.zero);       //When the Input is Disable make sure the character/animal is not moving.
            OnInputDisabled.Invoke();
        }

        void Start()
        {
            if (Camera.main != null)                                                //Get the transform of the main camera
                m_Cam = Camera.main.transform;
            else
                m_Cam = GameObject.FindObjectOfType<Camera>().transform;
        }

        void Update()
        {
            SetInput();
        }

        /// <summary>
        /// Send all the Inputs to the Animal
        /// </summary>
        protected virtual void SetInput()
        {
            // If the axis are pulled from Unity Input Manager. If not do nothing
            if (!injectingAxisExternally)
            {
                h = Horizontal.GetAxis;
                v = alwaysForward ? 1 : Vertical.GetAxis;
                //u = UpDown.GetAxis;
            }

            CharacterMove();

            foreach (var item in inputs) { var InputValue = item.GetInput;}             //This will set the Current Input value to the inputs and Invoke the Values
        }

        private void CharacterMove()
        {
            if (MoveCharacter && mCharacterMove != null)
            {
                if (cameraBaseInput)
                    mCharacterMove.Move(CameraInputBased());
                else
                    mCharacterMove.Move(new Vector3(h, 0, v), false);
            }
        }

        /// <summary>
        /// Calculate the Input Axis relative to the camera
        /// </summary>
        protected Vector3 CameraInputBased()
        {
            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, Vector3.one).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
            return m_Move;
        }

        /// <summary>
        /// Enable/Disable an Input Row
        /// </summary>
        public virtual void EnableInput(string inputName, bool value)
        {
            InputRow i;

            i = inputs.Find(item => item.name == inputName);

            if (i!=null)
                i.active = value;
        }


        /// <summary>
        /// Enable an Input Row
        /// </summary>
        public virtual void EnableInput(string inputName)
        {
            InputRow i;

            i = inputs.Find(item => item.name == inputName);

            if (i != null)
                i.active = true;
        }

        /// <summary>
        /// Disable an Input Row
        /// </summary>
        public virtual void DisableInput(string inputName)
        {
            InputRow i;

            i = inputs.Find(item => item.name == inputName);

            if (i != null)
                i.active = false;
        }

        /// <summary>
        /// Check if an Input Row  is active
        /// </summary>
        public virtual bool IsActive(string name)
        {
            InputRow input;

            if (DInputs.TryGetValue(name, out input))
                return input.active;

            return false;
        }

        /// <summary>
        /// Check if an Input Row  exist  and returns it
        /// </summary>
        public virtual InputRow FindInput(string name)
        {
            InputRow input = inputs.Find(item => item.name.ToUpper() == name.ToUpper());

            if (input != null) return input;

            return null;
        }

        private void Reset()
        {
            inputs = new List<InputRow>()
            {
                {new InputRow("Jump", "Jump", KeyCode.Space, InputButton.Press, InputType.Input)},
                {new InputRow("Shift", "Fire3", KeyCode.LeftShift, InputButton.Press, InputType.Input)},
                {new InputRow("Attack1", "Fire1", KeyCode.Mouse0, InputButton.Press, InputType.Input)},
                {new InputRow("Attack2", "Fire2", KeyCode.Mouse1, InputButton.Press, InputType.Input)},
                {new InputRow(false,"SpeedDown", "SpeedDown", KeyCode.Alpha1, InputButton.Down, InputType.Key)},
                {new InputRow(false,"SpeedUp", "SpeedUp", KeyCode.Alpha2, InputButton.Down, InputType.Key)},
                {new InputRow("Speed1", "Speed1", KeyCode.Alpha1, InputButton.Down, InputType.Key)},
                {new InputRow("Speed2", "Speed2", KeyCode.Alpha2, InputButton.Down, InputType.Key)},
                {new InputRow("Speed3", "Speed3", KeyCode.Alpha3, InputButton.Down, InputType.Key)},
                {new InputRow("Action", "Action", KeyCode.E, InputButton.Down, InputType.Key)},
                {new InputRow("Fly", "Fly", KeyCode.Q, InputButton.Down, InputType.Key)},
                {new InputRow("Dodge", "Dodge", KeyCode.R, InputButton.Down, InputType.Key)},
                {new InputRow("Down", "Down", KeyCode.C, InputButton.Press, InputType.Key)},
                {new InputRow("Up", "Jump", KeyCode.Space, InputButton.Press, InputType.Input) },
                {new InputRow("Stun", "Stun", KeyCode.H, InputButton.Press, InputType.Key)},
                {new InputRow("Damaged", "Damaged", KeyCode.J, InputButton.Down, InputType.Key)},
                {new InputRow("Death", "Death", KeyCode.K, InputButton.Down, InputType.Key)},
            };
        }

        /// <summary>
        /// Convert the List of Inputs into a Dictionary
        /// </summary>
        void List_to_Dictionary()
        {
            DInputs = new Dictionary<string, InputRow>();
            foreach (var item in inputs)
                DInputs.Add(item.name, item);
        }
    }
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    #region InputRow and Input Axis
    /// <summary>
    /// Input Class to change directly between Keys and Unity Inputs
    /// </summary>
    [System.Serializable]
    public class InputRow
    {
        public bool active = true;
        public string name = "InputName";
        public InputType type = InputType.Input;
        public string input = "Value";
        public KeyCode key = KeyCode.A;
        public InputButton GetPressed = InputButton.Press;
        /// <summary>
        /// Current Input Value
        /// </summary>
        public bool InputValue = false;
        /// <summary>
        /// Controls the option to inject the input value from outside this script
        /// </summary>
        public bool allowInputValueInjection = false;

        public UnityEvent OnInputDown = new UnityEvent();
        public UnityEvent OnInputUp = new UnityEvent();
        public UnityEvent OnLongPress = new UnityEvent();
        public UnityEvent OnDoubleTap = new UnityEvent();
        public BoolEvent OnInputChanged = new BoolEvent();

        protected IInputSystem inputSystem = new DefaultInput();

        public bool ShowEvents = false;

        #region LONG PRESS and Double Tap
        public float DoubleTapTime = 0.3f;                          //Double Tap Time
        public float LongPressTime = 0.5f;
        //public FloatReference LongPressTime = new FloatReference(0.5f);
        private bool FirstInputPress= false;
        private bool InputCompleted = false;
        private float InputCurrentTime;
        public UnityEvent OnInputPressed = new UnityEvent();
        public FloatEvent OnPressedNormalized = new FloatEvent();

        #endregion

        /// <summary>
        /// Return True or False to the Selected type of Input of choice
        /// </summary>
        public virtual bool GetInput
        {
            get
            {
                if (!active) return false;
                if (inputSystem == null) return false;

                var oldValue = InputValue;

                switch (GetPressed)
                {

                    case InputButton.Press:

                        if (!allowInputValueInjection)
                            InputValue = type == InputType.Input ? InputSystem.GetButton(input) : Input.GetKey(key);


                        if (oldValue != InputValue)
                        {
                            if (InputValue) OnInputDown.Invoke();
                            else OnInputUp.Invoke();

                            OnInputChanged.Invoke(InputValue);
                        }
                        if (InputValue) OnInputPressed.Invoke();

                        return InputValue;


                    //-------------------------------------------------------------------------------------------------------
                    case InputButton.Down:

                        if (!allowInputValueInjection)
                            InputValue = type == InputType.Input ? InputSystem.GetButtonDown(input) : Input.GetKeyDown(key);

                        if (oldValue != InputValue)
                        {
                            if (InputValue) OnInputDown.Invoke();
                            OnInputChanged.Invoke(InputValue);
                        }
                        return InputValue;


                    //-------------------------------------------------------------------------------------------------------
                    case InputButton.Up:

                        if (!allowInputValueInjection)
                            InputValue = type == InputType.Input ? InputSystem.GetButtonUp(input) : Input.GetKeyUp(key);

                        if (oldValue != InputValue)
                        {
                            if (InputValue) OnInputUp.Invoke();
                            OnInputChanged.Invoke(InputValue);
                        }
                        return InputValue;

                    //-------------------------------------------------------------------------------------------------------
                    case InputButton.LongPress:

                        if (!allowInputValueInjection)
                            InputValue = type == InputType.Input ? InputSystem.GetButton(input) : Input.GetKey(key);

                        if (InputValue)
                        {
                            if (!InputCompleted)
                            {
                                if (!FirstInputPress)
                                {
                                    InputCurrentTime = Time.time;
                                    FirstInputPress = true;
                                    OnInputDown.Invoke();
                                }
                                else
                                {
                                    if (Time.time - InputCurrentTime >= LongPressTime)
                                    {
                                        OnLongPress.Invoke();
                                        OnPressedNormalized.Invoke(1);
                                        InputCompleted = true;                     //This will avoid the longpressed being pressed just one time
                                        return (InputValue = true);
                                    }
                                    else
                                    {
                                        OnPressedNormalized.Invoke((Time.time - InputCurrentTime) / LongPressTime);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!InputCompleted && FirstInputPress)
                            {
                                OnInputUp.Invoke();      //If the Input was released before the LongPress was completed ... take it as Interrupted
                            }
                            FirstInputPress = InputCompleted = false;  //This will reset the Long Press
                        }
                        return (InputValue = false);

                    //-------------------------------------------------------------------------------------------------------
                    case InputButton.DoubleTap:

                        if (!allowInputValueInjection)
                            InputValue = type == InputType.Input ? InputSystem.GetButtonDown(input) : Input.GetKeyDown(key);

                        if (InputValue)
                        {
                            if (InputCurrentTime != 0 && (Time.time - InputCurrentTime) > DoubleTapTime)
                            {
                                FirstInputPress = false;    //This is in case it was just one Click/Tap this will reset it
                            }

                            if (!FirstInputPress)
                            {
                                OnInputDown.Invoke();
                                InputCurrentTime = Time.time;
                                FirstInputPress = true;
                            }
                            else
                            {
                                if ((Time.time - InputCurrentTime) <= DoubleTapTime)
                                {
                                    FirstInputPress = false;
                                    InputCurrentTime = 0;
                                    OnDoubleTap.Invoke();       //Sucesfull Double tap

                                    return (InputValue = true);
                                }
                                else
                                {
                                    FirstInputPress = false;
                                }
                            }
                        }
                      
                        return (InputValue = false);
                }
                return false;
            }
        }

        public IInputSystem InputSystem
        {
            get { return inputSystem; }
            set { inputSystem = value; }
        }

        #region Constructors

        public InputRow(KeyCode k)
        {
            active = true;
            type = InputType.Key;
            key = k;
            GetPressed = InputButton.Down;
            inputSystem = new DefaultInput();
        }

        public InputRow(string input, KeyCode key)
        {
            active = true;
            type = InputType.Key;
            this.key = key;
            this.input = input;
            GetPressed = InputButton.Down;
            inputSystem = new DefaultInput();
        }

        public InputRow(string unityInput, KeyCode k, InputButton pressed)
        {
            active = true;
            type = InputType.Key;
            key = k;
            input = unityInput;
            GetPressed = InputButton.Down;
            inputSystem = new DefaultInput();
        }

        public InputRow(string name, string unityInput, KeyCode k, InputButton pressed, InputType itype)
        {
            this.name = name;
            active = true;
            type = itype;
            key = k;
            input = unityInput;
            GetPressed = pressed;
            inputSystem = new DefaultInput();
        }

        public InputRow(bool active , string name, string unityInput, KeyCode k, InputButton pressed, InputType itype)
        {
            this.name = name;
            this.active = active;
            type = itype;
            key = k;
            input = unityInput;
            GetPressed = pressed;
            inputSystem = new DefaultInput();
        }

        public InputRow()
        {
            active = true;
            name = "InputName";
            type = InputType.Input;
            input = "Value";
            key = KeyCode.A;
            GetPressed = InputButton.Press;
            inputSystem = new DefaultInput();
        }

        #endregion
    }
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    [System.Serializable]
    public class InputAxis
    {
        public bool active = true;
        public string name = "NewAxis";
        public bool raw = true;
        public string input = "Value";
        IInputSystem inputSystem = new DefaultInput();
        public FloatEvent OnAxisValueChanged = new FloatEvent();
        float currentAxisValue = 0;


        /// <summary>
        /// Returns the Axis Value
        /// </summary>
        public float GetAxis
        {
            get
            {
                if (inputSystem == null || !active) return 0f;

                currentAxisValue = raw ? inputSystem.GetAxisRaw(input) : inputSystem.GetAxis(input);

             //   OnAxisValueChanged.Invoke(currentAxisValue);
                return currentAxisValue;
            }
        }

        /// <summary>
        /// Set/Get which Input System this Axis is using by Default is set to use the Unity Input System
        /// </summary>
        public IInputSystem InputSystem
        {
            get { return inputSystem; }
            set { inputSystem = value; }
        }

        public InputAxis()
        {
            active = true;
            raw = true;
            input = "Value";
            name = "NewAxis";
            inputSystem = new DefaultInput();
        }

        public InputAxis(string value)
        {
            active = true;
            raw = false;
            input = value;
            name = "NewAxis";
            inputSystem = new DefaultInput();
        }

        public InputAxis(string InputValue, bool active, bool isRaw)
        {
            this.active = active;
            this.raw = isRaw;
            input = InputValue;
            name = "NewAxis";
            inputSystem = new DefaultInput();
        }

        public InputAxis(string name, string InputValue, bool active, bool raw)
        {
            this.active = active;
            this.raw = raw;
            input = InputValue;
            this.name = name;
            inputSystem = new DefaultInput();
        }

    }
    #endregion
}