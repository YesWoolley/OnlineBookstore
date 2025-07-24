# Section 6: Building and Managing Controllers (API Endpoints)

## 1. Introduction
Welcome to the API development phase! In this section, you'll learn the concepts behind RESTful API controllers, services, and DTOs for your Online Bookstore. This section focuses on **concepts and preparation**—actual code implementation comes in Section 7.

---

## 2. Why Use Controllers, Services, and DTOs?
A well-structured API separates responsibilities:
- **Controllers** handle HTTP requests and responses.
- **Services** contain business logic and data processing.
- **DTOs** (Data Transfer Objects) define the data sent to and from the API.

**Benefits:**
- Easier to maintain and test
- Improved security (no leaking internal data)
- Clearer, more organized code

---

## 3. The Restaurant Analogy
Imagine your API as a restaurant:
- **Controller = Waiter:** Takes orders, serves food (handles requests/responses)
- **Service = Chef:** Prepares the food (business logic, data)
- **DTO = Plate:** Carries only what the customer needs (data transfer)

---

## 4. How They Work Together

| Layer        | Role            | Analogy   | Example Responsibility                |
|--------------|-----------------|-----------|---------------------------------------|
| Controller   | HTTP Layer      | Waiter    | Receives requests, returns responses  |
| Service      | Business Logic  | Chef      | Processes data, applies rules         |
| DTO          | Data Transfer   | Plate     | Carries only needed data              |

---

## 5. Step-by-Step Preparation

### Step 1: Install Required NuGet Packages
Before building controllers and services, install these essential NuGet packages in your `OnlineBookstore` project:

**Via Package Manager Console:**
```powershell
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
Install-Package AutoMapper.Extensions.Microsoft.DependencyInjection
```

**Via .NET CLI:**
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

These packages enable database access, identity management, and object mapping for your API.

---

### Step 2: Understanding API Controllers
API Controllers are like **waiters** in a restaurant—they take requests from clients (frontend) and serve responses. They handle HTTP requests and return data in JSON format.

**How They Work:**
```
Frontend (React) → HTTP Request → API Controller → Database → Response → Frontend
```

---

### Step 3: Tips for Building Controllers and DTOs

- **Tip 1: Use IEnumerable for Read-Only Collections:** `IEnumerable<T>` represents a collection you can loop through but not modify, making it ideal for returning lists of data from your API.

- **Tip 2: Use Dependency Injection for Cleaner Code:**
  Dependency Injection (DI) lets you declare what your class needs in its constructor, register those dependencies in `Program.cs`, and have ASP.NET Core automatically provide them—no manual `new` required!
  
  **Example:**
  ```csharp
  // 1. Declare dependencies in your class constructor
  public class AuthorsService
  {
      private readonly AppDbContext _context;
      private readonly ILogger<AuthorsService> _logger;
      public AuthorsService(AppDbContext context, ILogger<AuthorsService> logger)
      {
          _context = context;
          _logger = logger;
      }
  }

  // 2. Register dependencies in Program.cs
  builder.Services.AddDbContext<AppDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
  builder.Services.AddScoped<AuthorsService>();

  // 3. The DI system injects them automatically
  public class AuthorsController : ControllerBase
  {
      public AuthorsController(AuthorsService authorsService) { /* ... */ }
  }
  ```
  This makes your code more testable, maintainable, and clean.

  **Supplement: Why DI Makes Unit Testing Easy**

  With Dependency Injection (DI) – Easy to Mock
  Suppose you have a service that depends on an interface:
  ```csharp
  public interface IEmailSender
  {
      void Send(string to, string subject, string body);
  }

  public class NotificationService
  {
      private readonly IEmailSender _emailSender;
      public NotificationService(IEmailSender emailSender)
      {
          _emailSender = emailSender;
      }
      public void Notify(string userEmail)
      {
          _emailSender.Send(userEmail, "Hello!", "Welcome!");
      }
  }
  ```

  **Unit Test with a Mock:**
  ```csharp
  // Using Moq or a similar library
  var mockEmailSender = new Mock<IEmailSender>();
  var service = new NotificationService(mockEmailSender.Object);

  service.Notify("test@example.com");

  // Assert that Send was called
  mockEmailSender.Verify(m => m.Send("test@example.com", "Hello!", "Welcome!"), Times.Once);
  ```
  - You can inject a mock (`mockEmailSender.Object`) instead of a real email sender.
  - No real emails are sent during testing.

  Without Dependency Injection (Manual Instantiation) – Hard to Mock
  Suppose you write your service like this:
  ```csharp
  public class NotificationService
  {
      private readonly IEmailSender _emailSender = new RealEmailSender(); // Hardcoded!
      public void Notify(string userEmail)
      {
          _emailSender.Send(userEmail, "Hello!", "Welcome!");
      }
  }
  ```
  **Unit Test:**
  - You cannot inject a mock—the service always uses `RealEmailSender`.
  - Real emails might be sent during tests (bad!).
  - You’d have to change the code to test it properly.

  **Summary:**
  - With DI: You can inject a mock or fake dependency for testing. Your tests are isolated, safe, and fast.
  - Without DI: You’re stuck with the real implementation. Tests are hard to write and may have unwanted side effects.

  **This is why DI is so powerful and valuable for unit testing!**

