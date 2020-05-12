using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Client_Management
{
    /// <summary>
    /// Логика взаимодействия для NewClient.xaml
    /// </summary>   
    public partial class NewClient : Page
    {
        public NewClient()
        {
            InitializeComponent();
        }

        private async Task MainAsync()
        {            
            //Connecting to the mLab database
            string connectstring = "mongodb://liubov:zeli123@ds235775.mlab.com:35775/clients";
            var client = new MongoClient(connectstring);

            IMongoDatabase db = client.GetDatabase("clients");            

            var collection = db.GetCollection<BsonDocument>("clients");

            var filter = new BsonDocument();        
                       
            BsonDocument person = new BsonDocument
            {
                {"Name", textBoxName.Text},
                {"Email", textBoxEmail.Text},
                {"StartDate", dateStart.SelectedDate.ToString()},
                {"EndDate", dateEnd.SelectedDate.ToString()},
                {"Background", textBoxBackground.Text},
                {"ProjectPlan", textBoxPlan.Text}
            };
            await collection.InsertOneAsync(person);            

            ManagementPage managementPage = new ManagementPage();
            this.NavigationService.Navigate(managementPage);
        }

        private void dateStart_ValueChanged(object sender, EventArgs e)
        {          
            dateStart.DisplayDate = DateTime.Parse(dateStart.SelectedDate.ToString());
            dateEnd_ValueChanged(sender, e);
        }

        private void dateEnd_ValueChanged(object sender, EventArgs e)
        {
            dateEnd.SelectedDate = dateStart.DisplayDate.AddDays(30);           
        }

        //Check if the addded email is valid
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {          
            if (textBoxName.Text == "") MessageBox.Show("The name of the client is required!");
            else if (textBoxEmail.Text == "") MessageBox.Show("The email of the client is required!");
            else if (IsValidEmail(textBoxEmail.Text) == false) MessageBox.Show("The valid emal is required!");
            else if (dateStart.SelectedDate.ToString() == "") MessageBox.Show("The start date of subscription of the client is required!");
            else if (textBoxBackground.Text == "") MessageBox.Show("The background of the client is required!");
            else if (textBoxPlan.Text == "") MessageBox.Show("The plan of the client is required!");
            else MainAsync();            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ManagementPage managementPage = new ManagementPage();
            this.NavigationService.Navigate(managementPage);
        }
    }
}
