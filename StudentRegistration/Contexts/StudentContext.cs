﻿using Microsoft.EntityFrameworkCore;
using StudentRegistration.Models;

namespace StudentRegistration.Contexts;

public class StudentContext : DbContext
{
    public StudentContext(DbContextOptions<StudentContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
}
