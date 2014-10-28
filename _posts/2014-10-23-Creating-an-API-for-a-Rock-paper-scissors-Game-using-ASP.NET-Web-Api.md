---
layout: default
title: Creating An Api For A Rock Paper Scissors Game Using Asp.net Web Api
---


## Introduction

In earlier posts we often used the domain for a simple rock-papers-scissors game. In this post we're going to re-use the domain and create an HTTP API. The RPS domain was created using CQRS and event sourcing.
Will that affect the API ? 
As we'll find out - no, it doesn't have to, but the async nature could be benficial to use in the API. 

## Scenario

The simple RPS game only has two user interactions/commands. CreateGame (including the player one’s move) and MakeMove (player two’s move).

We're going to list all available games, the ones a player could participate in, and all games played (ended).

### Eventual consistency

The RPS game is eventual consistent. When a user creates a game the available games list is updated from the events caused by the CreateGame command. An immediate refresh of available games after we created a game might not contain our game.

When receiving a command we need to tell the API client that we received the command and will process it. We could also inform the API client where the game could be retrieved when the command is processed.

### Mapping Commands to HTTP Verbs

It's easy to start off designing your API with an RPC style approach like /api/Games/{id}/move. Instead we're going let the HTTP verb be the action (one step towards a more RESTful API). The HTTP verbs differ in the matter of idempotency (an idempotent method can be called many times without different outcomes). POST are not idempotent but GET, PUT, PATCH and DELETE are. In our resources what is the mapping between commands and verb/action?  

> What about when a resource has several commands mapping to the same verb?
One possible solution is with "5 levels of media type" - specifying the command in the content-type:
    
>		Content-Type:application/json;domain-model=CreateGameCommand 

> See resources at the bottom of the post for more information on "5 levels of media type".

### Creating a Game

Creating a game maps well to the HTTP verb POST, each invocation creates a new game. We're going to start with creating a game on api/games - POST.
To inform the API client that we have received the command for creating a game we use the HTTP Status Code Accepted - 202.

     [HttpPost]
     public async Task<HttpResponseMessage> Create(input.....)
     {
        ....
         await _commandBus.SendAsync(command);
         return Request.CreateResponse(HttpStatusCode.Accepted)
     }

We create an action for creating a game. We'll cover creating a command from input in a bit. With the command we await it being placed on the queue (via the commandbus), then we tell the API client we accepted the command.
We don't yet tell the API client where to find the game upon creation.

Let's add that information.
       

     [HttpPost]
     public async Task<HttpResponseMessage> Create(input.....)
     {
        var gameId = Guid.NewGuid();
         ....
        await _commandBus.SendAsync(command);
         return Request.CreateResponse(HttpStatusCode.Accepted)
           .Tap(message => message.Headers.Location = new Uri(Url.Link(RouteConfiguration.AvailableGamesRoute, new { id = gameId })));
        }

We control what ID the game instance should get. Knowing that and the URL for the endpoint for available games, we send a location header with the response. We avoid route strings by having a simple object RouteConfiguration with the routes.

> Note: Tap is custom extension to chain action upon an object.

What about the command? The API client doesn't need to know about domain command objects. When creating the API we form a public representation of the underlying domain. Parameters/fields that might be form post data or a JSON body, this might not be the same fields of the command we're issuing on the server side. We could represent these fields in an object that we bind to - a public command, part of the public domain.

Here is a public object that we use to modelbind, it has validation attributes.

      public class CreateGameCommand
      {
        [Required]
        public string PlayerName { get; set; }
        [Required]
        public string GameName { get; set; }
        [Required]
        public string Move { get; set; }
      }
 
This is the domain command with some infrastructure properties.

    public class CreateGameCommand : IGameCommand, ICommand
      {
        public CreateGameCommand(Guid gameId, string playerName, string gameName, Move firstMove);
        public Guid GameId { get; }
        public string PlayerName { get; }
        public string GameName { get; }
           public Move FirstMove { get; }
           public Guid AggregateId { get; }
        public Guid CorrelationId { get; }
      }


