
var builder = WebApplication.CreateBuilder(args);

// Set up Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allow our frontend to connect
builder.Services.AddCors(options =>
{
    options.AddPolicy("Client", policy =>
        policy.WithOrigins(
            "http://localhost:5501",
            "http://127.0.0.1:5501")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Client");

// Our temporary database
var books = new List<Book>
{
    
new(1, "Things Fall Apart", "Chinua Achebe", 3500m)
{
    Description = "A story about Okonkwo and life in an Igbo community during colonial change."
},
new(2, "The Alchemist", "Paulo Coelho", 4000m)
{
    Description = "A story about a young shepherd searching for his personal treasure."
},
new(3, "Clean Code", "Robert C. Martin", 6500m)
{
    Description = "A programming book that teaches developers how to write cleaner and easier-to-maintain code."
}

};

int nextId = 4;

// GET: View all books
app.MapGet("/api/books", () =>
{
    return Results.Ok(books);
});

//GET: View a Book by it's ID
app.MapGet("/api/books/{id:int}", (int id) =>
{
    var book = books.FirstOrDefault(b => b.Id == id);

    if (book == null)
    {
        return Results.NotFound("Book not found.");
    }

    return Results.Ok(book);
});

// POST: Add a new book
app.MapPost("/api/books", (BookInput input) =>
{
    if (string.IsNullOrWhiteSpace(input.Title) ||
        string.IsNullOrWhiteSpace(input.Author) ||
        input.Price <= 0)
    {
        return Results.BadRequest("Enter a title, author and valid price.");
    }

    var book = new Book(
        nextId++,
        input.Title.Trim(),
        input.Author.Trim(),
        input.Price
    );
    book.Description = input.Description?.Trim() ?? "";
    books.Add(book);

    return Results.Created($"/api/books/{book.Id}", book);
});

// PUT: Edit an existing book
app.MapPut("/api/books/{id:int}", (int id, BookInput input) =>
{
    var book = books.FirstOrDefault(b => b.Id == id);

    if (book == null)
        return Results.NotFound("Book not found.");

    if (string.IsNullOrWhiteSpace(input.Title) ||
        string.IsNullOrWhiteSpace(input.Author) ||
        input.Price <= 0)
    {
        return Results.BadRequest("Enter a title, author and valid price.");
    }

    book.Title = input.Title.Trim();
    book.Author = input.Author.Trim();
    book.Price = input.Price;

    if (input.Description != null)
    book.Description = input.Description.Trim();

    return Results.Ok(book);
});

// DELETE: Remove a book
app.MapDelete("/api/books/{id:int}", (int id) =>
{
    var book = books.FirstOrDefault(b => b.Id == id);

    if (book == null)
        return Results.NotFound("Book not found.");

    books.Remove(book);

    return Results.NoContent();
});

app.Run();

// A book and its information
class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = "";

    public Book(int id, string title, string author, decimal price)
    {
        Id = id;
        Title = title;
        Author = author;
        Price = price;
    }
}

// Information received from the frontend
record BookInput(string Title, string Author, decimal Price, string? Description = null);
