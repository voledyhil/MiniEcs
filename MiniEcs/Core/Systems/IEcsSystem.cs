namespace MiniEcs.Core.Systems
{
    public interface IEcsSystem
    {
        void Update(float deltaTime, EcsWorld world);
    }
}