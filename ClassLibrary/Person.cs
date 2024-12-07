using SQLite;
using System.Reflection.Metadata.Ecma335;

namespace ClassLibrary
{
    public enum GenderType //Mūsu definēts pārskaitāmais tips pie Gender īpašību
    {
        Man,
        Woman
    }
    //1. Izveidot abstraktu klasi „Person” ar:
    public abstract class Person
    {
        //Privāti atribūti un publiskām īpašībām „Name” un „Surname”.
        [Column("Name")]
        public string? Name { get; set; }
        [Column("Surname")]
        public string Surname { get; set; }
        //Tikai lasāmu īpašību „FullName”, kas atgriež vārda un uzvārda konkatenāciju ar vienu atstarpi pa vidu.
        public string FullName { get { return $"{Name} {Surname}"; } }
        
        // Īpašību Gender, kuras vērtība ir Jūsu definēts pārskaitāmais tips.
        [Column("Gender")]
        public GenderType Gender
        {
            get;
            set;
        }
        [Ignore]
        public string GenderString { get; set; }
        public string GenderToString(GenderType Gender)
        {
            if(Gender == GenderType.Man)
            {
                return "Man";
            }
            return "Woman";
        }
        // Pārdefinēt metodi ToString(), lai tā atgriež visu īpašību vērtības kā tekstu.
        public virtual string ToString()
        {
            return $"Full name:{ FullName},Gender:{GenderString}";
        }
    }
}