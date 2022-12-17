# Federal Court of Australia - ZipExtraction

> **Warning**
> Working in progress

This service validate and extract zip files uploaded by a party to a case. 

## Tech stack
C# .NET 6    
Autofac  
NUnit    
[Serilog](https://github.com/serilog/serilog)    
[TestStack.BDDfy](https://github.com/TestStack/TestStack.BDDfy)  
[NSubstitute](https://nsubstitute.github.io/) 

## How to use
The application fetches zip file with name `sample valid zipfile.zip` from Windows `Downloads` directory i.e. C:\Users\{account}\Downloads. 

If file types and party.xml are valid, the application will extract the zip to a folder on Windows `Desktop` i.e. C:\Users\{account}\Desktop  