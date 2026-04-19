namespace CM.Domain.Checkables.Queries;

/// <summary>
/// Handles the <see cref="GetCheckablesByCheckList"/> query.
/// </summary>
public sealed class GetCheckablesByCheckListHandler : IQueryHandler<GetCheckablesByCheckList, IReadOnlyList<CheckableDto>>
{
    private readonly ICheckableDataService _dataService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCheckablesByCheckListHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for checkable operations.</param>
    public GetCheckablesByCheckListHandler(ICheckableDataService dataService)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<CheckableDto>>> HandleAsync(GetCheckablesByCheckList query, CancellationToken cancellationToken = default)
    {
        var result = await _dataService.GetByCheckListAsync(query.CheckListId);
        if (result.IsError)
        {
            return result.Error;
        }

        return Result<IReadOnlyList<CheckableDto>>.Success(result.Value.Select(c => new CheckableDto(c)).ToList());
    }
}
