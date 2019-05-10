----------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------- DO NOT RUN BEFORE INSERTING YOUR PARAMETERS ----------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------- FOR PREVIEW PURPOSE ---------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------


----------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------- API BOOK ----------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------

/* api/ApiBook/GetMostPopular */
SELECT TOP(@count) * FROM BookInfo LEFT OUTER JOIN (
	SELECT BookId, COUNT(ReservationId) AS ReservationCount FROM Reservation GROUP BY BookId
) AS Res ON BookInfo.BookId = Res.BookId ORDER BY ReservationCount DESC


/* api/ApiBook/Search */
SELECT * FROM (
	SELECT ROW_NUMBER() OVER(ORDER BY _) AS[Row], BookInfo.* FROM BookInfo 
	LEFT OUTER JOIN(Select COUNT(BookId) AS[Count], BookId FROM Reservation GROUP BY BookId) AS Res 
	ON BookInfo.BookId = Res.BookId WHERE (BookTitle LIKE CONCAT('{3}',@key,'{3}') OR _=_) 
	AND(AuthorName LIKE CONCAT('{3}',@key,'{3}') OR _=_) AND (PublisherName LIKE CONCAT('{3}',@key,'{3}') OR _=_)
	AND (PublishingDate IS NULL OR YEAR(PublishingDate) BETWEEN @min AND @max)
) AS Result WHERE[Row] BETWEEN @start AND @end

SELECT COUNT(*) FROM BookInfo LEFT OUTER JOIN(
	Select COUNT(BookId) AS[Count], BookId FROM Reservation GROUP BY BookId) AS Res ON BookInfo.BookId = Res.BookId 
	WHERE (BookTitle LIKE CONCAT('{3}',@key,'{3}') OR _=_) AND (AuthorName LIKE CONCAT('{3}',@key,'{3}') OR _=_) 
	AND (PublisherName LIKE CONCAT('{3}',@key,'{3}') OR _=_) AND (PublishingDate IS NULL OR YEAR(PublishingDate) BETWEEN @min AND @max
)


/* api/ApiBook/Get */
Select * FROM BookInfo WHERE BookId = @id


/* api/ApiBook/Rent */
INSERT INTO Reservation (BookId, PickupDate, ReturnDate, IsDone, PersonId, Quantity) VALUES(@bookId, @date, @retDate, 0, @personId, 1)


/* api/ApiBook/GetBooks */
SELECT * FROM Book LEFT OUTER JOIN Author ON Author.AuthorId = Book.AuthorId LEFT OUTER JOIN Publisher 
ON Publisher.Publisherid = Book.PublisherId


/* api/ApiBook/GetAuthors */
SELECT * FROM Author


/* api/ApiBook/GetPublishers */
SELECT * FROM Publisher


/* api/ApiBook/UpdateBook */
UPDATE Book SET _ = _ WHERE BookId = @bid


/* api/ApiBook/LibrarianSearch */
SELECT * FROM ( 
	SELECT ROW_NUMBER() OVER(ORDER BY DateAdded DESC) AS[Row], Book.*, AuthorName, PublisherName FROM Book 
	LEFT OUTER JOIN Author ON Author.AuthorId = Book.AuthorId LEFT OUTER JOIN Publisher 
	ON Publisher.PublisherId = Book.PublisherId WHERE(BookTitle LIKE CONCAT('%', @key, '%')) 
) AS Result WHERE[Row] BETWEEN @start AND @end


/* api/ApiBook/Delete */
DELETE FROM Reservation WHERE BookId = @bid
DELETE FROM Book WHERE BookId = @bid


/* api/ApiBook/UpdateImage */
UPDATE Book SET ThumbnailImage = @img WHERE BookId = @id


/* api/ApiBook/AddBook */
INSERT INTO Book (BookTitle, BookDescription, AuthorId, PublisherId, Price, PublishingDate) 
VALUES (@btitle, @bdesc, @aid, @pid, @price, @pubDate)


/* api/ApiBook/AddPublisher */
INSERT INTO Publisher (PublisherName) VALUES (@pubname)


