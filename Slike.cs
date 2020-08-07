using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization;



namespace Arhiva
{
    public partial class Slike : Form
    {
        static string lokacija = @"lokacija/";
        string connectionString = Konekcija.konekcionistring;
        List<String> row;
        string ime;
        string prezime;
        string generacija;
        string odeljenje;
        string sifra;
        public Slike(List<String> row)
        {
            this.row = row;
            sifra = row[0];
            ime = row[1];
            prezime = row[2];
            generacija = row[3];
            odeljenje = row[4];

            InitializeComponent();
        }

        public class Sifrica
        {
            private static int sifra1;
            public static int getsifra()
            {
                return sifra1;
            }

            public static void setSifra(int sif) { sifra1 = sif; }
        }


        private void SidePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            int sifradalje = System.Convert.ToInt32(sifra);
            Sifrica.setSifra(sifradalje);

            //treba da prikazuje ime i prezime kako bi lakse spazio ako imas vise otvorenih formi sta znam
            string imepanel;
            ImePanel.Text = "Ime Prezime";
            Ime1.Text = ime;
            Prezime1.Text = prezime;
            Generacija1.Text = generacija;
            Odeljenje1.Text = odeljenje;
            if (Ime1.Text != "" && Prezime1.Text != "")
            {
                imepanel = Ime1.Text + " " + Prezime1.Text;
                ImePanel.Text = imepanel;
            }
            string sql = "SELECT Top 1 ID From Fajlovi ORDER BY Fajlovi.ID DESC";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                connection.Open();
                var myTable = new DataTable();
                adapter.Fill(myTable);
            }

