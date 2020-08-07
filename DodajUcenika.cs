using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arhiva
{
    public partial class DodajUcenika : Form
    {
        string connectionString = Konekcija.konekcionistring;
        public static string defaultGen = "Generacija ";
        static string defaultOdl = "Odeljenje ";
        //string sortBy = "Ime";
        //string sortWay = "ASC";
        string sql = "SELECT Ime, Prezime, Generacija, Odeljenje.Naziv  FROM Ucenici INNER JOIN Odeljenje ON Odeljenje.ID = Ucenici.IDOdeljenja";


        public DodajUcenika()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        OpenFileDialog ofd = new OpenFileDialog();

        

        private string getIDOdeljenja(string cBGen, string cBOdl)
        {
            pocetak:
            string generacija;
            string odeljenje;
            string iD;
            string sql = "SELECT ID, Naziv,Generacija FROM Arhivad.dbo.Odeljenje WHERE Generacija=" + cBGen + " AND Naziv='" + cBOdl + "'";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                connection.Open();

                var myTable = new DataTable();
                adapter.Fill(myTable);
                if (myTable.Rows.Count>0)
                {
                    iD = myTable.Rows[0][0].ToString();
                    connection.Close();
                    return iD;
                }
                    //PrikazTabele.DataSource = myTable;
                    //PrikazTabele.Columns["ID"].Visible = false;
                    connection.Close();
            }
            
            
                sql = "INSERT INTO Arhivad.dbo.Odeljenje (Naziv,Generacija) VALUES ('" + cBOdl + "','" + cBGen + "')";
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                goto pocetak;
            
        }
    

        private void Form2_Load(object sender, EventArgs e)
        {
            CBGeneracija.Items.Add(defaultGen);
            CBGeneracija.SelectedIndex = 0;
            LoadCBGen();
            LoadCBOdl();
        }

        private void LoadCBGen()
        {
            CBGeneracija.Items.Clear();
            CBGeneracija.Items.Add(defaultGen);
            CBGeneracija.SelectedIndex = 0;
            string sql = "SELECT Generacija FROM Arhivad.dbo.Odeljenje GROUP BY Generacija ORDER BY Generacija ASC";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                connection.Open();
                var myTable = new DataTable();
                adapter.Fill(myTable);
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    CBGeneracija.Items.Add(myTable.Rows[i][0].ToString());
                }
            
                //PrikazTabele.DataSource = myTable;
                //PrikazTabele.Columns["ID"].Visible = false;
                connection.Close();

            }
        }


        private void LoadCBOdl()
        {
            CBOdeljenje.Items.Clear();
            CBOdeljenje.Items.Add(defaultOdl);
            CBOdeljenje.SelectedIndex = 0;

            CBOdeljenje.Items.Add("A");
            CBOdeljenje.Items.Add("B");
            CBOdeljenje.Items.Add("C");
            CBOdeljenje.Items.Add("D");
            CBOdeljenje.Items.Add("E");
            CBOdeljenje.Items.Add("1");
            CBOdeljenje.Items.Add("2");
            CBOdeljenje.Items.Add("3");
            CBOdeljenje.Items.Add("4");
            CBOdeljenje.Items.Add("5");
            CBOdeljenje.Items.Add("6");
            CBOdeljenje.Items.Add("7");
            CBOdeljenje.Items.Add("8");
            CBOdeljenje.Items.Add("9");
            CBOdeljenje.Items.Add("10");



        }



        private void SaveButton_Click(object sender, EventArgs e)
        {
            string ime = Ime.Text;
            string prezime = Prezime.Text;
            string CBOdl = CBOdeljenje.Text.ToString();
            string CBGen = CBGeneracija.Text.ToString();

            if (ime == "" || prezime == "" || CBOdl == defaultOdl || CBGen == defaultGen)
            {
                MessageBox.Show("Unesite podatke u sva polja");
                return;
            }

            string sql = "INSERT INTO Arhivad.dbo.Ucenici (Ime,Prezime,IDOdeljenja) VALUES ('" + ime + "','" + prezime + "'," + getIDOdeljenja(CBGen, CBOdl) + ")";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            MessageBox.Show("Uspesno ste dodali ucenika " + ime + " " + prezime);
            this.Close();
        }

    }
}
