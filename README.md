Hello Arael! This is where you should start. I think trying to explain the process in the readme will just complicate
things too much, so you should go straight to the code.I detailed the process in each part
and it leads you to the next part after each explination. Start with the PokemonController.cs class,
You'll can find it in the Controllers folder in BillsPC_CleanArchitecture.Api.
Come back here after you're finished and look at this chart below, it will help you make sense of things.
If you have any questions or want to go over anything, just reach out to me in the discord, or send me a 
screenshot of anythign you're unsure about.

# After you're finished, here is an overview of the process:

Step	Layer						Action
--------------------------------------------------------------------------------
1		Controller					Calls GetAllPokemonWithImagesAsync()
2		PokemonService				Builds SQL request object
3		IDataAccess	Executes		SQL with Dapper
4		SqlConnectionFactory		Creates SQL connection
5		ReturnAllPokemonRequest		Supplies SQL string
6		Dapper						Maps rows to Pokemon_DTO list
7		PokemonService				Calls PokeApiService for image URLs
8		PokeApiService				Downloads & caches artwork from PokeAPI
9		Controller					Returns populated list to View
10		View						Renders cards/images using data and paths
