using ClassLibrary;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Views;

namespace Management_System_For_Teachers;

public partial class StudentPage : ContentPage
{
    //Savienošanās ar datubāzes instanci
    DatabaseHandler db = DatabaseHandler.Instance;
    public StudentPage()
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
        var students = db.GetStudents();
        var genderTypes = new List<GenderType>();
        genderTypes.Add(GenderType.Man);
        genderTypes.Add(GenderType.Woman);
        GenderPicker.ItemsSource = genderTypes;
        GenderPicker.SelectedItem = GenderType.Man;
        //Datu piešķiršana mūsu sarakstam
        listView.ItemsSource = null;
        listView.ItemsSource = students;
    }
    //Metode, kas norāda uz mūsu darbības veiksmīgumu.
    private void Success()
    {
        Application.Current.MainPage.DisplayAlert("Notification", "The operation has been successfully completed!", "OK");
    }
    //Tukšu lauku datu validācijas pārbaužu metode
    private bool Validate(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(StudentNameEntry.Text) ||
            String.IsNullOrEmpty(StudentSurnameEntry.Text) ||
            String.IsNullOrEmpty(StudentIdNumberEntry.Text) ||
            GenderPicker.SelectedItem == null)
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
        var student = (Student)e.Item;
        StudentNameEntry.Text = student.Name;
        StudentIdNumberEntry.Text = student.StudentIdNumber.ToString();
        StudentSurnameEntry.Text = student.Surname;
        if(student.Gender == GenderType.Man)
        {
            GenderPicker.SelectedItem = GenderType.Man;
        }
        else
        {
            GenderPicker.SelectedItem = GenderType.Woman;
        }
    }
    //Metode jaunu rindu pievienošanai datubāzē
    private void OnCreateClicked(object sender, EventArgs e)
    {
        //Pārbaudiet, vai veidlapā ir visi dati
        if (Validate(sender, e))
        {
            int stdId = 0;
            Int32.TryParse(StudentIdNumberEntry.Text, out stdId);
            var student = new Student(StudentNameEntry.Text, StudentSurnameEntry.Text, (GenderType)GenderPicker.SelectedItem , stdId);
            StudentNameEntry.Text = String.Empty;
            StudentSurnameEntry.Text = String.Empty;
            StudentIdNumberEntry.Text = String.Empty;
            db.Create(student);
            Success();
            GetItems();
        }
    }
    private void OnEditClicked(object sender, EventArgs e)
    {
        //Pārbaudiet saistošo kontekstu, lai pārliecinātos, ka tas atbilst klasei
        if (sender is Button button && button.BindingContext is Student student)
        {
        //Pārbaude, vai ir pieejami visi dati
            if (Validate(sender, e))
            {
                {
                    GenderType gender;
                    if ((GenderType)GenderPicker.SelectedItem == GenderType.Man)
                    {
                        gender = GenderType.Man;
                    }
                    else
                    {
                        gender = GenderType.Woman;
                    }
                    int stdId = 0;
                    Int32.TryParse(StudentIdNumberEntry.Text, out stdId);
                    student.Name = StudentNameEntry.Text;
                    student.Surname = StudentSurnameEntry.Text;
                    student.StudentIdNumber = stdId;
                    student.Gender = gender;
                    db.Update(student);
                    Success();
                    GetItems();
                }
            }
        }
    }
    private void OnDeleteClicked(object sender, EventArgs e)
    {
        // Pārbaudiet saistošo kontekstu, lai pārliecinātos, ka tas atbilst klasei
        if (sender is Button button && button.BindingContext is Student student)
        {
            db.Delete(student);
            Success();
            GetItems();
        }
    }
}