            DopremiFajlove();



        }





        OpenFileDialog file = new OpenFileDialog();
        int index = 0;
        bool selectedImage = false;
        Imenovanje imenovanje = new Imenovanje();
        Dictionary<string, Image> sveSlike = new Dictionary<string, Image>();
        Dictionary<string, string> dodatiFajlovi = new Dictionary<string, string>();




        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(Application.StartupPath + "\\" + sveSlike.Keys.ElementAt(index));

        }

        public void DopremiFajlove()
        {
            string sql = "SELECT Fajlovi.Vrsta,Fajlovi.Ime FROM Ucenici INNER JOIN (VezeUc INNER JOIN Fajlovi on VezeUC.IDFajl=Fajlovi.ID) ON Ucenici.Sifra=VezeUc.SifraUc WHERE Ucenici.Sifra='" + sifra + "'";
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                connection.Open();
                var myTable = new DataTable();
                adapter.Fill(myTable);
                for (int i = 0; i < myTable.Rows.Count; i++)
                {
                    string s = lokacija + myTable.Rows[i][1].ToString().Replace(" ", "") + ".jpg";
                    using (FileStream stream = new FileStream(s, FileMode.Open, FileAccess.Read))
                    {
                        sveSlike.Add(s, Image.FromStream(stream));
                    }
                    SviFajlovi.Items.Add(myTable.Rows[i][0]);
                }
                SledSlika();
                connection.Close();

            }

        }
        public void SledSlika()
        {
            if (--index < 0)
                index = sveSlike.Count - 1;
            if (sveSlike.Count == 0) return;
            pictureBox1.Image = (sveSlike.Values.ElementAt(index));
            //String tip = dodatiFajlovi.Keys.ElementAt(index);
            //int listBoxIndex = SviFajlovi.Items.IndexOf(tip);
            //SviFajlovi.SetSelected(listBoxIndex, true);
        }
        public void ProslaSlika()
        {
            if (++index > sveSlike.Count - 1)
                index = 0;
            if (sveSlike.Count == 0) return;
            pictureBox1.Image = (sveSlike.Values.ElementAt(index));
            //String tip = dodatiFajlovi.Keys.ElementAt(index);
            // int listBoxIndex = SviFajlovi.Items.IndexOf(tip);
            //SviFajlovi.SetSelected(listBoxIndex, true);
        }
        public void PrikaziSliku(int i)
        {
            if (i > -1)
            {
                pictureBox1.Image = (sveSlike.Values.ElementAt(i));
                index = i;
            }
        }


        private void SviFajlovi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SviFajlovi.SelectedIndex > -1)
                PrikaziSliku(SviFajlovi.SelectedIndex);
        }
        private void Sacuvaj_Click_1(object sender, EventArgs e)
        {

        }




        private void SviFajlovi_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                int index = SviFajlovi.IndexFromPoint(e.X, e.Y);
                SviFajlovi.SelectedIndex = index;
                MenuItem[] mi = new MenuItem[] { new MenuItem("Obrisi sliku") };
                ContextMenu cm = new ContextMenu(mi);
                SviFajlovi.ContextMenu = cm;
                SviFajlovi.ContextMenu.Show(SviFajlovi, new Point(e.X, e.Y));
                cm.MenuItems[0].Click += new EventHandler(deleteClick);
                //  cm.MenuItems[1].Click += new EventHandler(editClick);
            }
        }


        private void deleteClick(object sender, EventArgs e)
        {
            var putanja = sveSlike.Keys.ElementAt(index);
            var imefajla = putanja.TrimStart(lokacija.ToCharArray()).TrimEnd(".jpg".ToCharArray());
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlTransaction sqlTrans = sqlConnection.BeginTransaction())
                {
                    var broj = 0;
                    using (SqlCommand sqlCommand = new SqlCommand(@"select ID from fajlovi where ime=@ime", sqlConnection, sqlTrans))
                    {
                        sqlCommand.Parameters.Add("@ime", SqlDbType.VarChar).Value = imefajla;
                        Object o = sqlCommand.ExecuteScalar();
                        if (o != null)
                            broj = Int32.Parse(o.ToString());
                    }

                    using (SqlCommand sqlCommand = new SqlCommand(@"DELETE FROM VezeUc WHERE IDFajl=@broj", sqlConnection, sqlTrans))
                    {
                        sqlCommand.Parameters.Add("@broj", SqlDbType.Int).Value = broj;
                        sqlCommand.ExecuteNonQuery();
                    }

                    using (SqlCommand sqlCommand = new SqlCommand(@"DELETE FROM Fajlovi WHERE Fajlovi.ID=@broj", sqlConnection, sqlTrans))
                    {
                        sqlCommand.Parameters.Add("@broj", SqlDbType.Int).Value = broj;
                        sqlCommand.ExecuteNonQuery();
                    }

                    Application.DoEvents();
                    if (pictureBox1.Image != null)
                        pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                    Application.DoEvents();
                    if (pictureBox1.InitialImage != null)
                        pictureBox1.InitialImage.Dispose();
                    pictureBox1.InitialImage = null;
                    pictureBox1.Update();
                    Application.DoEvents();
                    pictureBox1.Refresh();


                    SviFajlovi.Items.RemoveAt(index);
                    sveSlike.Remove(putanja);

                    if (File.Exists(putanja))
                        File.Delete(putanja);

                    sqlTrans.Commit();
                }
            }


        }

        private void UcitajSliku_Click(object sender, EventArgs e)
        {
            file.Filter = "Images (*.jpg)|*.jpg";
            //file.Multiselect = true;
            if (file.ShowDialog() == DialogResult.OK)
            {
                string tip;
                imenovanje.ShowDialog();
                tip = imenovanje.Get();
                if (tip == null) return;
                dodatiFajlovi.Add(tip, file.FileName);
                SviFajlovi.Items.Add(tip);
                sveSlike.Add(file.FileName, Image.FromFile(file.FileName));
                //selectedImage = true;
                //Image image1 = Image.FromFile(file.FileName, true);
                PrikaziSliku(sveSlike.Count - 1);
            }
        }

        private void Sledeca_Click(object sender, EventArgs e)
        {
            SledSlika();
        }

        private void Prethodna_Click(object sender, EventArgs e)
        {
            ProslaSlika();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            foreach (var item in dodatiFajlovi)
            {
                var idFajla = 0;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlTransaction sqlTrans = sqlConnection.BeginTransaction())
                    {
                        string sql = "INSERT INTO Arhivad.dbo.Fajlovi(Ime,Vrsta) output INSERTED.ID VALUES('',@itemKey)";
                        string sql1 = "update Arhivad.dbo.Fajlovi set ime = @ime where id=@idFajla";
                        string sql2 = "INSERT INTO Arhivad.dbo.VezeUc (SifraUc,IDFajl) VALUES (@SifraUc,@idFajla)";
                        using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection, sqlTrans))
                        {
                            sqlCommand.Parameters.Add("@itemKey", SqlDbType.VarChar).Value = item.Key;
                            Object o = sqlCommand.ExecuteScalar();
                            if (o != null)
                            {
                                idFajla = Int32.Parse(o.ToString());
                            }
                            else
                            {
                                MessageBox.Show("Problem kreiranja fajla " + item.Key);
                                sqlTrans.Rollback();
                                continue;
                            }
                        }
                        using (SqlCommand sqlCommand = new SqlCommand(sql1, sqlConnection, sqlTrans))
                        {
                            sqlCommand.Parameters.Add("@ime", SqlDbType.VarChar).Value = ime + "_" + prezime + "_" + generacija + "_" + idFajla + "_" + item.Key;
                            sqlCommand.Parameters.Add("@idFajla", SqlDbType.VarChar).Value = idFajla;
                            sqlCommand.ExecuteNonQuery();
                        }
                        using (SqlCommand sqlCommand = new SqlCommand(sql2, sqlConnection, sqlTrans))
                        {
                            sqlCommand.Parameters.Add("@SifraUc", SqlDbType.VarChar).Value = sifra;
                            sqlCommand.Parameters.Add("@idFajla", SqlDbType.VarChar).Value = idFajla;
                            sqlCommand.ExecuteNonQuery();
                        }
                        try
                        {
                            File.Copy(item.Value, lokacija + ime + "_" + prezime + "_" + generacija + "_" + idFajla + "_" + item.Key + ".jpg");
                        }
                        catch (Exception)
                        {
                            sqlTrans.Rollback();
                            MessageBox.Show("Problem kopiranja fajla " + item.Value);
                            continue;
                        }
                        sqlTrans.Commit();
                    }
                }
            }


            this.Close();
        }
    }
}





