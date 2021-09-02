using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string path = @"C:\Users\AndUser\Downloads\Mapping field names_2021_08_27.xlsx";
            //var dt = ReadAsDataTable(path);
            //Console.WriteLine(dt.ToString());

            ReadToConsole(path);
        }

        public static DataTable ReadAsDataTable(string fileName)
        {
            DataTable dataTable = new DataTable();
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();

                foreach (Cell cell in rows.ElementAt(0))
                {
                    dataTable.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                }

                foreach (Row row in rows)
                {
                    if (!row.RowIndex.HasValue || (row.RowIndex.HasValue && row.RowIndex.Value < 12))
                        continue;

                    DataRow dataRow = dataTable.NewRow();
                    //for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                    //{
                        dataRow[0] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(1));
                        dataRow[1] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(2));
                        dataRow[2] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(3));
                    //}

                    dataTable.Rows.Add(dataRow);
                }

            }
            dataTable.Rows.RemoveAt(0);

            return dataTable;
        }

        public static void ReadToConsole(string fileName)
        {
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();


                int i = 0;
                foreach (Row row in rows)
                {
                    if (!row.RowIndex.HasValue || (row.RowIndex.HasValue && row.RowIndex.Value < 12))
                        continue;

                    var cells = row.ChildElements.Select(c => c as Cell).ToList();
                    //skip those that have value for Pre-proc tab (clmns: J, K)
                    Cell cellJPreProcTableName = cells.FirstOrDefault(c => c.CellReference.InnerText.StartsWith("J"));
                    Cell cellKPreProcColumnUiName = cells.FirstOrDefault(c => c.CellReference.InnerText.StartsWith("K"));
                    if (cellJPreProcTableName?.CellValue is not null && cellKPreProcColumnUiName?.CellValue is not null)
                        continue;

                    Cell cellM = cells.FirstOrDefault(c => c.CellReference.InnerText.StartsWith("M"));
                    Cell cellN = cells.FirstOrDefault(c => c.CellReference.InnerText.StartsWith("N"));
                    Cell cellV = cells.FirstOrDefault(c => c.CellReference.InnerText.StartsWith("V"));
                    if (cellM?.CellValue is not null && cellV?.CellValue is not null) //M, N, V
                    {
                        i++;
                        //Console.WriteLine($"Cell = {cellM.CellReference}");
                        Console.WriteLine(@$"
new MappingTableParameter()
{{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = ""{GetCellValue(spreadSheetDocument, cellV)}"",
    NameUi = ""{GetCellValue(spreadSheetDocument, cellM)}"",
    UnitOfMeasure = {(GetCellValue(spreadSheetDocument, cellN) == null ? "null" : "\"" + GetCellValue(spreadSheetDocument, cellN) + "\"")},
    DefaultValue = null,
}},
"
                        );
                    }
                }
                Console.WriteLine(i);
            }
        }

        private static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue?.InnerXml;

            if (value is not null && cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }
    }
}

