public interface IPooleable
{
    bool IsActive { get; }
    void Activate();
    void DeActivate();
}