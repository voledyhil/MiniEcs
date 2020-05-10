namespace MiniEcs.Core
{
    /// <summary>
    /// Interface for all components
    /// </summary>
    public interface IEcsComponent
    {
        /// <summary>
        /// Component Type
        /// </summary>
        byte Index { get; }
    }
}