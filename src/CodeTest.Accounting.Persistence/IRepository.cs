namespace CodeTest.Accounting.Persistence
{
    /// <summary>
    /// Generic repository, useful for this example.
    /// In a Enterprise application, each Service can implement their own Data Access layer and methods.
    /// </summary>
    /// <typeparam name="TEntity">The Entity type to store or retrieve from the Repository.</typeparam>
    public interface IRepository<TEntity>
    {
        // note: Int32 type Ids are picked just for easiness to work with
        // normally, we should be using GUIDs or more complex data types
        TEntity Get(int id);

        void Set(int id, TEntity entity);
    }
}
