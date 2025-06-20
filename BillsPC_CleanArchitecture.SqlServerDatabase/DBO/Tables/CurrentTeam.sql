CREATE TABLE [dbo].[CurrentTeam]
(
    TeamEntryID INT PRIMARY KEY IDENTITY(1,1),
    Slot INT NOT NULL CHECK (Slot BETWEEN 1 AND 6),
    PokemonID INT NOT NULL,
    CONSTRAINT FK_CurrentTeam_Pokemon FOREIGN KEY (PokemonID)
        REFERENCES dbo.Pokemon(PokemonID),
    CONSTRAINT UQ_CurrentTeam_Slot UNIQUE (Slot)
);
