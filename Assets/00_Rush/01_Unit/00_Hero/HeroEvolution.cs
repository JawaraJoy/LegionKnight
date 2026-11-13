namespace Rush
{
    public static class HeroEvolution
    {
        public const int MaxLevel = 30;
        public const int MaxStage = 12;
        public const int StarSlots = 3;

        public static int NextShardCost(int stage)
        {
            if (stage < 0 || stage >= MaxStage) return 0;
            return stage == 0 ? 10 : 20 * stage;
        }

        // Each pass upgrades the same three slots from left to right.
        public static int StarTier(int stage, int slot)
        {
            if (slot < 0 || slot >= StarSlots) return 0;
            stage = System.Math.Max(0, System.Math.Min(MaxStage, stage));
            return (stage + StarSlots - 1 - slot) / StarSlots;
        }
    }
}
