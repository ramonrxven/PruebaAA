using System.Collections.Generic;

namespace PruebaAA.Models
{

  /// <summary>
  /// clase que modela la instancia de los parametros de configuracion de la aplicación
  /// </summary>
  class Config
  {
    public string FieldSeparator {get; set; }
    public string RemoteSource {get; set; }
    public string LocalSource {get; set; }

    public bool IsRemoteSource {get; set;}
    public int RowContainFieldLabel {get; set;}

    public List<FileColumn> FileColumns {get; set;}

  }
}