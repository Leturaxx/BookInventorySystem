
const API_URL = "http://localhost:5234/api/books";

const form = document.getElementById("bookForm");
const table = document.getElementById("bookTable");
const message = document.getElementById("message");

let editingId = null;

// Create a table cell
function makeCell(value) {
    const cell = document.createElement("td");
    cell.textContent = value;
    return cell;
}

// GET: Load all books
async function loadBooks() {
    try {
        const response = await fetch(API_URL);

        if (!response.ok) {
            throw new Error("Could not load books");
        }

        const books = await response.json();

        table.innerHTML = "";

        books.forEach(book => {
            const row = document.createElement("tr");

            const actions = document.createElement("td");

            const editButton = document.createElement("button");
            editButton.textContent = "Edit";
            editButton.onclick = () => editBook(book);

            const deleteButton = document.createElement("button");
            deleteButton.textContent = "Delete";
            deleteButton.className = "delete";
            deleteButton.onclick = () => deleteBook(book.id);

            actions.append(editButton, deleteButton);

            row.append(
                makeCell(book.id),
                makeCell(book.title),
                makeCell(book.author),
                makeCell("₦" + Number(book.price).toLocaleString()),
                actions
            );

            table.appendChild(row);
        });

    } catch (error) {
        message.textContent = "Error: " + error.message;
    }
}

// POST or PUT: Add or update a book
form.addEventListener("submit", async function(event) {
    event.preventDefault();

    const book = {
        title: document.getElementById("title").value.trim(),
        author: document.getElementById("author").value.trim(),
        price: Number(document.getElementById("price").value)
    };

    const isEditing = editingId !== null;

    const url = isEditing
        ? `${API_URL}/${editingId}`
        : API_URL;

    const method = isEditing ? "PUT" : "POST";

    try {
        const response = await fetch(url, {
            method: method,
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(book)
        });

        if (!response.ok) {
            throw new Error("Could not save book");
        }

        resetForm();
        await loadBooks();

        message.textContent = isEditing
            ? "Book updated successfully!"
            : "Book added successfully!";

    } catch (error) {
        message.textContent = "Error: " + error.message;
    }
});

// Fill the form for editing
function editBook(book) {
    editingId = book.id;

    document.getElementById("title").value = book.title;
    document.getElementById("author").value = book.author;
    document.getElementById("price").value = book.price;

    document.getElementById("formTitle").textContent = "Edit Book";
    document.getElementById("saveButton").textContent = "Save Changes";
    document.getElementById("cancelButton").hidden = false;

    window.scrollTo({ top: 0, behavior: "smooth" });
}

// DELETE: Remove a book
async function deleteBook(id) {
    if (!confirm("Are you sure you want to delete this book?")) {
        return;
    }

    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: "DELETE"
        });

        if (!response.ok) {
            throw new Error("Could not delete book");
        }

        if (editingId === id) {
            resetForm();
        }

        await loadBooks();
        message.textContent = "Book deleted successfully!";

    } catch (error) {
        message.textContent = "Error: " + error.message;
    }
}

// Reset the form
function resetForm() {
    editingId = null;
    form.reset();

    document.getElementById("formTitle").textContent = "Add a Book";
    document.getElementById("saveButton").textContent = "Add Book";
    document.getElementById("cancelButton").hidden = true;
}

document.getElementById("cancelButton").onclick = resetForm;

// Display books when the website opens
loadBooks();
