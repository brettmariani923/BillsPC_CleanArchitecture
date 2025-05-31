CREATE PROCEDURE [dbo].[SeedTable_PokemonTypes]
AS
BEGIN
    PRINT 'Populating records in [dbo].[PokemonTypes]';

    DECLARE @PokemonID INT = 1;

    -- Temp table for seed data
    IF OBJECT_ID('tempdb..#SeedTypes') IS NOT NULL DROP TABLE #SeedTypes;

    CREATE TABLE #SeedTypes (
        TypeIDOne INT,
        TypeIDTwo INT
    );

    INSERT INTO #SeedTypes (TypeIDOne, TypeIDTwo)
    VALUES
        (4, 7),
        (4, 7),
        (4, 7),
        (1, 1),
        (1, 1),
        (1, 9),
        (2, 2),
        (2, 2),
        (2, 2),
        (11, 11),
        (11, 11),
        (11, 9),
        (11, 7),
        (11, 7),
        (11, 7),
        (0, 9),
        (0, 9),
        (0, 9),
        (0, 0),
        (0, 0),
        (0, 9),
        (0, 9),
        (7, 7),
        (7, 7),
        (3, 3),
        (3, 3),
        (8, 8),
        (8, 8),
        (7, 7),
        (7, 7),
        (7, 8),
        (7, 7),
        (7, 7),
        (7, 8),
        (0, 17),
        (0, 17),
        (1, 1),
        (1, 1),
        (0, 17),
        (0, 17),
        (7, 9),
        (7, 9),
        (4, 7),
        (4, 7),
        (4, 7),
        (11, 4),
        (11, 4),
        (11, 7),
        (11, 7),
        (8, 8),
        (8, 8),
        (0, 0),
        (0, 0),
        (2, 2),
        (2, 2),
        (6, 6),
        (6, 6),
        (1, 1),
        (1, 1),
        (2, 2),
        (2, 2),
        (2, 6),
        (10, 10),
        (10, 10),
        (10, 10),
        (6, 6),
        (6, 6),
        (6, 6),
        (4, 7),
        (4, 7),
        (4, 7),
        (2, 7),
        (2, 7),
        (12, 8),
        (12, 8),
        (12, 8),
        (1, 1),
        (1, 1),
        (2, 10),
        (2, 10),
        (3, 17),
        (3, 17),
        (0, 9),
        (0, 9),
        (0, 9),
        (2, 2),
        (2, 5),
        (7, 7),
        (7, 7),
        (2, 2),
        (2, 5),
        (13, 7),
        (13, 7),
        (13, 7),
        (12, 8),
        (10, 10),
        (10, 10),
        (2, 2),
        (2, 2),
        (3, 3),
        (3, 3),
        (4, 10),
        (4, 10),
        (8, 8),
        (8, 8),
        (6, 6),
        (6, 6),
        (0, 0),
        (7, 7),
        (7, 7),
        (8, 12),
        (8, 12),
        (0, 0),
        (4, 4),
        (0, 0),
        (2, 2),
        (2, 2),
        (2, 2),
        (2, 2),
        (2, 2),
        (2, 10),
        (10, 17),
        (11, 9),
        (5, 10),
        (3, 3),
        (1, 1),
        (11, 11),
        (0, 0),
        (2, 2),
        (2, 9),
        (2, 5),
        (0, 0),
        (0, 0),
        (2, 2),
        (3, 3),
        (1, 1),
        (0, 0),
        (12, 2),
        (12, 2),
        (12, 2),
        (12, 2),
        (12, 9),
        (0, 0),
        (5, 9),
        (3, 9),
        (1, 9),
        (14, 14),
        (14, 14),
        (14, 9),
        (10, 10),
        (10, 10)
          

    -- Insert into actual table
    INSERT INTO [dbo].[PokemonTypes] (PokemonID, TypeID, Slot)
    SELECT 
        ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS PokemonID,
        TypeIDOne,
        1 AS Slot
    FROM #SeedTypes;

    INSERT INTO [dbo].[PokemonTypes] (PokemonID, TypeID, Slot)
    SELECT 
        ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS PokemonID,
        TypeIDTwo,
        2 AS Slot
    FROM #SeedTypes;

    DROP TABLE #SeedTypes;

    PRINT 'Finished Populating records in [dbo].[PokemonTypes]';
END
GO

EXEC [dbo].[SeedTable_PokemonTypes];

DROP PROCEDURE IF Exists [dbo].[SeedTable_PokemonTypes];