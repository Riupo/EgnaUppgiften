using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maui_Project_Uppgift.Models;
using MongoDB.Driver;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui_Project_Uppgift.ViewModels
{
    internal partial class MailPageVM : ObservableObject
    {

        [ObservableProperty]
        ObservableCollection<Models.ComplimenterMailer> mails;

        [ObservableProperty]
        Guid id;
        [ObservableProperty]
        string email;
        [ObservableProperty]
        int dagar;
        [ObservableProperty]
        int dagarÅterstår;
        [ObservableProperty]
        string promp;
        [ObservableProperty]
        string sendersName;


        public Models.Emails Emails { get; set; }
        public MailPageVM()
        {
            Mails = new ObservableCollection<Models.ComplimenterMailer>();

            //Mails.Add(new Models.ComplimenterMailer
            //{
            //    Id = Guid.NewGuid(),
            //    Email = "Test@test.com",
            //    Dagar = 999,
            //    DagarÅterstår = 995,
            //    Promp = "X",
            //    SendersName = "Test"
            //});

            Emails = new Emails()
            {
                Title = "ComplimentMailer",
                HeaderImageSource = "lightbluerobot.png"
            };
        }
        [RelayCommand]
        public async void AddComplimentMail()
        {
            ComplimenterMailer mail = new ComplimenterMailer()
            {
                Id = Guid.NewGuid(),
                Email = Email,
                Dagar = Dagar,
                DagarÅterstår = DagarÅterstår,
                Promp = Promp,
                SendersName = SendersName
            };

            await GetDbCollection().InsertOneAsync(mail);

            Mails.Add(mail);
        }
        [RelayCommand]
        public async void DeleteMail(object p)
        {
            var prod = (ComplimenterMailer)p;
            await GetDbCollection().DeleteOneAsync(x => x.Id == prod.Id);
            Mails.Remove(prod);
        }

        public async Task GetMail()
        {
            List<ComplimenterMailer> mailFromDb = await GetDbCollection().AsQueryable().ToListAsync();
            await Task.Delay(100);
            mailFromDb.ForEach(x => Mails.Add(x));
        }

        public IMongoCollection<Models.ComplimenterMailer> GetDbCollection()
        {
            var settings = MongoClientSettings.FromConnectionString("mongodb+srv://EmailSender:EmailSender123@emailsenders.4adqsx8.mongodb.net/?retryWrites=true&w=majority");
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            var database = client.GetDatabase("ComplimentMailerDb");
            var myCollection = database.GetCollection<Models.ComplimenterMailer>("MyProductCollection");
            return myCollection;
        }
    }
}
