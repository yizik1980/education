-- Create Table
CREATE TABLE [BoardingSchools] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [PupilsCount] int NOT NULL,
    [AverageAge] float NOT NULL,
    CONSTRAINT [PK_BoardingSchools] PRIMARY KEY ([Id])
);
GO

-- Create Stored Procedures
CREATE PROCEDURE [GetBoardingSchools]
AS
BEGIN
    SELECT * FROM BoardingSchools;
END;
GO

CREATE PROCEDURE [GetBoardingSchoolById]
    @Id int
AS
BEGIN
    SELECT * FROM BoardingSchools WHERE Id = @Id;
END;
GO

CREATE PROCEDURE [InsertBoardingSchool]
    @Name nvarchar(max),
    @PupilsCount int,
    @AverageAge float
AS
BEGIN
    INSERT INTO BoardingSchools (Name, PupilsCount, AverageAge)
    VALUES (@Name, @PupilsCount, @AverageAge);
END;
GO

CREATE PROCEDURE [UpdateBoardingSchool]
    @Id int,
    @Name nvarchar(max),
    @PupilsCount int,
    @AverageAge float
AS
BEGIN
    UPDATE BoardingSchools
    SET Name = @Name, PupilsCount = @PupilsCount, AverageAge = @AverageAge
    WHERE Id = @Id;
END;
GO

CREATE PROCEDURE [DeleteBoardingSchool]
    @Id int
AS
BEGIN
    DELETE FROM BoardingSchools WHERE Id = @Id;
END;
GO
