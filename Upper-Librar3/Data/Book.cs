using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace library.Data
{
    [Table("Book")]
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public string Genre { get; set; }
    }
}
