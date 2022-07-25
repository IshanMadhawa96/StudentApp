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

        // passing db conex instance via DI
        private readonly ApplicationDbContext _context;

        // constructor
        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get All Students
        public async Task<IActionResult> Index()
        {
            var data = await _context.Student.ToListAsync();
            return View(data);
        }
    }   
}
