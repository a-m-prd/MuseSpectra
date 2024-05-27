USE master;
GO

-- Create the database if it does not exist
IF DB_ID(N'MuseSpectraDb') IS NULL
BEGIN
CREATE DATABASE MuseSpectraDb;
END
GO

USE MuseSpectraDb;
GO

-- User-Table
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
	SelectedColor NVARCHAR(20) DEFAULT 'Random',
    SelectedShape NVARCHAR(20) DEFAULT 'Ellipse',
	SelectedEffect NVARCHAR(20) DEFAULT 'None',
    SizeRange FLOAT DEFAULT 60
);
END
GO

/*-- Add indexes if database gets to large
CREATE INDEX IX_Users_Username ON dbo.Users(Username);
CREATE INDEX IX_Users_Email ON dbo.Users(Email);
GO
*/

-- Example-Data
INSERT INTO dbo.Users (Username, PasswordHash, Email, SelectedColor, SelectedShape, SelectedEffect, SizeRange)
VALUES
    ('alice', 'hashed_password_1', 'alice@example.com', 'Red', 'Rectangle', 'Pulsate', 75),
    ('bob', 'hashed_password_2', 'bob@example.com', 'Green', 'Triangle', 'Move', 50),
    ('charlie', 'hashed_password_3', 'charlie@example.com', 'Blue', 'Polygon', 'Gradient', 65);