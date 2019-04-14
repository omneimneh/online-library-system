CREATE VIEW BookInfo AS (
	Select Book.*, AuthorName, PublisherName FROM Book
	LEFT OUTER JOIN Author ON Author.AuthorId = Book.AuthorId
	LEFT OUTER JOIN Publisher ON Publisher.PublisherId = Book.PublisherId
)
