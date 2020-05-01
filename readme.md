# SVG Clock Blazor

This is implementation of SVG clock concept in Server Blazor technology (TargetFramework: netcoreapp3.1).
there are no need to use JsInterop because Blazor smoothly comunicate with native Html elements in browser via SignalR. 
In contrast to Canvas Blazor project to render/refresh canvas, to repaint html elements blazor uses StateHasChanged() method.
Every second elements are rebuild on web page.   
I used two blazor techniques: RenderFragment builder (svg objects are build every second in code, dynamically) or passing values by object parameters ("secondhand" svg object, object is declared in razor page from the beginning).


## Getting Started

Using this project is simple... 
You can run it on console or IDE of your choice like Vscode by typing 
```
dotnet run
```

### Prerequisites

Running this blazor project needs installed latest dotnet core SDK like 3.1 edition, it can be found at: https://dotnet.microsoft.com/download/dotnet-core/3.1


### Installing

1. Install latest dotnetcore sdk, like 3.1, this page: https://dotnet.microsoft.com/download/dotnet-core/3.1

2. Download source file from github, unzzip package
3. Open console or advanced editor like Visual Studio Code
4. move to directory where BlazorClockSVG.csproj is located
5. and type
```
    dotnet run
```

## Running the tests

This project does not contain test folder

## Deployment - container

This scenario does not include docker file to deploy, test and debug project in container

## Debugging

Debugging is available in VSCode on your host machine, two files are responsible for configuration: task and launch file in .vscode folder

## Contributing

Please feel free to download and test source code, if someone introduce any amendments or give feedback about comparing rendering effectiveness of dynamic objects creation vs. statically coded in razor pages, I would be appreciate.

## Authors

* **Krzysztof Szczerbowski** - *Initial work* 

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

Html, css, js
Source code I found on internet or developed myself many years ago in SVG solutions with JS

Moreover, I found inspiration based on fragments of blazor code:
1.  https://github.com/SQL-MisterMagoo/BlazorTest/tree/master/BlazorClock
2.  https://github.com/Lupusa87?tab=repositories



