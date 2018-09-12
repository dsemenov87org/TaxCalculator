FROM microsoft/aspnetcore
WORKDIR /app
COPY ./out/ .
ENTRYPOINT ["dotnet", "TaxCalculator.WebApi.dll"]