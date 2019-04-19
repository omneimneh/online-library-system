USE OnlineLibrarySystem;

DELETE FROM Reservation
DELETE FROM Book
DELETE FROM Author
DELETE FROM Publisher

INSERT INTO Author (AuthorName) VALUES ('Stephen Hawking')
DECLARE @A1 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('Albert Einstein')
DECLARE @A2 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('Steve McConnell')
DECLARE @A3 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('Avi Silberschatz')
DECLARE @A4 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('Chris Bernhardt')
DECLARE @A5 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('David Klein')
DECLARE @A6 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('G. H. Hardy')
DECLARE @A7 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('James Stewart')
DECLARE @A8 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('Richard Dawkins')
DECLARE @A9 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('David Klein')
DECLARE @A10 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('Bruce Alberts')
DECLARE @A11 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('Kenneth W. Clarkson')
DECLARE @A12 int = SCOPE_IDENTITY()
INSERT INTO Author (AuthorName) VALUES ('Harper Lee')
DECLARE @A13 int = SCOPE_IDENTITY()

INSERT INTO Publisher (PublisherName) VALUES ('McGraw-Hill Education')
DECLARE @P1 int = SCOPE_IDENTITY()
INSERT INTO Publisher (PublisherName) VALUES ('Hachette Livre')
DECLARE @P2 int = SCOPE_IDENTITY()
INSERT INTO Publisher (PublisherName) VALUES ('Penguin Random House')
DECLARE @P3 int = SCOPE_IDENTITY()


INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId, Quantity)
VALUES('A Brief History of Time', 'A Brief History of Time: From the Big Bang to Black Holes is a popular-science book on cosmology by British physicist Stephen Hawking. It was first published in 1988. Hawking wrote the book for nonspecialist readers with no prior knowledge of scientific theories.', @A1, '3-1-1988', @P1, 6)

INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId, Quantity)
VALUES('Relativity: The Special and the General Theory', 'Relativity: The Special and the General Theory began as a short paper and was eventually published as a book written by Albert Einstein', @A2, '1-1-1916', NULL, 1)

INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId, Quantity)
VALUES('Code Complete', 'Code Complete is a software development book, written by Steve McConnell and published in 1993 by Microsoft Press, encouraging developers to continue past code-and-fix programming and the big design up front and waterfall models.', @A3, '1-1-1993', @P3, 2)

INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId, Quantity)
VALUES('Operating System Concepts', 'New edition of the bestseller provides readers with a clear description of the concepts that underlie operating systems', @A4, '1-1-1982', @P1, 21)

INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId)
VALUES('Quantum Computing for Everyone', 'An accessible introduction to an exciting new area in computation, explaining such topics as qubits, entanglement, and quantum teleportation for the general reader.Quantum computing is a beautiful fusion', @A5, '2-22-2019', @P1)

INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId)
VALUES('Organic Chemistry as a Second Language', 'Building on the resounding success of the first volume (0-471-27235-3), Organic Chemistry as a Second Language, Volume 2 provides readers with clear, easy-to-understand explanations of fundamental principles.', @A6, '1-1-2004', @P1)

INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId)
VALUES('An Introduction to the Theory of Numbers', 'An Introduction to the Theory of Numbers is a classic book in the field of number theory, by G. H. Hardy and E. M. Wright. The book grew out of a series of lectures by Hardy and Wright and was first published in 1938.', @A7, '1-1-1938', @P2)

INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId)
VALUES('Calculus: Early Transcendentals', 'Appropriate for the traditional 2 or 3-term college calculus course, this textbook presents the fundamentals of calculus.', @A8, '1-1-1983', @p1)

INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId, Quantity)
VALUES('The Selfish Gene', 'The Selfish Gene is a 1976 book on evolution by Richard Dawkins, in which the author builds upon the principal theory of George C. Williams''s Adaptation and Natural Selection.', @A9, '1-1-1976', @P3, 5)

INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId, Quantity)
VALUES('Molecular Biology of the Cell', 'Molecular Biology of the Cell is a cellular and molecular biology textbook published by Garland Science and currently authored by Bruce Alberts, Alexander D. Johnson, Julian Lewis, David Morgan, Martin Raff, Keith Roberts and Peter Walter. The book was first published in 1983 and is now in its sixth edition.', @A10, '1-1-1983', @P1, 13)

INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId, Quantity)
VALUES('Business Law', 'A summarized case version of the best-selling title for this course.', @A11, '1-1-2005', @P2, 7)

INSERT INTO Book(BookTitle, BookDescription, AuthorId, PublishingDate, PublisherId, Quantity)
VALUES('To Kill a Mockingbird', 'To Kill a Mockingbird is a novel by Harper Lee published in 1960. Instantly successful, widely read in high schools and middle schools in the United States, it has become a classic of modern American literature winning the Pulitzer Prize.', @A12, '7-11-1960', @P3, 2)