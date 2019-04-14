CREATE VIEW BookInfo AS (
	 SELECT dbo.Book.BookId,
	 Book.BookTitle,
	 Book.BookDescription,
	 Book.AuthorId,
	 Book.PublisherId,
	 Book.PublishingDate,
	 Book.ThumbnailImage,
	 Author.AuthorName, 
	 Publisher.PublisherName,
	 (Book.Quantity - (SELECT COALESCE(SUM(Quantity),0) FROM Reservation WHERE Reservation.BookId = dbo.Book.BookId AND IsDone = 0)) AS Quantity,
	 (SELECT MIN(ReturnDate) FROM Reservation WHERE Reservation.BookId = dbo.Book.BookId AND IsDone = 0) AS NextAvailable
	 FROM dbo.Book 
	 LEFT OUTER JOIN dbo.Author ON dbo.Author.AuthorId = dbo.Book.AuthorId
	 LEFT OUTER JOIN dbo.Publisher ON dbo.Publisher.PublisherId = dbo.Book.PublisherId
);