- **Tip 3: Leverage AutoMapper to Map Entities to DTOs:**
  Map between your models and DTOs automatically to reduce repetitive code.

  **Short Example:**
  ```csharp
  var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(authors);
  ```

  **What does AutoMapper's `Map` do?**
  - `Map` takes an object (or collection of objects) of one type and creates a new object (or collection) of another type, copying over the relevant data.
  - For example, it can convert a list of `Author` entities (from your database) into a list of `AuthorDto` objects (Data Transfer Objects).

  **What is being mapped?**
  - **From:** Your entity objects (e.g., `authors` — a list of `Author` entities retrieved from your database).
  - **To:** DTO objects (e.g., `IEnumerable<AuthorDto>` — a list of `AuthorDto` objects, which are simplified versions of your entities, designed for sending data to the client).

  **Why do we use mapping/DTOs?**
  - Entities (like `Author`) often contain extra data, navigation properties, or sensitive information you don't want to expose to the client.
  - DTOs (like `AuthorDto`) are designed to contain only the data you want to send to the client—nothing more, nothing less.
  - AutoMapper automates the process of copying data from your entities to your DTOs, so you don't have to write repetitive code.

  **Example:**
  ```csharp
  public class Author {
      public int Id { get; set; }
      public string Name { get; set; }
      public string Biography { get; set; }
      public List<Book> Books { get; set; }
  }

  public class AuthorDto {
      public int Id { get; set; }
      public string Name { get; set; }
  }

  // Mapping usage:
  var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(authors);
  // AutoMapper will create a new list of AuthorDto objects, copying the Id and Name from each Author in the authors list.
  ```

  **Summary:**
  Using `_mapper.Map<IEnumerable<AuthorDto>>(authors)` converts a list of `Author` entities into a list of `AuthorDto` objects, making it easy to send only the necessary data to the client.

- **Tip 4: When to use IActionResult and ActionResult<T>:**

  - **POST, PUT, DELETE** (actions that may return only status codes):
    - **Use `IActionResult`**
    - When you only want to return status codes or response types, not a data object directly.
    
    **Short Example:**
    ```csharp
    [HttpDelete("{id}")]
    public IActionResult DeleteBook(int id)
    {
        // ... delete logic ...
        return NoContent();
    }
    ```

  - **GET** (actions that return data or a status code):
    - **Use `ActionResult<T>`**
    - When you want to return data (like a book or list of books) or a status code (like `NotFound`).
    
    **Short Example:**
    ```csharp
    [HttpGet("{id}")]
    public ActionResult<BookDto> GetBook(int id)
    {
        var book = _context.Books.Find(id);
        if (book == null)
            return NotFound();
        return book;
    }
    ```

  **Example:**
  ```csharp
  [HttpPost]
  public IActionResult CreateBook(BookDto dto)
  {
      if (!ModelState.IsValid)
          return BadRequest(ModelState);
      // ... create logic ...
  }

  [HttpPut("{id}")]
  public IActionResult UpdateBook(int id, BookDto dto)
  {
      if (!ModelState.IsValid)
          return BadRequest(ModelState);
      // ... update logic ...
  }
  ```

  **Summary:**
  Use `ModelState.IsValid` to ensure incoming data is valid before processing create or update operations in your API.

