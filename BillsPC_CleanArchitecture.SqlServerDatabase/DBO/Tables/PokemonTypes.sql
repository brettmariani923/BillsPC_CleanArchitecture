CREATE TABLE [dbo].[PokemonTypes]
(
    PokemonID INT NOT NULL,
    TypeID INT NOT NULL,
    Slot TINYINT NOT NULL CHECK (Slot IN (1, 2)),

    CONSTRAINT PK_PokemonTypes PRIMARY KEY (PokemonID, Slot),

    CONSTRAINT FK_PokemonTypes_Pokemon FOREIGN KEY (PokemonID)
        REFERENCES [dbo].[Pokemon](PokemonID)
        ON DELETE CASCADE,

    CONSTRAINT FK_PokemonTypes_Types FOREIGN KEY (TypeID)
        REFERENCES [dbo].[Types](Id)
        ON DELETE CASCADE
);