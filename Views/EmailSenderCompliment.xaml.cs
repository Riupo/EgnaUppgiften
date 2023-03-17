using Amazon.Runtime.Internal.Settings;
using Maui_Project_Uppgift.Models;
using Maui_Project_Uppgift.ViewModels;
using Microsoft.Maui.Controls;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.IO;
namespace Maui_Project_Uppgift.Views;

public partial class EmailSenderCompliment : ContentPage
{
    static string apiKey = "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";
    static HttpClient Http = new HttpClient();
    public EmailSenderCompliment()
    {
        InitializeComponent();
        BindingContext = new ViewModels.MailPageVM();
    }
    bool pageStarted = false;
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        if (!pageStarted)
        {
            Task t = (BindingContext as MailPageVM).GetMail(); // Metod i ViewModel
            pageStarted = true;
        }
    }
    private async void Send(object sender, EventArgs e)
    {
        var settings = MongoClientSettings.FromConnectionString("mongodb+srv://EmailSender:EmailSender123@emailsenders.4adqsx8.mongodb.net/?retryWrites=true&w=majority");
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(settings);
        var database = client.GetDatabase("ComplimentMailerDb");
        var myCollection = database.GetCollection<Models.ComplimenterMailer>("MyProductCollection");

        string subject = "Compliment of the day";
        var fromAddress = new MailAddress("mailsender@digitalfunz.com", "Complimenter"); // Skriv din email
        string fromPassword = "20Bilal20!?"; // Skriv in ditt lössenord
        var viewModel = BindingContext as MailPageVM;
        var userList = viewModel.Mails.ToList();
        var toAddresses = userList.Select(u => u.Email).ToArray();
        int mailSenderCount = toAddresses.Length;

        //foreach (var user in userList)
        Parallel.ForEach(userList, async (user) =>
        {
            // Get the quote text and author
            (string text, string author) = GetQuotes();

            // Declare the text and author variables separately
            string quoteText = text;
            string quoteAuthor = author;
            if (user.Dagar <= user.DagarÅterstår)
            {
                MySingleton.Instance.AddItem(user.Email);
                var filter = Builders<ComplimenterMailer>.Filter.Eq(c => c.Id, user.Id);
                await myCollection.DeleteOneAsync(filter);
                userList.Remove(user);
            }
            else
            {
                user.DagarÅterstår += 1;
                var filter = Builders<ComplimenterMailer>.Filter.Eq(c => c.Id, user.Id);
                var update = Builders<ComplimenterMailer>.Update.Set(c => c.DagarÅterstår, user.DagarÅterstår);
                await myCollection.UpdateOneAsync(filter, update);


                // Send a compliment to this user
                string compliment;
                var toAddress = new MailAddress(user.Email);
                if (user.Promp == "X")
                {
                    compliment = await WithoutPromp();
                }
                else
                {
                    compliment = await Promp(user.Promp);
                }
                string body;
                string imageFilePath = "C:\\Users\\Bilal\\OneDrive\\Documents\\Visual Studio 2022\\Demos\\Maui Project Uppgift\\Resources\\Images\\lightbluerobot.png";
                LinkedResource imageResource = new LinkedResource(imageFilePath, "image/png");
                imageResource.ContentId = "image1";     
                if (user.SendersName == "X" || user.SendersName == null)
                {
                    body = $@"<html>
    <body>
    <p><font size=""5""><strong>Todays Compliment</strong></font></p>
    <p>{compliment}</p>
    <p><font size=""5""><strong>Todays Quote</strong></font></p>
    <p>{quoteText} - {quoteAuthor}</p>
    <p>Sincerely,<br><strong>- DigitalFunz.com</strong></br></p>
    <p><img src=""cid:image1""></p>
    </body>
    </html>
    ";
                }
                else
                {
                    body = $@"<html>
    <body>
    <p><font size=""5""><strong>Todays Compliment</strong></font></p>
    <p>{compliment}</p>
        <p><font size=""5""><strong>Todays Quote</strong></font></p>
        <p>{quoteText} - {quoteAuthor}</p>
    <p>Sincerely,<br><strong>- {user.SendersName}</strong></br></p>
    <p><img src=""cid:image1""></p>
    </body>
    </html>
    ";
                }
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
                htmlView.LinkedResources.Add(imageResource);
                message.AlternateViews.Add(htmlView);

                var smtp = new SmtpClient
                {
                    Host = "mail.privateemail.com", // Skriv din egna smtp
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword)
                };
                //Sends The message
                smtp.Send(message);
            }
        });
        Skickade.Text = "Skickat!";
    }

    public static async Task<string> WithoutPromp()
    {
        Http.DefaultRequestHeaders.Remove("Authorization"); // Remove existing Authorization header
        Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        // JSON content for the API call
        var jsonContent = new
        {
            prompt = "Generate a random compliment between 20 and 50 words",
            model = "text-davinci-003",
            max_tokens = 1000
        };

        // Make the API call
        var responseContent = await Http.PostAsync("https://api.openai.com/v1/completions", new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json"));

        // Read the response as a string
        var resContext = await responseContent.Content.ReadAsStringAsync();

        // Deserialize the response into a dynamic object
        var data = JsonConvert.DeserializeObject<dynamic>(resContext);
        return data.choices[0].text;
    }
    public static async Task<string> Promp(string promp)
    {
        Http.DefaultRequestHeaders.Remove("Authorization"); // Remove existing Authorization header
        Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        // JSON content for the API call
        var jsonContent = new
        {
            prompt = "Generate a random compliment between 20 and 50 words, the person has " + promp,
            model = "text-davinci-003",
            max_tokens = 1000
        };


        // Make the API call
        var responseContent = await Http.PostAsync("https://api.openai.com/v1/completions", new StringContent(JsonConvert.SerializeObject(jsonContent), Encoding.UTF8, "application/json"));

        // Read the response as a string
        var resContext = await responseContent.Content.ReadAsStringAsync();

        // Deserialize the response into a dynamic object
        var data = JsonConvert.DeserializeObject<dynamic>(resContext);
        return data.choices[0].text;
    }
    public static (string text, string author) GetQuotes()
    {
        // Read the entire contents of the text file into a string variable
        string quotesJson = System.IO.File.ReadAllText("TextFile1.txt");

        // Deserialize the JSON string into an array of Quote objects
        Quote[] quotes = JsonConvert.DeserializeObject<Quote[]>(quotesJson);

        // Choose a random quote from the file
        Random rand = new Random();
        Quote randomQuote = quotes[rand.Next(quotes.Length)];

        // Return the chosen quote's text and author as separate values
        return (randomQuote.Text, randomQuote.Author);
    }
}