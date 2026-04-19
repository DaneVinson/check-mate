namespace CM.Domain.Users.Commands;

/// <summary>
/// Handles the <see cref="UpdateUser"/> command.
/// </summary>
public sealed class UpdateUserHandler : ICommandHandler<UpdateUser>
{
    private readonly IUserDataService _dataService;
    private readonly IMessenger<IEvent> _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for user operations.</param>
    /// <param name="messenger">The messenger used to send events.</param>
    public UpdateUserHandler(IUserDataService dataService, IMessenger<IEvent> messenger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <inheritdoc />
    public async Task HandleAsync(UpdateUser command, CancellationToken cancellationToken = default)
    {
        var validation = command.Validate();
        if (validation is not null)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, validation.Message), cancellationToken);
            return;
        }

        var getResult = await _dataService.GetByIdAsync(command.UserId);
        if (getResult.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, getResult.Error.Message), cancellationToken);
            return;
        }

        getResult.Value.Name = command.Name;

        var upsertResult = await _dataService.UpsertAsync(getResult.Value);
        if (upsertResult.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, upsertResult.Error.Message), cancellationToken);
            return;
        }

        await _messenger.SendAsync(new UserUpdated(command.Id, new UserDto(upsertResult.Value)), cancellationToken);
    }
}
