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
        var students = await _context.Students
        .FromSqlRaw("EXEC sp_GetAllStudents")
        .ToListAsync();

        ViewBag.Students = students;
        return View(new Student());
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
        var student = _context.Students
            .FromSqlRaw("EXEC sp_GetStudentById @Id", idParam)
            .AsEnumerable()
            .FirstOrDefault();

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
                await _context.Database
                    .ExecuteSqlRawAsync("EXEC sp_InsertStudent @Name, @Mobile, @Gender, @Email, @Address, @DOB, @Class, @FatherName, @MotherName",
                        namePara, mobilePara, genderPara, emailPara, addressPara, dobPara, classPara, fatherNamePara, motherNamePara);

                TempData["SuccessMessage"] = "Student added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }
        }
        else
        {
            TempData["ErrorMessage"] = "Validation failed.";
        }

        var students = await _context.Students
            .FromSqlRaw("EXEC sp_GetAllStudents")
            .ToListAsync();
        ViewBag.Students = students;
        return View("Index", student);
    }

    // GET: Student/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Call the stored procedure using ExecuteSqlRaw and then get the result separately
        var parameter = new SqlParameter("@Id", id);
        var student = _context.Students
            .FromSqlRaw("EXEC sp_GetStudentById @Id", parameter)
            .AsEnumerable() // This moves execution to client side
            .FirstOrDefault();

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

                TempData["SuccessMessage"] = "Student updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }
        }
        else
        {
            TempData["ErrorMessage"] = "Validation failed.";
        }

        return View(student);
    }

    // GET: Student/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            // Using stored procedure to delete student
            var idPara = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteStudent @Id", idPara);

            TempData["SuccessMessage"] = "Student deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error deleting student: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Student/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            // Using stored procedure to delete student
            var idPara = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteStudent @Id", idPara);

            TempData["SuccessMessage"] = "Student deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error deleting student: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(e => e.Id == id);
    }
}