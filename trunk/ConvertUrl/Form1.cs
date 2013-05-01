using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        bfinance_bfinancewebEntities ent = new bfinance_bfinancewebEntities();
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

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(CFunctions.install_urlname("Nhung-nha-quan-ly-quy-hedge-fund-manager-có-thu-nhap-hang-ty-USD.aspx"));
            //MessageBox.Show(Convert_Chuoi_Khong_Dau("Nhung-nha-quan-ly-quy-hedge-fund-manager-có-thu-nhap-hang-ty-USD.aspx"));
        }
        public static string Convert_Chuoi_Khong_Dau(string s)
        {
            s = s.Trim().Replace("  ", " ").Replace(",", "");
            s = s.Replace(" ", "-");
            s = s.Replace(":", "-");
            s = s.Replace("&", "");
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string strFormD = s.Normalize(NormalizationForm.FormD);
            string result = regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            result = result.Replace(":", "").Replace("\"", "");
            result = result.ToLower();
            return result;
        }
    }
}
