namespace CM.Domain.CheckLists;

/// <summary>
/// Defines data access operations for <see cref="CheckList"/> entities.
/// </summary>
public interface ICheckListDataService
{
    /// <summary>
    /// Deletes a <see cref="CheckList"/> by its unique identifier.
    /// </summary>
    /// <param name="checkListId">The identifier of the check list to delete.</param>
    /// <returns>A result indicating whether the deletion was successful.</returns>
    Task<Result<bool>> DeleteAsync(Guid checkListId);

    /// <summary>
    /// Retrieves a <see cref="CheckList"/> by its unique identifier.
    /// </summary>
    /// <param name="checkListId">The identifier of the check list to retrieve.</param>
    /// <returns>A result containing the matching check list.</returns>
    Task<Result<CheckList>> GetByIdAsync(Guid checkListId);

    /// <summary>
    /// Retrieves all <see cref="CheckList"/> items belonging to a user.
    /// </summary>
    /// <param name="userId">The identifier of the user whose check lists to retrieve.</param>
    /// <returns>A result containing the matching check lists.</returns>
    Task<Result<IReadOnlyList<CheckList>>> GetByUserAsync(Guid userId);

    /// <summary>
    /// Inserts or updates a <see cref="CheckList"/>.
    /// </summary>
    /// <param name="checkList">The check list to insert or update.</param>
    /// <returns>A result containing the upserted check list.</returns>
    Task<Result<CheckList>> UpsertAsync(CheckList checkList);
}
