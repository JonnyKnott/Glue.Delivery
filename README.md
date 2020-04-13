# Glue.Delivery

## Overview

Glue.Delivery is a delivery management system designed for use by Users and Partners. 

- The CRUD API enables the creation and management of deliveries.
- Deliveries can hold the following State:
    - Created
    - Approved
    - Completed
    - Cancelled
    - Expired
- Once created, the state advancement is managed through the DeliveryState API. A typical lifespan of a Delivery would be:
    - Created - Delivery created through CRUD API.
    - Created - Delivery details can be updated through the CRUD API, but State is managed internally.
    - Approved - A User can approve a delivery prior to the scheduled StartTime. Only approved deliveries can be completed.
    - Completed - A Partner can Complete a delivery. Only approved deliveries can be Completed.
- Non-typical State flows include:
    - Cancellation - A User or a Partner can Cancel a pending delivery (Created or Approved) at any time. 
        - Cancelled is a final state and cannot be modified.
    - Expiration - If the Delivery AccessWindow EndTime passes the system moves the Delivery into an expired state. (This operation is currently set to take place every minute, but this can be modified). 
        - Expired is a final state and cannot be modified.
        
The system manages State advancement permissions using JWT authentication and Claims based Authorization with the following roles:

- User - Assumed to be the customer. Is allowed access to Create, Read and Update Delivery details, and to Approve or Cancel existing deliveries.
- Partner - Assumed to be the retailer. Is allowed access to Create, Read and Update Delivery details, and to Complete and Approved deliveries.
- System - Assumed to be a system process. Has full API permissions. (Only a System user can outright Delete a Delivery).

##Pre-Requisites

- The application uses .NET Core 3.1.
- The application requires access to a dynamodb table to persist delivery. A local DynamoDb instance can be run using Docker, and the WebApi will create the table it needs on Startup.
- It is recommended to build and run the application using the docker-compose file.

##Running the application

### Using Docker (recommended)

The recommendation is to run the full solution in Docker. The following commands will build and run the required images, including setting up the DynamoDb table.

- Build the WebApi and HostedService images
 
     ```shell
    docker-compose build
    ```
- Bring up the docker container
    ```shell
    docker-compose up
    ``` 
  
The WebApi and the Worker should both be in a running state once the DynamoDb table has been created. The worker method should trigger at an interval set by the Environment Variable ScheduledService__Interval.

When the application is running in Docker base url for the API is `http://localhost:9003` 

### Using an IDE

If you would prefer to run the application in your IDE, you will first need to bring up a local DynamoDb instance. You can do this using the docker-compose.test.yml file.

Run the following command:
```shell
docker-compose -f docker-compose.test.yml up
```

Once the dynamoDb instance is up, you must run the WebApi. When in a Development environment the WebApi runs a HostedService to set up a DynamoDb table.

Once the WebApi is running and healthy (use postman collection described below to perform HealthCheck) then you can run the `Glue.Delivery.DeliveryState.Worker` Console Application.

When running the WebApi via your IDE the base url for the API is `http://localhost:5000`

### Running tests

In order to run the tests no setup should be required.

## Postman Collection

A postman collection and environment is included at `postman/Glue.Delivery.postman_collection.json` and `postman/Glue.Delivery.Dev.postman_environment.json`.

Import them into Postman and set the `base_url` value to `http://localhost:9003` or `http://localhost:5000` depending on whether you are using Docker or your IDE.

The postman collection contains the following folders:
- Authentication
    - Sign in as different roles in order to use the WebApi. See above for the permissions granted to each role.
- CRUD
    - Requests for the CRUD API. Creating a Delivery populates the DeliveryId in the environment variables, and thn subsequent requests made will affect that delivery specifically.
- State Operations
    - Requests for the DeliveryState API. You must be signed in asn an appropriate user to advance the state of the delivery.
    

## Notes and Assumptions

- Currently the API does not split Delivery by User/Partner, meaning that any User or Partner can act on all Deliveries.
- It was assumed that State should not be editable via the API and that Delete should be a require a high level of priveledge.
- Users are currently mocked in the API, as is the JWT Secret. The JWT Secret would be stored in secure cloud storage in a production ready system and never held in Source Control. 