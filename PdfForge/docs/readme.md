# PdfForge

[![NuGet](https://img.shields.io/nuget/v/PdfForge.svg)](https://www.nuget.org/packages/PdfForge)

**PdfForge** is a library for rendering PDF documents into PNG images. It supports both saving pages to files and converting pages into byte arrays, making it a flexible solution for applications that need to work with PDF files in raster format.

## Features

- Open PDF documents from a file.
- Render PDF pages to PNG images.
- Save pages as PNG files.
- Get pages as byte arrays (PNG format).
- Supports scaling when rendering pages.

## Installation

Install via NuGet by adding a reference to the **PdfForge** package:

```bash
dotnet add package PdfForge
```

# Usage
## Opening Document

To work with a PDF document, create an instance of the `PdfForgeDocument` class and provide the file path:

```csharp
using PdfForge;

var pdfDocument = new PdfForgeDocument("sample.pdf");
Console.WriteLine($"Total Pages: {pdfDocument.TotalPages}");
```

## Saving a Page as a PNG File

You can save a specific PDF page as a PNG file by specifying the page number and output file path:

```csharp
pdfDocument.SavePageToPngFile(1, "page1.png");
```

## Getting a Page as a Byte Array
You can also retrieve a PDF page as a byte array (in PNG format), useful for handling images in memory:

```csharp
byte[] pageBytes = pdfDocument.GetPageBytes(1);
```

## Releasing Resources
It's important to release resources when the document is no longer needed. You can call `Dispose` manually or use `using` to automatically release resources:
```csharp
pdfDocument.Dispose();
```
Or with `using`:
```csharp
using var pdfDocument = new PdfForgeDocument("sample.pdf");
```

# Requirements
- .NET 8.0 or later.

# License
This project is licensed under the GPL-3.0 license.
