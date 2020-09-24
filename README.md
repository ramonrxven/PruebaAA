PruebaAA
-----
## Aplicación de consola .NET Core en C#

Esta aplicación lee un fichero .CSV almacenado en una cuenta de almacenamiento de Azure/o ubicación local e inserta su contenido en una BD SQL Server.

La aplicación está desarrollada en .NetCore 3.1 y probada con MsSql Server 2017 como motor de base de datos.

#### Preparar la Base de datos.
Previo a ejecutar la aplicación por favor actualice los parametros conexión a su base de datos en _ConnectionString_ que se encuenta en el archivo __appsettings.json__. por defecto la aplicación tiene una base de datos configurada (PruebaAA), pero puedes configurar la que necesite.


#### Configuración de la aplicación.
En el archivo appsettings.json se encuentan todos los parametros de configuración de la
aplicación.

- **ConnectionStrings**: string de conexion a la base de datos.
- **GeneralSettings**: contiene los parametros para la carga y el procesamiento del archivo CSV.
    - _**FieldSeparator**_: indica cual es el separador de columnas para el archivo CSV.
    - _**RemoteSource**_: indica el origen remoto(URL) para el archivo CSV.
    - _**LocalSource**_: establece el path donde se encuentra el archivo CSV.
    - _**IsRemoteSource**_: es un valor boolean que indica cual origen va a utilizar la aplicación para tomar el archivo CSV, si es __true__ usara RemoteSource y si es __false__ LocalSource.
- RowContainFieldLabel: establece la línea en donde esta el encabezado de las columnas en el archivo CSV. por defecto Cero(0) como la primera línea del archivo.
FileColumns: establece la correspondencia entre las columnas del archivo CSV y los campos de la tabla, además de indicar el tipo de dato en que debe convertirse.
 ejemplo:
    - Position: posición de columna en el archivo CSV. Partiendo de Cero(0) como la primera.
    - Name: nombre del campo en la tabla.
    - Type: tipo de datos del campo en la tabla.

```
   {
      "Position" : 0,
      "Name" : "PointOfSale",
      "Type" : "String"
    },```
FileColumns de contener tantos elementos como campos se vayan a impactar de la tabla destino.
  ```{
  "ConnectionStrings": "Server=172.17.0.4;User Id=sa;Password=clAm318@lpha;Database=PruebaAA",
  "GeneralSettings": {
    "FieldSeparator" : ";",
    "RemoteSource" : "https://storage10082020.blob.core.windows.net/y9ne9ilzmfld/Stock.CSV",
    "LocalSource" : "Data/Stock.CSV",
    "IsRemoteSource" : false,
    "RowContainFieldLabel": 0,
    "FileColumns": [
      {
        "Position" : 0,
        "Name" : "PointOfSale",
        "Type" : "String"
      },
      {
        "Position" : 1,
        "Name" : "Product",
        "Type" : "String"
      },
      {
        "Position" : 2,
        "Name" : "Date",
        "Type" : "Date"
      },
      {
        "Position" : 3,
        "Name" : "Stock",
        "Type" : "Int32"
      }
    ]
  }
}
```


###### Ejecutar la Aplicación
```
C:\> dotnet restore
```
```
C:\> dotnet run
```

###### Sobre la aplicación.
La aplicación esta programada para leer un archivo CSV, desde un origen remoto o local
y construir una batch de registros que puedan ser insertados en una tabla especificada en el código.

La aplicación inicia verificado si existe la tabla destino, si  no existe la tabla en la
base de datos configurada, la aplicación corre un script que crea la tabla y el stored procedure requeridos para realizar la carga de los registros.

verificada el acceso a la base de datos, se verifica la existencia del archivo CSV y 
si contiene un encabezado, además si la cantidad de columnas configuradas en la aplicación
estan disponibles en el archivo CSV.

verificado todo esto, la aplicación abre el archivo y recorre linea por linea el archivo CSV, esto debido a el tamaño del archivo CSV, cargarlo todo en memoria no era una buena opción, facilmente agotaría la memoria del equipo que ejecute la aplicación.

los registros leídos ,transformados en registros volatiles y agrupados en lotes de 20 mil, para ser procesados por un stored procedure que los carga en modo batch y a la véz verifica que no existan registros duplicados en el lote que se carga.

###### Estrategia de carga de datos
**Lectura secuencial del archivo CSV**
la lectura secuencial permite manejar los archivos de texto de gran tamaño sin riesgo de
hacer colapsar los recursos de memoria dado el gran tamaño del archivo, por otra parte nos facilita controlar el tamaño del batch de ser necesario e incorporar mesajes en la consola para mostrar avance del proceso.
 
**¿Porque usar un stored procedure y carga en batch?**
Los datos son insertados en la tabla con lotes de registros en modo batch por un stored procedure. Esto resultó ser mas eficiente que los otros mecanismos probados como la carga secuencial via el "ORM" de EntityFramework.

Otra estrategía probada fue usar la extensión _BUlkExtensions.BulkInsert_ que permitio la carga masiva en unos tiempos excelentes, sin embargo BulkInsert tiene limitaciones a la hora de realizar alguna validación sobre los registos que inserta, solo permite validar por un campo, y en nuestro caso para evitar duplicidad tenia que validar todos los campos. razón por la cual también fue descartado. 

Otra ventaja del uso del stored procedure, es delegar en la base de datos la lógica de validación de registros duplicados, así logramos un mayor rendimiento en el proceso.

