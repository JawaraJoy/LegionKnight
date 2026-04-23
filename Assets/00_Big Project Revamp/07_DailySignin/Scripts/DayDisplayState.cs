namespace Rush
{
    public enum DayDisplayState
    {
        Claimed,    // already claimed
        Available,  // today, not yet claimed
        Locked,     // future day
        Complete    // past the cycle end, no loop
    }
}