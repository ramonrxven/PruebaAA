using System;
using PruebaAA.Services;
using PruebaAA.Repository;
using PruebaAA.Models;
using Microsoft.Extensions.Options;
namespace PruebaAA
{
    class ConsoleApp
  {
      private readonly Config _appSettings;
      private readonly IImportService _import;
      private readonly ICreateTable _createTable;
      public ConsoleApp(
        IOptions<Config> appSettings,
        IImportService import,
        ICreateTable createTable)
      {
          _createTable = createTable;
          _import = import;
          _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    }

      public void Run()
      {
        _createTable.PrepareDatabase();
        _import.ProcessFile();
      }
    }
}