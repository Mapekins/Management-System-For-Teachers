using ClassLibrary;
using System;

namespace Management_System_For_Teachers;

public partial class CoursePage : ContentPage
{
    //Savienošanās ar datubāzes instanci
    DatabaseHandler db = DatabaseHandler.Instance;
    private List<Teacher> teachers;
    public CoursePage()
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
        var courses = db.GetCourses();
        teachers = db.GetTeachers();
        TeacherPicker.ItemsSource = teachers;
        TeacherPicker.BindingContext = teachers;
        //Ja datubāzē ir dati, atzīmējiet pirmo vaicājumu kā atlasītu.
        if (teachers.Any())
        {
            TeacherPicker.SelectedItem = teachers[0];
        }
        //Kombinētā saraksta izveide priekš Binding
        var CoursesWithTeachers = courses.Select(course => new
        {
            course.Id,
            course.Name,
            course.TeacherId,
            TeacherName = teachers
            .FirstOrDefault(teacher => teacher.Id == course.TeacherId)?.FullName ?? "Missiong Teacher"
        }).ToList();
        //Datu piešķiršana mūsu sarakstam
        listView.ItemsSource = null;
        listView.ItemsSource = CoursesWithTeachers;
    }
    //Metode, kas norāda uz mūsu darbības veiksmīgumu.
    private void Success()
    {
        Application.Current.MainPage.DisplayAlert("Notification", "The operation has been successfully completed!", "OK");
    }
    //Tukšu lauku datu validācijas pārbaužu metode
    private bool Validate(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(CourseNameEntry.Text) ||
            TeacherPicker.SelectedItem == null
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
        var course = db.GetCourseById(selectedItem.Id);
        CourseNameEntry.Text = course.Name;
        //LINQ izmantošana saraksta meklēšanai
        TeacherPicker.SelectedItem = teachers.FirstOrDefault(x=>x.Id == course.TeacherId);
    }
    //Metode jaunu rindu pievienošanai datubāzē
    private void OnCreateClicked(object sender, EventArgs e)
    {
        //Pārbaudiet, vai veidlapā ir visi dati
        if(Validate(sender, e))
        {
            var teacher = (Teacher)TeacherPicker.SelectedItem;
            var course = new Course(CourseNameEntry.Text, teacher);
            db.Create(course);
            CourseNameEntry.Text = String.Empty;
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
                Teacher teacher = (Teacher)TeacherPicker.SelectedItem;
                //arī izveido jaunu objektu, kas būs tieši tāds pats kā datubāzē esošais.
                var course = new Course { Id = ((dynamic)bindingContext).Id, Name = CourseNameEntry.Text, TeacherId = teacher.Id };
                //https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interop/using-type-dynamic
                //Tā kā mūsu saraksts satur ne tikai mūsu galveno klasi,
                //bet arī citus datus, tas kļūst par anonīmās klases objektu,
                //un mēs nevaram ar to veikt visas tās pašas darbības, ko ar klasi.
                //Tāpēc pārveidojam to par dinamisku objektu, lai mēs varētu tieši mijiedarboties ar datiem.
                TeacherPicker.SelectedItem = teachers.FirstOrDefault(x => x.Id == course.TeacherId);
                db.Update(course);
                CourseNameEntry.Text = String.Empty;
                Success() ;
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
            //arī izveido jaunu objektu, kas būs tieši tāds pats kā datubāzē esošais.
            var course = new Course { Id = ((dynamic)bindingContext).Id, Name = ((dynamic)bindingContext).Name, TeacherId = ((dynamic)bindingContext).TeacherId };
            //https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interop/using-type-dynamic
            //Tā kā mūsu saraksts satur ne tikai mūsu galveno klasi,
            //bet arī citus datus, tas kļūst par anonīmās klases objektu,
            //un mēs nevaram ar to veikt visas tās pašas darbības, ko ar klasi.
            //Tāpēc pārveidojam to par dinamisku objektu, lai mēs varētu tieši mijiedarboties ar datiem.
            db.Delete(course);
            Success();
            GetItems();
        }
    }
}