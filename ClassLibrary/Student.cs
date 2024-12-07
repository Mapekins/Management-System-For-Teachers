using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClassLibrary
{
    //3. Izveidot klasei „Person” apakšklasi „Student” ar:
    [Table("Student")]
    public class Student : Person
    {
        [PrimaryKey, AutoIncrement]
        [Column("Id")]
        public int Id { get; set; }
        //Īpašība „StudentIdNumder”.
        [Column("StudentIdNumber")]
        public int StudentIdNumber { get; set; }
        //Klasei izveidot 2 konstruktorus, no kuriem viens kā parametru saņem visu īpašību vērtības, kas arī tiek uzstādītas jaunajam objektam.
        public Student() { }
        public Student(string? name, string? surname, GenderType gender, int stdId)
        {
            Name = name;
            Surname = surname;
            Gender = gender;
            GenderString = GenderToString(Gender);
            StudentIdNumber = stdId;
        }
        //Pārdefinēt metodi ToString(), lai tā atgriež visu (arī mantoto) īpašību vērtības kā tekstu.
        public override string? ToString()
        {
            return base.ToString() + "," + " StudentIdNumber: " + StudentIdNumber;
        }
    }
}
