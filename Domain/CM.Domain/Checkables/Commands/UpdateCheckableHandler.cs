namespace CM.Domain.Checkables.Commands;

/// <summary>
/// Handles the <see cref="UpdateCheckable"/> command.
/// </summary>
public sealed class UpdateCheckableHandler : ICommandHandler<UpdateCheckable>
{
    private readonly ICheckableDataService _dataService;
    private readonly IMessenger<IEvent> _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCheckableHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for checkable operations.</param>
    /// <param name="messenger">The messenger used to send events.</param>
    public UpdateCheckableHandler(ICheckableDataService dataService, IMessenger<IEvent> messenger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <inheritdoc />
    public async Task HandleAsync(UpdateCheckable command, CancellationToken cancellationToken = default)
    {
        var validation = command.Validate();
        if (validation is not null)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, validation.Message), cancellationToken);
            return;
        }

        var getResult = await _dataService.GetByIdAsync(command.CheckableId);
        if (getResult.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, getResult.Error.Message), cancellationToken);
            return;
        }

        getResult.Value.Description = command.Description;

        var upsertResult = await _dataService.UpsertAsync(getResult.Value);
        if (upsertResult.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, upsertResult.Error.Message), cancellationToken);
            return;
        }

        await _messenger.SendAsync(new CheckableUpdated(new CheckableDto(upsertResult.Value), command.Id), cancellationToken);
    }

    /// <inheritdoc />
    Task ICommandHandler.HandleAsync(object command, CancellationToken cancellationToken)
        => HandleAsync((UpdateCheckable)command, cancellationToken);
}