- **Tip 5: Using ModelState.IsValid for Validation:**

  `ModelState.IsValid` is a property in ASP.NET Core controllers that checks if the data sent by the client (usually to a POST or PUT action) meets all the validation rules defined on your model or DTO (like `[Required]`, `[StringLength]`, etc.).

  You typically use `ModelState.IsValid` in functions that handle creating (POST) or updating (PUT) resources, such as:
  - `CreateBook(BookDto dto)` (POST)
  - `UpdateBook(int id, BookDto dto)` (PUT)

  **Short Example:**
  ```csharp
  if (!ModelState.IsValid)
      return BadRequest(ModelState);
  ```

  **Full Example:**
  ```csharp
  [HttpPost]
  public IActionResult CreateBook(BookDto dto)
  {
      if (!ModelState.IsValid)
          return BadRequest(ModelState);
      // ... create logic ...
  }

  [HttpPut("{id}")]
  public IActionResult UpdateBook(int id, BookDto dto)
  {
      if (!ModelState.IsValid)
          return BadRequest(ModelState);
      // ... update logic ...
  }
  ```

  **Summary:**
  Use `ModelState.IsValid` to ensure incoming data is valid before processing create or update operations in your API.

- **Tip 6: Using CreatedAtAction for Resource Creation Responses:**

  `CreatedAtAction` is a helper method in ASP.NET Core controllers that returns a `201 Created` HTTP response. It also includes a Location header in the response, pointing to the URL where the newly created resource can be retrieved (usually a GET endpoint).

  **Example:**
  ```csharp
  return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
  ```

  **What does the response look like?**
  - **Status code:** `201 Created`
  - **Location header:** URL to the new resource (e.g., `/api/authors/5`)
  - **Body:** The created resource (the `author` object)

  **Why use it?**
  - Follows REST best practices by telling the client where to find the new resource.
  - Provides both the new resource and its location in a single response.

- **Tip 7: When to Check for null or Failure in Controllers:**

  **In short:**
  - Check for `null` after fetching data from the database or service.
  - Check for `false` or failure after calling a service or method that can fail.
  - Return an appropriate HTTP response (`NotFound`, `BadRequest`, etc.) when the check fails.

  **Brief Examples:**
  
  *Check for null after fetching:*
  ```csharp
  var author = _context.Authors.Find(id);
  if (author == null)
      return NotFound();
  return author;
  ```

  *Check for false after a service call:*
  ```csharp
  var result = _authorService.DeleteAuthor(id);
  if (!result)
      return NotFound();
  return NoContent();
  ```

  **Supplement:**
  If your service (e.g., `SearchAuthorsAsync`) always returns a list (even if empty), you do **not** need a null check in your controller. Returning an empty list (`[]`) is standard for search endpoints when no results are found.

  **How to always return an empty list:**
  - Use `.ToListAsync()` or `.ToList()` in your service method, as these always return a list (never null), even if there are no matches.
  - If you use mapping or other logic that could return null, use the null-coalescing operator (`??`) to ensure a list is always returned.

  **Example:**
  ```csharp
  public async Task<IEnumerable<AuthorDto>> SearchAuthorsAsync(string q)
  {
      var authors = await _context.Authors
          .Where(a => a.Name.Contains(q))
          .ToListAsync(); // Always returns a list

      var result = _mapper.Map<IEnumerable<AuthorDto>>(authors);

      // Ensure result is never null
      return result ?? new List<AuthorDto>();
  }
  ```

- **Tip 8: When to Use ArgumentException and InvalidOperationException:**

  **1. ArgumentException**
  - **Use when:** An argument is invalid (wrong value, format, or not allowed), but not null.
  - **Example:**
    ```csharp
    public void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        // ... logic ...
    }
    ```
  - *Apply this in controller/service logic when the argument is not acceptable, but not missing.*

  **2. InvalidOperationException**
  - **Use when:** The method call is invalid for the object’s current state, even if arguments are valid.
  - **Example:**
    ```csharp
    public void Withdraw(decimal amount)
    {
        if (!_accountIsOpen)
            throw new InvalidOperationException("Cannot withdraw from a closed account.");
        // ... logic ...
    }
    ```
  - *Apply this in controller/service logic when the operation doesn’t make sense in the current state.*

---

## 6. How Controllers, Services, and DTOs Work Together (Recap)
Think of your API as a restaurant:
- The **Controller** is the waiter, handling requests and responses.
- The **Service** is the chef, preparing the business logic and data.
- The **DTO** is the plate, carrying only the data the customer (frontend) needs.

This separation keeps your code clean, secure, and easy to maintain.

---

## 7. What's Next?
In Section 7, you'll put these concepts into practice by implementing backend services and controllers for author management. Continue to the next section for hands-on coding!