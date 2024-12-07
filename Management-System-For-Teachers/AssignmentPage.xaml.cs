using ClassLibrary;
using System;

namespace Management_System_For_Teachers;

public partial class AssignmentPage : ContentPage
{
    //Savienošanās ar datubāzes instanci
    DatabaseHandler db = DatabaseHandler.Instance;
    private List<Course> courses;
    public AssignmentPage()
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
        var assignments = db.GetAssignments();
        courses = db.GetCourses();
        CoursePicker.ItemsSource = courses;
        CoursePicker.BindingContext = courses;
        //Ja datubāzē ir dati, atzīmējiet pirmo vaicājumu kā atlasītu.
        if (courses.Any())
        {
            CoursePicker.SelectedItem = courses[0];
        }
        //Kombinētā saraksta izveide priekš Binding
        var assignmentsWithCourses = assignments.Select(assignment => new
        {
            assignment.Id,
            assignment.Description,
            assignment.Deadline,
            assignment.CourseId,
            CourseName = courses
            .FirstOrDefault(course => course.Id == assignment.CourseId)?.Name ?? "Unknown Teacher"
        }).ToList();
        //Datu piešķiršana mūsu sarakstam
        listView.ItemsSource = null;
        listView.ItemsSource = assignmentsWithCourses;
    }
    //Metode, kas norāda uz mūsu darbības veiksmīgumu.
    private void Success()
    {
        Application.Current.MainPage.DisplayAlert("Notification", "The operation has been successfully completed!", "OK");
    }
    //Tukšu lauku datu validācijas pārbaužu metode
    private bool Validate(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(AssignmentDescriptionEntry.Text)||
            CoursePicker.SelectedItem == null
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
    //Metode, kas tiek izsaukta, kad noklikšķinot uz objekta sarakstā, objekta dati tiek pārsūtīti uz veidlapu.
    private void OnListItemTapped(object sender, ItemTappedEventArgs e)
    {
        //https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interop/using-type-dynamic
        //Tā kā mūsu saraksts satur ne tikai mūsu galveno klasi,
        //bet arī citus datus, tas kļūst par anonīmās klases objektu,
        //un mēs nevaram ar to veikt visas tās pašas darbības, ko ar klasi.
        //Tāpēc pārveidojam to par dinamisku objektu, lai mēs varētu tieši mijiedarboties ar datiem.
        var selectedItem = e.Item as dynamic;
        var assignment = db.GetAssignmentById(selectedItem.Id);
        AssignmentDescriptionEntry.Text = assignment.Description;
        AssignmentDatePicker.Date = assignment.Deadline;
        //LINQ izmantošana saraksta meklēšanai
        CoursePicker.SelectedItem = courses.FirstOrDefault(x => x.Id == assignment.CourseId);
    }
    //Metode jaunu rindu pievienošanai datubāzē
    private void OnCreateClicked(object sender, EventArgs e)
    {
        //Pārbaude, vai ir pieejami visi dati
        if (Validate(sender, e))
        {
            var course = (Course)CoursePicker.SelectedItem;
            var assignment = new Assignment(AssignmentDatePicker.Date, course, AssignmentDescriptionEntry.Text);
            db.Create(assignment);
            AssignmentDescriptionEntry.Text = String.Empty;
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
                Course course = (Course)CoursePicker.SelectedItem;
                //arī izveido jaunu objektu, kas būs tieši tāds pats kā datubāzē esošais.
                var assignment = new Assignment
                {        //https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interop/using-type-dynamic
                         //Tā kā mūsu saraksts satur ne tikai mūsu galveno klasi,
                         //bet arī citus datus, tas kļūst par anonīmās klases objektu,
                         //un mēs nevaram ar to veikt visas tās pašas darbības, ko ar klasi.
                         //Tāpēc pārveidojam to par dinamisku objektu, lai mēs varētu tieši mijiedarboties ar datiem.
                    Id = ((dynamic)bindingContext).Id,
                    Description = AssignmentDescriptionEntry.Text,
                    Deadline = AssignmentDatePicker.Date,
                    CourseId = course.Id
                };
                CoursePicker.SelectedItem = courses.FirstOrDefault(course => course.Id == assignment.CourseId);
                db.Update(assignment);
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
            //https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interop/using-type-dynamic
            //Tā kā mūsu saraksts satur ne tikai mūsu galveno klasi,
            //bet arī citus datus, tas kļūst par anonīmās klases objektu,
            //un mēs nevaram ar to veikt visas tās pašas darbības, ko ar klasi.
            //Tāpēc pārveidojam to par dinamisku objektu, lai mēs varētu tieši mijiedarboties ar datiem.

            //arī izveido jaunu objektu, kas būs tieši tāds pats kā datubāzē esošais.
            var assignment = new Assignment {
                Id = ((dynamic)bindingContext).Id,
                Description = ((dynamic)bindingContext).Description,
                CourseId = ((dynamic)bindingContext).CourseId,
                Deadline = ((dynamic)bindingContext).Deadline
            };
            db.Delete(assignment);
            Success();
            GetItems();
        }
    }
}