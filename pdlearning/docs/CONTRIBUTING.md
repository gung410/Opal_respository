# Contribute to the OPAL2.0 documentation

This document covers the process for contributing to the articles of OPAL2.0 application development. Typo corrections and new articles are welcome contributions.

## Docs Authoring Pack extension in Visual Studio Code

If you use Visual Studio Code to contribute to the Thunder documentation, you can boost your productivity by installing the [Docs Authoring Pack](https://marketplace.visualstudio.com/items?itemName=docsmsft.docs-authoring-pack) extension. The extension provides a variety of tools that helps with Markdown linting, code spell checking, and article templates.

## Markdown syntax

Articles are written in [DocFx-flavored Markdown](https://dotnet.github.io/docfx/spec/docfx_flavored_markdown.html), which is a superset of [GitHub-flavored Markdown (GFM)](https://guides.github.com/features/mastering-markdown/). For examples of DFM syntax for UI features commonly used in the Thunder documentation, see [Metadata and Markdown Template](https://github.com/dotnet/docs/blob/master/styleguide/template.md) in the .NET Docs repo style guide.

## Folder structure conventions

For each Markdown file, a folder for images and a folder for sample code may exist. If the article is [getting-started/index.md](https://github.com/aspnet/Docs/blob/master/aspnetcore/fundamentals/configuration/index.md), the images are in [getting-started/_static](https://github.com/aspnet/Docs/tree/master/aspnetcore/fundamentals/static-files/\_static) and the sample app project files are in [getting-started/sample](https://github.com/aspnet/Docs/tree/master/aspnetcore/fundamentals/static-files/samples/). An image in the *getting-started/index.md* file is rendered by the following Markdown:

```
![description of image for alt attribute](_static/imagename.png)
```

All images should have [alternative (alt) text](https://wikipedia.org/wiki/Alt_attribute). For advice on specifying alt text, see online resources, such as [WebAIM: Alternative Text](https://webaim.org/techniques/alttext/).

Use lowercase for Markdown file names and image file names.

## Internal links

Internal links should use the `uid` of the target article with an xref link (link text is set to the linked content's title):

```
<xref:uid_of_the_topic>
```

If the title of the article is unsuitable for link text (for example, a word or phrase in a sentence is the link text), specify the xref link and link text with the following:

```
[link text](xref:uid_of_the_topic)
```

For more information, see the [DocFX Cross Reference](https://dotnet.github.io/docfx/spec/docfx_flavored_markdown.html#cross-reference).

## Images and screenshots

Don't include images with articles, except:

* In basic onboarding (beginner) tutorials.
* When an image is needed for clarity.

These restrictions reduce the size of the repository.

As an optional step, ensure that any images and screenshots used in the documentation are compressed, which helps with file size and page load performance. A few popular tools include TinyPNG (using the [TinyPNG website](https://tinypng.com/) or the [TinyPNG API](https://tinypng.com/developers)) or the [Image Optimizer](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.ImageOptimizer) Visual Studio extension.

## Code snippets

Articles frequently contain code snippets to illustrate points. DFM allows you to copy code into the Markdown file or refer to a separate code file. We prefer to use separate code files whenever possible to minimize the chance of errors in the code. The code files are stored in the repo using the folder structure described earlier for sample projects.

The following examples illustrate [DFM code snippet syntax](https://dotnet.github.io/docfx/spec/docfx_flavored_markdown.html#code-snippet) for use in a *configuration/index.md* file.

To render an entire code file as a snippet:

```
[!code-csharp[](configuration/index/sample/Program.cs)]
```

To render a portion of a file as a snippet by using line numbers:

```
[!code-csharp[](configuration/index/sample/Program.cs?range=1-10,20,30,40-50]
[!code-html[](configuration/index/sample/Views/Home/Index.cshtml?range=1-10,20,30,40-50]
```

For C# snippets, reference a [C# region](https://docs.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-region). Whenever possible, use regions rather than line numbers because line numbers in a code file tend to change and become out of sync with line number references in Markdown. C# regions can be nested. If referencing the outer region, the inner `#region` and `#endregion` directives aren't rendered in a snippet.

To render a C# region named "snippet_Example":

```
[!code-csharp[](configuration/index/sample/Program.cs?name=snippet_Example)]
```

To highlight selected lines in a rendered snippet (usually renders as yellow background color):

```
[!code-csharp[](configuration/index/sample/Program.cs?name=snippet_Example&highlight=1-3,10,20-25)]
[!code-csharp[](configuration/index/sample/Program.cs?range=10-20&highlight=1-3]
[!code-html[](configuration/index/sample/Views/Home/Index.cshtml?range=10-20&highlight=1-3]
[!code-javascript[](configuration/index/sample/UsingOptionsSample.csproj?range=10-20&highlight=1-3]
```

## Voice and tone

Our goal is to write documentation that is easily understandable by the widest possible audience. To that end, we established guidelines for writing style that we ask our contributors to follow. For more information, see [Voice and tone guidelines](https://github.com/dotnet/docs/blob/master/styleguide/voice-tone.md) in the .NET repo.

## Microsoft Writing Style Guide

The [Microsoft Writing Style Guide](https://docs.microsoft.com/style-guide/welcome/) provides writing style and terminology guidance for all forms of technology communication, including the ASP.NET Core documentation.

