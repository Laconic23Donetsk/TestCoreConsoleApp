using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

namespace TestCoreConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var persons = UserDetails.GetData();
            WriteExcelFile(persons);

        }

        static void WriteExcelFile(IEnumerable<UserDetails> persons)
        {
            // Lets converts our object data to Datatable for a simplified logic.
            // Datatable is most easy way to deal with complex datatypes for easy reading and formatting. 
            string serializedPersons = JsonSerializer.Serialize(persons);
            //DataTable table = JsonSerializer.Deserialize<DataTable>(serializedPersons);
            //DataTable table = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(serializedPersons);
            DataTable table = new DataTable();
            table.Columns.Add(nameof(UserDetails.Id));
            table.Columns.Add(nameof(UserDetails.Name));
            table.Columns.Add(nameof(UserDetails.City));

            foreach (var person in persons)
                table.Rows.Add(person.Id, person.Name, person.City);

            using (SpreadsheetDocument document = SpreadsheetDocument.Create("TestNewData.xlsx", SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                sheets.Append(sheet);

                Row headerRow = new Row();

                List<String> columns = new List<string>();
                foreach (System.Data.DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);

                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column.ColumnName);
                    headerRow.AppendChild(cell);
                }

                sheetData.AppendChild(headerRow);

                foreach (DataRow dsrow in table.Rows)
                {
                    Row newRow = new Row();
                    foreach (String col in columns)
                    {
                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(dsrow[col].ToString());
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                workbookPart.Workbook.Save();
            }
        }

        private class UserDetails
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string City { get; set; }


            public static UserDetails[] GetData()
            {
                return new UserDetails[]
                {
                    new UserDetails { Id = 1, Name = "Name 1", City = "City 1" },
                    new UserDetails { Id = 2, Name = "Name 2", City = "City 2" },
                    new UserDetails { Id = 3, Name = "Name 3", City = "City 3" },
                    new UserDetails { Id = 3, Name = "Name 4", City = "City 4" },
                };
            }
        }
    }
}
