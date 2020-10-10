# Overview
InsertSamplesIntoMarkDown is a utility program that specially-marked-up sample code into a specially-marked-up README.md markdown file.

# Sample Code Markup
Sample code itended for insertion into the readme file is marked and identified by a ```#region SectionIdentifier``` ```#endregion``` section where the name of the region is the section identifier.

# Readme Markup
A MarkDown comment in the readme file identifies the location for the sample code and the section identifier corresponding to that code.  
```[//]: # (SectionIdentifier)```
The sample code will be inserted as code just after the comment line.

# Usage
The easiest way to use this program is to copy the .dll and .runtimeconfig.json files into the project where the sample code is located.
Then, add the following to the project as a Post-Build step:

```dotnet InsertSamplesIntoMarkDown.dll -- Samples.cs ..\README.md```

The first argument is the relative path and name of the sample code file, and the second argument is the relative path and name of the readme file.

# Program Information

## Author and License
InsertSamplesIntoMarkDown is written and maintained by James Ivie as a utility for the AmbientServices projects.

InsertSamplesIntoMarkDown is licensed under [MIT](https://opensource.org/licenses/MIT).

## Language and Tools
InsertSamplesIntoMarkDown is written in C#, using .NET Standard 2.1.

The code can be built using either Microsoft Visual Studio 2017+, Microsoft Visual Studio Code, or .NET Core command-line utilities.

Binaries are available at https://www.nuget.org/packages/InsertSamplesIntoMarkDown
