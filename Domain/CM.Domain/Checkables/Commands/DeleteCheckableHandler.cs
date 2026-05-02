namespace CM.Domain.Checkables.Commands;

/// <summary>
/// Handles the <see cref="DeleteCheckable"/> command.
/// </summary>
public sealed class DeleteCheckableHandler : ICommandHandler<DeleteCheckable>
{
    private readonly ICheckableDataService _dataService;
    private readonly IMessenger<IEvent> _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCheckableHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for checkable operations.</param>
    /// <param name="messenger">The messenger used to send events.</param>
    public DeleteCheckableHandler(ICheckableDataService dataService, IMessenger<IEvent> messenger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <inheritdoc />
    public async Task HandleAsync(DeleteCheckable command, CancellationToken cancellationToken = default)
    {
        var result = await _dataService.DeleteAsync(command.CheckableId);
        if (result.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, result.Error.Message), cancellationToken);
            return;
        }

        await _messenger.SendAsync(new CheckableDeleted(command.CheckableId, command.Id), cancellationToken);
    }

    /// <inheritdoc />
    Task ICommandHandler.HandleAsync(object command, CancellationToken cancellationToken)
        => HandleAsync((DeleteCheckable)command, cancellationToken);
}
