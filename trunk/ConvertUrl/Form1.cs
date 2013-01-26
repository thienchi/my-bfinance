using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using dangdongcmm.utilities;

namespace ConvertUrl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bfinanceEntities ent = new bfinanceEntities();
        private void button1_Click(object sender, EventArgs e)
        {
            var list = ent.vndd_news.ToList();
            foreach (var vnddNewse in list)
            {
                vnddNewse.alias = CFunctions.install_urlname(vnddNewse.name).Replace(".aspx", "");
                
            }
            
            ent.SaveChanges();
            MessageBox.Show("done");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var ent2 = new DataClasses1DataContext();
            var list = ent2.vndd_category2s.ToList();
            foreach (var vnddNewse in list)
            {
                vnddNewse.alias = CFunctions.install_urlname(vnddNewse.name).Replace(".aspx", "");

            }
            ent2.SubmitChanges();
            MessageBox.Show("done");
            
        }
        
    }
}