/*

new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b2_kanal_rad",
    NameUi = "Rotor width air ventilations",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b_kanal_rad",
    NameUi = "Stator width air ventilations",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "laeuferart",
    NameUi = "Type of rotor",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "simocalc",
    NameUi = "Calculation programm",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "version",
    NameUi = "Version from calculation programm",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_reibung",
    NameUi = "Friction losses",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kr_wicklung",
    NameUi = "Stator winding current displacement factor",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r_eisen_betrieb",
    NameUi = "Resistance iron losses",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kappa2_stab",
    NameUi = "Conductivity outer rotor bar warm",
    UnitOfMeasure = "Sm/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kappa2_stab_k",
    NameUi = "Conductivity outer rotor bar cold",
    UnitOfMeasure = "Sm/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kappa2_stab2",
    NameUi = "Conductivity inner rotor bar warm",
    UnitOfMeasure = "Sm/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kappa2_stab2_k",
    NameUi = "Conductivity inner rotor bar cold",
    UnitOfMeasure = "Sm/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kappa2_ring",
    NameUi = "Conductivity outer rotor short circuit ring",
    UnitOfMeasure = "Sm/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kappa2_ring_mitte",
    NameUi = "Conductivity rotor intermediate ring",
    UnitOfMeasure = "Sm/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "k1_carter",
    NameUi = "Carter factor stator slot",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "k1_carter_vent",
    NameUi = "Carter factor stator air ventilation slots",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "k2_carter",
    NameUi = "Carter factor rotor slot",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "k2_carter_vent",
    NameUi = "Carter factor rotor air ventilation slots",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "k_carter_kurz",
    NameUi = "Carter factor core length differences",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "k_carter",
    NameUi = "Carter-Factor total",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "k1_saettigung",
    NameUi = "Stator saturation level",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "k2_saettigung",
    NameUi = "Rotor saturation level",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_streu_nut_betrieb",
    NameUi = "Slot leakage reactance stator operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_streu_stirn_betrieb",
    NameUi = "Coil overhang leakage reactance stator operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_streu_dv_betrieb",
    NameUi = "Double-linked leakage reactance stator operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_streu_betrieb",
    NameUi = "Total leakage reactance stator operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_haupt_betrieb",
    NameUi = "Main reactance operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "KSB",
    NameUi = "Factor for overhang leakage at operation",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "_3xR1_kalt",
    NameUi = "Stator resistance all phases in series cold",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "_3xR1_warm",
    NameUi = "Stator resistance all phases in series warm",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_streu_nut_anlauf",
    NameUi = "Leakage reactance stator slot locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_streu_stirn_anlauf",
    NameUi = "Coil overhang leakage reactance stator locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_streu_dv_anlauf",
    NameUi = "Double-linked leakage reactance stator locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_streu_anlauf",
    NameUi = "Total leakage reactance stator locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_haupt_anlauf",
    NameUi = "Main reactance locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "KSA",
    NameUi = "Factor for coil overhang leakage at locked rotor",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "lam_stirn",
    NameUi = "Conductivity coil overhang leakage stator",
    UnitOfMeasure = "Sm/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_stab_kr_betrieb",
    NameUi = "Resistance outer rotor cage bars operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_ring_kr_betrieb",
    NameUi = "Resistance outer rotor cage short circuit ring  operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_staffelring_betrieb",
    NameUi = "Resistance rotor intermediate short circuit ring operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_ges_kr_betrieb",
    NameUi = "Resistance of outer rotor cage operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_ges_stern_betrieb",
    NameUi = "Resistance of outer rotor cage based on stator operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2s_stern_betrieb",
    NameUi = "Resistance of outer rotor cage slip-dependent based on stator operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_stab_kr_anlauf",
    NameUi = "Resistance outer rotor cage bars locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_ring_kr_anlauf",
    NameUi = "Resistance outer rotor cage short circuit ring  locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_ges_kr_anlauf",
    NameUi = "Resistance of outer rotor cage locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_ges_stern_anlauf",
    NameUi = "Resistance of outer rotor cage based on stator locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2s_stern_anlauf",
    NameUi = "Resistance of outer rotor cage slip-dependent based on stator locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_stab2_kr_betrieb",
    NameUi = "Resistance inner rotor cage bars operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_ring2_kr_betrieb",
    NameUi = "Resistance inner rotor cage short circuit ring  operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_ges2_kr_betrieb",
    NameUi = "Resistance of inner rotor cage operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "3_r2_strang_k",
    NameUi = "Resistance of slip ring rotor all phases in series cold",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_stab2_kr_anlauf",
    NameUi = "Resistance inner rotor cage bars locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_ring2_kr_anlauf",
    NameUi = "Resistance inner rotor cage short circuit ring  locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "r2_ges2_kr_anlauf",
    NameUi = "Resistance of inner rotor cage locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "3_r2_strang_w",
    NameUi = "Resistance of slip ring rotor all phases in series warm",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x2_streu_dv_betrieb",
    NameUi = "Double-linked leakage reactance rotor operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x2_streu_schraeg_betrieb",
    NameUi = "Skew leakage reactance rotor operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x2_streu_steg_betrieb",
    NameUi = "Leakage path reactance rotor operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x2_streu_nut_betrieb",
    NameUi = "Slot leakage reactance rotor operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x2_streu_betrieb",
    NameUi = "Leakage reactance rotor operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x2_streu_dv_anlauf",
    NameUi = "Double-linked leakage reactance rotor locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x2_streu_schraeg_anlauf",
    NameUi = "Skew leakage reactance rotor locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x2_streu_steg_anlauf",
    NameUi = "Leakage path reactance rotor locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x2_streu_nut_anlauf",
    NameUi = "Slot leakage reactance rotor locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x2_streu_anlauf",
    NameUi = "Leakage reactance rotor locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_ersatz_betrieb",
    NameUi = "Reactance of ECD operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "z_ersatz_betrieb",
    NameUi = "Impedance of ECD operation",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kr2_stab_betrieb",
    NameUi = "Current displacement factor rotor bar resistance operation",
    UnitOfMeasure = " ",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kr2_ring_betrieb",
    NameUi = "Current displacement factor outer rotor cage short circuit ring resistance operation",
    UnitOfMeasure = " ",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kx2_nut_betrieb",
    NameUi = "Current displacement factor slot leakage reactance rotor operation",
    UnitOfMeasure = " ",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_ersatz_betr_rel_z_masch",
    NameUi = "Ratio reactance ECD and impedance ECD operation",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_ersatz_anlauf",
    NameUi = "Reactance of ECD locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "z_ersatz_anlauf",
    NameUi = "Impedance of ECD locked rotor",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kr2_stab_anlauf",
    NameUi = "Current displacement factor rotor bar resistance locked rotor",
    UnitOfMeasure = " ",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kr2_ring_anlauf",
    NameUi = "Current displacement factor outer rotor cage short circuit ring resistance locked rotor",
    UnitOfMeasure = " ",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "kx2_nut_anlauf",
    NameUi = "Current displacement factor slot leakage reactance rotor locked rotor",
    UnitOfMeasure = " ",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "x_ersatz_anlf_rel_z_masch",
    NameUi = "Ratio reactance ECD and impedance ECD locked rotor",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "z_ersatz_kurzschluss",
    NameUi = "Short circuit impedance",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "z_ersatz_kurzschluss_imag",
    NameUi = "Short circuit reactance",
    UnitOfMeasure = "Ω",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "u_phase",
    NameUi = "Phase voltage",
    UnitOfMeasure = "V",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "emk_betrieb",
    NameUi = "Induced voltage operation",
    UnitOfMeasure = "V",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "emk_leerlauf",
    NameUi = "Induced voltage no-load",
    UnitOfMeasure = "V",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "phi_luft_leerlauf",
    NameUi = "Flux no-load",
    UnitOfMeasure = "mVs",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi_joch_leerlauf",
    NameUi = "Flux density stator yoke",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi_zahn_fuss_leerlauf",
    NameUi = "Flux density stator tooth bottom",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi_zahn_mitte_leerlauf",
    NameUi = "Flux density stator tooth center",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi_zahn_kopf_leerlauf",
    NameUi = "Flux density stator tooth top",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi_zahn_mittelwert_leerlauf",
    NameUi = "Average Flux Density stator tooth",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi_zahn_max_leerlauf",
    NameUi = "Maximum flux density stator tooth",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi_luft_leerlauf",
    NameUi = "Flux density air gap",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi_luft_ung_leerlauf",
    NameUi = "Flux density air gap unsaturated",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi2_zahn_kopf_leerlauf",
    NameUi = "Flux density inner rotor cage top",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi2_zahn_mitte_leerlauf",
    NameUi = "Flux density inner rotor cage center",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi2_zahn_fuss_leerlauf",
    NameUi = "Flux density inner rotor cage bottom",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi2_joch_leerlauf",
    NameUi = "Flux density rotor yoke",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi2_zahn_mittelwert_leerlauf",
    NameUi = "Average flux density rotor tooth",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi2_zahn_max_leerlauf",
    NameUi = "Maximum flux density rotor tooth",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "bi2_kanal_steg",
    NameUi = "Average flux density between rotor cooling slots",
    UnitOfMeasure = "T",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "v_joch_leerlauf",
    NameUi = "Magnetic flux stator yoke",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "v_zahn_leerlauf",
    NameUi = "Magnetic flux stator tooth",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "v_luft_leerlauf",
    NameUi = "Magnetic flux air gap",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "v2_zahn_leerlauf",
    NameUi = "Magnetic flux rotor tooth",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "v2_joch_leerlauf",
    NameUi = "Magnetic flux rotor yoke",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "v_gesamt_leerlauf",
    NameUi = "Magnetic flux total",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "u_lage",
    NameUi = "Coil layer voltage",
    UnitOfMeasure = "V",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "u20_schleifr",
    NameUi = "Locked rotor phase voltage slipring rotor",
    UnitOfMeasure = "V",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "u_spule",
    NameUi = "Coil voltage",
    UnitOfMeasure = "V",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "u_kurzschluss",
    NameUi = "Short circuit voltage",
    UnitOfMeasure = "V",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "eta_betrieb",
    NameUi = "Efficiency calculated",
    UnitOfMeasure = "%",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "th_delta",
    NameUi = "Stator temperature rise",
    UnitOfMeasure = "K",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "th2_delta",
    NameUi = "Rotor temperature rise",
    UnitOfMeasure = "K",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "ma_mb",
    NameUi = "Ratio of starting torque calculated",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "ia_ib",
    NameUi = "Ratio of starting current calculated",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "mk_mb",
    NameUi = "Ratio of breakdown torque calculated",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i_leerlauf",
    NameUi = "No-load current",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i_magnet",
    NameUi = "Magnetization current",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i_betrieb",
    NameUi = "Rated current stator calculated",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i_anlauf",
    NameUi = "Starting current stator calculated",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i_ideell",
    NameUi = "Ideal short circuit current stator",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i2_betrieb",
    NameUi = "Rated current rotor calculated",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i_magnet_rel_betrieb",
    NameUi = "Ratio magnetization current and rated current",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i2_anlauf",
    NameUi = "Starting current rotor calculated",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "s_strang_betrieb",
    NameUi = "Current density stator operation",
    UnitOfMeasure = "A/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "s2_stab_betrieb",
    NameUi = "Current density outer cage bar operation",
    UnitOfMeasure = "A/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "s2_ring_betrieb",
    NameUi = "Current density inner cage short circuit ring operation",
    UnitOfMeasure = "A/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "s2_stab2_betrieb",
    NameUi = "Current density inner cage bar operation",
    UnitOfMeasure = "A/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "s2_ring2_betrieb",
    NameUi = "Current density rotor operation",
    UnitOfMeasure = "A/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "s_strang_anlauf",
    NameUi = "Current density stator locked rotor",
    UnitOfMeasure = "A/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "s2_stab_anlauf",
    NameUi = "Current density outer cage bar locked rotor",
    UnitOfMeasure = "A/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "s2_ring_anlauf",
    NameUi = "Current density inner cage short circuit ring locked rotor",
    UnitOfMeasure = "A/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "s2_stab2_anlauf",
    NameUi = "Current density inner cage bar locked rotor",
    UnitOfMeasure = "A/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "s2_ring2_anlauf",
    NameUi = "Current density rotor locked rotor",
    UnitOfMeasure = "A/mm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_joch",
    NameUi = "Iron losses stator yoke",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_joch",
    NameUi = "Iron losses rotor yoke",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_zahn",
    NameUi = "Iron losses stator tooth",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_zahn",
    NameUi = "Iron losses rotortooth",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_unabh",
    NameUi = "Additional losses load-independent",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen",
    NameUi = "Iron losses load",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_ohm_w",
    NameUi = "Copper losses stator",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_ohm_kr",
    NameUi = "Copper losses stator current displacement",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_ohm_w",
    NameUi = "Copper losses rotor",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_abhg",
    NameUi = "Additional losses load-dependent calculated",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_buersten",
    NameUi = "Brush losses",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_aequivalent",
    NameUi = "Equivalent losses stator",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_aequivalent",
    NameUi = "Equivalent losses rotor",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_pulsation",
    NameUi = "Pulsation losses load-independent stator",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_pulsation",
    NameUi = "Pulsation losses load-independent rotor",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_oberflaeche",
    NameUi = "Surface losses load-independent stator",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_oberflaeche",
    NameUi = "Surface losses load-independent rotor",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_pulsation12",
    NameUi = "Pulsation losses load-dependent stator",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_pulsation21",
    NameUi = "Pulsation losses load-dependent rotor",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_oberflaeche12",
    NameUi = "Surface losses load-dependent stator",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_oberflaeche21",
    NameUi = "Surface losses load-dependent rotor",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_nue21",
    NameUi = "Thermal losses rotor",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_leerlauf",
    NameUi = "Iron losses no-load",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_summe",
    NameUi = "Losses total",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "c_phi_leerlauf",
    NameUi = "Power factor no-load",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "c_phi_anlauf",
    NameUi = "Power factor starting",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "c_phi_max",
    NameUi = "Maximum power factor",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "c_phi_betrieb",
    NameUi = "Power factor operation",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "m2_betrieb",
    NameUi = "Operation torque",
    UnitOfMeasure = "Nm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "m2_kipp",
    NameUi = "Breakdown torque",
    UnitOfMeasure = "Nm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "m2_anlauf",
    NameUi = "Starting toque ",
    UnitOfMeasure = "Nm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "m2_kipp_gen",
    NameUi = "Breakdown torque generator",
    UnitOfMeasure = "Nm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "n2_kipp",
    NameUi = "Breakdown speed",
    UnitOfMeasure = "1/min",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "n2_synchron",
    NameUi = "Synchronous speed",
    UnitOfMeasure = "1/min",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "n2_kipp_gen",
    NameUi = "Breakdown speed generator",
    UnitOfMeasure = "1/min",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "schlupf_betrieb",
    NameUi = "Operation slip",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "schlupf_kipp",
    NameUi = "Breakdown slip",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "schlupf_kipp_gen",
    NameUi = "Breakdown slip generator",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "p2_mech_betrieb",
    NameUi = "Mechanical power output",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "th_delta_dt",
    NameUi = "Stator temp. Gradient",
    UnitOfMeasure = "K/s",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "th2_delta_dt",
    NameUi = "Rotor temp. Gradient",
    UnitOfMeasure = "K/s",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_leerlauf",
    NameUi = "No-load losses",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "p2_mech_ent_5",
    NameUi = "Power output 125% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "p2_mech_ent_4",
    NameUi = "Power output 100% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "p2_mech_ent_3",
    NameUi = "Power output 75% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "p2_mech_ent_2",
    NameUi = "Power output 50% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "p2_mech_ent_1",
    NameUi = "Power output 25% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_ohm_w_5",
    NameUi = "Stator copper losses 125% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_ohm_w_4",
    NameUi = "Stator copper losses 100% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_ohm_w_3",
    NameUi = "Stator copper losses 75% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_ohm_w_2",
    NameUi = "Stator copper losses 50% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_ohm_w_1",
    NameUi = "Stator copper losses 25% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_ohm_w_5",
    NameUi = "Rotor copper losses 125% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_ohm_w_4",
    NameUi = "Rotor copper losses 100% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_ohm_w_3",
    NameUi = "Rotor copper losses 75% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_ohm_w_2",
    NameUi = "Rotor copper losses 50% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_ohm_w_1",
    NameUi = "Rotor copper losses 25% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_5",
    NameUi = "Iron and friction losses 125% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_4",
    NameUi = "Iron and friction losses 100% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_3",
    NameUi = "Iron and friction losses 75% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_2",
    NameUi = "Iron and friction losses 50% ",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_1",
    NameUi = "Iron and friction losses 25%",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_brush_5",
    NameUi = "Brush losses 125%",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_brush_4",
    NameUi = "Brush losses 100%",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_brush_3",
    NameUi = "Brush losses 75%",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_brush_2",
    NameUi = "Brush losses 50%",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_brush_1",
    NameUi = "Brush losses 25%",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_zusatz_5",
    NameUi = "Additional losses 125% standard",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_zusatz_4",
    NameUi = "Additional losses 100% standard",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_zusatz_3",
    NameUi = "Additional losses 75% standard",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_zusatz_2",
    NameUi = "Additional losses 50% standard",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv2_zusatz_1",
    NameUi = "Additional losses 25% standard",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_abhg_5",
    NameUi = "Additional losses 125% calculated",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_abhg_4",
    NameUi = "Additional losses 100% calculated",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_abhg_3",
    NameUi = "Additional losses 75% calculated",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_abhg_2",
    NameUi = "Additional losses 50% calculated",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_eisen_abhg_1",
    NameUi = "Additional losses 25% calculated",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_summe_5",
    NameUi = "Total losses 125%",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_summe_4",
    NameUi = "Total losses 100%",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_summe_3",
    NameUi = "Total losses 75%",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_summe_2",
    NameUi = "Total losses 50%",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "pv_summe_1",
    NameUi = "Total losses 25%",
    UnitOfMeasure = "W",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "c_phi_5",
    NameUi = "Power factor 125%",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "c_phi_4",
    NameUi = "Power factor 100%",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "c_phi_3",
    NameUi = "Power factor 75%",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "c_phi_2",
    NameUi = "Power factor 50%",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "c_phi_1",
    NameUi = "Power factor 25%",
    UnitOfMeasure = null,
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "eta_betrieb_5",
    NameUi = "Efficiency 125%",
    UnitOfMeasure = "%",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "eta_betrieb_4",
    NameUi = "Efficiency 100%",
    UnitOfMeasure = "%",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "eta_betrieb_3",
    NameUi = "Efficiency 75%",
    UnitOfMeasure = "%",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "eta_betrieb_2",
    NameUi = "Efficiency 50%",
    UnitOfMeasure = "%",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "eta_betrieb_1",
    NameUi = "Efficiency 25%",
    UnitOfMeasure = "%",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i_betrieb_5",
    NameUi = "Current 125%",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i_betrieb_4",
    NameUi = "Current 100%",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i_betrieb_3",
    NameUi = "Current 75%",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i_betrieb_2",
    NameUi = "Current 50%",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "i_betrieb_1",
    NameUi = "Current 25%",
    UnitOfMeasure = "A",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "n2_mech_5",
    NameUi = "Speed 125%",
    UnitOfMeasure = "1/min",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "n2_mech_4",
    NameUi = "Speed 100%",
    UnitOfMeasure = "1/min",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "n2_mech_3",
    NameUi = "Speed 75%",
    UnitOfMeasure = "1/min",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "n2_mech_2",
    NameUi = "Speed 50%",
    UnitOfMeasure = "1/min",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "n2_mech_1",
    NameUi = "Speed 25%",
    UnitOfMeasure = "1/min",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "tau_nut",
    NameUi = "Tooth pitch",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "tau_pol",
    NameUi = "Pole pitch",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b_steg_ersatz_anlauf",
    NameUi = "Effective stator slot dimension W6 locked rotor",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b_steg_ersatz_betrieb",
    NameUi = "Effective stator slot dimension W6 operation",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b_zahn_fuss",
    NameUi = "Stator tooth width bottom",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b_zahn_mitte",
    NameUi = "Stator tooth width center",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b_zahn_kopf",
    NameUi = "Stator tooth width top",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b_zahn_min",
    NameUi = "Minimum stator tooth width",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "h_nut_result",
    NameUi = "Magnetic path length stator tooth",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "m_zahn",
    NameUi = "Stator tooth weight",
    UnitOfMeasure = "kg",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "d_joch",
    NameUi = "Stator yoke diameter center",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "h_joch",
    NameUi = "Stator yoke height",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "h_joch_result",
    NameUi = "Effective stator yoke height",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "l_joch",
    NameUi = "Magnetic path length stator yoke",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "m_joch",
    NameUi = "Stator yoke weight",
    UnitOfMeasure = "kg",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "d_wikopf",
    NameUi = "Overhang outer diameter flat wire",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "o_paket",
    NameUi = "Stator surface for stator heat dissipation",
    UnitOfMeasure = "m²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "m2_aktiv",
    NameUi = "Weight of rotor   including bracing plates   but without shaft [kg]",
    UnitOfMeasure = "kg",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "o2_paket",
    NameUi = "Rotor surface for rotor heat dissipation",
    UnitOfMeasure = "m²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "tau2_nut",
    NameUi = "Slot pitch on outer rotor diameter",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "a_luftspalt_result",
    NameUi = "Effective Air Gap",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b2_steg_ersatz_anlauf",
    NameUi = "Effective rotor slot dimension W4 locked rotor",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b2_steg_ersatz_betrieb",
    NameUi = "Effective rotor slot dimension W4 operation",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b2_kanal_steg",
    NameUi = "Tooth width rotor cooling slots",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b2_zahn_kopf",
    NameUi = "Tooth width inner rotor cage top",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b2_zahn_mitte",
    NameUi = "Tooth width inner rotor cage center",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "b2_zahn_fuss",
    NameUi = "Tooth width inner rotor cage bottom",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "h2_nut_result",
    NameUi = "Magnetic path length rotor tooth",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "m2_zahn",
    NameUi = "Rotor tooth weight",
    UnitOfMeasure = "kg",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "l2_joch",
    NameUi = "Magnetic path length rotor yoke",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "h2_joch",
    NameUi = "Rotor yoke height",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "h2_joch_result",
    NameUi = "Effective rotor yoke height",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "d2_joch",
    NameUi = "Rotor yoke diameter center",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "m2_joch",
    NameUi = "Rotor yoke weight",
    UnitOfMeasure = "kg",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "a2_paket_ring",
    NameUi = "Length rotor bar between core and outer short circuit ring",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "a2_paket_ring2",
    NameUi = "Length rotor bar between core and inner short circuit ring",
    UnitOfMeasure = "mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "j2_rotor",
    NameUi = "Moment of inertia rotor",
    UnitOfMeasure = "kgm²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "schub",
    NameUi = "Rotary thrust",
    UnitOfMeasure = "kN/m²",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "esson",
    NameUi = "Utilization according to esson",
    UnitOfMeasure = "kW*min/m3",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "c_magn",
    NameUi = "Magnetic spring constant",
    UnitOfMeasure = "kN/mm",
    DefaultValue = null,
},


new MappingTableParameter()
{
    ParentName = VariantSearchNamesConstants.Attributes,
    Name = "j2_aktiv",
    NameUi = "Moment of inertia activ part",
    UnitOfMeasure = "kgm²",
    DefaultValue = null,
},


*/