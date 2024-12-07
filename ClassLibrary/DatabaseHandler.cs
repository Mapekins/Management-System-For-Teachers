using SQLite;
using System.Diagnostics;

namespace ClassLibrary
{
    public class DatabaseHandler
    {
        private const string DB_NAME = "SQLiteDB.db";
        private readonly SQLiteConnection _connection;
        private static DatabaseHandler? _instance;
        private string DBPath;
        private DatabaseHandler(string path)
        {
            try
            {
                DBPath = path;
                _connection = new SQLiteConnection(DBPath);
                _connection.CreateTable<Teacher>();
                _connection.CreateTable<Course>();
                _connection.CreateTable<Assignment>();
                _connection.CreateTable<Student>();
                _connection.CreateTable<Submission>();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error" + ex);
            }
        }
        public static DatabaseHandler Initialize(string DBPath)
        {
            try
            {
            if (_instance == null)
            {
                _instance = new DatabaseHandler(Path.Combine(DBPath, DB_NAME));
            }
            return _instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return null;
            }
    }
    public static DatabaseHandler Instance
        {
            get
            {
                try
                {
                if (_instance == null)
                {
                    throw new InvalidOperationException("Database is not initialized. Call Initialize() first.");
                }
                return _instance;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error" + ex);
                    return null;
                }
            }
        }

        public SQLiteConnection Connection => _connection;
        public bool CreateTestData()
        {
            try
            {
                int count = _connection?.Table<Course>()?.Count() ?? 0;
                if (count == 0)
                {
                    var student = new Student("Mareks", "Vasilevskis", GenderType.Man, 23041);
                    Create(student);
                    var teacher = new Teacher("Elina", "Kalnina", GenderType.Woman, DateTime.Parse("02.09.2024"));
                    Create(teacher);
                    var course = new Course("Lietotnu izstrade .NET vide", teacher);
                    Create(course);
                    var assignment = new Assignment(DateTime.Parse("07.10.2024"), course, "1. majas darbs 2024 - klases");
                    Create(assignment);
                    var submission = new Submission(assignment, student, DateTime.Now, 100);
                    Create(submission);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return false;
            }
        }
        public List<Student> GetStudents()
        {
            try
            {
                return _connection.Table<Student>().ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return null;
            }
        }
        public List<Teacher> GetTeachers()
        {
            try
            {
                return _connection.Table<Teacher>().ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return null;
            }
        }
        public List<Course> GetCourses()
        {
            try
            {
                return _connection.Table<Course>().ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return null;
            }
        }
        public List<Assignment> GetAssignments()
        {
            try
            {
                return _connection.Table<Assignment>().ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return null;
            }
        }
        public List<Submission> GetSubmissions()
        {
            try
            {
                return _connection.Table<Submission>().ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return null;
            }
        }


        public Student GetStudentById(int id)
        {
            try
            {
                return _connection.Table<Student>().Where(x => x.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return null;
            }
        }
        public Teacher GetTeacherById(int id)
        {
            try
            {
                return _connection.Table<Teacher>().Where(x => x.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return null;
            }
        }
        public Course GetCourseById(int id)
        {
            try
            {
                return _connection.Table<Course>().Where(x => x.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return null;
            }
        }
        public Assignment GetAssignmentById(int id)
        {
            try
            {
                return _connection.Table<Assignment>().Where(x => x.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return null;
            }
        }
        public Submission GetSubmissionById(int id)
        {
            try
            {
                return _connection.Table<Submission>().Where(x => x.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return null;
            }
        }

        public bool ManualCreateCourse(string name, int teacherId)
        {
            try
            {
                var teacher = GetTeacherById(teacherId);
                var course = new Course(name,teacher);
                int i = _connection.Insert(course);
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return false;
            }
        }
        public bool ManualCreateSubmission(int assignmentId, int studentId, DateTime submissionTime, int score)
        {
            try
            {
                var assignment = GetAssignmentById(assignmentId);
                var student = GetStudentById(studentId);
                var submission = new Submission(assignment,student,submissionTime,score);
                int i = _connection.Insert(submission);
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return false;
            }
        }
        public bool ManualCreateAssignment(DateTime deadline, int courseId, string description)
        {
            try
            {
                var course = GetCourseById(courseId);
                var assignment = new Assignment(deadline,course,description);
                int i = _connection.Insert(assignment);
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return false;
            }
        }
        public bool ManualUpdateCourse(string name, int teacherId, int id)
        {
            try
            {
                var course = new Course {Id = id, Name = name,TeacherId = teacherId };
                int i = _connection.Update(course);
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return false;
            }
        }
        public bool ManualUpdateAssignment(DateTime deadline,int courseId, string description, int id)
        {
            try
            {
                var assignment = new Assignment { CourseId = courseId, Deadline = deadline, Description = description, Id = id};
                int i = _connection.Update(assignment);
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return false;
            }
        }
        public bool ManualUpdateSubmission(int studentId, int id, int assignmentId, DateTime submissionTime, int score)
        {
            try
            {
                var submission = new Submission { Id=id,StudentId=studentId,AssignmentId=assignmentId,SubmissionTime=submissionTime,Score=score };
                int i = _connection.Update(submission);
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return false;
            }
        }
        public bool Create<T>(T obj) where T : class
        {
            try
            {
                int i = _connection.Insert(obj);
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return false;
            }
        }

        public bool Update<T>(T obj) where T : class
        {
            try
            {
                int i = _connection.Update(obj);
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return false;
            }
        }

        public bool Delete<T>(T obj) where T : class
        {
            try
            {
                int i = _connection.Delete(obj);
                if (i == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error" + ex);
                return false;
            }
        }
    }
}
