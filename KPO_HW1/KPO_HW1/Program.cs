using KPO_HW1;
using KPO_HW1.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<IVeterinaryClinic>(_ => new VeterinaryClinic(70));
services.AddSingleton<IInventoryNumberProvider, InMemoryNumberProvider>();
services.AddSingleton<IAnimalFactory, AnimalFactory>();
services.AddSingleton<IThingFactory, ThingFactory>();
services.AddSingleton<IZoo, Zoo>();
services.AddSingleton<App>();

services.AddSingleton<IAnimalCreator, AnimalCreator<Wolf>>();
services.AddSingleton<IAnimalCreator, AnimalCreator<Tiger>>();
services.AddSingleton<IAnimalCreator, HerbivoreAnimalCreator<Rabbit>>();
services.AddSingleton<IAnimalCreator, HerbivoreAnimalCreator<Monkey>>();

services.AddSingleton<IThingCreator, ThingCreator<Computer>>();
services.AddSingleton<IThingCreator, ThingCreator<Table>>();

using var sp = services.BuildServiceProvider();
sp.GetRequiredService<App>().Run();