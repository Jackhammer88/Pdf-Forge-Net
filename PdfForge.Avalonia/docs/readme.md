# PdfForge.Avalonia

[![NuGet](https://img.shields.io/nuget/v/PdfForge.Avalonia.svg)](https://www.nuget.org/packages/PdfForge.Avalonia)

**PdfForge.Avalonia** is a library that provides a custom PdfView control for rendering and displaying PDF documents in Avalonia UI applications. The control is built on top of the PdfForge library, offering smooth PDF rendering and easy integration into Avalonia applications.

## Features

- Display PDF documents inside Avalonia applications.
- Render specific pages with customizable scale.
- Panning support for easy navigation within the document.
- Show loading indicators during rendering.
- Expose document dimensions for flexible UI layouts.

## Installation

Install the **PdfForge.Avalonia** package via NuGet:

```bash
dotnet add package PdfForge.Avalonia
```

You also need to add styles in your app:
```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://PdfForge.Avalonia/Styles.axaml"/>
</Application.Styles>
```

# Usage
## XAML Example

You can use the `PdfView` control directly in your XAML to display PDF content. Bind the `Document`, `PageNumber`, and `Scale` properties to control the document and rendering behavior:

```xml
<Window
xmlns:avalonia="clr-namespace:PdfForge.Avalonia;assembly=PdfForge.Avalonia">

<avalonia:PdfView Document="{Binding PdfDocument}"
                  PageNumber="{Binding PageNumber}"
                  Scale="{Binding Scale}" />
</Window>
```

## Code-Behind Example

You can control the PdfView via code by setting properties like `Document`, `PageNumber`, and `Scale`:

```csharp
var pdfView = new PdfView
{
    Document = new PdfForgeDocument("sample.pdf"),
    PageNumber = 0,
    Scale = 1.0
};
```

## Basic Example (XAML with Button Controls)
Here’s an example that demonstrates how to integrate PdfView with buttons for page navigation and zoom:

```xml
<Grid>
    <avalonia:PdfView x:Name="PdfViewer"
                      Document="{Binding PdfDocument}"
                      PageNumber="{Binding PageNumber}"
                      Scale="{Binding Scale}" />
    
    <StackPanel Orientation="Horizontal" VerticalAlignment="Bottom">
        <Button Content="Previous Page" Command="{Binding PrevPageCommand}" />
        <Button Content="Next Page" Command="{Binding NextPageCommand}" />
        <Button Content="Zoom In" Command="{Binding ZoomInCommand}" />
        <Button Content="Zoom Out" Command="{Binding ZoomOutCommand}" />
    </StackPanel>
</Grid>

```

## Control Properties
- **Document**: The PdfForgeDocument instance to render.
- **PageNumber**: The current page number to display.
- **Scale**: The scaling factor for the PDF page rendering.

# Requirements
- **.NET 8.0**+
- **Avalonia UI** for building the user interface (11.1.4+).
- **PdfForge** library for PDF rendering.

# License
This project is licensed under the Apache v2.0 license.