using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    //4. Izveidot klasi "Course" ar:
    [Table("Course")]
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        [Column("Id")]
        public int Id { get; set; }
        public Course() { }
        public Course(string n, Teacher t)
        {
            this.Name = n;
            this.TeacherId = t.Id;
        }
        //Īpašību "Name", kas ir teksts
        [Column("Name")]
        public string Name { get; set; }
        //Īpasību "Teacher" ar tipu Teacher.
        [Indexed]
        [Column("TeacherId")]
        public int TeacherId { get; set; }
        //Pārdefinēt metodi ToString(), lai tā atgriež visu (arī mantoto) īpašību vērtības kā tekstu.
        public override string? ToString()
        {
            return $"Name: {Name}";
        }
    }
}
