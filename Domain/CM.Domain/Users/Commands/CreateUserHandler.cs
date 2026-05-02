namespace CM.Domain.Users.Commands;

/// <summary>
/// Handles the <see cref="CreateUser"/> command.
/// </summary>
public sealed class CreateUserHandler : ICommandHandler<CreateUser>
{
    private readonly IUserDataService _dataService;
    private readonly IMessenger<IEvent> _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for user operations.</param>
    /// <param name="messenger">The messenger used to send events.</param>
    public CreateUserHandler(IUserDataService dataService, IMessenger<IEvent> messenger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <inheritdoc />
    public async Task HandleAsync(CreateUser command, CancellationToken cancellationToken = default)
    {
        var validation = command.Validate();
        if (validation is not null)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, validation.Message), cancellationToken);
            return;
        }

        var existsResult = await _dataService.ExistsByEmailAsync(command.Email);
        if (existsResult.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, existsResult.Error.Message), cancellationToken);
            return;
        }

        if (existsResult.Value)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, "A user with this email address already exists."), cancellationToken);
            return;
        }

        var result = await _dataService.UpsertAsync(new User(DateTimeOffset.UtcNow, command.Email, Guid.CreateVersion7(), command.Name));
        if (result.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, result.Error.Message), cancellationToken);
            return;
        }

        await _messenger.SendAsync(new UserCreated(command.Id, new UserDto(result.Value)), cancellationToken);
    }

    /// <inheritdoc />
    Task ICommandHandler.HandleAsync(object command, CancellationToken cancellationToken)
        => HandleAsync((CreateUser)command, cancellationToken);
}
