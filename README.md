# Assumptions

* All dates are in UTC

# How to build executables

* Make sure `dotnet SDK 9` is installed.  
* In root of the repository run the command:
  * To build executable for Windows:  
`dotnet publish .\src\Bookery\ -r win-x64 -p:PublishSingleFile=true --self-contained true -o artifacts`
  * To build executable for Linux  
`dotnet publish .\src\Bookery\ -r linux-x64 -p:PublishSingleFile=true --self-contained true -o artifacts`
* Executables will be at `.\artifacts`