/* api/ApiBook/AddAuthor */
INSERT INTO Author (AuthorName) VALUES (@auname)


/* api/ApiBook/DeleteAuthor */
UPDATE Book SET AuthorId = NULL WHERE AuthorId = @aid
DELETE FROM Author WHERE AuthorId = @aid

/* api/ApiBook/DeletePublisher */
UPDATE Book SET PublisherId = NULL WHERE PublisherId = @pid
DELETE FROM Publisher WHERE PublisherId = @pid


----------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------- API ACCOUNT -------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------

/* api/AccountApi/Login */
SELECT PersonId FROM PersonInfo WHERE Username = @user AND UserPassword = @pass


/* api/AccountApi/Signup */
Select PersonId FROM PersonInfo WHERE username = @user
INSERT INTO Person(Username, UserPassword) VALUES (@user, @pass)
INSERT INTO Student(PersonId) VALUES (@id)


/* api/AccountApi/GetPerson */
SELECT * FROM PersonInfo WHERE PersonId = @id


/* api/AccountApi/GetPersonOrders */
SELECT TOP(@count) Reservation.*, Book.BookTitle, Reservation.Quantity * Book.Price AS Price,
(CASE WHEN IsDone = 1 THEN 0 WHEN IsPickedUp = 1 AND GETDATE() > ReturnDate THEN 3 WHEN IsPickedUp = 1 THEN 2 ELSE 1 END) 
AS Status FROM Reservation JOIN Book ON Book.BookId = Reservation.BookId WHERE PersonId = @id ORDER BY OrderDate DESC


/* api/AccountApi/UpdateProfile */
UPDATE Person SET Email = @email, Phone = @phone WHERE PersonId = @id


/* api/AccountApi/SetProfilePicture */
UPDATE Person SET ProfileImage = @img WHERE PersonId = @id


/* api/AccountApi/AddPerson */
INSERT INTO Person (Username , UserPassword) VALUES (@lname, @lpass) 
INSERT INTO Librarian (PersonId) VALUES (@pid)
INSERT INTO Professor (PersonId) VALUES (@pid)
INSERT INTO Maintainer (PersonId) VALUES (@pid)


/* api/AccountApi/GetLibrarians */
SELECT * FROM Librarian JOIN PersonInfo ON PersonInfo.PersonId = Librarian.PersonId WHERE Librarian.PersonId != @pid


/* api/AccountApi/GetPersons */
SELECT * FROM PersonInfo WHERE PersonInfo.PersonId != @pid

/* api/AccountApi/DeletePerson */
UPDATE Person SET Deleted = 1 WHERE PersonId = @pid


/* api/AccountApi/ChangePassword */
UPDATE Person SET UserPassword = @pass WHERE PersonId = @pid


----------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------- API RESERVATION ---------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------

/* api/ApiReservation/ReservationSearch */
SELECT Reservation.*, PersonInfo.Username, Book.Price * Reservation.Quantity AS Price, Book.BookTitle, 
(CASE WHEN IsDone = 1 THEN 0 WHEN IsPickedUp = 1 AND GETDATE() > ReturnDate THEN 3 WHEN IsPickedUp = 1 THEN 2 ELSE 1 END) AS Status
FROM Reservation JOIN PersonInfo ON PersonInfo.PersonId = Reservation.PersonId JOIN Book ON Reservation.BookId = Book.BookId 
WHERE IsDone = 0 AND (Username LIKE CONCAT('%', @key ,'%')OR BookTitle LIKE CONCAT('%', @key, '%')) 
AND (CAST(PickupDate AS DATE) = CAST(GETDATE() AS DATE) OR CAST(ReturnDate AS DATE) = CAST(GETDATE() AS DATE) OR _=_)
ORDER BY PickupDate, OrderDate


/* api/ApiReservation/Checkout */
UPDATE Reservation SET IsDone = (CASE WHEN IsPickedUp = 0 THEN 0 ELSE 1 END), IsPickedUp = 1 WHERE ReservationId = @rid

----------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------
