# Pet Game

## Description
A virtual pet game. The user can create a new player, adopt a pet and feed it or pet it. A pet's happiness descreases over time until you pet it and its hunger increases over time until you feed it.
There are 3 different types of pets (Dog, Cat and Parrot) and their happiness and hunger change at different rates.

## How to run
- Install docker and docker compose if they're not already installed on your machine
- Navigate to the solution's root folder (clone/download the solution first)
- Open a terminal
- Run `docker compose build`
- Run `docker compose up`

That's it, the API should be up and running.

## How to run the tests
- Run `docker compose -f docker-compose.infra.yml up` to spin up the SQL server dependency (required for the integration tests)
- Run the tests through visual studio or open a terminal and run `dotnet test`

## API specs
I didn't have time for a proper swagger specs file, so I'll briefly go through the APIs here:

### Base URL
http://localhost:5888

### Player API
Path: `/player`

#### Create new player
- Verb: Post

Example request:
```json
{
    "FirstName": "test",
    "MiddleName": "test",
    "LastName": "test",
    "UserName": "test",
    "Email": "test@test.com"
}
```

Example response:
```json
{
    "id": 12
}
```

#### Get player info
- Path:`/{id}`
- Verb: Get

Example response:
```
{
    "id": 12,
    "firstName": "test",
    "middleName": "test",
    "lastName": "test",
    "userName": "test",
    "email": "test@test.com",
    "registrationDate": "2022-03-27T18:20:13.0960985+00:00",
    "pets": [
        {
            "id": 8,
            "name": "test",
            "type": "Cat",
            "happiness": 50,
            "hunger": 50,
            "adoptionDate": "2022-03-27T18:23:30.3438406+00:00",
            "lastPetted": "2022-03-27T18:23:30.3438406+00:00",
            "lastFed": "2022-03-27T18:23:30.3438406+00:00"
        }
    ]
}
```

### Pet API
Path: `/pet`

#### Adopt new pet
- Verb: Post

Example request:
```json
{
    "PlayerId": 12,
    "Name": "test",
    "Type": "Cat"
}
```

Example response:
```json
{
    "id": 8
}
```

#### Get pet info
- Path:`/{id}`
- Verb: Get

Example response:
```
{
    "id": 8,
    "name": "test",
    "type": "Cat",
    "happiness": 50,
    "hunger": 50,
    "adoptionDate": "2022-03-27T18:23:30.3438406+00:00",
    "lastPetted": "2022-03-27T18:23:30.3438406+00:00",
    "lastFed": "2022-03-27T18:23:30.3438406+00:00"
}
```

#### Update pet
- Path:`/{id}`
- Verb: Put

Example request:
```json
{
    "FeedingPoints": 50,
    "PettingPoints": 100
}
```

## Game behaviour
Each pet's hunger and happiness change at different rates (arbitratily chosen), but these can be configured in the `src\PetGame\appsettings.json` file. Each rate is a timespan value indicating how much time has to pass before
there's a change of 1 point in the stat's value. 

For example, the dog's default rates of `00:20:00` means that its hunger will decrease by 1 point every 20 minutes and its happiness will decrease by 1 point every 20 minutes.
The stats have a neutral value of 50 points, a minimum value of 0 points and a maximum value of 100 points. If you want to experiment with the stats changing and don't have time to wait for the default rates, feel free to change
them to faster rates, e.g. `00:00:05` (1 stat change every 5 seconds). Don't forget to relaunch the app following the steps outlined above in order for the changes to take effect.

You can pet or feed an animal by using the PUT API (give a number of FeedingPoints/PettingPoints to adjust the stats by).

## Future work
I did not have time to fully finish the game, so here are a few things that I would like to work on in the future:
- Proper swagger specs file
- Unit tests for pet and player services
- More integration tests
- Validation
- Unique index on username/email etc
- Add more pets