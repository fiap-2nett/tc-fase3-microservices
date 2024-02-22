namespace HelpDesk.Core.Domain.Abstractions
{
    public interface ISoftDeletableEntity
    {
        #region ISoftDeletableEntity Properties

        bool IsDeleted { get; }

        #endregion
    }
}
