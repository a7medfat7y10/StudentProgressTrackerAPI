using StudentProgressTracker.Models;
using StudentProgressTracker.Services;
using StudentProgressTracker.Tests.Tests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProgressTracker.Tests.Tests.Services
{
    public class StudentServiceTests
    {
        [Fact]
        public async Task GetStudents_ReturnsExpectedNumber()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            context.Students.AddRange(
                new Student { FullName = "Ahmed", Grade = 1, AssignedTeacherId = "teacher1" },
                new Student { FullName = "Fatma", Grade = 2, AssignedTeacherId = "teacher1" }
            );
            await context.SaveChangesAsync();

            var service = new StudentService(context);

            // Act
            var result = await service.GetStudents("Teacher", "teacher1", 1, 10);

            // Assert
            Assert.Equal(2, result.Count);
        }
    }
}
