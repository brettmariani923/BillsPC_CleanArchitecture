
  CREATE PROCEDURE [dbo].[SeedTable_Types] AS
BEGIN

    PRINT 'Populating records in [dbo].[Types]';

    IF OBJECT_ID('tempdb.dbo.#dbo_SeedTypes') IS NOT NULL DROP TABLE #dbo_SeedTypes;

    SELECT 
        [Id], [Name] 
    INTO #dbo_SeedTypes 
    FROM [dbo].[Types] 
    WHERE 0 = 1;

    INSERT INTO #dbo_SeedTypes 
        ( [Id], [Name] )
    SELECT 
          [Id], [Name] 
    FROM 
    (  VALUES
          (0, 'Normal')
        , (1, 'Fire')
        , (2, 'Water')
        , (3, 'Electric')
        , (4, 'Grass')
        , (5, 'Ice')
        , (6, 'Fighting')
        , (7, 'Poison')
        , (8, 'Ground')
        , (9, 'Flying')
        , (10, 'Psychic')
        , (11, 'Bug')
        , (12, 'Rock')
        , (13, 'Ghost')
        , (14, 'Dragon')
        , (15, 'Dark')
        , (16, 'Steel')
        , (17, 'Fairy')

          


    ) AS v ( [Id], [Name] );

    WITH cte_data as 
        (SELECT 
            [Id], [Name] 
        FROM #dbo_SeedTypes)
    MERGE [dbo].[Types] as t
        USING cte_data as s ON t.[Id] = s.[Id]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT 
                ([Id], [Name])
            VALUES 
                (s.[Id], s.[Name])
        WHEN MATCHED 
            THEN UPDATE SET 
                [Name] = s.[Name]
    ;

    DROP TABLE #dbo_SeedTypes;

    PRINT 'Finished Populating records in [dbo].[Types]'
END
GO

EXEC [dbo].[SeedTable_Types];

DROP PROCEDURE IF EXISTS [dbo].[SeedTable_Types] 
GO