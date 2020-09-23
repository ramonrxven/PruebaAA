using PruebaAA.Repository;
using PruebaAA.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace PruebaAA.Repository.Impl
{
    /// <summary>
    /// Clase que contiene el metodo que comprueba 
    /// si existe la tabla y el Stored Procedure requeridos para cargar el archivo CSV.
    /// </summary>
   public class CreateTable: ICreateTable
   {

      private readonly PruebaAAContext _db;
      
      /// <summary>
      /// constructor de la clase.
      /// 
      /// se inyecta una instancia del contexto de datos.
      /// </summary>
      /// <param name="db"></param>
      public CreateTable(PruebaAAContext db)
      {
         _db = db;
      }


      /// <summary>
      /// comprueba que existan los objetos necesarios en la base datos.
      /// 
      /// comprueba que exista la tabla y el stored procedure en la base de datos
      /// si no existe las crea.
      /// </summary>
      public void PrepareDatabase()
      {
         string sqlCommand = @"IF NOT (EXISTS (
            select   *
            from	information_schema.TABLES
            where TABLE_SCHEMA = 'dbo'
            and TABLE_NAME = 'Stock'))
            BEGIN
            CREATE TABLE dbo.Stock (
               Id int IDENTITY(1,1) NOT NULL,
               PointOfSale varchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
               Product varchar(300) NULL,
               [Date] Date NULL,
               Stock float NULL,
               CONSTRAINT PK__Stock PRIMARY KEY (Id)
            )
         ALTER TABLE dbo.Stock   
         ADD CONSTRAINT AK_Stock UNIQUE (PointOfSale, Product, [Date], stock);
         END;
         IF NOT (EXISTS (
            select
               *
            from
               sys.types
            where
               name = 'tblTypeStock'))
            BEGIN
               CREATE TYPE tblTypeStock AS TABLE  
               (  
                  PointOfSale varchar(300),
                  Product varchar(300),
                  [Date] Date,
                  Stock float
               )
            END
            IF NOT (EXISTS (
               select
                  *
               from
                  information_schema.routines
               where
                  SPECIFIC_SCHEMA = 'dbo'
                  and SPECIFIC_NAME = 'spBulkImportStock'))
               BEGIN
                  EXEC('CREATE PROCEDURE spBulkImportStock  
                  (  
                        @tblStockTableType [dbo].tblTypeStock Readonly  
                  )  
                  AS  
                  BEGIN  
                     MERGE Stock  AS dbStock  
                     USING @tblStockTableType AS tblTypeStk  
                     ON (dbStock.PointOfSale = tblTypeStk.PointOfSale AND dbStock.Product = tblTypeStk.Product AND dbStock.[Date] = tblTypeStk.[Date] AND dbStock.Stock = tblTypeStk.Stock  )  
                     WHEN NOT MATCHED THEN  
                        INSERT ([PointOfSale],[Product],[Date],Stock)  
                        VALUES (tblTypeStk.PointOfSale,tblTypeStk.Product,tblTypeStk.[Date],tblTypeStk.Stock);  
                  END;') 
               END";
            _db.Database.ExecuteSqlRaw(sqlCommand);

      }
   }
}