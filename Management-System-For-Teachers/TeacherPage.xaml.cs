using ClassLibrary;
using System;

namespace Management_System_For_Teachers;

public partial class TeacherPage : ContentPage
{
    //Savienošanās ar datubāzes instanci
    DatabaseHandler db = DatabaseHandler.Instance;
    public TeacherPage()
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
        var teachers = db.GetTeachers();
        //Datu piešķiršana mūsu sarakstam
        listView.ItemsSource = null;
        listView.ItemsSource = teachers;
        var genderTypes = new List<GenderType>();
        genderTypes.Add(GenderType.Man);
        genderTypes.Add(GenderType.Woman);
        GenderPicker.ItemsSource = genderTypes;
        GenderPicker.SelectedItem = GenderType.Man;
    }
    //Metode, kas norāda uz mūsu darbības veiksmīgumu.
    private void Success()
    {
        Application.Current.MainPage.DisplayAlert("Notification", "The operation has been successfully completed!", "OK");
    }
    //Metode, kas tiek izsaukta, kad noklikšķinot uz objekta sarakstā, objekta dati tiek pārsūtīti uz veidlapu.
    private void OnListItemTapped(object sender, ItemTappedEventArgs e)
    {
        var teacher = (Teacher)e.Item;
        TeacherNameEntry.Text = teacher.Name;
        TeacherContractDatePicker.Date = teacher.ContractDate;
        TeacherSurnameEntry.Text = teacher.Surname;
        GenderPicker.SelectedItem = teacher.Gender;
    }
    //Tukšu lauku datu validācijas pārbaužu metode
    private bool Validate(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(TeacherNameEntry.Text) ||
            String.IsNullOrEmpty(TeacherSurnameEntry.Text) ||
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
    //Metode jaunu rindu pievienošanai datubāzē
    private void OnCreateClicked(object sender, EventArgs e)
    {
        //Pārbaudiet, vai veidlapā ir visi dati
        if (Validate(sender, e))
        {
            var teacher = new Teacher(TeacherNameEntry.Text, TeacherSurnameEntry.Text, (GenderType)GenderPicker.SelectedItem, TeacherContractDatePicker.Date);
            TeacherNameEntry.Text = String.Empty;
            TeacherSurnameEntry.Text = String.Empty;
            db.Create(teacher);
            Success();
            GetItems();
        }
    }
    private void OnEditClicked(object sender, EventArgs e)
    {
        //Pārbaudiet saistošo kontekstu, lai pārliecinātos, ka tas atbilst klasei
        if (sender is Button EditButton && EditButton.BindingContext is Teacher teacher)
        {
        //Pārbaude, vai ir pieejami visi dati
            if (Validate(sender, e))
            {
                {
                    teacher.Name = TeacherNameEntry.Text;
                    teacher.Surname = TeacherSurnameEntry.Text;
                    teacher.Gender = (GenderType)GenderPicker.SelectedItem;
                    teacher.ContractDate = TeacherContractDatePicker.Date;
                    db.Update(teacher);
                    Success();
                    GetItems();
                }
            }
        }
    }
    private void OnDeleteClicked(object sender, EventArgs e)
    {
        //Pārbaudiet binding kontekstu, lai pārliecinātos, ka tas atbilst klasei
        if (sender is Button button && button.BindingContext is Teacher teacher)
        {
            db.Delete(teacher);
            Success();
            GetItems();
        }
    }
}