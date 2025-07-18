using Xunit;
using Moq;
using library.Controllers;
using library.Data;
using library.Models; // Assuming your Book model is here
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject1
{
    public class UnitTest1
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly AdController _controller;

        public UnitTest1()
        {
            // Setup DbContext mocking
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AspnetIdentityYt")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            _mockContext = new Mock<ApplicationDbContext>(options);
            _mockContext.Setup(m => m.Books).Returns(dbContext.Books);

            _controller = new AdController(_mockContext.Object);
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnViewWithBooks()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var controller = new AdController(context);

            var books = new List<Book>
    {
        new Book { BookId = 1, Title = "Book1", Author = "Author1", Genre = "Genre1" },
        new Book { BookId = 2, Title = "Book2", Author = "Author2", Genre = "Genre2" }
    };

            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetAllBooks();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewBagBooks = Assert.IsAssignableFrom<List<Book>>(viewResult.ViewData["Books"]);
            Assert.Equal(2, viewBagBooks.Count);
        }


        [Fact]
        public async Task AddAnBook_ValidData_ShouldRedirectToAddBook()
        {
            // Act
            var result = await _controller.AddAnBook("TestTitle", "TestAuthor", "TestGenre");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddBook", redirectResult.ActionName);
        }

        [Fact]
        public void DeleteBook_ValidId_ShouldRedirectToGetAllBooks()
        {
            // Arrange
            var book = new Book { BookId = 14, Title = "Test", Author = "Author", Genre = "Genre" };
            _mockContext.Object.Books.Add(book);
            _mockContext.Object.SaveChanges();

            // Act
            var result = _controller.DeleteBook(14);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("GetAllBooks", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_ValidId_ShouldReturnViewWithBook()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Test", Author = "Author", Genre = "Genre" };
            await _mockContext.Object.Books.AddAsync(book);
            await _mockContext.Object.SaveChangesAsync();

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Book>(viewResult.Model);
            Assert.Equal(book.BookId, model.BookId);
        }

        [Fact]
        public async Task Edit_ValidBook_ShouldRedirectToGetAllBooks()
        {
            // Arrange
            var book = new Book { BookId = 1, Title = "Original", Author = "Author", Genre = "Genre" };
            await _mockContext.Object.Books.AddAsync(book);
            await _mockContext.Object.SaveChangesAsync();

            var updatedBook = new Book { BookId = 1, Title = "Updated", Author = "UpdatedAuthor", Genre = "UpdatedGenre" };

            // Act
            var result = await _controller.Edit(updatedBook);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("GetAllBooks", redirectResult.ActionName);
        }
        [Fact]
        public async Task AddBook_Should_ThrowException_When_TitleIsMissing()
        {
            // Arrange
            var invalidBook = new Book { Title = "", Author = "Author", Genre = "Fiction" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _controller.AddAnBook(invalidBook.Title, invalidBook.Author, invalidBook.Genre);
            });
        }
        [Fact]
        public async Task AddBook_Should_AddBook_When_ValidData()
        {
            // Arrange: InMemoryDatabase kullanarak ApplicationDbContext oluştur
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var controller = new AdController(context);

            var newBook = new Book { Title = "Test Book", Author = "Author", Genre = "Fiction" };

            // Act: Kitap ekleme işlemini gerçekleştir
            await controller.AddAnBook(newBook.Title, newBook.Author, newBook.Genre);

            // Assert: Kitabın veritabanına eklenip eklenmediğini kontrol et
            var addedBook = context.Books.FirstOrDefault(b => b.Title == "Test Book");
            Assert.NotNull(addedBook);
            Assert.Equal("Author", addedBook.Author);
            Assert.Equal("Fiction", addedBook.Genre);
        }

    }
}

