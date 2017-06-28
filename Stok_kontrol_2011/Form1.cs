using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using System.Threading;
using System.Xml;

namespace Stok_kontrol_2011
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection cnn = new SqlConnection("server=192.168.51.6; database=ob_gk2v3;UID=logo;PWD=logo; trusted_connection=false");
        SqlConnection cnn1 = new SqlConnection("server=192.168.51.6; database=ob_gk2v3;UID=logo;PWD=logo; trusted_connection=false");

        double kur;
        string EURO;

        private void Form1_Load(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("SELECT CODE FROM LG_017_ITEMS WHERE ACTIVE=0 AND CARDTYPE=1 AND CODE NOT LIKE 'SMA-REP%'", cnn);

            SqlCommand cmd1 = new SqlCommand("SELECT RATES4 FROM L_DAILYEXCHANGES WHERE EDATE=(SELECT CONVERT(VARCHAR(10), GETDATE(), 120) AS [YYYY-MM-DD]) and CRTYPE=20", cnn1);

            cnn.Open();
            cnn1.Open();
            cmd.CommandTimeout = 60;

            SqlDataReader rd = cmd.ExecuteReader();
            kur = Convert.ToDouble(cmd1.ExecuteScalar());



            label2.Text = kur.ToString();


            while (rd.Read())
            {

                comboBox1.Items.Add(rd[0].ToString());

            }



            cnn.Close();
            cnn1.Close();
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;



        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            ListviewColumnNamesDefault();
            listView1.Items.Clear();

            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Lütfen listeden malzeme seçiniz");
            }
            else
            {

                SqlCommand cmd = new SqlCommand(@"SELECT I.CODE, I.NAME, SUM(ONHAND), SUM(ONHAND - RESERVED), W.NAME, PRICE, 'PERAKENDE'
FROM LV_017_01_STINVTOT S JOIN LG_017_ITEMS I ON S.STOCKREF = I.LOGICALREF JOIN L_CAPIWHOUSE W ON S.INVENNO = W.NR JOIN LG_017_PRCLIST P ON P.CARDREF = I.LOGICALREF
WHERE I.CODE=@kod  AND P.PTYPE = 2 AND P.CLIENTCODE = '*' AND P.PRIORITY = '0' GROUP BY I.CODE, I.NAME, W.NAME, P.PRICE, P.DEFINITION_, W.FIRMNR, INVENNO
HAVING SUM(ONHAND) <> 0 AND W.FIRMNR = 017 AND INVENNO <> -1
UNION SELECT I.CODE, I.NAME, SUM(ONHAND), SUM(ONHAND - RESERVED), W.NAME, PRICE, 'TOPTAN'
FROM LV_017_01_STINVTOT S JOIN LG_017_ITEMS I ON S.STOCKREF = I.LOGICALREF JOIN L_CAPIWHOUSE W ON S.INVENNO = W.NR JOIN LG_017_PRCLIST P ON P.CARDREF = I.LOGICALREF
WHERE I.CODE=@kod  AND P.PTYPE = 2 AND P.CLIENTCODE = '*' AND P.PRIORITY = '99' GROUP BY I.CODE, I.NAME, W.NAME, P.PRICE, P.DEFINITION_, W.FIRMNR, INVENNO
HAVING SUM(ONHAND) <> 0 AND W.FIRMNR = 017 AND INVENNO <> -1  ", cnn);
                SqlCommand cmd1 = new SqlCommand("SELECT RATES4 FROM L_DAILYEXCHANGES WHERE EDATE=(SELECT CONVERT(VARCHAR(10), GETDATE(), 120) AS [YYYY-MM-DD]) and CRTYPE=20", cnn1);

                cnn.Open();
                cmd.Parameters.AddWithValue("@kod", comboBox1.SelectedItem.ToString());
                cmd.CommandTimeout = 60;

                SqlDataReader rd = cmd.ExecuteReader();
                cnn1.Open();


                kur = Convert.ToDouble(cmd1.ExecuteScalar());

                double tlFiyat = 0;
                double tlFiyat1 = 0;

                {
                    if (!rd.HasRows)
                    {
                        MessageBox.Show("ürün bulunamadı");
                        cnn.Close();
                        cnn1.Close();


                        SqlCommand sql1 = new SqlCommand(@"SELECT I.CODE, I.NAME, PRICE, 'PERAKENDE'
FROM LG_017_ITEMS I JOIN LG_017_PRCLIST P ON P.CARDREF = I.LOGICALREF
WHERE I.CODE=@kod  AND P.PTYPE = 2 AND P.CLIENTCODE = '*' AND P.PRIORITY = '0' 
UNION SELECT I.CODE, I.NAME,  PRICE, 'TOPTAN'
FROM LG_017_ITEMS I JOIN LG_017_PRCLIST P ON P.CARDREF = I.LOGICALREF
WHERE I.CODE=@kod  AND P.PTYPE = 2 AND P.CLIENTCODE = '*' AND P.PRIORITY = '99'", cnn);
                        SqlCommand cmd2 = new SqlCommand("SELECT RATES4 FROM L_DAILYEXCHANGES WHERE EDATE=(SELECT CONVERT(VARCHAR(10), GETDATE(), 120) AS [YYYY-MM-DD]) and CRTYPE=20", cnn1);
                        cnn.Open();
                        sql1.Parameters.AddWithValue("@kod", comboBox1.SelectedItem.ToString());
                        sql1.CommandTimeout = 60;
                        SqlDataReader rdr = sql1.ExecuteReader();
                        cnn1.Open();
                        kur = Convert.ToDouble(cmd2.ExecuteScalar());

                        if (!rdr.HasRows)
                        {
                            MessageBox.Show("Ürün fiyatı sistemde bulunamadı");
                            cnn.Close();
                            cnn1.Close();
                            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                        }
                        else
                        {


                            while (rdr.Read())
                            {

                                if (rdr[2] == null)
                                {
                                    tlFiyat = 0;
                                }
                                else

                                {
                                    tlFiyat = Convert.ToDouble(rdr[2]) * Convert.ToDouble(kur);
                                }

                                Listviewcolumnnames();


                                ListViewItem itm = new ListViewItem();
                                itm.Text = rdr[0].ToString();
                                itm.SubItems.Add(rdr[1].ToString());
                                itm.SubItems.Add("0");
                                itm.SubItems.Add("0");
                                itm.SubItems.Add("-");
                                itm.SubItems.Add(rdr[2].ToString());
                                itm.SubItems.Add(tlFiyat.ToString());
                                itm.SubItems.Add(rdr[3].ToString());
                                
                                itm.SubItems.Add(tlFiyat1.ToString());
                                listView1.Items.Add(itm);

                            }
                            cnn.Close();
                            cnn1.Close();
                            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                        }
                    }

                    else
                    {
                        while (rd.Read())
                        {
                            if (rd[5] == null)
                            {
                                tlFiyat = 0;
                            }
                            else

                            {
                                tlFiyat = Convert.ToDouble(rd[5]) * Convert.ToDouble(kur);
                            }



                            ListViewItem itm = new ListViewItem();
                            itm.Text = rd[0].ToString();
                            itm.SubItems.Add(rd[1].ToString());
                            itm.SubItems.Add(rd[2].ToString());
                            itm.SubItems.Add(rd[3].ToString());
                            itm.SubItems.Add(rd[4].ToString());
                            itm.SubItems.Add(rd[5].ToString());
                            itm.SubItems.Add(tlFiyat.ToString());
                            itm.SubItems.Add(rd[6].ToString());
                            itm.SubItems.Add(tlFiyat1.ToString());
                            listView1.Items.Add(itm);

                        }

                        cnn.Close();
                        cnn1.Close();
                        listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                    }
                }

            }
        }
  
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {


        }

        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ListviewColumnNamesDefault();

            listView1.Items.Clear();

            if (textBox1.Text == "")
            {
                MessageBox.Show("Lütfen malzeme ismi giriniz");
            }
            else
            {

                SqlCommand cmd = new SqlCommand(@"SELECT I.CODE,I.NAME,SUM (ONHAND),SUM(ONHAND-RESERVED),W.NAME,PRICE,'PERAKENDE' 
FROM LV_017_01_STINVTOT S JOIN LG_017_ITEMS I ON S.STOCKREF=I.LOGICALREF JOIN L_CAPIWHOUSE W ON S.INVENNO=W.NR JOIN LG_017_PRCLIST P ON P.CARDREF=I.LOGICALREF 
WHERE I.NAME LIKE '%' + @isim + '%'  AND P.PTYPE=2 AND P.CLIENTCODE='*' AND P.PRIORITY='0' GROUP BY I.CODE,I.NAME,W.NAME,P.PRICE,P.DEFINITION_,W.FIRMNR,INVENNO  
HAVING SUM(ONHAND)<>0 AND W.FIRMNR=017 AND INVENNO<>-1 
UNION SELECT I.CODE,I.NAME,SUM (ONHAND),SUM(ONHAND-RESERVED),W.NAME,PRICE,'TOPTAN' 
FROM LV_017_01_STINVTOT S JOIN LG_017_ITEMS I ON S.STOCKREF=I.LOGICALREF JOIN L_CAPIWHOUSE W ON S.INVENNO=W.NR JOIN LG_017_PRCLIST P ON P.CARDREF=I.LOGICALREF 
WHERE I.NAME LIKE '%' + @isim + '%'  AND P.PTYPE=2 AND P.CLIENTCODE='*' AND P.PRIORITY='99' GROUP BY I.CODE,I.NAME,W.NAME,P.PRICE,P.DEFINITION_,W.FIRMNR,INVENNO  
HAVING SUM(ONHAND)<>0 AND W.FIRMNR=017 AND INVENNO<>-1   ", cnn);
                SqlCommand cmd1 = new SqlCommand("SELECT RATES4 FROM L_DAILYEXCHANGES WHERE EDATE=(SELECT CONVERT(VARCHAR(10), GETDATE(), 120) AS [YYYY-MM-DD]) and CRTYPE=20", cnn1);

                cnn.Open();

                cmd.Parameters.AddWithValue("@isim", textBox1.Text);

                cmd.CommandTimeout = 60;
                cmd1.CommandTimeout = 60;
                SqlDataReader rd = cmd.ExecuteReader();

                cnn1.Open();

                double kur = Convert.ToDouble(cmd1.ExecuteScalar());
                double tlFiyat = 0;
                double tlFiyat1 = 0;

                if (!rd.HasRows)
                {
                    MessageBox.Show("ürün bulunamadı");
                    cnn.Close();
                    cnn1.Close();


                    SqlCommand sql1 = new SqlCommand(@"SELECT I.CODE, I.NAME, PRICE, 'PERAKENDE'
FROM LG_017_ITEMS I JOIN LG_017_PRCLIST P ON P.CARDREF = I.LOGICALREF
WHERE I.NAME LIKE '%' + @isim + '%'  AND P.PTYPE = 2 AND P.CLIENTCODE = '*' AND P.PRIORITY = '0' 
UNION SELECT I.CODE, I.NAME,  PRICE, 'TOPTAN'
FROM LG_017_ITEMS I JOIN LG_017_PRCLIST P ON P.CARDREF = I.LOGICALREF
WHERE I.NAME LIKE '%' + @isim + '%' AND P.PTYPE = 2 AND P.CLIENTCODE = '*' AND P.PRIORITY = '99'", cnn);
                    SqlCommand cmd2 = new SqlCommand("SELECT RATES4 FROM L_DAILYEXCHANGES WHERE EDATE=(SELECT CONVERT(VARCHAR(10), GETDATE(), 120) AS [YYYY-MM-DD]) and CRTYPE=20", cnn1);
                    cnn.Open();
                    sql1.Parameters.AddWithValue("@isim", textBox1.Text);
                    sql1.CommandTimeout = 60;
                    SqlDataReader rdr = sql1.ExecuteReader();
                    cnn1.Open();
                    kur = Convert.ToDouble(cmd2.ExecuteScalar());

                    while (rdr.Read())
                    {

                        if (rdr[2] == null)
                        {
                            tlFiyat = 0;
                        }
                        else

                        {
                            tlFiyat = Convert.ToDouble(rdr[2]) * Convert.ToDouble(kur);
                        }

                        Listviewcolumnnames();


                        ListViewItem itm = new ListViewItem();
                        itm.Text = rdr[0].ToString();
                        itm.SubItems.Add(rdr[1].ToString());
                        itm.SubItems.Add("0");
                        itm.SubItems.Add("0");
                        itm.SubItems.Add("-");
                        itm.SubItems.Add(rdr[2].ToString());
                        itm.SubItems.Add(tlFiyat.ToString());
                        itm.SubItems.Add(rdr[3].ToString());
                        
                        itm.SubItems.Add(tlFiyat1.ToString());
                        listView1.Items.Add(itm);

                    }
                    cnn.Close();
                    cnn1.Close();
                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                }
                else
                {



                    {
                        while (rd.Read())
                        {
                            if (rd[5] == null)
                            {
                                tlFiyat = 0;
                            }
                            else

                            {
                                tlFiyat = Convert.ToDouble(rd[5]) * Convert.ToDouble(kur);
                            }


                            ListViewItem itm = new ListViewItem();
                            itm.Text = rd[0].ToString();
                            itm.SubItems.Add(rd[1].ToString());
                            itm.SubItems.Add(rd[2].ToString());
                            itm.SubItems.Add(rd[3].ToString());
                            itm.SubItems.Add(rd[4].ToString());
                            itm.SubItems.Add(rd[5].ToString());
                            itm.SubItems.Add(tlFiyat.ToString());
                            itm.SubItems.Add(rd[6].ToString());
                            itm.SubItems.Add(tlFiyat1.ToString());
                            listView1.Items.Add(itm);
                        }
                    }
                    cnn.Close();
                    cnn1.Close();
                    //cnn2.Close();
                    listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Listviewcolumnnames()
        {
            listView1.Columns[0].Name = "Malzeme Kodu";
            listView1.Columns[1].Name = "Açıklaması";
            listView1.Columns[2].Name = "Fiyat";
            listView1.Columns[3].Name = "Fiyat Açıklaması";


        }


        private void ListviewColumnNamesDefault()
        {
            listView1.Columns[0].Name = "Malzeme Kodu";
            listView1.Columns[1].Name = "Açıklaması";
            listView1.Columns[2].Name = "Stok";
            listView1.Columns[3].Name = "Sevkedilebilir";
            listView1.Columns[4].Name = "Ambar";
            listView1.Columns[5].Name = "Fiyat Euro";
            listView1.Columns[6].Name = "Fiyat TL";
            listView1.Columns[7].Name = "Fiyat Açıklaması";
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);


        }

    }
}
