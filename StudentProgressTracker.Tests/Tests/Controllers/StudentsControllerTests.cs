using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentProgressTracker.Controllers;
using StudentProgressTracker.Data;
using StudentProgressTracker.Models;
using StudentProgressTracker.Tests.Tests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StudentProgressTracker.Tests.Tests.Controllers
{
    public class StudentsControllerTests
    {
        private StudentsController GetController(ApplicationDbContext context)
        {
            var controller = new StudentsController(context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "teacher1"),
            new Claim("role", "Teacher")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        [Fact]
        public async Task GetStudents_ReturnsStudentsAssignedToTeacher()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            context.Students.AddRange(
                new Student { FullName = "Ali", Grade = 3, AssignedTeacherId = "teacher1" },
                new Student { FullName = "Laila", Grade = 4, AssignedTeacherId = "teacher2" }
            );
            await context.SaveChangesAsync();

            var controller = GetController(context);

            // Act
            var result = await controller.GetStudents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Single(value.students);
        }
    }
}