Using these commands we could make our end point for creating a game look like:

        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Create(Game.ReadModel.CreateGameCommand input)
        {
            var gameId = Guid.NewGuid();

            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            Move move;
            if (!Enum.TryParse(input.Move, true, out move))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid move");

            var command = new RPS.Game.Domain.CreateGameCommand(gameId, input.PlayerName, input.GameName, move);
            await _commandBus.SendAsync(command); 

            return Request.CreateResponse(HttpStatusCode.Accepted)
                .Tap(message => message.Headers.Location = new Uri(Url.Link(RouteConfiguration.AvailableGamesRoute, new { id = gameId })));
        }

Here we use model valdidation, and parses the move string to our enum.

### Making a move

As a player you could only make a move on an available game. The MakeMoveCommand maps well to the HTTP verb PUT. We're going to add an action for PUT on api/games/available/{id}. Using attribute routing with a controller prefix "api/games", the PUT action looks like this; 

        [HttpPut]
        [Route("available/{id:Guid}")]
        public async Task<HttpResponseMessage> Move(Guid id, Game.ReadModel.MakeMoveCommand input)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            Move move;
            if (!Enum.TryParse(input.Move, true, out move))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid move");

            var command = new Game.Domain.MakeMoveCommand(id, move, input.PlayerName);

            await _commandBus.SendAsync(command);
            return Request.CreateResponse(HttpStatusCode.Accepted).Tap(
                    r => r.Headers.Location = new Uri(Url.Link(RouteConfiguration.EndedGamesRoute, new { id })));
        }

> Note: The API client know the game ID, used as a part of the URI.

When we have accepted the command we return 202 - accepted, with a location URL pointing to the EndedGamesRoute.

### Available and Ended games

Querying the available and ended games matches HTTP GET. We're going to have four GET endpoints: two for collections of games and two for single games. The endpoints for single games is to simplify the location headers for Create and Move.

Let's take a look at one of them - the single available game.

        [Route("available/{id:Guid}", Name = RouteConfiguration.AvailableGamesRoute)]
        public IHttpActionResult GetAvailableGame(Guid id)
        {
            var game = _readService
                .AvailableGames
                .SingleOrDefault(x => x.GameId == id);

            if (game != null)
                return Ok(game);

            return NotFound();
        }

Here we simply queries for a game with a given ID, if found, we return 200 with the game otherwise we return 404 - not found.

> Note that we name the route, to aid the location headers in Create and Move.

### The HTTP API

#### PlayerOne - Create a game with first move

> POST (api/games)
    
    { playerName ="player", gameName = "testGame", move = "paper"}

returns Accepted (202) with location header.

#### PlayerTwo - Make a move

> PUT (api/games/available/{gameId})

    { "playerName" : "player2", "move" : "rock" }

returns Accepted (202) with location header.

#### Available Games

> GET (api/Games/available/)

returns available games.

> GET api/Games/available/{ id }

returns single available game (200/404)

#### Ended Games

> GET (api/Games/ended)

returns ended games. (200)

> GET api/Games/ended/{ id }

returns single ended game (200/404)

### E-tags
We have not used ETags in this solution. ETags could help us with caching and conditional requests, on the GET action as well as concurrency checks on PUT, PATCH and DELETE actions.

E-tags could help the API client when reading games. In the make move case we could use If-Match to report if move already been made.

This could be the topic of another post.

### Conclusion

In the beginning we asked if the domain using CQRS, event sourcing and eventual consistency would affect the HTTP API. In our implementation we used 202 accept when retrieving a command with a location header for the client to go pull. So the async nature and eventual consistency part affected the API, other than that the public domain doesn't tell about the domain implementation. 

We could have created an API where even the eventual consistency wasn't notable to the API client. The server could have issued commands and waited for to read model to updated before returning to the API client. The added responibility would affect the server. But it's up to your what fits you API best.
  
Enjoy!

### Resources

- [Exposing CQRS Through a RESTful API](http://www.infoq.com/articles/rest-api-on-cqrs) 
- [5 levels of media type](http://byterot.blogspot.co.uk/2012/12/5-levels-of-media-type-rest-csds.html)
- [Nuget.org - FiveLevelsOfMediaType](https://www.nuget.org/packages/AspNetWebApi.FiveLevelsOfMediaType/)
- [REST CookBook - What are idempotent and/or safe methods?](http://restcookbook.com/HTTP%20Methods/idempotency/)
- [Wikipedia - HTTP ETag](http://en.wikipedia.org/wiki/HTTP_ETag)
