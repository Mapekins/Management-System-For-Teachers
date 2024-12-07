using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    //2. Izveidot klasei „Person” apakšklasi „Teacher” ar:
    [Table("Teacher")]
    public class Teacher : Person
    {
        [PrimaryKey, AutoIncrement]
        [Column("Id")]
        public int Id { get; set; }
        // Īpašība "ContractDate", kas ir datums.
        [Column("ContractDate")]
        public DateTime ContractDate { set; get; }
        public Teacher() { }
        public Teacher(string n, string s, GenderType g, DateTime c)
        {
            this.Name = n;
            this.Gender = g;
            this.Surname = s;
            this.ContractDate = c;
        }
        //Pārdefinēt metodi ToString(), lai tā atgriež visu (arī mantoto) īpašību vērtības kā tekstu.
        public override string? ToString()
        {
            return base.ToString() + "," + " ContractDate: " + ContractDate;
        }
    }
}
