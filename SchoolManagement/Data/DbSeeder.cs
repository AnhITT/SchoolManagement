using SchoolManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace SchoolManagement.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<User>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();
            var context = service.GetService<ApplicationDbContext>();

            // Seed Roles
            await SeedRoles(roleManager);

            // Seed Admin User
            await SeedAdminUser(userManager);

            // Seed Teacher User
            await SeedTeacherUser(userManager);

            // Seed Other Data
            await SeedOtherData(context);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await roleManager.RoleExistsAsync("Teacher"))
                await roleManager.CreateAsync(new IdentityRole("Teacher"));
        }

        private static async Task SeedAdminUser(UserManager<User> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                var user = new User
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    FullName = "System Admin",
                    DateBirthdayUser = new DateTime(1990, 1, 1),
                    Role = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        private static async Task SeedTeacherUser(UserManager<User> userManager)
        {
            var teacherUser = await userManager.FindByEmailAsync("teacher@example.com");
            if (teacherUser == null)
            {
                var user = new User
                {
                    UserName = "teacher@example.com",
                    Email = "teacher@example.com",
                    FullName = "John Teacher",
                    DateBirthdayUser = new DateTime(1985, 1, 1),
                    Role = "Teacher",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Teacher@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Teacher");
                }
            }
        }

        private static async Task SeedOtherData(ApplicationDbContext context)
        {
            if (!context.Classes.Any())
            {
                // Seed Classes
                var classes = new List<Class>
                {
                    new Class { ClassName = "20DTHE1", Description = "Lớp 20DTHE1 khóa 2020-2024" },
                    new Class { ClassName = "20DTHE2", Description = "Lớp 20DTHE2 khóa 2020-2024" },
                    new Class { ClassName = "20DTHE3", Description = "Lớp 20DTHE3 khóa 2020-2024" }
                };
                context.Classes.AddRange(classes);
                await context.SaveChangesAsync();

                // Seed Subjects
                var subjects = new List<Subject>
                {
                    new Subject { SubjectCode = "MATH101", SubjectName = "C", Description = "C"},
                    new Subject { SubjectCode = "PHY101", SubjectName = "Java", Description = "Java" },
                    new Subject { SubjectCode = "CHEM101", SubjectName = "C#", Description = "C#" }
                };
                context.Subjects.AddRange(subjects);
                await context.SaveChangesAsync();

                // Seed Students
                var students = new List<Student>
                {
                    new Student {
                        StudentCode = "ST001",
                        FullName = "Nguyễn Văn A",
                        DateOfBirth = new DateTime(2005, 1, 1),
                        Email = "nguyenvana@example.com",
                        Phone = "0123456789",
                        ClassId = classes[0].Id
                    },
                    new Student {
                        StudentCode = "ST002",
                        FullName = "Trần Thị B",
                        DateOfBirth = new DateTime(2005, 2, 2),
                        Email = "tranthib@example.com",
                        Phone = "0123456790",
                        ClassId = classes[0].Id
                    },
                    new Student {
                        StudentCode = "ST003",
                        FullName = "Lê Văn C",
                        DateOfBirth = new DateTime(2005, 3, 3),
                        Email = "levanc@example.com",
                        Phone = "0123456791",
                        ClassId = classes[1].Id
                    }
                };
                context.Students.AddRange(students);
                await context.SaveChangesAsync();

                // Seed Sessions
                var sessions = new List<Session>
                {
                    new Session {
                        SubjectId = subjects[0].Id,
                        SessionDate = DateTime.Now.Date,
                        Room = "A101",
                        Description = "Buổi học Toán"
                    },
                    new Session {
                        SubjectId = subjects[1].Id,
                        SessionDate = DateTime.Now.Date.AddDays(1),
                        Room = "A102",
                        Description = "Buổi học Vật Lý"
                    }
                };
                context.Sessions.AddRange(sessions);
                await context.SaveChangesAsync();

                // Seed StudentSubjects
                var studentSubjects = new List<StudentSubject>
                {
                    new StudentSubject { StudentId = students[0].Id, SubjectId = subjects[0].Id },
                    new StudentSubject { StudentId = students[0].Id, SubjectId = subjects[1].Id },
                    new StudentSubject { StudentId = students[1].Id, SubjectId = subjects[0].Id },
                    new StudentSubject { StudentId = students[2].Id, SubjectId = subjects[1].Id }
                };
                context.StudentSubjects.AddRange(studentSubjects);
                await context.SaveChangesAsync();

                // Seed some Attendance records
                var attendances = new List<Attendance>
                {
                    new Attendance {
                        StudentId = students[0].Id,
                        SessionId = sessions[0].Id,
                        IsPresent = true,
                        CheckTime = DateTime.Now,
                        Note = "Đi học đúng giờ"
                    },
                    new Attendance {
                        StudentId = students[1].Id,
                        SessionId = sessions[0].Id,
                        IsPresent = false,
                        CheckTime = DateTime.Now,
                        Note = "Vắng có phép"
                    }
                };
                context.Attendances.AddRange(attendances);
                await context.SaveChangesAsync();
            }
        }
    }
} 