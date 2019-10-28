namespace MalbersAnimations
{
    interface IWayPoint
    {
        float StoppingDistance { get; }

        /// <summary>
        /// Next Transform Target to go to
        /// </summary>
        UnityEngine.Transform NextTarget { get; }

        /// <summary>
        /// Wait time to go to the next Waypoint
        /// </summary>
        float WaitTime { get; }

        /// <summary>
        /// Which type of environment is the waypoint
        /// </summary>
        WayPointType PointType { get; }
    }
}