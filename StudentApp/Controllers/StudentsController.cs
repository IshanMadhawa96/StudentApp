using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Helpers;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StudentApp.Data;
using StudentApp.Models;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace StudentApp.Controllers
{
 
    public class StudentsController : Controller //inherting framework class controller
    {
        //IN MICROSOFT DOCUMENTATION 
        //https://docs.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-6.0
        //Articals
        //https://www.findandsolve.com/articles/iactionresult-vs-actionresult
        // passing db conex instance via DI
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        // constructor
        public StudentsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        //Get All Students

        /*
            IActionResult type. The IActionResult return type is appropriate when multiple ActionResult return 
            types are possible in an action. The ActionResult types represent various HTTP status codes. 
            Any non-abstract class deriving from ActionResult qualifies as a valid return type

            IActionResult is an interface and ActionResult is an implementation of that interface. 
            ActionResults is an abstract class and action results like ViewResult, PartialViewResult, 
            JsonResult, etc., derive from ActionResult.
         */
        public async Task<IActionResult> Index()
        {
            var data = await _context.Student.OrderByDescending(s => s.CreatedDateTime).ToListAsync();
            return View(data);
        }

        //Add new Student View
        public IActionResult Create()
        {
            return View();
        }
        //Add new Student Form Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentName,StudentAge,StudentAddress,StudentPhone,StudentEmail")] Student student)
        {
            if (ModelState.IsValid)
            {
                student.CreatedDateTime = DateTime.Now;
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }
        //get specific Students data to views  for edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Student == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        //update actions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentName,StudentAge,StudentAddress,StudentPhone,StudentEmail")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }
        private bool StudentExists(int id)
        {
            return (_context.Student?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // view Student Deaild
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Student == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        //Delete Students
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Student == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Student == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Student'  is null.");
            }
            var student = await _context.Student.FindAsync(id);
            if (student != null)
            {
                _context.Student.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //CSV FILE HANDELING
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile postedFile,Student student)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "csv", postedFile.FileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await postedFile.CopyToAsync(fileStream);
            fileStream.Close();
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[5] { 
                                /*new DataColumn("Id", typeof(int)),*/
                                new DataColumn("StudentName", typeof(string)),
                                new DataColumn("StudentAge"),
                                new DataColumn("StudentAddress",typeof(string)),
                                new DataColumn("StudentPhone"),
                                new DataColumn("StudentEmail",typeof(string)),
                               
            });
            string csvData = System.IO.File.ReadAllText(filePath);

            //Execute a loop over the rows.
            foreach (string row in csvData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    dt.Rows.Add();
                    int i = 0;

                    //Execute a loop over the columns.
                    foreach (string cell in row.Split(','))
                    {
                        dt.Rows[dt.Rows.Count - 1][i] = cell;
                        i++;
                    }
                }
            }


            /* string conString = "Server=(local)\\sqlexpress;Database=StudentDB;Trusted_Connection=True;";
             using (SqlConnection con = new SqlConnection(conString))
             {
                 using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                 {
                     //Set the database table name.
                     sqlBulkCopy.DestinationTableName = "Student";

                     //[OPTIONAL]: Map the DataTable columns with that of the database table

                     sqlBulkCopy.ColumnMappings.Add("StudentName", "StudentName");
                     sqlBulkCopy.ColumnMappings.Add("StudentAge", "StudentAge");
                     sqlBulkCopy.ColumnMappings.Add("StudentAddress", "StudentAddress");
                     sqlBulkCopy.ColumnMappings.Add("StudentPhone", "StudentPhone");
                     sqlBulkCopy.ColumnMappings.Add("StudentEmail", "StudentEmail");
                     sqlBulkCopy.ColumnMappings.Add("CreatedDateTime", "CreatedDateTime");

                     con.Open();
                     sqlBulkCopy.WriteToServer(dt);
                     con.Close();
                 }
             }*/
            var count = dt.Columns.Count;

            List<Student> Studentlist = new List<Student>();
          
            Studentlist = CommonMethod.ConvertToList<Student>(dt);
            _context.Student.AddRange(Studentlist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
    public static class CommonMethod
    {
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row => {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name.ToLower()))
                    {
                        try
                        {
                            pro.SetValue(objT, row[pro.Name]);
                        }
                        catch (Exception ex) { }
                    }
                }
                return objT;
            }).ToList();
        }
    }

}   

