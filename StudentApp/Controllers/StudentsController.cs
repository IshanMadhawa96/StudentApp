using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StudentApp.Data;
using StudentApp.Models;

namespace StudentApp.Controllers
{
    public class CSVSTUDENT
    {
        
        public string StudentName { get; set; }
        public int StudentAge { get; set; }
        public string StudentAddress { get; set; }
        public int StudentPhone { get; set; }
        public string StudentEmail { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
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
        public async Task<IActionResult> Index(IFormFile postedFile)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "csv", postedFile.FileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            
            await postedFile.CopyToAsync(fileStream);
            fileStream.Close();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                
            };
            using (var reader = new StreamReader(filePath))
            
            using (var csv = new CsvReader(reader, config))
            {
               
                //csv.Configuration.HeaderValidated = null;
                var record = csv.GetRecords<CSVSTUDENT>().ToArray();
                await _context.AddRangeAsync(record);
                await _context.SaveChangesAsync();
                /*_context.Add(student);
                await _context.SaveChangesAsync();*/
            }
           
           
            return RedirectToAction(nameof(Index));
        }

       
    }   
}
