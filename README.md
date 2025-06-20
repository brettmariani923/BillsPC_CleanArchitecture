Hello Arael! This is where you should start. I wont try to explain the process in the readme as I think that will just complicate
things too much, so I put it in the code so you can see whats happening and where. I detailed the process in each part
and it leads you to the next part after each explination. Start with the PokemonController.cs class,
you can find it in the Controllers folder in BillsPC_CleanArchitecture.Api.
If you have any questions or want to go over anything, just reach out to me in the discord, or send me a 
screenshot of anything you're unsure about.

The database portion requires you to have a SQL Server Database.

# Pokémon Database (Razor Pages, .NET 8)

## Purpose

This project is a comprehensive Pokémon Database web application built with ASP.NET Core (.NET 8). It provides detailed information on all Pokémon species up to Generation 8 (926 entries), making it a reliable, searchable, and easy-to-use resource for Pokémon fans and collectors.

## Features

- **Complete Pokémon Data:** View base stats, types, abilities, and other attributes for every Pokémon up to Gen 8.
- **Dynamic Artwork:** Official artwork and sprites are fetched from [PokeAPI](https://pokeapi.co) and cached locally for fast access.
- **Search Functionality:** Search box supports partial matches for quick and easy Pokémon lookup.
- **Bulk Data Import:** SQL scripts provided for easy database setup and seeding.
- **Modern Web UI:** Built with Razor Pages and follows best practices for .NET 8 web applications.

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

## Feedback

For questions or feedback, reach out via [Discord](https://discord.com/channels/brett_mariani/).

---

Thanks for looking!