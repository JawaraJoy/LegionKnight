namespace Rush
{
    [System.Serializable]
    public struct AttackResult
    {
        public AttackerField AttackerField;
        public bool IsCritical;

        public AttackResult(AttackerField attackerField, bool isCritical)
        {
            AttackerField = attackerField;
            IsCritical = isCritical;
        }
    }
}