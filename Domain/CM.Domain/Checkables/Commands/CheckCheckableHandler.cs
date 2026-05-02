namespace CM.Domain.Checkables.Commands;

/// <summary>
/// Handles the <see cref="CheckCheckable"/> command.
/// </summary>
public sealed class CheckCheckableHandler : ICommandHandler<CheckCheckable>
{
    private readonly ICheckableDataService _dataService;
    private readonly IMessenger<IEvent> _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckCheckableHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for checkable operations.</param>
    /// <param name="messenger">The messenger used to send events.</param>
    public CheckCheckableHandler(ICheckableDataService dataService, IMessenger<IEvent> messenger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <inheritdoc />
    public async Task HandleAsync(CheckCheckable command, CancellationToken cancellationToken = default)
    {
        var getResult = await _dataService.GetByIdAsync(command.CheckableId);
        if (getResult.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, getResult.Error.Message), cancellationToken);
            return;
        }

        getResult.Value.Checked = true;

        var upsertResult = await _dataService.UpsertAsync(getResult.Value);
        if (upsertResult.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, upsertResult.Error.Message), cancellationToken);
            return;
        }

        await _messenger.SendAsync(new CheckableChecked(new CheckableDto(upsertResult.Value), command.Id), cancellationToken);
    }

    /// <inheritdoc />
    Task ICommandHandler.HandleAsync(object command, CancellationToken cancellationToken)
        => HandleAsync((CheckCheckable)command, cancellationToken);
}
