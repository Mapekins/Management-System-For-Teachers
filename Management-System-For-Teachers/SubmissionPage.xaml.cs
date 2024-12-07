using ClassLibrary;

namespace Management_System_For_Teachers;

public partial class SubmissionPage : ContentPage
{
    //Savienošanās ar datubāzes instanci
    DatabaseHandler db = DatabaseHandler.Instance;
    //Mainīgie lielumi, kas mums būs nepieciešami turpmākajam darbam
    private List<Assignment> assignments;
    private List<Student> students;
    public SubmissionPage()
    {
        InitializeComponent();
        GetItems();
    }
    //Metode, kurai jāatjaunina dati, kad mēs apmeklējam lapu.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        GetItems();
    }
    //Galvenā datu iegūšanas metode no datubāzes un binding'us
    private void GetItems()
    {
        assignments = db.GetAssignments();
        AssignmentPicker.ItemsSource = assignments;
        AssignmentPicker.BindingContext = assignments;
        //Ja datubāzē ir dati, atzīmējiet pirmo vaicājumu kā atlasītu.
        if (assignments.Any())
        {
            AssignmentPicker.SelectedItem = assignments[0];
        }
        students = db.GetStudents();
        StudentPicker.ItemsSource = students;
        StudentPicker.BindingContext = students;
        //Ja datubāzē ir dati, atzīmējiet pirmo vaicājumu kā atlasītu.
        if (students.Any())
        {
            StudentPicker.SelectedItem = students[0];
        }
        var submissions = db.GetSubmissions();
        //Kombinētā saraksta izveide priekš Binding
        var submissionsExtended = submissions.Select(submission => new
        {
            submission.Id,
            submission.SubmissionTime,
            submission.Score,
            submission.StudentId,
            submission.AssignmentId,
            AssignmentDescription = assignments
            .FirstOrDefault(assignment => assignment.Id == submission.AssignmentId)?.Description ?? "Missing Assignment",
            StudentFullName = students
            .FirstOrDefault(student => student.Id == submission.StudentId)?.FullName ?? "Missing Student"
        }).ToList();
        //Datu piešķiršana mūsu sarakstam
        listView.ItemsSource = null;
        listView.ItemsSource = submissionsExtended;
    }
    //Vienkārša metode, lai atjauninātu Label ar vērtību, ko nosaka Slider.
    private void ScoreSliderValueChanged(object sender, EventArgs e)
    {
        ScoreSliderIndicator.Text = ((int)ScoreSlider.Value).ToString();
    }
    //Tukšu lauku datu validācijas pārbaužu metode
    private bool Validate(object sender, EventArgs e)
    {
        if (StudentPicker.SelectedItem == null ||
            AssignmentPicker.SelectedItem == null
            )
        {
            Application.Current.MainPage.DisplayAlert("Notification", "Some fields are empty!", "OK");
            return false;
        }
        else
        {
            return true;
        }
    }
    //Metode, kas norāda uz mūsu darbības veiksmīgumu.
    private void Success()
    {
        Application.Current.MainPage.DisplayAlert("Notification", "The operation has been successfully completed!", "OK");
    }
    //Metode, kas tiek izsaukta, kad noklikšķinot uz objekta sarakstā, objekta dati tiek pārsūtīti uz veidlapu.
    private void OnListItemTapped(object sender, ItemTappedEventArgs e)
    {
        //https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interop/using-type-dynamic
        //Tā kā mūsu saraksts satur ne tikai mūsu galveno klasi,
        //bet arī citus datus, tas kļūst par anonīmās klases objektu,
        //un mēs nevaram ar to veikt visas tās pašas darbības, ko ar klasi.
        //Tāpēc pārveidojam to par dinamisku objektu, lai mēs varētu tieši mijiedarboties ar datiem.
        var selectedItem = e.Item as dynamic;
        var submission = db.GetSubmissionById(selectedItem.Id);
        ScoreSlider.Value = (double)submission.Score;
        SubmissionTimePicker.Date = submission.SubmissionTime;
        //LINQ izmantošana saraksta meklēšanai
        StudentPicker.SelectedItem = students.FirstOrDefault(x => x.Id == submission.StudentId);
        AssignmentPicker.SelectedItem = assignments.FirstOrDefault(x => x.Id == submission.AssignmentId);
    }
    //Metode jaunu rindu pievienošanai datubāzē
    private void OnCreateClicked(object sender, EventArgs e)
    {
        //Pārbaudiet, vai veidlapā ir visi dati
        if (Validate(sender, e))
        {
            var student = (Student)StudentPicker.SelectedItem;
            var assignment = (Assignment)AssignmentPicker.SelectedItem;
            var submission = new Submission(assignment,student,SubmissionTimePicker.Date, (int)ScoreSlider.Value);
            db.Create(submission);
            Success();
            GetItems();
        }
    }
    private void OnEditClicked(object sender, EventArgs e)
    {
        //Pārbaudiet, vai ir binding konteksts
        if (sender is Button EditButton && EditButton.BindingContext != null)
        {
        //Pārbaude, vai ir pieejami visi dati
            if (Validate(sender, e))
            {
                var bindingContext = EditButton.BindingContext;
                Student student = (Student)StudentPicker.SelectedItem;
                Assignment assignment = (Assignment)AssignmentPicker.SelectedItem;
            //arī izveido jaunu objektu, kas būs tieši tāds pats kā datubāzē esošais.
                var submission = new Submission
                {
                    //https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interop/using-type-dynamic
                    //Tā kā mūsu saraksts satur ne tikai mūsu galveno klasi,
                    //bet arī citus datus, tas kļūst par anonīmās klases objektu,
                    //un mēs nevaram ar to veikt visas tās pašas darbības, ko ar klasi.
                    //Tāpēc pārveidojam to par dinamisku objektu, lai mēs varētu tieši mijiedarboties ar datiem.
                    Id = ((dynamic)bindingContext).Id,
                    Score = (int)(ScoreSlider.Value),
                    SubmissionTime = SubmissionTimePicker.Date,
                    AssignmentId = assignment.Id,
                    StudentId = student.Id,
                };
                StudentPicker.SelectedItem = students.FirstOrDefault(student => student.Id == submission.StudentId);
                AssignmentPicker.SelectedItem = assignments.FirstOrDefault(assignment => assignment.Id == submission.AssignmentId);
                db.Update(submission);
                Success();
                GetItems();
            }
        }
    }
    private void OnDeleteClicked(object sender, EventArgs e)
    {
        //Pārbaudiet, vai ir binding konteksts
        if (sender is Button DeleteButton && DeleteButton.BindingContext != null)
        {
            var bindingContext = DeleteButton.BindingContext;
            var submission = new Submission
            {
                //https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interop/using-type-dynamic
                //Tā kā mūsu saraksts satur ne tikai mūsu galveno klasi,
                //bet arī citus datus, tas kļūst par anonīmās klases objektu,
                //un mēs nevaram ar to veikt visas tās pašas darbības, ko ar klasi.
                //Tāpēc pārveidojam to par dinamisku objektu, lai mēs varētu tieši mijiedarboties ar datiem.
                Id = ((dynamic)bindingContext).Id,
                SubmissionTime = ((dynamic)bindingContext).SubmissionTime,
                AssignmentId = ((dynamic)bindingContext).AssignmentId,
                StudentId = ((dynamic)bindingContext).StudentId,
                Score = ((dynamic)bindingContext).Score
            };
            db.Delete(submission);
            Success();
            GetItems();
        }
    }
}