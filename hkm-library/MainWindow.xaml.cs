using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
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


namespace hkm_library_db
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {

        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DA;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();

        string filename;


        public MainWindow()
        {

            InitializeComponent();

            //creating database file

            if (!File.Exists("MyDatabase.sqlite"))
            {
                SQLiteConnection.CreateFile("MyDatabase.sqlite");
            }
            //setConnection();    //setting connection

            //sql_con.Open();      //opening connection


            
            

            //creating table
            createDTable();

            /////////////////////////////

            //adding values to table
            //Add();


            //loading data to DataGrid
            LoadData();

            dataGrid1.IsReadOnly = true;
            dataGrid2.IsReadOnly = true;


            //sql_con.Close();  //closing connection

           
        }


        private void createDTable()
        {
            setConnection();    //setting connection

            sql_con.Open();      //opening connection


            string sql = "create table if not exists library (No integer primary key autoincrement,Book_Name varchar(30), Author_Name varchar(30),Genre varchar(10),ISBN varchar(20),Code varchar(10),link varchar(20) )";

            SQLiteCommand command = new SQLiteCommand(sql, sql_con);
           
            command.ExecuteNonQuery();

            sql_con.Close();  //closing connection

        }




        private void setConnection()
        {
            sql_con = new SQLiteConnection
                ("Data Source=MyDatabase.sqlite;Version=3;");
        }





        private void executeQuery(string txtQuery)
        {
            setConnection();    //setting connection

            sql_con.Open();      //opening connection

            

            sql_cmd = sql_con.CreateCommand();

            sql_cmd.CommandText = txtQuery;

            sql_cmd.ExecuteNonQuery();


            sql_con.Close();  //closing connection
            
        }




        private void LoadData()
        {
            setConnection();    //setting connection

            sql_con.Open();      //opening connection
          

            sql_cmd = sql_con.CreateCommand();

            string commandText = "select * from library order by No asc";    //which data is going to be loaded is determined by sql command

            DA = new SQLiteDataAdapter(commandText, sql_con);


            try
            {

                DS.Reset();

            }
            catch(System.NullReferenceException e)
            {
                //e.Message;

            }
             

            DA.Fill(DS);

            DT = DS.Tables[0];

            dataGrid1.ItemsSource = DT.DefaultView;
          
            dataGrid1.MinColumnWidth = 35;
            dataGrid1.MaxColumnWidth = 200;

            //dataGrid1.Columns[1].MinWidth = 35;

            dataGrid2.ItemsSource = DT.DefaultView;
           
            dataGrid2.MinColumnWidth = 35;
            dataGrid2.MaxColumnWidth = 200;

      
            
       
           



            
            sql_con.Close();  //closing connection

        }

        public void SetMinWidths(object source, EventArgs e)
        {
            foreach (var column in dataGrid1.Columns)
            {
                column.MinWidth = column.ActualWidth;
                column.Width = 100;
                    //new DataGridLength(1, DataGridLengthUnitType.SizeToHeader);
            }
        }

        public void SetMinWidths2(object source, EventArgs e)
        {
            foreach (var column in dataGrid2.Columns)
            {
                column.MinWidth = column.ActualWidth;
                column.Width = 35;
                //new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }





        private void Add()
        {
            string txtSQLQuery = "insert into  library (No,Book_Name,Author_Name,Genre,ISBN,Code) values (null,'Great Expectations','Charles Dickens','Classic Fiction','978-3-16-148410-0',1234)";
            executeQuery(txtSQLQuery);
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Add2();
            LoadData();

            //dataGrid1.Loaded += SetMinWidths;
        }



        private void Add2()
        {
            string txtSQLQuery = "insert into  library (No,Book_Name,Author_Name,Genre,ISBN,Code,link) values (null,'" + txtbx2.Text.ToString() + "','" + txtbx1.Text.ToString() + "','" + txtbx3.Text.ToString() + "','" + txtbx4.Text.ToString() + "','" + txtbx5.Text.ToString() + "','" + image_link.Text.ToString() + "')";
             
            executeQuery(txtSQLQuery);
        }



        private void update()
        {

            string txtSQLQuery = "UPDATE library SET Book_Name='" + txtbx2_copy.Text.ToString() + "',Author_Name='" + txtbx1_copy.Text.ToString() + "',Genre='" + txtbx3_copy.Text.ToString() + "',ISBN='" + txtbx4_copy.Text.ToString() + "',Code='" + txtbx5_copy.Text.ToString() + "',link='" + txtbx5_copy.Text.ToString() + "' WHERE No ="+no_label.Content.ToString();
            executeQuery(txtSQLQuery);
            


        }





        private void dataGrid2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          
           object item = dataGrid2.SelectedItem;
           string Auth_name = (dataGrid2.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
           string Bk_name = (dataGrid2.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
           string Genre = (dataGrid2.SelectedCells[3].Column.GetCellContent(item) as TextBlock).Text;
           string Code = (dataGrid2.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text;
           string isbn = (dataGrid2.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text;
           string no = (dataGrid2.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;

           try
           {
               string link = (dataGrid2.SelectedCells[6].Column.GetCellContent(item) as TextBlock).Text;

               Up_image_link.Text = link;


               Up_book_image.Source = new BitmapImage(new Uri(link, UriKind.Absolute));
           }
           catch
           {
               Up_image_link.Text = "Please paste a valid link.";

           }
            txtbx1_copy.Text = Auth_name;
            txtbx2_copy.Text = Bk_name;
            txtbx3_copy.Text = Genre;
            txtbx4_copy.Text = isbn;
            txtbx5_copy.Text = Code;
            no_label.Content = no;
            
            
        }

        private void save_image(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.jpg)|*.jpg"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                filename = dlg.FileName;

                book_image.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                string apppath = "C:\\Users\\Ali\\Documents\\Visual Studio 2013\\Projects\\WpfApplication2\\WpfApplication2\\bin\\Debug\\Saved Images\\";
                ///////////////exception yet to define
                string name = txtbx2.Text.ToString()+".jpg";

                image_link.Text = apppath + name;

                //if(name.Equals(name))
               
          //////////////////////////////
                if (File.Exists(apppath+name))
                {
                    File.Delete(apppath + name);
                    //MessageBox.Show("Replaced");
                }
                
                    File.Copy(filename, apppath + name);
                


            }

        }

        private void update_button(object sender, RoutedEventArgs e)
        {
            update();

            LoadData();
        }

  
    }
}
