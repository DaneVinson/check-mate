namespace CM.Domain.CheckLists.Queries;

/// <summary>
/// Handles the <see cref="GetCheckList"/> query.
/// </summary>
public sealed class GetCheckListHandler : IQueryHandler<GetCheckList, CheckListDto?>
{
    private readonly ICheckListDataService _dataService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCheckListHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for check list operations.</param>
    public GetCheckListHandler(ICheckListDataService dataService)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
    }

    /// <inheritdoc />
    public async Task<Result<CheckListDto?>> HandleAsync(GetCheckList query, CancellationToken cancellationToken = default)
    {
        var result = await _dataService.GetByIdAsync(query.CheckListId);
        if (result.IsError)
        {
            return result.Error;
        }

        return new CheckListDto(result.Value);
    }
}
