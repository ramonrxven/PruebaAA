
namespace PruebaAA.Services
{
  interface IImportService
  {


    /// <summary>
    /// metodo de punto de entrada para iniciar el proceso de importación.
    ///
    /// la importación se realiza de forma masiva de 100mil registros a la vez
    /// </summary>
    void ImportFile();

    /// <summary>
    /// Metodo EntryPoint del proceso.
    ///
    /// Punto de entrada para iniciar el Procesa de importar el archivo CSV.
    /// </summary>
    public void ProcessFile();
  }

}