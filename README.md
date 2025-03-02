# PaymentGatewayModule
**Payment Gateway Module** with an event-driven architecture. This system should process payments, integrate with a mock third-party API, and demonstrate the ability to handle asynchronous events such as transaction status updates.

## This solution consists:
1. DemoPaymentAPI (Mock third-party api)
2. PaymentWebAPI
3. Event Driven Architecture with RabbitMQ.
4. Logging (Serilog).
5. Retry Policy (Polly).
6. Global Exception
7. Email Service (SMTP, Mailkit)
8. PaymentApp (Angular).
9. Simple Authentication and Authorization.

## How to setup this solution:
1. Clone this project.
2. Restore all the dependencies (packages)
3. Changes the appsettings for: Database Provider connection string, RabbitMQ (default credential), Serilog Configuration and Email Configuration.
4. Build and Run solution, this will create a database and tables.
5. Build ans Run Angular App (localhost:4200)
6. Now Angular app will be shown in browser.
