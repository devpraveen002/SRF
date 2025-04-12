using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StudentRegistration.Contexts;
using StudentRegistration.Models;

namespace StudentRegistration.Controllers;

public class StudentController : Controller
{
    private readonly StudentContext _context;

    public StudentController(StudentContext context)
    {
        _context = context;
    }

    // GET: Student
    public async Task<IActionResult> Index()
    {
        return View(await _context.Students.ToListAsync());
    }

    // GET: Student/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View("Index", new Student());
    }

    // GET: Student/GetAll
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _context.Students
            .FromSqlRaw("EXEC sp_GetAllStudents")
            .ToListAsync();
        return Json(students);
    }

    // GET: Student/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var idParam = new SqlParameter("@Id", id);
        var student = await _context.Students
            .FromSqlRaw("EXEC sp_GetStudentById @Id", idParam)
            .FirstOrDefaultAsync();

        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // POST: Student/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Mobile,Gender,Email,Address,DOB,Class,FatherName,MotherName")] Student student)
    {
        if (ModelState.IsValid)
        {
            // Using stored procedure to insert student
            var namePara = new SqlParameter("@Name", student.Name);
            var mobilePara = new SqlParameter("@Mobile", student.Mobile);
            var genderPara = new SqlParameter("@Gender", student.Gender);
            var emailPara = new SqlParameter("@Email", student.Email);
            var addressPara = new SqlParameter("@Address", student.Address ?? (object)DBNull.Value);
            var dobPara = new SqlParameter("@DOB", student.DOB);
            var classPara = new SqlParameter("@Class", student.Class);
            var fatherNamePara = new SqlParameter("@FatherName", student.FatherName);
            var motherNamePara = new SqlParameter("@MotherName", student.MotherName);

            try
            {
                var result = await _context.Database
                    .ExecuteSqlRawAsync("EXEC sp_InsertStudent @Name, @Mobile, @Gender, @Email, @Address, @DOB, @Class, @FatherName, @MotherName",
                        namePara, mobilePara, genderPara, emailPara, addressPara, dobPara, classPara, fatherNamePara, motherNamePara);

                return Json(new { success = true, message = "Student added successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        return Json(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }

    // GET: Student/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var idParam = new SqlParameter("@Id", id);
        var student = await _context.Students
            .FromSqlRaw("EXEC sp_GetStudentById @Id", idParam)
            .FirstOrDefaultAsync();

        if (student == null)
        {
            return NotFound();
        }
        return View(student);
    }

    // POST: Student/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Mobile,Gender,Email,Address,DOB,Class,FatherName,MotherName")] Student student)
    {
        if (id != student.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Using stored procedure to update student
                var idPara = new SqlParameter("@Id", student.Id);
                var namePara = new SqlParameter("@Name", student.Name);
                var mobilePara = new SqlParameter("@Mobile", student.Mobile);
                var genderPara = new SqlParameter("@Gender", student.Gender);
                var emailPara = new SqlParameter("@Email", student.Email);
                var addressPara = new SqlParameter("@Address", student.Address ?? (object)DBNull.Value);
                var dobPara = new SqlParameter("@DOB", student.DOB);
                var classPara = new SqlParameter("@Class", student.Class);
                var fatherNamePara = new SqlParameter("@FatherName", student.FatherName);
                var motherNamePara = new SqlParameter("@MotherName", student.MotherName);

                await _context.Database
                    .ExecuteSqlRawAsync("EXEC sp_UpdateStudent @Id, @Name, @Mobile, @Gender, @Email, @Address, @DOB, @Class, @FatherName, @MotherName",
                        idPara, namePara, mobilePara, genderPara, emailPara, addressPara, dobPara, classPara, fatherNamePara, motherNamePara);

                return RedirectToAction(nameof(Index));
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
        }
        return View(student);
    }

    // POST: Student/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            // Using stored procedure to delete student
            var idPara = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteStudent @Id", idPara);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(e => e.Id == id);
    }
}
