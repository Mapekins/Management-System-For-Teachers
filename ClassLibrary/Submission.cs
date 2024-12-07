using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    //6. Izveidot  klasi "Submission" ar:
    [Table("Submission")]
    public class Submission
    {
        [PrimaryKey, AutoIncrement]
        [Column("Id")]
        public int Id { get; set; }

        [Indexed]
        [Column("AssignmentId")]
        public int AssignmentId { get; set; }

        [Indexed]
        [Column("StudentId")]
        public int StudentId { get; set; }

        [Column("SubmissionTime")]
        public DateTime SubmissionTime { get; set; }

        [Column("Score")]
        public int Score { get; set; }
        [Ignore]
        private string assignmentText { get; set; }
        [Ignore]
        private string studentText { get; set; }
        public Submission() { }
        public Submission(Assignment assignment, Student student, DateTime submissionTime, int score)
        {
            AssignmentId = assignment.Id;
            StudentId = student.Id;
            SubmissionTime = submissionTime;
            Score = score;
            assignmentText = assignment.ToString();
            studentText = student.ToString();
        }

        public override string ToString()
        {
            return $"Assignment: {assignmentText}, Student: {studentText}, Submission Time: {SubmissionTime}, Score: {Score}";
        }
    }
}
