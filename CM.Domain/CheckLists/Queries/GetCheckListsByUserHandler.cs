namespace CM.Domain.CheckLists.Queries;

/// <summary>
/// Handles the <see cref="GetCheckListsByUser"/> query.
/// </summary>
public sealed class GetCheckListsByUserHandler : IQueryHandler<GetCheckListsByUser, IReadOnlyList<CheckListDto>>
{
    private readonly ICheckListDataService _dataService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCheckListsByUserHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for check list operations.</param>
    public GetCheckListsByUserHandler(ICheckListDataService dataService)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<CheckListDto>>> HandleAsync(GetCheckListsByUser query, CancellationToken cancellationToken = default)
    {
        var result = await _dataService.GetByUserAsync(query.UserId);
        if (result.IsError)
        {
            return result.Error;
        }

        return Result<IReadOnlyList<CheckListDto>>.Success(result.Value.Select(cl => new CheckListDto(cl)).ToList());
    }
}
