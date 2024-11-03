# Search Engine API Documentation

**Base URL:** `http://localhost:5009/api`

---

## 1. Index Gutenberg Book

**Endpoint:** `POST /index/gutenberg/{bookId}`

**Description:**  
Indexes a book from Project Gutenberg by its ID.

### Request Parameters:
- **Path Parameter:**  
  - `bookId` *(integer)* — The Project Gutenberg book ID to index.

### Success Response:
- **Status:** `200 OK`
- **Body:**
  ```json
  {
    "documentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "wordCount": 50000
  }
  ```

### Error Responses:
- **Status:** `404 Not Found`  
  When book is not found in Project Gutenberg.
  - **Body:**
    ```json
    {
      "message": "Book with ID {bookId} not found",
      "traceId": "00-1234567890abcdef-1234567890abcdef-00"
    }
    ```

- **Status:** `500 Internal Server Error`  
  When the server encounters a processing error.
  - **Body:**
    ```json
    {
      "message": "An unexpected error occurred.",
      "traceId": "00-1234567890abcdef-1234567890abcdef-00"
    }
    ```

---

## 2. Search Documents

**Endpoint:** `GET /search`

**Description:**  
Searches through indexed documents with pagination support.

### Request Parameters:
- **Query Parameters:**
  - `query` *(string, required)* — Search terms.
  - `pageNumber` *(integer, optional, default: 1)* — Page number.
  - `pageSize` *(integer, optional, default: 10, max: 50)* — Results per page.

### Success Response:
- **Status:** `200 OK`
- **Body:**
  ```json
  {
    "items": [
      {
        "documentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "title": "Pride and Prejudice",
        "path": "https://www.gutenberg.org/cache/epub/1342/pg1342.txt",
        "score": 0.75,
        "termOccurrences": [
          {
            "termId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "value": "pride",
            "frequency": 10
          }
        ]
      }
    ],
    "totalCount": 100,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 10
  }
  ```

### Error Responses:
- **Status:** `400 Bad Request`  
  When query parameters are invalid.
  - **Body:**
    ```json
    {
      "message": "Invalid search parameters",
      "traceId": "00-1234567890abcdef-1234567890abcdef-00"
    }
    ```

- **Status:** `500 Internal Server Error`  
  When the server encounters a processing error.
  - **Body:**
    ```json
    {
      "message": "An unexpected error occurred.",
      "traceId": "00-1234567890abcdef-1234567890abcdef-00"
    }
    ```

---

## 3. List Documents

**Endpoint:** `GET /document`

**Description:**  
Retrieves a paginated list of all indexed documents, ordered by creation date (newest first).

### Request Parameters:
- **Query Parameters:**
  - `page` *(integer, optional, default: 1)* — Page number.
  - `pageSize` *(integer, optional, default: 10, max: 50)* — Items per page.

### Success Response:
- **Status:** `200 OK`
- **Body:**
  ```json
  {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "title": "Pride and Prejudice",
        "path": "https://www.gutenberg.org/cache/epub/1342/pg1342.txt",
        "wordCount": 50000,
        "createdAt": "2024-01-31T12:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5,
    "totalCount": 50,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
  ```

### Error Responses:
- **Status:** `400 Bad Request`  
  When pagination parameters are invalid.
  - **Body:**
    ```json
    {
      "message": "Invalid pagination parameters",
      "traceId": "00-1234567890abcdef-1234567890abcdef-00"
    }
    ```

- **Status:** `500 Internal Server Error`  
  When the server encounters a processing error.
  - **Body:**
    ```json
    {
      "message": "An unexpected error occurred.",
      "traceId": "00-1234567890abcdef-1234567890abcdef-00"
    }
    ```

---

## 4. Get Search Engine Statistics

**Endpoint:** `GET /statistics`

**Description:**  
Retrieves statistical information about the search engine's indexed content, including document counts, term counts, and last indexing time.

### Request Parameters:
None

### Success Response:
- **Status:** `200 OK`
- **Body:**
  ```json
  {
    "totalDocuments": 42,
    "totalUniqueTerms": 15783,
    "totalTermOccurrences": 157392,
    "totalWordCount": 892456,
    "lastIndexedAt": "2024-03-15T14:30:22Z"
  }
  ```

### Response Fields:
- `totalDocuments` *(integer)* — Total number of indexed documents
- `totalUniqueTerms` *(integer)* — Number of unique terms across all documents
- `totalTermOccurrences` *(integer)* — Total number of term occurrences
- `totalWordCount` *(long)* — Total word count across all documents
- `lastIndexedAt` *(string, ISO 8601)* — Timestamp of the last indexed document

### Cache Behavior:
- Results are cached for 5 minutes
- HTTP response includes cache headers

### Example Request:
```bash
curl -X GET http://localhost:5009/api/statistics
```
