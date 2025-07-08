using Microsoft.AspNetCore.Identity;
using StudentProgressTracker.Models;

namespace StudentProgressTracker.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // 1️⃣ Create roles if they don’t exist
            string[] roles = ["Admin", "Teacher"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed the admin user
            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed teachers
            var teacherEmails = new[] { "teacher1@school.com", "teacher2@school.com", "teacher3@school.com" };
            var teacherUsers = new List<IdentityUser>();

            foreach (var email in teacherEmails)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new IdentityUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, "Teacher123!");
                    var teacherRole = roles[1];
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(user, teacherRole);
                }

                teacherUsers.Add(user!);
            }

            // 3️⃣ Seed students if not already there
            if (context.Students.Any())
                return;

            var students = new List<Student>();
            var random = new Random();
            var subjects = new[] { "Math", "Reading", "Science", "Social Studies" };
            var grades = Enumerable.Range(1, 12).ToArray();

            for (int i = 1; i <= 20; i++)
            {
                var assignedTeacher = teacherUsers[random.Next(teacherUsers.Count)];

                var student = new Student
                {
                    FullName = $"Student {i}",
                    Email = $"student{i}@gmail.com",
                    Grade = grades[random.Next(grades.Length)],
                    AssignedTeacherId = assignedTeacher.Id, // ✅ Link to teacher
                    Progress = new List<StudentProgress>()
                };

                foreach (var subject in subjects)
                {
                    student.Progress.Add(new StudentProgress
                    {
                        Subject = subject,
                        AssignmentCompletionRate = random.Next(50, 100),
                        CompletionPercentage = random.Next(50, 100),
                        TimeSpentMinutes = random.Next(30, 300),
                        LastActivity = DateTime.UtcNow.AddDays(-random.Next(0, 90)),
                        AssessmentScore = random.Next(50, 100)
                    });
                }

                students.Add(student);
            }

            context.Students.AddRange(students);
            await context.SaveChangesAsync();
        }
    }
}
