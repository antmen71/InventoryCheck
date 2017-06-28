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


        private void Form1_Load(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("SELECT CODE FROM LG_013_ITEMS WHERE ACTIVE=0", cnn);
            cnn.Open();

            cmd.CommandTimeout = 60;

            SqlDataReader rd = cmd.ExecuteReader();
            
            //set the SelectCommand of the adapter

            SqlCommand cmd1 = new SqlCommand("SELECT RATES4 FROM dbo.LG_EXCHANGE_013 WHERE EDATE=(SELECT CONVERT(VARCHAR(10), GETDATE(), 120) AS [YYYY-MM-DD]) and CRTYPE=20", cnn1);

            cnn1.Open();
            double kur = Convert.ToDouble(cmd1.ExecuteScalar());
            label2.Text = kur.ToString();
            cnn1.Close();

            while (rd.Read())
            {
                //comboBox1.AutoCompleteCustomSource.Add(rd[0].ToString());
                comboBox1.Items.Add(rd[0].ToString());

            }



            cnn.Close();
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;



        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {

            listView1.Items.Clear();

            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Lütfen listeden malzeme seçiniz");
            }
            else
            {
                //SELECT I.CODE,I.NAME,SUM (ONHAND), W.NAME,P.PRICE,P.DEFINITION_, FROM LG_013_01_STINVTOT S JOIN LG_013_ITEMS I ON S.STOCKREF=I.LOGICALREF JOIN L_CAPIWHOUSE W ON S.INVENNO=W.NR JOIN LG_013_PRCLIST P ON P.CARDREF=I.LOGICALREF WHERE I.CODE=@kod AND P.PTYPE=2 AND P.CLIENTCODE='*' AND P.PRIORITY='0' AND W.FIRMNR=11 GROUP BY I.CODE,I.NAME,S.INVENNO,P.PRICE,P.DEFINITION_,W.FIRMNR,W.NAME  HAVING SUM(ONHAND)<>0  ORDER BY I.CODE
                SqlCommand cmd = new SqlCommand("SELECT I.CODE,I.NAME,SUM (ONHAND),SUM(ONHAND-RESERVED), W.NAME,P.PRICE,(SELECT PRICE FROM LG_013_PRCLIST PR JOIN LG_013_ITEMS I ON PR.CARDREF=I.LOGICALREF WHERE I.CODE=@kod AND PR.CLIENTCODE='*' AND PR.PRIORITY='99'),P.DEFINITION_ FROM LG_013_01_STINVTOT S JOIN LG_013_ITEMS I ON S.STOCKREF=I.LOGICALREF JOIN L_CAPIWHOUSE W ON S.INVENNO=W.NR JOIN LG_013_PRCLIST P ON P.CARDREF=I.LOGICALREF WHERE I.CODE=@kod AND P.PTYPE=2 AND P.CLIENTCODE='*' AND P.PRIORITY='0' AND W.FIRMNR=13 GROUP BY I.CODE,I.NAME,S.INVENNO,P.PRICE,P.DEFINITION_,W.FIRMNR,W.NAME  HAVING SUM(ONHAND)<>0  ORDER BY I.CODE", cnn);

                SqlCommand cmd1 = new SqlCommand("SELECT RATES4 FROM dbo.LG_EXCHANGE_013 WHERE EDATE=(SELECT CONVERT(VARCHAR(10), GETDATE(), 120) AS [YYYY-MM-DD]) and CRTYPE=20", cnn1);
                cnn.Open();
                cmd.Parameters.AddWithValue("@kod", comboBox1.SelectedItem.ToString());



                cmd.CommandTimeout = 60;
                cmd1.CommandTimeout = 60;
                SqlDataReader rd = cmd.ExecuteReader();
                cnn1.Open();
                
                 
                
                double kur = Convert.ToDouble(cmd1.ExecuteScalar());
                 while (rd.Read())
                    {
                        double tlFiyat = Convert.ToDouble(rd[5]) * kur;
                        double tlFiyat1 = Convert.ToDouble(rd[6]) * kur;

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
                        itm.SubItems.Add(rd[7].ToString());

                        listView1.Items.Add(itm);

                    }
                //}
                //else
                //{
                //    MessageBox.Show("Stok bulunamadı");
                    
                //}
                cnn.Close();
                cnn1.Close();
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }
        //}
        void doldur()
        {






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
            listView1.Items.Clear();

            if (textBox1.Text == "")
            {
                MessageBox.Show("Lütfen malzeme ismi giriniz");
            }
            else
            {

                SqlCommand cmd = new SqlCommand("SELECT I.CODE,I.NAME,SUM (ONHAND),SUM(ONHAND-RESERVED),W.NAME,P.PRICE,P.DEFINITION_ FROM LG_013_01_STINVTOT S JOIN LG_013_ITEMS I ON S.STOCKREF=I.LOGICALREF JOIN L_CAPIWHOUSE W ON S.INVENNO=W.NR JOIN LG_013_PRCLIST P ON P.CARDREF=I.LOGICALREF WHERE I.NAME LIKE '%' + @isim + '%'  AND P.PTYPE=2 AND P.CLIENTCODE='*' AND P.PRIORITY='0' GROUP BY I.CODE,I.NAME,W.NAME,P.PRICE,P.DEFINITION_,W.FIRMNR  HAVING SUM(ONHAND)<>0 AND W.FIRMNR=13 ORDER BY I.CODE", cnn);
                SqlCommand cmd1 = new SqlCommand("SELECT RATES4 FROM LG_EXCHANGE_013 WHERE EDATE=(SELECT CONVERT(VARCHAR(10), GETDATE(), 130) AS [YYYY-MM-DD]) and CRTYPE=20", cnn1);
                cnn.Open();

                cmd.Parameters.AddWithValue("@isim", textBox1.Text);
                cmd.CommandTimeout = 60;
                cmd1.CommandTimeout = 60;
                SqlDataReader rd = cmd.ExecuteReader();
                cnn1.Open();



                double kur = Convert.ToDouble(cmd1.ExecuteScalar());
                

                //if (!rd.Read())
                //{
                //    MessageBox.Show("Stok bulunamadı");
                //}
                //else
                {
                    while (rd.Read())
                    {
                        double tlFiyat = Convert.ToDouble(rd[5]) * kur;
                        //double tlFiyat1 = Convert.ToDouble(rd[5]) * kur;

                        ListViewItem itm = new ListViewItem();
                        itm.Text = rd[0].ToString();
                        itm.SubItems.Add(rd[1].ToString());
                        itm.SubItems.Add(rd[2].ToString());
                        itm.SubItems.Add(rd[3].ToString());
                        itm.SubItems.Add(rd[4].ToString());

                        itm.SubItems.Add(rd[5].ToString());
                        itm.SubItems.Add(tlFiyat.ToString());
                        //itm.SubItems.Add(rd[4].ToString());
                        //itm.SubItems.Add(rd[5].ToString());

                        //itm.SubItems.Add(tlFiyat1.ToString());

                        listView1.Items.Add(itm);
                    }
                }
                cnn.Close();
                cnn1.Close();
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}
