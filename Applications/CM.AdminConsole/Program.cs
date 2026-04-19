var services = new ServiceCollection();
services.AddSingleton<ILiteDatabase>(_ => new LiteDatabase("CheckMate.db"));
services.AddDefaultHandlers();
services.AddBogusDataServices();
// services.AddLiteDbDataServices();

var provider = services.BuildServiceProvider();

var checkListService = provider.GetRequiredService<ICheckListDataService>();
var userId = DataSeed.GetUsers()[0].Id;
var result = await checkListService.GetByUserAsync(userId);

foreach (var checkList in result.Value)
{
    Console.WriteLine(checkList.Name);
}


