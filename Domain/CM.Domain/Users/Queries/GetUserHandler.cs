namespace CM.Domain.Users.Queries;

/// <summary>
/// Handles the <see cref="GetUser"/> query.
/// </summary>
public sealed class GetUserHandler : IQueryHandler<GetUser, UserDto?>
{
    private readonly IUserDataService _dataService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for user operations.</param>
    public GetUserHandler(IUserDataService dataService)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
    }

    /// <inheritdoc />
    public async Task<Result<UserDto?>> HandleAsync(GetUser query, CancellationToken cancellationToken = default)
    {
        var result = await _dataService.GetByIdAsync(query.UserId);
        if (result.IsError)
        {
            return result.Error;
        }

        return new UserDto(result.Value);
    }
}
