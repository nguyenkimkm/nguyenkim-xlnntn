using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace XLNNTN_NC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[,] pcky;
        List<string> vanpham = new List<string>();
        List<string> cnf = new List<string>();
        List<string> cnf32 = new List<string>();
        List<string> output = new List<string>();
        string dulieu;
        string[] m;
        string stringleft = "";
        string stringdown = "";
        Dictionary<string, double> tsn = new Dictionary<string, double>();
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream fileStream = openFileDialog1.OpenFile();
                StreamReader rd = new StreamReader(fileStream);
                dulieu = rd.ReadToEnd();
                layvanpham(dulieu);
                LoadListView();
                fileStream.Close();
            }

        }

        private void LoadListView()
        {
            Dictionary<string, double> ts = new Dictionary<string, double>();
            tsn.Clear();
            listView1.Items.Clear();
            ListViewItem dong;

            for (int l = 0; l < cnf.Count; l++)
            {
                if (ts.ContainsKey(cnf[l]))
                {
                    ts[cnf[l]] = ts[cnf[l]] + 1;
                }
                else
                {
                    ts[cnf[l]] = 1;
                }
            }


            for (int i = 0; i < vanpham.Count; i++)
            {
                if (ts.ContainsKey(vanpham[i]))
                {
                    ts[vanpham[i]] = ts[vanpham[i]] + 1;
                }
                else
                {
                    ts[vanpham[i]] = 1;
                }
            }
            //tinh tan so tren nhom
            foreach (var vp in ts)
            {
                string d = vp.Key.Substring(1, vp.Key.IndexOf('-') - 2);
                if(tsn.ContainsKey(d))
                {
                    tsn[d] = tsn[d] + vp.Value;
                }
                else
                {
                    tsn[d] = vp.Value;
                }
            }
            foreach (var vp in ts)
            {
                dong = new ListViewItem();
                dong.Text = vp.Key;
                string dts = vp.Key.Substring(1, vp.Key.IndexOf('-') - 2);
                string f = string.Format("{0:0.00000}", vp.Value / tsn[dts]);
                dong.SubItems.Add(f);
                listView1.Items.Add(dong);
                       
            }
        }

        private void layvanpham(string dulieu)
        {
            string[] mang = dulieu.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < mang.Length; i++)
            {
                for (int k = 0; k < mang[i].Length; k++)
                {
                    if ((mang[i][k] == ')') && (k == mang[i].Length - 1))
                    {
                        string[] hoanthanh = mang[i].Substring(1, mang[i].Length - 2).Split(' ');
                        string vpht = string.Format(" {0} -->", hoanthanh[0]);
                        for (int l = 1; l < hoanthanh.Length; l++)
                        {
                            vpht = vpht + " " + hoanthanh[l];
                        }
                        vanpham.Add(vpht);
                        break;
                    }
                    if (mang[i][k] == ')')
                    {
                        for (int j = k; j >= 0; j--)
                        {
                            if (mang[i][j] == '(')
                            {
                                string sub = mang[i].Substring(j + 1, k - j - 1);
                                string[] mangsub = sub.Split(' ');
                                string vp = string.Format(" {0} -->", mangsub[0]);
                                for (int m = 1; m < mangsub.Length; m++)
                                {
                                    vp = vp + " " + mangsub[m];
                                }
                                vanpham.Add(vp);
                                mang[i] = mang[i].Replace(mang[i].Substring(j, k - j + 1), " " + mangsub[0]);
                                k = k - (sub.Length + 2 - mangsub[0].Length - 1);
                                break;
                            }
                        }
                    }
                }

            }
        }

        private void btnCNF_Click(object sender, EventArgs e)
        {

            int n = cnf32.Count + 1;
            for (int i = 0; i < vanpham.Count; i++)
            {

                string[] m = vanpham[i].Substring(vanpham[i].IndexOf('>') + 2).Split(' ');
                string nw = vanpham[i].Substring(1, vanpham[i].IndexOf('-') - 2);
                if (m.Length > 2)
                {
                    int vt = kiemtra(cnf32, m[0] + " " + m[1]);
                    if (vt >= 0)
                    {
                        vanpham[i] = vanpham[i].Replace(m[0] + " " + m[1], cnf32[vt].Substring(1, cnf32[vt].IndexOf('-') - 2));
                        vanpham.Add(cnf32[vt]);
                    }
                    else
                    {
                        string s = string.Format(" {0} --> {1} {2}", "X" + n.ToString(), m[0], m[1]);
                        vanpham[i] = vanpham[i].Replace(m[0] + " " + m[1], "X" + n.ToString());
                        n = n + 1;
                        vanpham.Add(s);
                        cnf32.Add(s);
                    }

                }
                if ((m.Length == 1) && ((nw == "NP") || (nw == "PP") || (nw == "VP")))
                {

                    vanpham[i - 1] = vanpham[i - 1].Replace(m[0], nw);
                    vanpham.RemoveAt(i);
                    i = i - 1;
                }
            }
            for (int i = 0; i < vanpham.Count; i++)
            {
                cnf.Add(vanpham[i]);
            }
            vanpham.Clear();
            LoadListView();
        }

        private int kiemtra(List<string> l, string vp)
        {
            int vitri = -1;
            for (int i = 0; i < l.Count; i++)
            {
                string c = l[i].Substring(l[i].IndexOf('>') + 2);
                if (string.Compare(c, vp) == 0)
                {
                    vitri = i;
                    return vitri;
                }
            }
            return vitri;
        }

        private void PCKY()
        {
            Dictionary<string, double> ts = new Dictionary<string, double>();
            m = textBox1.Text.Trim().Split(' ');
            int n = m.Length;
            pcky = new string[n, n];
            for (int l = 0; l < cnf.Count; l++)
            {
                if (ts.ContainsKey(cnf[l]))
                {
                    ts[cnf[l]] = ts[cnf[l]] + 1;
                }
                else
                {
                    ts[cnf[l]] = 1;
                }
            }
            for (int i = 0; i < n; i++)
            {
                foreach (var vp in ts)
                {
                    if (string.Compare(m[i], vp.Key.Substring(vp.Key.IndexOf('>') + 2)) == 0)
                    {
                        string dau = vp.Key.Substring(1, vp.Key.IndexOf('-') - 2);
                        double sx = vp.Value / tsn[dau];
                        if (string.IsNullOrEmpty(pcky[i,i]) == true)
                        {
                            string s3 = string.Format("{0}\r\n{1}\r\n{2:0.0000000}\r\n", i, dau, sx);
                            pcky[i, i] = pcky[i, i] + s3;
                        }
                        else
                        {
                            string s2 = string.Format("{0}\r\n{1:0.0000000}\r\n", dau, sx);
                            pcky[i, i] = pcky[i, i] + s2;
                        }
                        
                    }
                }

            }
            // PCKY
            for (int i = 2; i <= n; i++)
            {
                for (int j = 1; j <= n - i + 1; j++)
                {
                    for (int k = 1; k <= i - 1; k++)
                    {
                        if (string.IsNullOrEmpty(pcky[j - 1, j - 1 + k - 1]) == false && string.IsNullOrEmpty(pcky[j - 1 + k, j - 1 + k + i - k - 1]) == false)
                        {
                            string[] m1 = pcky[j - 1, j - 1 + k - 1].Substring(pcky[j - 1, j - 1 + k - 1].IndexOf('\n') + 1).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            string[] m2 = pcky[j - 1 + k, j - 1 + k + i - k - 1].Substring(pcky[j - 1 + k, j - 1 + k + i - k - 1].IndexOf('\n') + 1).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var vp in ts)
                            {
                                string sau = vp.Key.Substring(vp.Key.IndexOf('>') + 2);
                                string[] ms = sau.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if ( ms.Length > 1)
                                {
                                    for (int l = 0; l < m1.Length; l += 2)
                                    {
                                        for (int t = 0; t < m2.Length; t += 2)
                                        {
                                            if (string.Compare(m1[l] + " " + m2[t], sau) == 0)
                                            {
                                                string dau = vp.Key.Substring(1, vp.Key.IndexOf('-') - 2);
                                                double p = double.Parse(m1[l + 1]) * double.Parse(m2[t + 1]) * vp.Value / tsn[dau];
                                                string td = (j - 1).ToString() + (j - 1 + k - 1).ToString() + (j - 1 + k).ToString() + (j - 1 + k + i - k - 1).ToString();
                                                if (string.IsNullOrEmpty(pcky[j - 1, j - 1 + k + i - k - 1]) == true)
                                                {
                                                    string s3 = string.Format("{0}\r\n{1}\r\n{2:0.0000000}\r\n",td, dau, p);
                                                    pcky[j - 1, j - 1 + k + i - k - 1] = pcky[j - 1, j - 1 + k + i - k - 1] + s3;
                                                }
                                                else
                                                {
                                                    string s2 = string.Format("{0}\r\n{1:0.0000000}\r\n", dau, p);
                                                    pcky[j - 1, j - 1 + k + i - k - 1] = pcky[j - 1, j - 1 + k + i - k - 1] + s2;
                                                }
                                            }
                                        }   
                                    }
                                }
                            }
                        }
                    }
                }


            }
            //load gridview
            dataGridView1.ColumnCount = n;
            for(int i = 0; i < n; i ++)
            {
                string colum = string.Format("{0}\r\n{1}", i + 1, m[i]);
                dataGridView1.Columns[i].Name = colum;
            }
            for(int i = 0; i < n; i ++ )
            {
                string[] temp = new string[n];
                for(int j = 0; j < n; j ++)
                {
                    if (string.IsNullOrEmpty(pcky[i, j]) == false)
                    {
                        string toadocon = pcky[i, j].Substring(0, pcky[i, j].IndexOf('\r'));
                        string mvp = pcky[i, j].Substring(pcky[i, j].IndexOf('\n') + 1);
                        if (toadocon.Length > 1)
                        {
                            int a = int.Parse(toadocon[0].ToString());
                            int b = int.Parse(toadocon[1].ToString());
                            int c = int.Parse(toadocon[2].ToString());
                            int d = int.Parse(toadocon[3].ToString());
                            string td = string.Format("({0},{1})+({2},{3})\r\n", a, b + 1, c, d + 1);
                            temp[j] = td + mvp;
                        }
                        else
                        {
                            int a = int.Parse(toadocon);
                            string td = string.Format("({0},{1})\r\n", a, a + 1);
                            temp[j] = td + mvp;
                        }
                        
                    }
                }
                dataGridView1.Rows.Add(temp);
            }
            


        }

        private void btnCal_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            PCKY();
            
        }
        private void dequy(string toado)
        {
            
            string toadocon = toado.Substring(0, toado.IndexOf('\r'));
            if (toadocon.Length > 1)
            {
                int a = int.Parse(toadocon[0].ToString());
                int b = int.Parse(toadocon[1].ToString());
                int c = int.Parse(toadocon[2].ToString());
                int d = int.Parse(toadocon[3].ToString());
                string[] md = toado.Substring(toado.IndexOf('\n') + 1).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] ms1 = pcky[a, b].Substring(pcky[a, b].IndexOf('\n') + 1).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] ms2 = pcky[c, d].Substring(pcky[c, d].IndexOf('\n') + 1).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if(md.Length < 3)
                {
                    for(int i = 0; i < ms1.Length; i +=2)
                    {
                        for(int k = 0;  k < ms2.Length; k +=2)
                        {
                            string vp = string.Format(" {0} --> {1} {2}", md[0], ms1[i], ms2[k]);
                            if (cnf.Contains(vp) == true)
                            {
                                output.Add(vp);
                            }
                        }
                    }
                    if (ms1.Length > 2)
                    {
                        stringleft = output[output.Count - 1].Substring(output[output.Count - 1].IndexOf('-'));
                    }
                    if(ms2.Length > 2)
                    {
                        stringdown = output[output.Count - 1].Substring(output[output.Count - 1].IndexOf('-'));
                    }
                    dequy(pcky[a, b]);
                    dequy(pcky[c, d]);
                }
                else
                {
                    int vt = 0;
                    
                    for(int l = 0; l < md.Length; l +=2)
                    {
                        if ((stringleft.Contains(md[l]) == true) || (stringdown.Contains(md[l]) == true))
                        {
                            vt = l;
                            break;
                        }
                    }
                    for (int i = 0; i < ms1.Length; i += 2)
                    {
                        for (int k = 0; k < ms2.Length; k += 2)
                        {
                            string vp = string.Format(" {0} --> {1} {2}", md[vt], ms1[i], ms2[k]);
                            if (cnf.Contains(vp) == true)
                            {
                                output.Add(vp);
                            }
                        }
                    }
                    if (ms1.Length > 2)
                    {
                        stringleft = output[output.Count - 1].Substring(output[output.Count - 1].IndexOf('-'));
                    }
                    if (ms2.Length > 2)
                    {
                        stringdown = output[output.Count - 1].Substring(output[output.Count - 1].IndexOf('-'));
                    }
                    dequy(pcky[a, b]);
                    dequy(pcky[c, d]);

                }
            }
            else
            {
                int a = int.Parse(toadocon);
                string[] md = toado.Substring(toado.IndexOf('\n') + 1).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string vp = string.Format(" {0} --> {1}", md[0], m[a]);
                output.Add(vp);
            }
            
        }

        private void btnExtr_Click(object sender, EventArgs e)
        {
            output.Clear();
            listView2.Items.Clear();
            if (string.IsNullOrEmpty(pcky[0, m.Length - 1]) == false)
            {
                string[] mangS = pcky[0, m.Length - 1].Substring(pcky[0, m.Length - 1].IndexOf('\n') + 1).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (mangS.Length < 3)
                {
                    dequy(pcky[0, m.Length - 1]);
                    for (int i = 0; i < output.Count; i++)
                    {
                        listView2.Items.Add(output[i]);
                    }
                }
            }
            
        }

        
    }
}