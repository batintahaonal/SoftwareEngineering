using Humanizer.Localisation;
using library.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace library.Controllers
{
    public class AdController : Controller
    {
        
        private readonly ApplicationDbContext context;

        public AdController(ApplicationDbContext context)
        {
            this.context = context;
        }
        
        
     
        public IActionResult Index()
        {
            return View();
        }

    

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await this.context.Books.ToListAsync();
            ViewBag.Books = books;

            return View();
        }
        public IActionResult AddBook()
        {
           
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAnBook(string title, string author, string genre)
        {
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(author) && !string.IsNullOrWhiteSpace(genre))
            {
                var newBook = new Book { Title = title, Author = author, Genre = genre };
                await this.context.Books.AddAsync(newBook);
                await this.context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Tüm boşluklar dolu olmalı.");

            }
            return RedirectToAction("AddBook");
        }


        [HttpPost]
        public IActionResult DeleteBook(int id)
        {
            var book = this.context.Books.Find(id);
            if (book != null)
            {
                this.context.Books.Remove(book);
                this.context.SaveChanges();
            }
            return RedirectToAction("GetAllBooks");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            
            var book = await this.context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound(); 
            }
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Book book)
        {
            var existingBook = await this.context.Books.FindAsync(book.BookId);
            if (existingBook == null)
            {
                ModelState.AddModelError("", "Düzenlemek istediğiniz kitap bulunamadı.");
                ViewBag.Books = this.context.Books.ToList();
                return RedirectToAction("GetAllBooks", "Ad");
            }

            if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author) || string.IsNullOrWhiteSpace(book.Genre))
            {
                ModelState.AddModelError("", "Tüm alanları doldurmanız gerekmektedir.");
                ViewBag.Books = this.context.Books.ToList();
                return View(book);
            }

            try
            {
                existingBook.Author = book.Author;
                existingBook.Genre = book.Genre;
                existingBook.Title = book.Title;

                await this.context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Bir hata oluştu: {ex.Message}");
                ViewBag.Books = this.context.Books.ToList();
                return View(book);
            }

            return RedirectToAction("GetAllBooks", "Ad");
        }






    }
}
