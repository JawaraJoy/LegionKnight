
namespace Rush
{
    public interface IAttackMod : IHasAttack
    {
        void SetAttack(float value);
        void AddAttack(float value);
        void Multiply(float value);
    }
    public interface IHasAttack
    {
        int Attack { get; }
    }
}
