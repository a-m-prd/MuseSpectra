USE master;
GO

IF DB_ID(N'MuseSpectraDb') IS NULL
  CREATE DATABASE MuseSpectraDb;
GO
USE MuseSpectraDb;
GO

-- User-Table
IF OBJECT_ID('Users') IS NULL
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
	SelectedColor NVARCHAR(20) DEFAULT 'Random',
    SelectedShape NVARCHAR(20) DEFAULT 'Ellipse',
    SizeRange FLOAT DEFAULT 50
);

-- Example-Data
INSERT INTO Users (Username, Password, Email) VALUES ('alice', 'password123', 'alice@example.com');
INSERT INTO Users (Username, Password, Email) VALUES ('bob', 'password456', 'bob@example.com');
INSERT INTO Users (Username, Password, Email) VALUES ('charlie', 'password789', 'charlie@example.com');