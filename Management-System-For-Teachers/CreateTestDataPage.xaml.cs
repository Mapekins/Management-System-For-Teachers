using ClassLibrary;

namespace Management_System_For_Teachers;

public partial class CreateTestDataPage : ContentPage
{
    //Savienošanās ar datubāzes instanci
    private DatabaseHandler db = DatabaseHandler.Instance;
	public CreateTestDataPage()
	{
		InitializeComponent();

    }
    private void Success()
    {
        Application.Current.MainPage.DisplayAlert("Notification", "The operation has been successfully completed!", "OK");
    }
    private void Error()
    {
        Application.Current.MainPage.DisplayAlert("Notification", "Database already have data!", "OK");
    }
    private void OnCreateTestDataClicked(object sender, EventArgs e)
	{
        if (db.CreateTestData())
        {
            Success();
        }
        else
        {
            Error();
        }
    }
}