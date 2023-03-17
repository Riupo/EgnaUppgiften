using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui_Project_Uppgift.Models
{
    public class ComplimenterMailer
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public int Dagar { get; set; }
        public int DagarÅterstår { get; set; }
        public string Promp { get; set; }
        public string SendersName { get; set; }
    }
}
