using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TripCrystalReports.Models;
using System.Diagnostics;



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

	//Opens Report in new tab
        public ActionResult OpenReport()
        {
          
                // Generate the Crystal Report and get the report document
                var reportDocument = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            // Set the path to your Crystal Report file (.rpt)
            reportDocument.Load(Path.Combine(Server.MapPath("~/Reports"), "Employee.rpt")); // Path to your report template

            reportDocument.SetDataSource(ConvertListToDataTable<tbl_Employee>(db.tbl_Employee.ToList()));

            // Export the report to a PDF file
            var reportStream = reportDocument.ExportToStream(
                    CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                // Create a new window or browser tab and display the report PDF
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "inline; filename=YourReport.pdf");
            byte[] byteArray = new byte[reportStream.Length];
            reportStream.Read(byteArray, 0, (int)reportStream.Length);
            Response.BinaryWrite(byteArray);
                Response.Flush();
                Response.End();

                return View();
            
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