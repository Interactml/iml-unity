using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MalbersAnimations
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Animal))]
    public class AnimalEditor : Editor
    {
        protected Animal M;
        MonoScript script;
        protected SerializedProperty
            animalTypeID, GroundLayer, StartSpeed, Height, WalkSpeed, TrotSpeed, RunSpeed,

            FallRayDistance, YAxisPositiveMultiplier, YAxisNegativeMultiplier,

            maxAngleSlope, SlowSlopes, forwardJumpControl, smoothJumpForward,

            GotoSleep, SnapToGround , AlingToGround, waterLine, swimSpeed, underWaterSpeed, bank,

            life, defense, attackStrength, FallRayMultiplier, debug, attackTotal, attackDelay, activeAttack, damageInterrupt,

            FlySpeed, upDownSmoothness, StartFlying, land, animalProxy;

        UnityEditor.Animations.AnimatorController controller;
        List<AnimatorControllerParameter> parameters;
        private void OnEnable()
        {
            M = (Animal)target;
            script = MonoScript.FromMonoBehaviour(M);
            FindProperties();

           if(M.Anim) controller = (UnityEditor.Animations.AnimatorController) M.Anim.runtimeAnimatorController;

           if (controller)  parameters = controller.parameters.ToList(); ///
        }

        bool FindParameter(int ParameterHash, AnimatorControllerParameterType Ptype)
        {
            if (parameters != null)
            {
                AnimatorControllerParameter founded = parameters.Find(item => item.nameHash == ParameterHash && item.type == Ptype);

                if (founded != null)   return true;
            }
            return false;
        }


        protected void FindProperties()
        {
            animalTypeID = serializedObject.FindProperty("animalTypeID");
            animalProxy = serializedObject.FindProperty("animalProxy");
            GroundLayer = serializedObject.FindProperty("GroundLayer");
            StartSpeed = serializedObject.FindProperty("StartSpeed");
            Height = serializedObject.FindProperty("height");

            WalkSpeed = serializedObject.FindProperty("walkSpeed");
            TrotSpeed = serializedObject.FindProperty("trotSpeed");
            RunSpeed = serializedObject.FindProperty("runSpeed");

            maxAngleSlope = serializedObject.FindProperty("maxAngleSlope");
            SlowSlopes = serializedObject.FindProperty("SlowSlopes");

            GotoSleep = serializedObject.FindProperty("GotoSleep");
            SnapToGround = serializedObject.FindProperty("SnapToGround");

            AlingToGround = serializedObject.FindProperty("AlingToGround");


            waterLine = serializedObject.FindProperty("waterLine");

            swimSpeed = serializedObject.FindProperty("swimSpeed");
            underWaterSpeed = serializedObject.FindProperty("underWaterSpeed");

            FlySpeed = serializedObject.FindProperty("flySpeed");
            StartFlying = serializedObject.FindProperty("StartFlying");
            land = serializedObject.FindProperty("land");

            life = serializedObject.FindProperty("life");
            defense = serializedObject.FindProperty("defense");
            attackStrength = serializedObject.FindProperty("attackStrength");
            attackDelay = serializedObject.FindProperty("attackDelay");

            attackTotal = serializedObject.FindProperty("TotalAttacks");
            activeAttack = serializedObject.FindProperty("activeAttack");
            damageInterrupt = serializedObject.FindProperty("damageInterrupt");

            FallRayDistance = serializedObject.FindProperty("FallRayDistance");
            FallRayMultiplier = serializedObject.FindProperty("FallRayMultiplier");

            forwardJumpControl = serializedObject.FindProperty("forwardJumpControl");
            smoothJumpForward = serializedObject.FindProperty("smoothJumpForward");


            upDownSmoothness = serializedObject.FindProperty("upDownSmoothness");
            YAxisNegativeMultiplier = serializedObject.FindProperty("YAxisNegativeMultiplier");
            YAxisPositiveMultiplier = serializedObject.FindProperty("YAxisPositiveMultiplier");

            debug = serializedObject.FindProperty("debug");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawAnimalInspector();
            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawAnimalInspector()
        {
            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Locomotion System", MessageType.None, true);
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            EditorGUI.BeginDisabledGroup(true);
            script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginChangeCheck();


            //────────────────────────────────── General ──────────────────────────────────

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            M.EditorGeneral = EditorGUILayout.Foldout(M.EditorGeneral, "General");
            EditorGUI.indentLevel--;

            if (M.EditorGeneral)
            {
                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
                //EditorGUILayout.PropertyField(animalProxy, new GUIContent("Animal Proxy", "This will initialize a proxy for the animal so it can be accesed anywhere using Unity Events"));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stance"), new GUIContent("Active Stance", "Current Active Stance on the Animal.. To change it use the ToggleStance() or the Stance property"));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.PropertyField(GroundLayer, new GUIContent("Ground Layer", "Specify wich layer are Ground"));
                EditorGUILayout.PropertyField(StartSpeed, new GUIContent("Start Speed", "Activate the correct additive Animation to offset the Bones"));

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(Height, new GUIContent("Height", "Distance from the ground"));
                if (GUILayout.Button( new GUIContent("C","Calculate the Height of the Animal, the Chest or Hip Pivot must be setted"), EditorStyles.miniButton, GUILayout.Width(18)))
                {
                    if (!CalculateHeight())
                    {
                        EditorGUILayout.HelpBox("No pivots found, please add at least one Pivot (CHEST or HIP)", MessageType.Warning);
                    }
                } 
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                M.canSwim = GUILayout.Toggle( M.canSwim, new GUIContent("Can Swim", "Activate the Swim Logic\nif the Animator Controller of this animal does not have a 'Swim' Bool Parameter\nThis option should be disabled\nIt might cause unwanted behaviours"), EditorStyles.miniButton);
                M.canFly = GUILayout.Toggle( M.canFly, new GUIContent("Can Fly", "Activate the Fly Logic\nif the Animator Controller of this animal does not have a 'Fly' Bool Parameter\nThis option should be disabled\nIt might cause unwanted behaviours"), EditorStyles.miniButton);
                EditorGUILayout.EndHorizontal();

                if (M.canFly)
                {
                    if (!FindParameter(Hash.Fly, AnimatorControllerParameterType.Bool))
                    {
                        EditorGUILayout.HelpBox("The Animator Controller of this animal does not have a 'Fly' Bool Parameter\nThis option should be disabled\nIt might cause unwanted behaviours", MessageType.Warning);
                    }
                }

                if (M.canSwim)
                {
                    if (!FindParameter(Hash.Swim, AnimatorControllerParameterType.Bool))
                    {
                        EditorGUILayout.HelpBox("The Animator Controller of this animal does not have a 'Swim' Bool Parameter\nThis option should be disabled\nIt might cause unwanted behaviours", MessageType.Warning);
                    }
                }


                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            //────────────────────────────────── Ground ──────────────────────────────────

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            M.EditorGround = EditorGUILayout.Foldout(M.EditorGround, "Ground");
            EditorGUI.indentLevel--;

            if (M.EditorGround)
            {
                DrawSpeed(WalkSpeed, "Walk");
                DrawSpeed(TrotSpeed, "Trot");
                DrawSpeed(RunSpeed, "Run");
            }

            EditorGUILayout.EndVertical();


            //────────────────────────────────── Water ──────────────────────────────────
            if (M.canSwim)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                M.EditorWater = EditorGUILayout.Foldout(M.EditorWater, "Swim");
                EditorGUI.indentLevel--;
                if (M.EditorWater)
                {
                    DrawSpeed(swimSpeed, "Swim");
                    EditorGUILayout.PropertyField(waterLine, new GUIContent("Water Line", "Aling the animal to the Water Surface"));
                    M.CanGoUnderWater = GUILayout.Toggle(M.CanGoUnderWater, new GUIContent("Can go Underwater", "Activate the UnderWater Logic"), EditorStyles.miniButton);

                    if (M.CanGoUnderWater)
                    {
                        if (M.CanGoUnderWater)
                        {
                            if (!FindParameter(Hash.Underwater, AnimatorControllerParameterType.Bool))
                            {
                                EditorGUILayout.HelpBox("The Animator Controller of this animal does not have a 'Underwater' Bool Parameter\nThis option should be disabled\nIt might cause unwanted behaviours", MessageType.Warning);
                            }
                        }

                        DrawSpeed(underWaterSpeed, "UnderWater");
                    }
                }
                EditorGUILayout.EndVertical();

            }

            //────────────────────────────────── AIR ──────────────────────────────────
            if (M.canFly)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                M.EditorAir = EditorGUILayout.Foldout(M.EditorAir, "Fly");
                EditorGUI.indentLevel--;
                if (M.EditorAir)
                {
                    DrawSpeed(FlySpeed, "Fly");

                    EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
                    EditorGUILayout.PropertyField(StartFlying, new GUIContent("Start Flying", "Start in the FlyMode"));
                    EditorGUILayout.PropertyField(land, new GUIContent("Land", "When the animal is close to the Floor Disable the fly mode (LAND)"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("LockUp"), new GUIContent("Lock Up", "The animal cannot fly upwards... just fly forward or down..."));
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }

            //────────────────────────────────── Atributes ──────────────────────────────────

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            M.EditorAttributes = EditorGUILayout.Foldout(M.EditorAttributes, "Attributes");
            EditorGUI.indentLevel--;

            if (M.EditorAttributes)
            {
                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
                EditorGUILayout.PropertyField(life, new GUIContent("Life", "Life Points"));
                EditorGUILayout.PropertyField(defense, new GUIContent("Defense", "Defense Points"));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(attackTotal, new GUIContent("Total Attacks", "Total of Animation Attacks"), GUILayout.MinWidth(25));
                EditorGUIUtility.labelWidth = 40;
                EditorGUILayout.PropertyField(activeAttack, new GUIContent("Active", "Currrent active Attack\n if value is -1 means it will play a random attack according the Total Attacks"), GUILayout.MinWidth(0));
                EditorGUIUtility.labelWidth = 0;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(attackStrength, new GUIContent("Attack Points", "Attack Hit Points"));
                EditorGUILayout.PropertyField(attackDelay, new GUIContent("Attack Delay", "Time for this animal to be able to Attack again. \nGreater number than the animation itself will be ignored"));

                if (M.attackDelay <= 0)
                {
                    EditorGUILayout.HelpBox("The Attack will not be interrupted if AttackDelay is below 0", MessageType.Info);
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("damageDelay"), new GUIContent("Damage Inmunity", "The Animal will not receive Damage during this time"));
                EditorGUILayout.PropertyField(damageInterrupt, new GUIContent("Damage Interrupt", "After receiving damage, time that the Animal can move to flee"));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inmune"), new GUIContent("Inmune", "This animal cannot recieve damage"));
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndVertical();

            //────────────────────────────────── Air Control ──────────────────────────────────

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            M.EditorAirControl = EditorGUILayout.Foldout(M.EditorAirControl, "Jump/Fall Control");
            EditorGUI.indentLevel--;
            if (M.EditorAirControl)
            {
                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("airRotation"), new GUIContent("Jump/Fall Rotation", "Enables you to rotate the animal while jumping or falling"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AirControl"), new GUIContent("Air Control", "Allows the Inputs to control the Fall and Jump movement on Air"));

                if (M.AirControl)
                {
                   // EditorGUILayout.PropertyField(serializedObject.FindProperty("airMaxSpeed"), new GUIContent("Air Max Speed", "Maximum Speed to move while on the air"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("airSmoothness"), new GUIContent("Air Smoothness", "Lerp between air stand and moving forward"));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("JumpPress"), new GUIContent("Jump Press", "If There's Jump Height value this will controll will keel adding more Jump if the JumpInput is pressed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AirForwardMultiplier"), new GUIContent("Jump Forward", "Adds More Height to the Jump. Check the JumpBehaviour on the Animator Controller"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("JumpHeightMultiplier"), new GUIContent("Jump Height", "Adds More Height to the Jump. Check the JumpBehaviour on the Animator Controller"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("CanDoubleJump"), new GUIContent("Double Jump", "Can the Animal make a double Jump"));

                EditorGUILayout.EndVertical();
            }
            //────────────────────────────────── Advanced ──────────────────────────────────
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            M.EditorAdvanced = EditorGUILayout.Foldout(M.EditorAdvanced, "Advanced");
            EditorGUI.indentLevel--;

            if (M.EditorAdvanced)
            {
                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);

                EditorGUILayout.PropertyField(animalTypeID, new GUIContent("Animal Type ID", "Enable the Additive Pose Animation to offset the Bones"));
                EditorGUILayout.PropertyField(GotoSleep, new GUIContent("Go to Sleep", "Number of Idles before going to sleep (AFK)"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("useShift"), new GUIContent("Use Sprint", "Allows to use the Shift/Sprint Key"));
                EditorGUILayout.PropertyField(SnapToGround, new GUIContent("Snap to ground", "Smoothness to Snap to terrain"));
                EditorGUILayout.PropertyField(AlingToGround, new GUIContent("Align to ground", "Smoothness to aling to terrain"));

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
                EditorGUILayout.PropertyField(maxAngleSlope, new GUIContent("Max Angle Slope", "Max Angle that the animal can walk"));
                EditorGUILayout.PropertyField(SlowSlopes, new GUIContent("Slow Slopes", "if the animal is going uphill: Slow it down"));
                EditorGUILayout.EndVertical();

               

                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
                EditorGUILayout.PropertyField(FallRayDistance, new GUIContent("Front Fall Ray", "Multiplier to set the Fall Ray in front of the animal"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BackFallRayDistance"), new GUIContent("Back Fall Ray", "Multiplier to set the Fall Ray back of the animal (While going backwards)"));
                EditorGUILayout.PropertyField(FallRayMultiplier, new GUIContent("Fall Ray Multiplier", "Multiplier for the Fall Ray"));
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PivotsRayInterval"), new GUIContent("Alignment Ray Cast", "Cast the Aligment Rays every X frames"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FallRayInterval"), new GUIContent("Fall Ray Cast", "Cast the Check for Fall Ray Every X frames"));

                if (M.canSwim)
                {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("WaterRayInterval"), new GUIContent("Water Ray Cast", "Cast the Check for Water Ray every X frames"));
                }
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginHorizontal(MalbersEditor.FlatBox);
                EditorGUILayout.LabelField(new GUIContent("Locomotion Speed", "This are the values for the Animator Locomotion Blend Tree when the velocity is changed"), GUILayout.MaxWidth(120));
                M.movementS1 = EditorGUILayout.FloatField(M.movementS1, GUILayout.MinWidth(28));
                M.movementS2 = EditorGUILayout.FloatField(M.movementS2, GUILayout.MinWidth(28));
                M.movementS3 = EditorGUILayout.FloatField(M.movementS3, GUILayout.MinWidth(28));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("SmoothVertical"), new GUIContent("Smooth Vertical", "Smooth transitions when Vertical axis changes (Set it to false for Keyboard, true for Gamepads/Touch Controllers)"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("TurnMultiplier"), new GUIContent("Turn Multiplier", "When Using Directions or (CameraBasedInput) this will highly increase the rotation of the turn animations"));

                EditorGUILayout.PropertyField(upDownSmoothness, new GUIContent("Y Axis Smoothness", "Smoothness of the UPDOWN axis. when pressing 'Jump' to go UP or 'Down' to go Down"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IgnoreYDir"), new GUIContent("Ignore (Y)", "When Using Directions or (CameraBasedInput) the Y value of the Direction Vector will be ignored"));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("animatorSpeed"), new GUIContent("Animator Speed", "The global animator speed on the animal"));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            var EditorAnimParams = serializedObject.FindProperty("EditorAnimatorParameters");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            EditorAnimParams.boolValue = EditorGUILayout.Foldout(EditorAnimParams.boolValue, "Animator Parameters");
            EditorGUI.indentLevel--;

            if (EditorAnimParams.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Vertical"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Horizontal"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_UpDown"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Stand"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Jump"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Fly"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Fall"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Attack1"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Attack2"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Stunned"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Damaged"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Shift"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Death"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Dodge"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Underwater"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Swim"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Action"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_IDAction"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_IDFloat"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_IDInt"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Slope"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Type"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SpeedMultiplier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_StateTime"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Stance"));
            }

            EditorGUILayout.EndVertical();

            //──────────────────────────────────────────────────── Events ────────────────────────────────────────────────────

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            M.EditorEvents = EditorGUILayout.Foldout(M.EditorEvents, "Events");
            EditorGUI.indentLevel--;

            if (M.EditorEvents)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnJump"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAttack"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnGetDamaged"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnDeathE"), new GUIContent("On Death"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAction"));
                if (M.canFly) EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFly"));
                if (M.canSwim) EditorGUILayout.PropertyField(serializedObject.FindProperty("OnSwim"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStanceChange"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAnimationChange"));
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(debug, new GUIContent("Debug"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Animal Values Changed");
                EditorUtility.SetDirty(target);
            }
        }

        private bool CalculateHeight()
        {
            M.SetPivots();

            if (!M.Pivot_Hip && !M.Pivot_Chest) return false;

            Pivots pivot = M.Pivot_Hip ? M.Pivot_Hip : M.Pivot_Chest;

            Ray newHeight = new Ray()
            {
                origin = pivot.transform.position,
                direction = -Vector3.up * 5
            };

            RaycastHit hit;
            if (Physics.Raycast(newHeight, out hit, pivot.multiplier * M.ScaleFactor, M.GroundLayer))
            {
                M.height = hit.distance;
                serializedObject.ApplyModifiedProperties();
            }
            return false;
        }

        string position = "position";
        string AnimatorSpeed = "animator";
        string SmoothTS = "lerpPosition";
        string SmoothAS = "lerpAnimator";
        string turnSpeed = "rotation";
        string SmoothTurnSpeed = "lerpRotation";

        protected void DrawSpeed(SerializedProperty speed, string name)
        {
            float with = 48;
            EditorGUILayout.BeginVertical(MalbersEditor.FlatBox);
            EditorGUILayout.LabelField(name + " Speed", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(speed.FindPropertyRelative(position), new GUIContent("Position", "Additional " + name + " Speed added to the position"), GUILayout.MinWidth(with));
            EditorGUIUtility.labelWidth = 18;
            EditorGUILayout.PropertyField(speed.FindPropertyRelative(SmoothTS), new GUIContent("L", "Position " + name + " Lerp interpolation, higher value more Responsiveness"), GUILayout.MaxWidth(with));
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(speed.FindPropertyRelative(AnimatorSpeed), new GUIContent("Animator", "Animator " + name + " Speed"), GUILayout.MinWidth(with));
            EditorGUIUtility.labelWidth = 18;
            EditorGUILayout.PropertyField(speed.FindPropertyRelative(SmoothAS), new GUIContent("L", "Animator " + name + " Lerp interpolation, higher value more Responsiveness"), GUILayout.MaxWidth(with));
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(speed.FindPropertyRelative(turnSpeed), new GUIContent("Turn", "Aditional " + name + " Turn Speed"), GUILayout.MinWidth(with));
            EditorGUIUtility.labelWidth = 18;
            EditorGUILayout.PropertyField(speed.FindPropertyRelative(SmoothTurnSpeed), new GUIContent("L", "Rotation " + name + " Lerp interpolation, higher value more Responsiveness"), GUILayout.MaxWidth(with));
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}