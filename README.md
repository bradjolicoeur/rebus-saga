# Rebus Saga Example

This is an example that uses ReBus, RabbitMQ and 

### Workflow

1. Web API controller sends ProcessPayment Command to Saga Endpoint
2. ProcessPayment initializes Saga state in PaymentSaga endpoint and sets a reply timeout
3. ProcessPaymentSaga sends MakePayment Command to PaymentProcessorWorker Endpoint
4. MakePayment handler simulates making a payment and publishes ICompletedMakePayment event
5. ProcessPaymentSaga consumes ICompletedMakePayment, updates state and sends reply to controller
6. Controller displays the results to the caller

![Context Diagram](https://github.com/bradjolicoeur/NServiceBusAWS/blob/master/ContextDiagram.PNG "Solution Context Diagram")


If step 4 is delayed, the timeout will triger sending a 'Pending' reply to the controller and the eventual accept message will still get handled in the web api assembly for sending an asyncronous message to the caller.

Note that any keys or passwords included in this repo are provided as examples only.  Make sure you update keys and passwords to appropriate values in your system.