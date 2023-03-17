using Maui_Project_Uppgift.Models;

namespace Maui_Project_Uppgift.Views;

public partial class BortagnaEmails : ContentPage
{
	public BortagnaEmails()
	{
		InitializeComponent();
        Borttaget.Text = "Alla borttagna  Emails: \n " + String.Join("\n ", MySingleton.Instance.GetItems());
    }
}