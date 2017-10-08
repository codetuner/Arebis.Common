namespace Arebis.Data.Entity
{
    /// <summary>
    /// Marks an entity as containing document data.
    /// </summary>
    public interface IHybridDocument
    {
        /// <summary>
        /// The document data.
        /// </summary>
        string DocumentData { get; set; }
    }
}
