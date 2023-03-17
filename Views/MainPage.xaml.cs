namespace Maui_Project_Uppgift;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
    private async void OnClickedGoEmail(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Views.EmailSenderCompliment());
    }

    private void OnClickedGoBortagna(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Views.BortagnaEmails());
    }
}

