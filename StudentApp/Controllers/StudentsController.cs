using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentApp.Data;
using StudentApp.Models;

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

        // constructor
        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
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
            var data = await _context.Student.OrderByDescending(s=>s.CreatedDateTime).ToListAsync();
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
    }   
}
