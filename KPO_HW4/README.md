# Prerequisites
- dotnet sdk 10.0
- docker

# How to deploy
- `dotnet publish /t:PublishContainer`
- `cd ./deploy`
- `docker compose up`


## Стек

- **.NET 10 / C#**
- **.NET Aspire** для оркестрации
- **PostgreSQL**
- **RabbitMQ**
- **MassTransit** + **EntityFramework Outbox/Inbox**
- **Blazor** UI

### Swagger
- Orders Service: `http://localhost:8080/api/orders/scalar`
- Payments Service: `http://localhost:8080/api/accounts/scalar`

### API Gateway
- Проксирование запросов

### Orders Service
- REST API:
  - Создать заказ
  - Список заказов
  - Получить заказ по id
  - Публикует `PaymentRequested` в RabbitMQ
- Обновление статуса заказа:
  - Использует `PaymentSucceeded` / `PaymentFailed` и переводит заказ в `FINISHED` / `CANCELLED`.
- Realtime:
  - SignalR Hub `/hub/orderNotifications` — push-уведомления о смене статуса.

### Payments Service
- REST API:
  - Создать счёт
  - Пополнить баланс
  - Получить баланс
  - История транзакций
  - Публикует `PaymentSucceeded` / `PaymentFailed` в RabbitMQ

### UI
- `Orders` — страница с зказамии real-time обновления статуса
- `Account` — страница баланс и транзакций
- Push уведомления с SignalR
