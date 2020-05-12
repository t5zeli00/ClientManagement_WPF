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
using MongoDB.Driver.Core;
using System.Configuration;
using System.Timers;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Client_Management
{
    /// <summary>
    /// Логика взаимодействия для ManagementPage.xaml
    /// </summary>
    /// 
    //The description of client
    class Person
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Background { get; set; }
        public string ProjectPlan { get; set; }
    }    

    public partial class ManagementPage : Page
    {     
        public string theName = ""; //name of the selected client
        public string email = ""; //email of the selected client

        public string backg = ""; //background of the selected client
        public string plan = ""; //plan of the selected client
        public string startdate = ""; //subscription start date of selected client
        public string enddate = ""; //subscription end date of selected client      

        public ManagementPage()
        {
            InitializeComponent();

            //string con = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;

            MainAsync(theName, false);
            

            //Timer initialization
            System.Timers.Timer tmr = new System.Timers.Timer();
            tmr.Elapsed += new ElapsedEventHandler(OnTimedEvent); //calling the function to save changed data in the databasse 
            tmr.Interval = 1000; //interval of 1 sec
            tmr.Enabled = true; //turn on the timer           

        }

        //The constructor of the information which is saved in .xml file
        public class information
        {
            private string theName;
            private string email;

            private string backg;
            private string plan;
            private string startdate;
            private string enddate;

            public string ClientName
            {
                get { return theName; }
                set { theName = value; }
            }

            public string ClientEmail
            {
                get { return email; }
                set { email = value; }
            }

            public string ClientBackground
            {
                get { return backg; }
                set { backg = value; }
            }

            public string ClientPlan
            {
                get { return plan; }
                set { plan = value; }
            }

            public string ClientStart
            {
                get { return startdate; }
                set { startdate = value; }
            }

            public string ClientEnd
            {
                get { return enddate; }
                set { enddate = value; }
            }
        }
        
        private async Task MainAsync(string selectedName, bool update)
        {
            //Connecting to the mLab database
            string connectstring = "mongodb://liubov:zeli123@ds235775.mlab.com:35775/clients";
            var client = new MongoClient(connectstring);

            IMongoDatabase db = client.GetDatabase("clients");
            
            //var collection = db.GetCollection<BsonDocument>("clients");

            var collection = db.GetCollection<Person>("clients");

            var filter = new BsonDocument();
            var projection = Builders<Person>.Projection.Expression(p => new Person {
                _id = p._id,
                Name = p.Name,
                Email = p.Email,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Background = p.Background,
                ProjectPlan = p.ProjectPlan
            });
            var employees = await collection.Find(filter).Project<Person>(projection).ToListAsync();            

            //The database update of subscription dates, background and plan information
            if (update == true)
            {               
               
                var filter1 = Builders<Person>.Filter.Eq("Name", selectedName);

                var update1 = Builders<Person>.Update
                     //.Set("Background", textBoxBackground.Text)
                     .Set("Background", backg)
                     .Set("ProjectPlan", plan)
                     .Set("StartDate", startdate)
                     .Set("EndDate", enddate)
                     .CurrentDate("lastModified");

                collection.UpdateOne(filter1, update1);

                //MessageBox.Show("Data for " + selectedName + " updated");             
                           

            } else
            {
                //Retrieving information from the database 
                foreach (var e in employees)
                {
                    if (selectedName == "") clientlistBox.Items.Add(e.Name); //creating the list of clients                     
                    else
                    {                        
                        if (selectedName == e.Name) //retrieving information about one selected client
                        {                         
                           Name.Content = e.Name;
                           Email.Content = e.Email;
                           dateStart.SelectedDate = DateTime.Parse(e.StartDate);
                           //dateEnd.SelectedDate = DateTime.Parse(e.EndDate);                           
                           //dateEnd.SelectedDate = dateStart.DisplayDate.AddDays(30);
                           textBoxBackground.Text = e.Background;
                           textBoxPlan.Text = e.ProjectPlan;

                           theName = e.Name;
                           email = e.Email;
                        }
                    }
                    //Console.WriteLine("{0}", e.Name);                
                }
            }      
                                 
            /*using (var cursor = await collection.FindAsync(new BsonDocument()))
            {
                while (await cursor.MoveNextAsync())
                {
                    var people = cursor.Current;
                    foreach (Person doc in people)
                    {
                        Console.WriteLine("{0} - {1}", doc._id, doc.Name);
                    }
                }
            }*/

            /*using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(new BsonDocument()))
            {
                while (await cursor.MoveNextAsync())
                {   
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        Console.WriteLine(document);
                        Console.WriteLine();
                    }
                }
            }*/
        }

        /*private async Task ClientLoad()
        {
            try
            {
                string connectstring = "mongodb://liubov:zeli123@ds235775.mlab.com:35775/clients";                
                var client = new MongoClient(connectstring);
                var db = client.GetDatabase("clients");                
                var collection = db.GetCollection<BsonDocument>("clients");                                

                /*BsonDocument client1 = new BsonDocument
                {
                    {"Name", "Bill"},
                    {"Age", 32},
                    {"Languages", new BsonArray{"english", "german"}}
                };
                await collection.InsertOneAsync(client1);


                //var filter1 = Builders<BsonDocument>.Filter.Empty;
                var filter = new BsonDocument();
                using (var cursor = await collection.FindAsync(filter))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        var people = cursor.Current;
                        foreach (var document in people)
                        {
                            textBlock.Text = document[1].ToString();                            
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }*/
                
        private void clientListBox_MouseClick(object sender, RoutedEventArgs e)
        {
            //textBox.Text = (this.clientlistBox.SelectedItem).ToString();
            theName = clientlistBox.SelectedItem.ToString();
            MainAsync(clientlistBox.SelectedItem.ToString(), false); //calling function to retrieve information about selected client           
        }        

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MainAsync(clientlistBox.SelectedItem.ToString(), true); //calling function to update information about selected client          
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e)
        {                        
            MainAsync(theName, true); //calling function to update information about selected client every 30 sec

            try
            {
                information info = new information();
                info.ClientName = theName;
                info.ClientEmail = email;
                info.ClientBackground = backg;
                info.ClientPlan = plan;
                info.ClientStart = startdate;
                info.ClientEnd = enddate;
                savedata(info, theName + ".xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected void textBoxBackground_TextChanged(object sender, EventArgs e)
        {
            backg = textBoxBackground.Text.ToString();
        }

        protected void textBoxPlan_TextChanged(object sender, EventArgs e)
        {
            plan = textBoxPlan.Text.ToString();
        }

        private void dateStart_ValueChanged(object sender, EventArgs e)
        {            
            startdate = dateStart.SelectedDate.ToString();

            dateStart.DisplayDate = DateTime.Parse(dateStart.SelectedDate.ToString());
            dateEnd_ValueChanged(sender, e);
        }

        private void dateEnd_ValueChanged(object sender, EventArgs e)
        {            
            dateEnd.SelectedDate = dateStart.DisplayDate.AddDays(30);

            enddate = dateEnd.SelectedDate.ToString();
        }

        public static void savedata(object obj, string filename)
        {
            XmlSerializer sr = new XmlSerializer(obj.GetType());
            TextWriter writer = new StreamWriter(filename);
            sr.Serialize(writer, obj);
            writer.Close();
        }

        private void buttonNewClient_Click(object sender, RoutedEventArgs e)
        {
            NewClient newclientPage = new NewClient();
            this.NavigationService.Navigate(newclientPage);
        }

        /*private void button1_Click(object sender, RoutedEventArgs e)
        {

            /*ObjectCache cache = MemoryCache.Default;
            string fileContents = cache["filecontents"] as string;

            if (fileContents == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration =
                    DateTimeOffset.Now.AddSeconds(10.0);

                List<string> filePaths = new List<string>();
                filePaths.Add("c:\\cache\\cacheText.txt");

                policy.ChangeMonitors.Add(new
                    HostFileChangeMonitor(filePaths));

                // Fetch the file contents.
                fileContents = File.ReadAllText("c:\\cache\\cacheText.txt") + "\n" + DateTime.Now.ToString();

                cache.Set("filecontents", fileContents, policy);

            }
            MessageBox.Show(fileContents);*/

        /*SaveFileDialog saveFileDialog = new SaveFileDialog();

        if (saveFileDialog.ShowDialog() == true)
        {
            string filename = saveFileDialog.FileName;                
            File.WriteAllText(filename, backg);
        }
    }*/
    }
}
