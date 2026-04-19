namespace CM.Domain.CheckLists.Commands;

/// <summary>
/// Handles the <see cref="CreateCheckList"/> command.
/// </summary>
public sealed class CreateCheckListHandler : ICommandHandler<CreateCheckList>
{
    private readonly ICheckListDataService _dataService;
    private readonly IMessenger<IEvent> _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCheckListHandler"/> class.
    /// </summary>
    /// <param name="dataService">The data service for check list operations.</param>
    /// <param name="messenger">The messenger used to send events.</param>
    public CreateCheckListHandler(ICheckListDataService dataService, IMessenger<IEvent> messenger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <inheritdoc />
    public async Task HandleAsync(CreateCheckList command, CancellationToken cancellationToken = default)
    {
        var validation = command.Validate();
        if (validation is not null)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, validation.Message), cancellationToken);
            return;
        }

        var result = await _dataService.UpsertAsync(new CheckList(DateTimeOffset.UtcNow, Guid.CreateVersion7(), command.Name, command.UserId));
        if (result.IsError)
        {
            await _messenger.SendAsync(new CommandFailed(command.Id, result.Error.Message), cancellationToken);
            return;
        }

        await _messenger.SendAsync(new CheckListCreated(new CheckListDto(result.Value), command.Id), cancellationToken);
    }
}
