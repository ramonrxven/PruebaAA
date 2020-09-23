using System.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using PruebaAA.Models;
using Microsoft.Extensions.Options;
using EFCore.BulkExtensions;
using System.Diagnostics;
using PruebaAA.Repository;

namespace PruebaAA.Services.Impl
{

  /// <summary>
  /// Capa de negocio para el proceso de importar el archivo a la base de datos.
  ///
  /// esta clase contiene los metodos para  cargar la información leida en el
  /// archivo CSV y llevarla a la tabla.
  /// </summary>
  class ImportService : IImportService
  {
    private readonly Config _config;
    private readonly IReadFileService _readFile;
    private readonly ILineMappingService _lineMap;
    private readonly ISqlBulkImport _bulkImport;

    private readonly PruebaAAContext _db;

    /// <summary>
    /// constructor de la clase que implementa IImportService.
    ///
    /// se implemta este constructor para inyectar dependecias de otras clases.
    /// </summary>
    /// <param name="config">instancia de la clase que contiene la configuración general de la aplicación.</param>
    /// <param name="readFile">instancia de la clase IReadFileService que se inyecta en esta clase.</param>
    /// <param name="lineMap">instancia de la clase ILineMapperService que se inyecta en esta clase.</param>
    public ImportService(IOptions<Config> config, IReadFileService readFile, ILineMappingService lineMap, PruebaAAContext db, ISqlBulkImport bulkImport)
    {
      _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
      _readFile = readFile;
      _lineMap = lineMap;
      _bulkImport = bulkImport;
      _db = db;
    }

    /// <summary>
    /// metodo de punto de entrada para iniciar el proceso de importación.
    ///
    /// la importación se realiza de forma masiva de 100mil registros a la vez
    /// </summary>
    public void ImportFile()
    {
      List<StockModel> stockList = new List<StockModel>();
      int breakingList = 30000;
      // int max = 300000;
      int pointer = 0;
      int aceptados = 0;
      DataTable stock4bulk = CreateDataTableStock4Bulk();
      foreach (var line in _readFile.LineRunner())
      {
        if (pointer != _config.RowContainFieldLabel){
          object[] lnm = _lineMap.MappStockLine4Bulk(line);
          stock4bulk.Rows.Add(lnm);
          // stockList.Add(_lineMap.MappStockLine4Bulk(line));
        }

        /// <summary>
        /// Ejecuta el guardado por lote cada catidad de registros
        ///  dados por el valor de [breakingList]
        /// </summary>
        /// <value></value>
        if (pointer !=0 && pointer % breakingList == 0){
          // stock4bulk.Select()
          _bulkImport.BulkImport(stock4bulk);
          Console.WriteLine($"[lote#: {pointer/breakingList}] - aceptados: {aceptados}/{stockList.Count} Procesados ACM: {pointer}");
          // stockList = new List<StockModel>();
          stock4bulk.Clear();
        }
        pointer++;
      }
      _bulkImport.BulkImport(stock4bulk);
      Console.WriteLine($"[lote final] - procesado: {stockList.Count} TOTAL Procesados: {pointer}");
      stock4bulk.Clear();
    }


    // public void ImportFile()
    // {
    //   List<StockModel> stockList = new List<StockModel>();
    //   int breakingList = 10000;
    //   int max = 100000;
    //   int pointer = 0;
    //   int aceptados = 0;
    //   foreach (var line in _readFile.LineRunner(max))
    //   {
    //     if (pointer != _config.RowContainFieldLabel){
    //       stockList.Add(_lineMap.MappStockLine(line));
    //     }

    //     /// <summary>
    //     /// Ejecuta el guardado por lote cada catidad de registros
    //     ///  dados por el valor de [breakingList]
    //     /// </summary>
    //     /// <value></value>
    //     if (pointer !=0 && pointer % breakingList == 0){
    //       aceptados = SaveStockList(stockList);
    //       Console.WriteLine($"[lote#: {pointer/breakingList}] - aceptados: {aceptados}/{stockList.Count} Procesados ACM: {pointer}");
    //       stockList = new List<StockModel>();
    //     }
    //     pointer++;
    //   }
    //   SaveStockList(stockList);
    //   Console.WriteLine($"[lote final] - procesado: {stockList.Count} TOTAL Procesados: {pointer}");
    //   stockList = new List<StockModel>();
    // }


    /// <summary>
    /// Proceso de carga masiva que almacena la lista de objetos Stock.
    ///
    /// Guarda en la tabla la lista de registros leidos del archivo CSV.
    /// </summary>
    /// <param name="stockList">lista de objectos tipo Stock</param>
    private int SaveStockList(List<StockModel> stockList)
    {

      // stockList = stockList.Where(s => !_db.Stocks.Any(e => e.PointOfSale == s.PointOfSale
      // && e.Product == s.Product && e.Date == s.Date && e.Stock == s.Stock)).ToList();
      var bulkConfig = new BulkConfig { SqlBulkCopyOptions =  Microsoft.Data.SqlClient.SqlBulkCopyOptions.FireTriggers};
      _db.BulkInsert(stockList, bulkConfig);
      return stockList.Count;
    }


    private DataTable CreateDataTableStock4Bulk()
    {
      DataTable dataTableStock = new DataTable();
      dataTableStock.Columns.Add(new DataColumn("PointOfSale", typeof(string)));
      dataTableStock.Columns.Add(new DataColumn("Product", typeof(string)));
      dataTableStock.Columns.Add(new DataColumn("Date", typeof(string)));
      dataTableStock.Columns.Add(new DataColumn("Stock", typeof(float)));
      return dataTableStock;
    }

    /// <summary>
    /// Procesa el archivo CSV registro a registro y lo almacena en la tabla designada
    /// </summary>
    public void ProcessFile()
    {
      Console.WriteLine("Comienza el proceso de registro del archivo...");
      if (_readFile.ValidateFile())
      {
        Console.WriteLine("Archivo Verificado");
          DateTime dateIni =  DateTime.Now;
          Stopwatch sw = new Stopwatch(); // Creación del Stopwatch.
          sw.Start(); // Iniciar la medición.
          ImportFile();
          Console.WriteLine($"Hora de Inicio: {dateIni.ToString("dddd, dd MMMM yyyy ")}");
          Console.WriteLine($"Hora de Culminación: {DateTime.Now.ToString("dddd, dd MMMM yyyy")}");
          sw.Stop(); // Detener la medición.
          Console.WriteLine("Tiempo Transcurrido: {0}", sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff")); //
      }
    }
  }
}