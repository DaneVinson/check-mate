namespace CM.Domain.Checkables.Commands;

/// <summary>
/// Handles the <see cref="CreateCheckable"/> command.
/// </summary>
public sealed class CreateCheckableHandler : ICommandHandler<CreateCheckable>
{
    private readonly ICheckableDataService _dataService;
    private readonly IMessenger<IEvent> _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCheckableHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for checkable operations.</param>
    /// <param name="messenger">The messenger used to send events.</param>
    public CreateCheckableHandler(ICheckableDataService dataService, IMessenger<IEvent> messenger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <inheritdoc />
    public async Task HandleAsync(CreateCheckable command, CancellationToken cancellationToken = default)
    {
        var validation = command.Validate();
        if (validation is not null)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, validation.Message), cancellationToken);
            return;
        }

        var result = await _dataService.UpsertAsync(new Checkable(false, command.CheckListId, DateTimeOffset.UtcNow, command.Description, Guid.CreateVersion7(), command.UserId));
        if (result.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, result.Error.Message), cancellationToken);
            return;
        }

        await _messenger.SendAsync(new CheckableCreated(new CheckableDto(result.Value), command.Id), cancellationToken);
    }
}
