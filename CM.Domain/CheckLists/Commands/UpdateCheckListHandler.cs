namespace CM.Domain.CheckLists.Commands;

/// <summary>
/// Handles the <see cref="UpdateCheckList"/> command.
/// </summary>
public sealed class UpdateCheckListHandler : ICommandHandler<UpdateCheckList>
{
    private readonly ICheckListDataService _dataService;
    private readonly IMessenger<IEvent> _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCheckListHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for check list operations.</param>
    /// <param name="messenger">The messenger used to send events.</param>
    public UpdateCheckListHandler(ICheckListDataService dataService, IMessenger<IEvent> messenger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <inheritdoc />
    public async Task HandleAsync(UpdateCheckList command, CancellationToken cancellationToken = default)
    {
        var validation = command.Validate();
        if (validation is not null)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, validation.Message), cancellationToken);
            return;
        }

        var getResult = await _dataService.GetByIdAsync(command.CheckListId);
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

        await _messenger.SendAsync(new CheckListUpdated(new CheckListDto(upsertResult.Value), command.Id), cancellationToken);
    }
}
