using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace Client_Management
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();            
        }

        //The database backup function
        public static void MainBackup()
        {
            //This ProcessStartInfo class define the environment to open the cmd prompt
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                //Right under this directory is the MongoDump Directory
                WorkingDirectory = "C:\\Users\\Любовь\\Documents\\Client_database_backup",
                //The program to execute. Because mongodump.exe is added to the environment path, the cmd line does not need an absolute path to find the executable
                FileName = "mongodump",
                Arguments = "-h ds235775.mlab.com:35775 -d clients -u liubov -p zeli123 -o MongoDump\\" + DateTime.Now.ToString("yyyyMMddhhmm"),
                //CreateNoWindow = true,  //The prompt window will not show
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (Process exeProcess = Process.Start(startInfo))
            {
                if (exeProcess != null) exeProcess.WaitForExit();
                //We don't need any return words, therefore you can ignore the code below
                while (!exeProcess.StandardOutput.EndOfStream)
                {
                    string line = exeProcess.StandardOutput.ReadLine();                    
                    Console.WriteLine(line);
                }
            }
        }

        //Calling the database backup function when the app is closing
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            MainBackup();
        }
    }
} 
