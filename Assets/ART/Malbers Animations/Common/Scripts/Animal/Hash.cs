using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// Faster way to work with Animator Controller Parameters
    /// </summary>
    public static class Hash 
    {
        public static readonly int Vertical = Animator.StringToHash("Vertical");
        public static readonly int Horizontal = Animator.StringToHash("Horizontal");
        public static readonly int UpDown = Animator.StringToHash("UpDown");

        public static readonly int Stand = Animator.StringToHash("Stand");
        public static readonly int Grounded = Animator.StringToHash("Grounded");

        public static readonly int _Jump = Animator.StringToHash("_Jump");

        public static readonly int Dodge = Animator.StringToHash("Dodge");
        public static readonly int Fall = Animator.StringToHash("Fall");
        public static readonly int Type = Animator.StringToHash("Type");


        public static readonly int Slope = Animator.StringToHash("Slope");

        public static readonly int Shift = Animator.StringToHash("Shift");

        public static readonly int Fly = Animator.StringToHash("Fly");

        public static readonly int Attack1 = Animator.StringToHash("Attack1");
        public static readonly int Attack2 = Animator.StringToHash("Attack2");

        public static readonly int Death = Animator.StringToHash("Death");

        public static readonly int Damaged = Animator.StringToHash("Damaged");
        public static readonly int Stunned = Animator.StringToHash("Stunned");

        public static readonly int IDInt = Animator.StringToHash("IDInt");
        public static readonly int IDFloat = Animator.StringToHash("IDFloat");

        public static readonly int Swim = Animator.StringToHash("Swim");
        public static readonly int Underwater = Animator.StringToHash("Underwater");

        public static readonly int IDAction = Animator.StringToHash("IDAction");
        public static readonly int Action = Animator.StringToHash("Action");


        public static readonly int Null = Animator.StringToHash("Null");
        public static readonly int Empty = Animator.StringToHash("Empty");


        public static readonly int State = Animator.StringToHash("State");
        public static readonly int Stance = Animator.StringToHash("Stance");
        public static readonly int Mode = Animator.StringToHash("Mode");
        public static readonly int StateTime = Animator.StringToHash("StateTime");



        //---------------------------HAP-----------------------------------------


        public readonly static int IKLeftFoot = Animator.StringToHash("IKLeftFoot");
        public readonly static int IKRightFoot = Animator.StringToHash("IKRightFoot");

        public readonly static int Mount = Animator.StringToHash("Mount");
        public readonly static int MountSide = Animator.StringToHash("MountSide");

        public readonly static int Tag_Mounting= Animator.StringToHash("Mounting");
        public readonly static int Tag_Unmounting = Animator.StringToHash("Unmounting");

    }

    /// <summary>
    /// Store the Common Tags of the Animator
    /// </summary>
    public static class AnimTag
    {
        public readonly static int Locomotion = Animator.StringToHash("Locomotion");
        public readonly static int Idle = Animator.StringToHash("Idle");
        public readonly static int Recover = Animator.StringToHash("Recover");
        public readonly static int Sleep = Animator.StringToHash("Sleep");
        public readonly static int Attack = Animator.StringToHash("Attack");
        public readonly static int Attack2 = Animator.StringToHash("Attack2");
        public readonly static int JumpEnd = Animator.StringToHash("JumpEnd");
        public readonly static int JumpStart = Animator.StringToHash("JumpStart");
        public readonly static int Jump = Animator.StringToHash("Jump");
        public readonly static int SwimJump = Animator.StringToHash("SwimJump");
        public readonly static int NoAlign = Animator.StringToHash("NoAlign");
        public readonly static int Action = Animator.StringToHash("Action");
        public readonly static int Swim = Animator.StringToHash("Swim");
        public readonly static int Underwater = Animator.StringToHash("Underwater");
        public readonly static int Fly = Animator.StringToHash("Fly");
        public readonly static int Dodge = Animator.StringToHash("Dodge");
        public readonly static int Fall = Animator.StringToHash("Fall");

        public readonly static int Mounting = Animator.StringToHash("Mounting");
        public readonly static int Unmounting = Animator.StringToHash("Unmounting");

    }


    /// <summary>
    /// Current Ability the Animal is doing
    /// </summary>
    public static class States
    {
        public readonly static int Unknown = -1;
        public readonly static int Idle = 0;

        public readonly static int Locomotion = 1;
        public readonly static int Jump = 2;
        public readonly static int Climb = 3;
        public readonly static int Fall = 4;
        public readonly static int Recover = 5;

        public readonly static int Action = 6;
        public readonly static int Swim = 7;
        public readonly static int UnderWater = 8;
        public readonly static int Fly = 10;
        public readonly static int Stun = 11;
        public readonly static int Death = 12;
        public readonly static int Rebirth = 13;
    }

    /// <summary>
    /// Actions can be used with the state and the stances
    /// </summary>
    public static class Mode
    {
        public readonly static int None = 0;
        public readonly static int Shift = 1;
        public readonly static int Attack1 = 2;
        public readonly static int Attack2 = 3;
        public readonly static int Damaged = 4;
        public readonly static int Dodge = 5;
        public readonly static int Block = 6;
        public readonly static int Parry = 7;

        public readonly static int isAttacking1 = 11;
        public readonly static int isAttacking2 = 12;
        public readonly static int isTakingDamage = 13;
        public readonly static int isDodging = 14;
        public readonly static int isDefending = 15;
    }

    /// <summary>
    /// Current Stance the animal is on
    /// </summary>
    public static class Stance
    {
        public readonly static int Default = 0;
        public readonly static int Sneak = 1;
        public readonly static int Combat = 2;
        public readonly static int Injured = 3;
        public readonly static int Strafe = 4;
    }
}