# BillsPC_CleanArchitecture
#### Video Demo:  https://youtu.be/UzbG0W6VnmI
#### Description:
The database portion requires you to have a SQL Server Database.

# Pokémon Database (Razor Pages, .NET 8)

## Purpose

This project is a comprehensive Pokémon Database web application built with ASP.NET Core (.NET 8). It provides detailed information on all Pokémon species up to Generation 8 (926 entries), it is a reliable, searchable, and easy-to-use resource for Pokémon fans and collectors.

## Features

- **Complete Pokémon Data:** View base stats, types, abilities, and other attributes for every Pokémon up to Gen 8.
- **Dynamic Artwork:** Official artwork and sprites are fetched from [PokeAPI](https://pokeapi.co) and cached locally for fast access.
- **Search Functionality:** Search box supports partial matches for quick and easy Pokémon lookup.
- **Bulk Data Import:** SQL scripts provided for easy database setup and seeding.
- **Modern Web UI:** Built with Razor Pages and follows best practices for .NET 8 web applications.
- **Battle Simulator:** Has 6 vs 6 and a 1 vs 1, allowing you to test out different pokemon and team combinations in battle.

## Usage Instructions

1. **Clone the Repository**

2. **Database Setup**
- Create a SQL Server database.
- Run the provided SQL scripts (found in the SQLServer layer) to create tables and seed data.
- Update the `DefaultConnection` string in `appsettings.json` (inside the `BillsPC_CleanArchitecture.Api` project) to point to your database.

3. **Run the Application**
- Open the solution in Visual Studio.
- Set the API project as the startup project.

4. **Using the App**
- Browse Pokémon by name or attributes.
- Use the search box for partial name matches.
- The first load of all Pokémon images may take up to 2 minutes; images are cached for faster subsequent access.
- The team builder allows you to create a team of up to 6 pokemon to use in the battle feature. Pokemon added to the team builder are displayed under the Bill's Pc art on the homepage.
- The my team tab allows you to see the stat information of your team before you use them in battle.
- The Battle tab allows you to select between a 1 vs 1 and a 6 vs 6 battle. For the 1 vs 1, you can select any combination of two pokemon to test out in battle. For the 6 vs 6, you make a team of up to 6 different pokemon and battle them against a randomly generated ai team of 6.

## 🧠 Project Architecture Overview

This app is built around **Clean Architecture**, which separates responsibilities into clear layers:

Client (View)
↓

Controller (Web Layer)
↓

Abstraction (Interfaces)
↓

Implementation (Services)
↓

Requests (SQL Queries)
↓

Database (SQL Server)

-
---
//Part 1 (very beginning): This is where it starts. We start by making our call to PokemonService.
//A user on one of the view pages will press a search button or the info tab, and this is where the process of getting them the info they request begins.
//A good thing to repokemon is that views move through controllers, and controllers move through services. The controller 
//is your entrypoint, it accepts requests and returns ressponses. Go to PokemonService.cs in the Application layer next for the next step in the process.

     public async Task<IActionResult> ReturnAllPokemon()
     {
     var allPokemon = await _pokemonService.GetAllPokemonWithImagesAsync();
     return View("ReturnAllPokemon", allPokemon);
     }

This is where the controller facciltates getting the data it needs. For example, using the GetAllPokemonWithImagesAsync method (last method down below),
//once the user has pressed The "View All Pokemon" button on the webpage and caused a get request, it causes the controller to call to the service layer(here).
//The service layer will then call the data layer(to ReturnAllPokemonRequest in the requests folder) to make a request to get the data it needs from the database. So to sum it all up so far,
//the View All Pokemon button will call the controller, which calls the services layer(you are here). The next step will then be be to access the data layer to load all the pokemon,
//and call the PokeApiService to get the images for each pokemon (using the AddImagesToPokemonListAsync method below), and then return the list of pokemon with images to the controller,
//which will then return it to a new webpage(view) for the user. We will cover this more in the next steps, if it seems like a lot right now it will become clearer as you go.
//go to ReturnAllPokemonRequest.cs in the Data layer next to see how the data access layer is called to get the data we need.


    public async Task<List<Pokemon_DTO>> GetAllPokemonWithImagesAsync()
    {
        var request = new ReturnAllPokemonRequest();
        var pokemon = (await _dataAccess.FetchListAsync<Pokemon_DTO>(request)).ToList();
        await AddImagesToPokemonListAsync(pokemon);
        return pokemon;
    }

  // This class holds the a SQL request object we are going to use to retrieve the data we need from the database. 
    // It doesnt execute the query itself, it just provides the SQL command and any parameters needed for the query.
    // It has to go the data access layer first to execute the query and return the results.

  // So to reiterate where we are in the process, the "View All Pokémon" button is pressed, get request --> controller --> service layer --> data request (You are here)
    // and then after this request object (ReturnAllPokemonRequest) is created, it then gets passed to the data access class to actually return the results.
    // Inside DataAccess, it gets handled by the FetchListAsync<T> method, which executes the query using Dapper and returns it as a list of results.

  // If you're wondering how or why this connects to the DataAccess class: the methods GetSql() and GetParameters() are defined 
    // by the IDataFetchList<T> interface. Since ReturnAllPokemonRequest implements this interface, the DataAccess class knows how to retrieve the SQL and parameters from it and execute the query
    // which we will go over more in the next step.

  // Go to the DataAccess class in the Implementation folder next to see how this request is executed.

    public class ReturnAllPokemonRequest : IDataFetchList<Pokemon_DTO>
    {
      public string GetSql()
      {
          return "SELECT * FROM dbo.Pokemon";
      }

      public object? GetParameters()
      {
          return null;
      }

 // The DataAccess class is responsible for actually executing the SQL commands using Dapper. It doesn't define the queries itself, it gets those from the request objects(in this case ReturnAllPokemonRequest)
 // that implement interfaces like IDataFetchList<T>, IDataFetch<T>, or IDataExecute.
 //
 // In the example we've been using, the ReturnAllPokemonRequest class implements IDataFetchList<Pokemon_DTO>, which provides the GetSql() and GetParameters() methods. The definitions for the SQL query and parameters(SELECT * FROM dbo.pokemon, null) are what that will be used in this step.
 // Now that they've been defined, the FetchListAsync method below (last in Public IdataAcess methods below) takes that request, retrieves the SQL and parameters via the interface, and uses Dapper's QueryAsync<T>() to actually execute it and return the pokemon info
 // we asked for. So to reiterate where we are in the process,
 // the "View All Pokémon" button is pressed, get request --> controller (ReturnAllPokemon()) --> service layer (GetAllPokemonWithImagesAsync())--> data request (ReturnAllPokemonRequest) --> data access layer (You are here, FetchListAsync<TResponse>)
 // --> Dapper executes the query and returns the results.
 //
 // This might seem overly complicated or like a huge pain, but by setting it up like this it separates the query definition (ReturnAllPokemonRequest) from the query execution (DataAccess). By doing it like this, we have a separation of concerns that we dont get in other patterns.
 // it ends up being a lot more flexible, maintainable, and most of all its a lot safer. Because the SQL statement is separated from the execution logic, it keeps responsibilities clearly divided and makes it easier to maintain.
 // Only known request types are allowed to use the DataAccess class, which is a lot safer than just having it all in one place where any code could execute any SQL command. 
 //
 //Breifly I want to go over the Pokemon_DTO class, I dont think it warrants an entire visit. Its basically just the data structure of a Pokémon in the application. It contains properties like Id, Name, Type, and ImageUrl, that are populated by the database, and used to transfer that data
 //from the database to our website. If you have any questions about it, feel free to ask me about it in on discord and we can go over it more.
 // Now that we have safely recieved the data we need, we can return to the PokemonService class in the Application layer to see how it uses this data.

          public async Task<IEnumerable<TResponse>> FetchListAsync<TResponse>(IDataFetchList<TResponse> request) => await HandleRequest(async _ => await _.QueryAsync<TResponse>(request.GetSql(), request.GetParameters()));

//This helper method used in the method above is what allows us to open a connection string to actually interact with the database

     private async Task<T> HandleRequest<T>(Func<IDbConnection, Task<T>> funcHandleRequest)
     {
       using var connection = _connectionFactory.NewConnection();

       connection.Open();

       return await funcHandleRequest.Invoke(connection);
     }

//PART 2(***read after DataAccess.cs section***): Hello again! For our example method GetAllPokemonWithImagesAsync, we left this class to to go to the ReturnAllPokemonRequest() class, which took us to the dataaccess class, which returned the info we needed.
// So to reiterate, we went through * var request = new ReturnAllPokemonRequest(); *, and that took us to the DataAccess class, which executed the SQL query and returned the results finishing * var result = await _dataAccess.FetchListAsync<Pokemon_DTO>(request); *.
// so the for the last step, we just need to get the images for each pokemon. This is done using the AddImagesToPokemonListAsync() method here, which goes to the PokeApiService class, which will fetch the images from the PokeAPI and save them to our local server.

    public async Task<List<Pokemon_DTO>> GetAllPokemonWithImagesAsync()
    {
        var request = new ReturnAllPokemonRequest();
        var pokemon = (await _dataAccess.FetchListAsync<Pokemon_DTO>(request)).ToList();
        await AddImagesToPokemonListAsync(pokemon);
        return pokemon;
    }

//Part 3) Now the AddImagesToPokemonListAsync method will take the list of Pokemon_DTO objects and fetch the images for each one using the PokeApiService.
// once that is finsished, we will have completed the final step of our method *  await AddImagesToPokemonListAsync(pokemon);  *
// all that is left now to do is the *  return pokemon;  * line, which will return the list of Pokemon_DTO objects with their images to the controller, which will then return it to the view(webpage) for the user to see.
// go back to PokemonController.cs in the Api layer for the final steps, to see how the controller handles the results and returns them to the view for the user to see.

    public async Task<List<Pokemon_DTO>> GetAllPokemonWithImagesAsync()
    {
        var request = new ReturnAllPokemonRequest();
        var pokemon = (await _dataAccess.FetchListAsync<Pokemon_DTO>(request)).ToList();
        await AddImagesToPokemonListAsync(pokemon);
        return pokemon;
    }

// This is where the results are returned to the view for the user to see. It takes that information we worked so hard to get from different layers and returns it to the view for the user to see.
// So to break it down *  var allPokemon = await _pokemonService.GetAllPokemonWithImagesAsync();  * gets the data from the service layer, which is where we were during the previous step.
// Once it has the information it needs, it returns it to the view using *  return View("ReturnAllPokemon", allPokemon);  *. This directs the information to the "ReturnAllPokemon" view, which is where the user will see the results of their request.
// Great job! You've made it through the entire process of how the data flows from the user request to the final view. If you have any questions about this or any other part of the code, feel free to ask me on discord and we can go over it together.
// If you are curious, look at the ReturnAllPokemon.cshtml file in the Views/Pokemon folder to see how the data is displayed on the webpage.

     public async Task<IActionResult> ReturnAllPokemon()
     {
     var allPokemon = await _pokemonService.GetAllPokemonWithImagesAsync();
     return View("ReturnAllPokemon", allPokemon);
     }

// And this is the view, where we populate our HTML elements with the information we got from the API and the database! If you made it this far thank you for reading!!

    @model List<BillsPC_CleanArchitecture.Data.DTO.Pokemon_DTO>
    
    @{
        ViewData["Title"] = "All Pokémon";
    }
    
    <h2>All Pokémon</h2>
    
    @if (!Model.Any())
    {
        <p>No Pokémon found.</p>
    }
    else
    {
        <table class="table table-bordered table-striped text-center align-middle">
            <thead>
                <tr>
                    <th>Image</th>
                    <th>PokemonID</th>
                    <th>Name</th>
                    <th>HP</th>
                    <th>Attack</th>
                    <th>Defense</th>
                    <th>SpecialAttack</th>
                    <th>SpecialDefense</th>
                    <th>Speed</th>
                    <th>Ability</th>
                    <th>Legendary</th>
                    <th>Region</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var p in Model)
                {
                    <tr>
                        <td>
                            @if (!string.IsNullOrEmpty(p.ImageUrl))
                            {
                                <img src="@p.ImageUrl" alt="@p.Name" width="50" />
                            }
                            else
                            {
                                <span>No image</span>
                            }
                        </td>
                        <td>@p.PokemonID</td>
                        <td>@p.Name</td>
                        <td>@p.HP</td>
                        <td>@p.Attack</td>
                        <td>@p.Defense</td>
                        <td>@p.SpecialAttack</td>
                        <td>@p.SpecialDefense</td>
                        <td>@p.Speed</td>
                        <td>@p.Ability</td>
                        <td>@(p.Legendary ? "Yes" : "No")</td>
                        <td>@p.Region</td>
                    </tr>
                }
            </tbody>
        </table>
    }

##Things covered by this project that we learned in CS50

-MVC
-Routing
-HTML & Templating
-Server Side Programming
-SQL Querying 
-Databases
-Separation of Concerns
-Forms and HTTP Requests
-Project Structuring
    
## Feedback
    
For questions or feedback, reach out via [Discord](https://discord.com/channels/brett_mariani/).
    
    ---

Thanks for looking!
