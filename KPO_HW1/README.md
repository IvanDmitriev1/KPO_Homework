# Конструирование ПО | ДЗ 1 — «Зоопарк»

Консольное приложение для учёта животных и имущества зоопарка. Архитектура построена на ООП, SOLID и DI.

---

### Требования
- **.NET SDK 9.0.200+**

### Сборка и запуск
```bash
dotnet build
dotnet run --project KPO_HW1
```

### Тесты
```bash
dotnet test
```

---

## Что умеет приложение

- Добавлять животное (приём только после проверки **ветклиники**).
- Показывать всех животных (вид, имя, еда кг/сутки, здоровье %, «доброта» для травоядных).
- Показывать **кандидатов в контактный зоопарк**: травоядные.
- Считать **суммарное потребление**.
- Добавлять вещи (например, `Table`, `Computer`).
- Показывать вещи и полную **инвентарную опись** (животные и вещи с номерами).

UI сделан на **Spectre.Console** (меню, таблицы, валидация вводов), DI — на **Microsoft.Extensions.DependencyInjection**.

---

## Структура проекта

```
KPO_HW1.slnx
KPO_HW1/
  Abstractions/
    IAlive.cs
    IInventory.cs
    IHerbivore.cs
    IPredator.cs
    IVeterinaryClinic.cs
    IZoo.cs
    IInventoryNumberProvider.cs
    IAnimalCreator.cs / IAnimalFactory.cs
    IThingCreator.cs  / IThingFactory.cs
  Models/
    Animals/
      Animal.cs / AnimalCreateOptions.cs
      HerbivoreAnimal.cs / HerbivoreAnimalCreateOptions.cs
      Tiger.cs, Wolf.cs, Rabbit.cs, Monkey.cs
    Things/
      Thing.cs, Table.cs, Computer.cs
  Services/
    Zoo.cs
    VeterinaryClinic.cs
    InMemoryNumberProvider.cs
    AnimalCreator*.cs / AnimalFactory.cs
    HerbivoreAnimalCreator.cs
    ThingCreator*.cs  / ThingFactory.cs
  App.cs
  Program.cs
  KPO_HW1.csproj
KPO_HW1.Tests/
  *.cs
  KPO_HW1.Tests.csproj
```

## SOLID: где и зачем

**S — Single Responsibility**
- `Zoo` — учет домена (животные/вещи, приём).
- `VeterinaryClinic` — одна задача: проверка «здоров/не здоров» по порогу.
- `App` — интерфейс пользователя (меню, таблицы, ввод/валидация).
- `InMemoryNumberProvider` — генерация инвентарных номеров.
- `*Creator`/`*Factory` — изолированное создание экземпляров и ведение реестра доступных типов.

**O — Open/Closed**
- Чтобы добавить новый вид животного — достаточно:
  1) класс-наследник `Animal` или `HerbivoreAnimal`,
  2) соответствующий `AnimalCreator<T>`/`HerbivoreAnimalCreator<T>`,
  3) регистрация креатора в DI (`Program.cs`).
  Бизнес-логика (`Zoo`), UI (`App`) и фабрики не меняются.
- Аналогично для вещей через `Thing` + `ThingCreator<T>`.

**L — Liskov Substitution**
- Везде, где ожидается `Animal`, корректно подставляется `Tiger/Wolf/Rabbit/Monkey`.
- Методы работают через базовые типы и интерфейсы, обеспечивая поведенческую совместимость.

**I — Interface Segregation**
- Интерфейсы: `IAlive` (еда), `IInventory` (инв. номер), `IHerbivore` (доброта), `IVeterinaryClinic` (проверка здоровья), а также фабричные интерфейсы.

**D — Dependency Inversion**
- `App` зависит от абстракций `IZoo`, `IAnimalFactory`, `IThingFactory`.
- `Zoo` зависит от `IVeterinaryClinic`.
- Фабрики зависят от `IEnumerable<I…Creator>` — реестр собирается DI.
- Связывание с конкретными реализациями выполняется только в `Program.cs`