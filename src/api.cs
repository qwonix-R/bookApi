using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace bookApi;



[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly BookContext _context;

    private readonly string booksPath = "/app/uploads";


    // Внедрение зависимости через конструктор
    public BookController(BookContext context)
    {
        _context = context;
    }



    [HttpGet("{bookId:long}", Name = "GetBook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Book>> GetBook(long bookId)
    {
        var book = await _context.books.FindAsync(bookId);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }
    [HttpGet("{bookId:long}/content", Name = "GetPath")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPath(int bookId)
    {
        string bookPath = Path.Combine(booksPath, $"{bookId}.pdf");

        return Ok(new { bookPath });
    }
    [HttpGet("{bookId:long}/comment", Name = "GetComments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<Comment>>> GetComments(long bookId)
    {
        List<Comment> comments = _context.comments.Where(p => p.book_id == bookId).ToList();

        if (comments.Count == 0)
        {
            return NotFound();
        }

        return Ok(comments);
    }
    [HttpGet("{bookId:long}/comment/{commentId:long}", Name = "GetComment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Comment>> GetComment(long bookId, long commentId)
    {
        var comment = _context.comments.Where(p => p.id == commentId && p.book_id == bookId);

        if (comment == null)
        {
            return NotFound();
        }

        return Ok(comment);
    }
    [HttpPost("{bookId:long}/comment", Name = "PostComment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Comment>> PostComment(long bookId, [FromBody] Comment comment)
    {
        comment.book_id = bookId;
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        await _context.comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        return Created();
    }
    [HttpPost(Name = "PostBook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Book>> PostBook([FromBody] Book book)
    {
        
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        await _context.books.AddAsync(book);
        await _context.SaveChangesAsync();

        return Created();
    }
    [HttpPost("{bookId:long}/content", Name = "PostBookContent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> PostBookContent(IFormFile file, long bookId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Invalid file");
        }
        var allowedExtensions = new[] { ".txt", ".docx", ".doc", ".pdf", };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
            return BadRequest("Invalid file type");

        
        var filePath = Path.Combine(booksPath, $"{bookId.ToString()}.pdf");

        // Сохранение файла
        try
        {
           

            using (MemoryStream inputStream = new MemoryStream())
            {
                file.CopyTo(inputStream);
                inputStream.Position = 0;
                

                using (MemoryStream pdfStream = new MemoryStream())
                {
                    if (fileExtension != "pdf")
                    {
                        await PdfConverter.ConvertToPdf(inputStream, pdfStream, fileExtension);
                    }

                    pdfStream.Position = 0;

                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        pdfStream.WriteTo(fileStream);
                    }


                    return Created();
                }
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); return StatusCode(500);  }

    }
}