namespace CM.Domain.Checkables;

/// <summary>
/// Defines data access operations for <see cref="Checkable"/> entities.
/// </summary>
public interface ICheckableDataService
{
    /// <summary>
    /// Deletes a <see cref="Checkable"/> by its unique identifier.
    /// </summary>
    /// <param name="checkableId">The identifier of the checkable item to delete.</param>
    /// <returns>A result indicating whether the deletion was successful.</returns>
    Task<Result<bool>> DeleteAsync(Guid checkableId);

    /// <summary>
    /// Retrieves all <see cref="Checkable"/> items belonging to a check list.
    /// </summary>
    /// <param name="checkListId">The identifier of the check list whose items to retrieve.</param>
    /// <returns>A result containing the matching checkable items.</returns>
    Task<Result<IReadOnlyList<Checkable>>> GetByCheckListAsync(Guid checkListId);

    /// <summary>
    /// Retrieves a <see cref="Checkable"/> by its unique identifier.
    /// </summary>
    /// <param name="checkableId">The identifier of the checkable item to retrieve.</param>
    /// <returns>A result containing the matching checkable item.</returns>
    Task<Result<Checkable>> GetByIdAsync(Guid checkableId);

    /// <summary>
    /// Inserts or updates a <see cref="Checkable"/>.
    /// </summary>
    /// <param name="checkable">The checkable item to insert or update.</param>
    /// <returns>A result containing the upserted checkable item.</returns>
    Task<Result<Checkable>> UpsertAsync(Checkable checkable);
}
