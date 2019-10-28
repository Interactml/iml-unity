namespace MalbersAnimations
{
    public enum InputType
    {
        Input, Key
    }

    public enum WayPointType
    {
        Ground, Air, Water
    }

    public enum InputButton
    {
        Press = 0, Down = 1, Up = 2, LongPress = 3, DoubleTap =4
    }

    public enum StateTransition
    {
        First = 0,
        Last = 1
    }

    public enum TypeMessage
    {
        Bool = 0,
        Int = 1,
        Float = 2,
        String = 3,
        Void = 4,
        IntVar = 5
    }

    //This is mainly use for the Rider Combat Animator nothing more
    public enum WeaponType 
    {
        None = 0,
        Melee = 1,
        Bow = 2,
        Spear = 3,
        Pistol = 4,
        Rifle = 5
    }

    public enum WeaponHolder
    {
        None = 0,
        Left = 1,
        Right = 2,
        Back = 3
    }

    public enum AxisDirection
    {
        None,
        Right,
        Left,
        Up,
        Down,
        Forward,
        Backward
    }

    //Weapons Actions ... positive values are for Attacks
    public enum WeaponActions
    {
        None = 0,
        DrawFromRight = -1,
        DrawFromLeft = -2,
        StoreToRight = -3,
        StoreToLeft = -7,
        Idle = -4,
        AimRight = -5,
        AimLeft = -6,
        ReloadRight  = -8,
        ReloadLeft = -9,
        Hold = -10,         //Used for the Bow when holding the String
        Equip = -100,
        Unequip = -101,

        //Positive Values are the Attack IDs
        
        //Melee
        Atk_RSide_RHand_Forward = 1,                //Attack Right Side with Right Hand Forward
        Atk_RSide_RHand_Backward = 2,               //Attack Right Side with Right Hand Backward

        Atk_LSide_RHand_Forward = 3,                //Attack Left Side with Right Hand Forwards
        Atk_LSide_RHand_Backward = 4,               //Attack Left Side with Right Hand Backward

        Atk_RSide_LHand_Forward = 5,                //Attack Right Side with Left Hand Forward
        Atk_RSide_LHand_Backward = 6,               //Attack Right Side with Left Hand Backward

        Atk_LSide_LHand_Forward = 7,                //Attack Left Side with Left Hand Forward
        Atk_LSide_LHand_Backward = 8,               //Attack Left Side with Left Hand Backward

        //Ranged
        Fire_Proyectile = 9,
    }
}