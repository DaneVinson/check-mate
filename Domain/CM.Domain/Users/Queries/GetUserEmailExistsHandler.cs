namespace CM.Domain.Users.Queries;

/// <summary>
/// Handles the <see cref="GetUserEmailExists"/> query.
/// </summary>
public sealed class GetUserEmailExistsHandler : IQueryHandler<GetUserEmailExists, bool>
{
    private readonly IUserDataService _dataService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserEmailExistsHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for user operations.</param>
    public GetUserEmailExistsHandler(IUserDataService dataService)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
    }

    /// <inheritdoc />
    public async Task<Result<bool>> HandleAsync(GetUserEmailExists query, CancellationToken cancellationToken = default)
    {
        var result = await _dataService.ExistsByEmailAsync(query.Email);
        if (result.IsError)
        {
            return result.Error;
        }

        return result.Value;
    }
}
