CREATE TABLE [dbo].[Pokemon]
(
    PokemonID INT PRIMARY KEY IDENTITY(1,1) NOT NULL, 
    Name NVARCHAR(50) NOT NULL,
    HP INT CHECK (HP >= 1) NOT NULL,
    Attack INT CHECK (Attack >= 0) NOT NULL,
    Defense INT CHECK (Defense >= 0) NOT NULL,
    Speed INT CHECK (Speed >= 0) NOT NULL,
    SpecialAttack INT CHECK (SpecialAttack >= 0) NOT NULL,
    SpecialDefense INT CHECK (SpecialDefense >= 0) NOT NULL,
    Ability NVARCHAR(50) NOT NULL,
    Legendary BIT NOT NULL,
    Region NVARCHAR(50) NOT NULL
);
