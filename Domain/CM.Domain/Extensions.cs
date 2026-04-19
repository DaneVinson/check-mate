namespace CM.Domain;

/// <summary>
/// Extension methods for <c>CM.Domain</c> types.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers all default command and query handlers as scoped services.
    /// </summary>
    /// <param name="services">The service collection to register handlers into.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddDefaultHandlers(this IServiceCollection services)
    {
        // Checkable command handlers
        services.AddScoped<ICommandHandler<CheckCheckable>, CheckCheckableHandler>();
        services.AddScoped<ICommandHandler<CreateCheckable>, CreateCheckableHandler>();
        services.AddScoped<ICommandHandler<DeleteCheckable>, DeleteCheckableHandler>();
        services.AddScoped<ICommandHandler<UncheckCheckable>, UncheckCheckableHandler>();
        services.AddScoped<ICommandHandler<UpdateCheckable>, UpdateCheckableHandler>();

        // Checkable query handlers
        services.AddScoped<IQueryHandler<GetCheckablesByCheckList, IReadOnlyList<CheckableDto>>, GetCheckablesByCheckListHandler>();

        // CheckList command handlers
        services.AddScoped<ICommandHandler<CreateCheckList>, CreateCheckListHandler>();
        services.AddScoped<ICommandHandler<DeleteCheckList>, DeleteCheckListHandler>();
        services.AddScoped<ICommandHandler<UpdateCheckList>, UpdateCheckListHandler>();

        // CheckList query handlers
        services.AddScoped<IQueryHandler<GetCheckList, CheckListDto?>, GetCheckListHandler>();
        services.AddScoped<IQueryHandler<GetCheckListsByUser, IReadOnlyList<CheckListDto>>, GetCheckListsByUserHandler>();

        // User command handlers
        services.AddScoped<ICommandHandler<CreateUser>, CreateUserHandler>();
        services.AddScoped<ICommandHandler<UpdateUser>, UpdateUserHandler>();

        // User query handlers
        services.AddScoped<IQueryHandler<GetUser, UserDto?>, GetUserHandler>();
        services.AddScoped<IQueryHandler<GetUserEmailExists, bool>, GetUserEmailExistsHandler>();

        return services;
    }
}
