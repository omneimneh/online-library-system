
BEGIN TRAN InitDatabase; GO

-- Creating the database
CREATE DATABASE OnlineLibrarySystem GO

USE OnlineLibrarySystem GO

-- All personnels should be stored in this table
CREATE TABLE Person (
	PersonId int PRIMARY KEY IDENTITY(0, 1),
	Username varchar(20) NOT NULL UNIQUE,
	UserPassword varchar(100) NOT NULL
); GO

-- Personnels can be in one of the following tables
CREATE TABLE Professor (
	PersonId int PRIMARY KEY FOREIGN KEY REFERENCES Person(PersonId)
);

CREATE TABLE Student (
	PersonId int PRIMARY KEY FOREIGN KEY REFERENCES Person(PersonId)
);

CREATE TABLE Librarian (
	PersonId int PRIMARY KEY FOREIGN KEY REFERENCES Person(PersonId)
); GO

-- Personnels with extended privileges
CREATE TABLE Maintainer (
	PersonId int PRIMARY KEY FOREIGN KEY REFERENCES Librarian(PersonId)
);
GO

-- Books are stored in here
CREATE TABLE Book (
	BookId int PRIMARY KEY IDENTITY(0, 1),
	BookTitle varchar(100) NOT NULL,
	AuthorName varchar(100),
	PublishingDate DATE
); GO

-- Rental storage
CREATE TABLE Reservation (
	ReservationId int PRIMARY KEY IDENTITY(0, 1),
	PersonId int FOREIGN KEY REFERENCES Person(PersonId),
	BookId int FOREIGN KEY REFERENCES Book(BookId),
	OrderDate DATE DEFAULT GETDATE(),
	PickupDate DATE NOT NULL,
	ReturnDate DATE NOT NULL,
	Quantity int DEFAULT 1 CHECK (Quantity > 0),
	IsDone bit DEFAULT 0

	-- EXPLANATION:
	-- @PersonId		@OrderDate:	I need @BookId ready to pick it up on @Pickup
	-- LibrarySystem	@OrderDate:	You should return it @DeadlineDate
); GO

-- Save everything
COMMIT TRAN InitDatabase; GO
