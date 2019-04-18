-- Creating the database
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'OnlineLibrarySystem')
BEGIN
	DROP DATABASE OnlineLibrarySystem;
END

CREATE DATABASE OnlineLibrarySystem;
GO

USE OnlineLibrarySystem;

-- Define some domains
CREATE TYPE small_field FROM varchar(32);
CREATE TYPE field FROM varchar(128);
CREATE TYPE long_field FROM varchar(1024);
CREATE TYPE url_field FROM varchar(2083);
GO

-- All personnels should be stored in this table
CREATE TABLE Person (
	PersonId int PRIMARY KEY IDENTITY(0, 1),
	Username small_field NOT NULL UNIQUE,
	UserPassword field NOT NULL
);

-- Personnels can be in one of the following tables
CREATE TABLE Professor (
	PersonId int PRIMARY KEY FOREIGN KEY REFERENCES Person(PersonId)
);

CREATE TABLE Student (
	PersonId int PRIMARY KEY FOREIGN KEY REFERENCES Person(PersonId)
);

CREATE TABLE Librarian (
	PersonId int PRIMARY KEY FOREIGN KEY REFERENCES Person(PersonId)
);

-- Personnels with extended privileges
CREATE TABLE Maintainer (
	PersonId int PRIMARY KEY FOREIGN KEY REFERENCES Librarian(PersonId)
);

CREATE TABLE Author (
	AuthorId int PRIMARY KEY IDENTITY(0, 1),
	AuthorName field NOT NULL
);

CREATE TABLE Publisher (
	PublisherId int PRIMARY KEY IDENTITY(0, 1),
	PublisherName field NOT NULL
);

-- Books are stored in here
CREATE TABLE Book (
	BookId int PRIMARY KEY IDENTITY(0, 1),
	BookTitle field NOT NULL,
	BookDescription long_field,
	AuthorId int FOREIGN KEY REFERENCES Author,
	PublisherId int FOREIGN KEY REFERENCES Publisher,
	PublishingDate DATE,
	Quantity int NOT NULL DEFAULT 1,
	ThumbnailImage url_field DEFAULT NULL,
);

-- Rental storage
CREATE TABLE Reservation (
	ReservationId int PRIMARY KEY IDENTITY(0, 1),
	PersonId int FOREIGN KEY REFERENCES Person(PersonId),
	BookId int FOREIGN KEY REFERENCES Book(BookId),
	OrderDate DATETIME DEFAULT GETDATE(),
	PickupDate DATE NOT NULL,
	ReturnDate DATE NOT NULL,
	Quantity int DEFAULT 1 CHECK (Quantity > 0),
	IsDone bit DEFAULT 0

	-- EXPLANATION:
	-- @PersonId		@OrderDate:	I need @BookId ready to pick it up on @Pickup
	-- LibrarySystem	@OrderDate:	You should return it @DeadlineDate
);
