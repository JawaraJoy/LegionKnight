
namespace Rush
{
    public interface IDefenseMod : IHasDefense
    {
        void SetDefense(int defense);
        void AddDefense(int defense);
        void MultiplyDefense(int defense);
    }
    public interface IHasDefense
    {
        int Defense { get; }
    }
}
