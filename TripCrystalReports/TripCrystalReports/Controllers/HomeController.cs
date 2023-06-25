using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TripCrystalReports.Models;

namespace TripCrystalReports.Controllers
{
    public class HomeController : Controller
    {
        db_TestEntities3 db = new db_TestEntities3();
        public ActionResult Index()
        {            
            return View(db.tbl_Employee.ToList());            
        }

        public ActionResult ExportReport()
        {
            // Generate the report using the reporting library
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Reports"), "Employee.rpt")); // Path to your report template
            report.SetDataSource(ConvertListToDataTable<tbl_Employee>(db.tbl_Employee.ToList()));
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                // Return the report as a downloadable file
                Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "ItemReport.pdf");
            }
            catch
            {
                throw;
            }
        }

        private DataTable ConvertListToDataTable<T>(List<T> list)
        {

            DataTable dataTable = new DataTable();

            // Get the properties of the object type
            var properties = typeof(T).GetProperties();

            // Create columns in the DataTable based on the object properties
            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            // Add data rows to the DataTable
            foreach (var item in list)
            {
                var dataRow = dataTable.NewRow();
                foreach (var property in properties)
                {
                    dataRow[property.Name] = property.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }


      


    }
}