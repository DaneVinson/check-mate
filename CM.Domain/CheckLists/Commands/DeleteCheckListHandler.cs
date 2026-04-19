namespace CM.Domain.CheckLists.Commands;

/// <summary>
/// Handles the <see cref="DeleteCheckList"/> command.
/// </summary>
public sealed class DeleteCheckListHandler : ICommandHandler<DeleteCheckList>
{
    private readonly ICheckListDataService _dataService;
    private readonly IMessenger<IEvent> _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCheckListHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for check list operations.</param>
    /// <param name="messenger">The messenger used to send events.</param>
    public DeleteCheckListHandler(ICheckListDataService dataService, IMessenger<IEvent> messenger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <inheritdoc />
    public async Task HandleAsync(DeleteCheckList command, CancellationToken cancellationToken = default)
    {
        var result = await _dataService.DeleteAsync(command.CheckListId);
        if (result.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, result.Error.Message), cancellationToken);
            return;
        }

        await _messenger.SendAsync(new CheckListDeleted(command.CheckListId, command.Id), cancellationToken);
    }
}
