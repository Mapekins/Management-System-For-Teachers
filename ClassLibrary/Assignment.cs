using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    //5. Izveidot  klasi "Assignement" ar:
    [Table("Assignment")]
    public class Assignment
    {
        [PrimaryKey, AutoIncrement]
        [Column("Id")]
        public int Id { get; set; }
        [Column("Deadline")]
        public DateTime Deadline { get; set; }
        // īpašība "Course", datu tips Course.
        [Indexed]
        [Column("CourseId")]
        public int CourseId { get; set; }
        // īpašība "Description", kas ir teksts.
        [Column("Description")]
        public string Description { get; set; }
        [Ignore]
        private string CourseName { get; set; }
        public Assignment() { }
        public Assignment(DateTime dl, Course cr, string desc)
        {
            this.Deadline = dl;
            this.CourseId = cr.Id;
            this.Description = desc;
            this.CourseName = cr.Name;
        }
        //pārdefinēt metodi ToString (), lai tā atgriež visu (arī mantoto) īpašību vērtības kā tekstu.
        public override string? ToString()
        {
            return $"Course: {CourseName}, Description:{Description}, Deadline: {Deadline}";
        }
    }
}
