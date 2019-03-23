using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CsvReader
{
    public partial class CsvReader : Form
    {
        bool addTemplate = false;
        bool openstatus = false;
        string[] filename;
        string UpperValue,LowerValue;
        string[] TestItem= new string[500];
        string[] TestData = new string[500];
        string[] testHeader = new string[500];
        string[] testDate = new string[500];
        string[] testDateFull = new string[500];
        string[] testStation = new string[500];
        int TestDatalen,TestHeaderlen;
        DataTable datatable = new DataTable();
        string testitemtem,teststatustem,getHeader,getLineStatus;
        double interval = 0.1;
        double avg,delta,max,min,cp,cpl,cpu,cpk;
        string getStation;
        string getSearch = string.Empty;
        string zippathImagetem;
        string savefoldertem;

        public CsvReader()
        {
            InitializeComponent();
        }
        public DataTable ReadCsv(string filename)
        {
            DataTable dt;
            dt = new DataTable("Data");
            using (OleDbConnection cn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + Path.GetDirectoryName(filename) + "\";Extended Properties='text;HKR=yes;FMT=Delimited(,)';"))
            {
                using (OleDbCommand cmd = new OleDbCommand(string.Format("select *from [{0}]", new FileInfo(filename).Name), cn))
                {
                    cn.Open();
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        adapter.Fill(dt);                        
                    }
                }
            }
            return dt;

        }//read Csv Data and save header, data, station, date, item
        private void btnOpen_Click(object sender, EventArgs e)
        {
            //Open File
            if (openstatus == true) { MessageBox.Show("Please close the file first!"); } else {                
                try
                {
                    using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "CSV|*.csv", ValidateNames = true, Multiselect = true })
                    {
                        
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            if(addTemplate == false) {
                                if (ofd.FileNames.Length == 1)
                                {
                                    datatable = ReadCsv(ofd.FileNames[0]);
                                }
                                if (ofd.FileNames.Length != 1)
                                {
                                    datatable = ReadCsv(ofd.FileNames[0]);
                                    for (int i = 1; i < ofd.FileNames.Length; i++)
                                    {
                                        DataTable tem = ReadCsv(ofd.FileNames[i]);
                                        tem.Rows[0].Delete();
                                        tem.Rows[1].Delete();
                                        tem.AcceptChanges();
                                        foreach (DataRow dr in tem.Rows)
                                        {
                                            datatable.ImportRow(dr);
                                        }
                                    }
                                }
                            }
                            if(addTemplate == true)
                            {
                                MessageBox.Show("Add Template");
                                //datatable = ReadCsv(ofd.FileNames[0]);
                                for (int i = 0; i < ofd.FileNames.Length; i++)
                                {
                                    DataTable tem = ReadCsv(ofd.FileNames[i]);
                                    tem.Rows[0].Delete();
                                    tem.Rows[1].Delete();
                                    tem.AcceptChanges();
                                    foreach (DataRow dr in tem.Rows)
                                    {
                                        datatable.ImportRow(dr);
                                    }
                                }
                            }
                        }
                        datatable.AcceptChanges();
                        int failcount = 0;                        
                        for(int i = 2; i < datatable.Rows.Count; i++)
                        {
                            if (datatable.Rows[i][5].ToString()=="FAIL")
                            {
                                datatable.Rows[i].Delete();
                                failcount = failcount + 1;
                                //datatable.Rows.RemoveAt(i);
                            }
                        }
                        datatable.AcceptChanges();
                        //MessageBox.Show(failcount.ToString());
                        int columnlength = datatable.Rows.Count-2;
                        //MessageBox.Show(columnlength.ToString());
                        labelTotal.Text = "Total Items: " + columnlength.ToString();
                        labelFail.Text = "Fail items: " + failcount.ToString();
                        dataGridView.DataSource = datatable;
                        filename = ofd.FileNames;
                        //Add ComboListBox
                        string s = datatable.Rows.Count.ToString();
                        TestDatalen = ConvertInt(s) - 2;
                        string t = datatable.Columns.Count.ToString();
                        TestHeaderlen = ConvertInt(t) - 6;
                        for (int j = 0; j < TestHeaderlen; j++)
                        {
                            testHeader[j] = datatable.Columns[j + 6].ColumnName;
                        }
                        for (int i = 0; i < TestDatalen; i++)
                        {
                            testStation[i] = datatable.Rows[i + 2][1].ToString().Trim();
                            testDate[i] = datatable.Rows[i + 2][2].ToString().Trim().Substring(0,8);
                            testDateFull[i] = datatable.Rows[i + 2][2].ToString().Trim();
                        }
                        TestItem[0] = testHeader[0];
                        //this.comboBox.AutoCompleteCustomSource.Clear();
                        for (int i = 0; i < TestHeaderlen; i++)
                        {
                            this.comboBox.AutoCompleteCustomSource.AddRange(testHeader);
                            this.comboBox.Items.Add(testHeader[i]);
                        }
                        for (int i = 0; i < RemoveSame(testStation).Length; i++)
                        {
                            cListBoxStation.Items.Add(RemoveSame(testStation)[i]);
                            getStation += RemoveSame(testStation)[i] + ",";
                        }
                        for (int i = 0; i < RemoveSame(testDate).Length; i++)
                        {
                            cListBoxDate.Items.Add(RemoveSame(testDate)[i]);
                        }
                        cbBoxstatus.Items.Add("Scatter Line");
                        cbBoxstatus.Items.Add("Normal Distribution");
                        cbBoxstatus.Items.Add("Combined Scatter Line");
                        cbBoxstatus.Items.Add("Combined Normal Distribution");
                        for (int i = 0; i < cListBoxStation.Items.Count; i++) { cListBoxStation.SetItemChecked(i, true); }
                        for (int i = 0; i < cListBoxDate.Items.Count; i++) { cListBoxDate.SetItemChecked(i, true); }
                        openstatus = true;
                        chart.Visible = false;
                        Display(true);
                        cbBoxstatus.SelectedIndex = 0;
                        rBtnDate.Checked = true;
                        rBtnStation.Checked = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }                
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (filename == null) { MessageBox.Show("Please open a .csv file!"); }
            else
            {
                if (chart.Visible == false) { MessageBox.Show("Please draw the chart first!"); }
                else
                {
                    FolderBrowserDialog sfd = new FolderBrowserDialog();
                    sfd.ShowNewFolderButton = true;
                    sfd.Description = "Please choose the saving path";
                    if(sfd.ShowDialog() == DialogResult.OK)
                    {
                        string savefolder = sfd.SelectedPath;
                        chart.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
                        chart.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
                        string timestamp = DateTime.Now.ToLocalTime().ToString("yyyyMMddhhmmss");
                        string pathImage = savefolder + "\\" + "(" + testitemtem + ")" + "[" + teststatustem + "]"+timestamp + ".png";
                        if (pathImage.Length >= 248) { MessageBox.Show("File name is too long!"); pathImage = savefoldertem + "\\" + "(" + testitemtem + ")" + "[" + teststatustem + "]" + ".png"; chart.SaveImage(pathImage, ChartImageFormat.Png); }
                        else
                        {
                            chart.SaveImage(pathImage, ChartImageFormat.Png);
                        }
                        MessageBox.Show("Chart is saved!");
                        for (int i = 0; i < chart.Series.Count; i++)
                        {
                            chart.Series[i].Points.Clear();
                        }
                        chart.Titles.Clear();
                        chart.Legends["Legend1"].CustomItems.Clear();
                        chart.Visible = false;
                    }
                }
            }
        }
        private void btnDraw_Click(object sender, EventArgs e)
        {
            if (filename == null) { MessageBox.Show("Please open a .csv file!"); }
            else
            {
                if(cListBoxStation.CheckedItems.Count == 0) { MessageBox.Show("Please select the station!"); } else
                {
                    if(cListBoxDate.CheckedItems.Count == 0) { MessageBox.Show("Please select the date!"); } else
                    {
                        if(comboBox.SelectedItem != null)
                        {
                            getHeader = comboBox.SelectedItem.ToString();                            
                            int count = FindColumn(getHeader);
                            UpperValue = datatable.Rows[1][count + 6].ToString();
                            LowerValue = datatable.Rows[0][count + 6].ToString();
                            for (int i = 0; i < TestDatalen; i++){TestData[i] = datatable.Rows[i + 2][count + 6].ToString(); }
                            chart.Visible = true;
                            testitemtem = getHeader;
                            teststatustem = getLineStatus;
                            chart.Titles.Clear();
                            chart.ChartAreas[0].AxisX.CustomLabels.Clear();
                            for (int i = 0; i < chart.Series.Count; i++){chart.Series[i].Points.Clear();}
                            chart.Series.Clear();
                            chart.Legends["Legend1"].CustomItems.Clear();
                            AddTitle();
                            var array = new double[TestDatalen];
                            for (int i = 0; i < TestDatalen; i++)
                            {
                                if (TestData[i] == "")
                                {
                                    MessageBox.Show("There is failed test!");
                                }
                                else { array[i] = ConvertDouble(TestData[i]); }
                            }
                            max = Math.Max(array.Max(), ConvertDouble(UpperValue));
                            min = Math.Min(array.Min(), ConvertDouble(LowerValue));
                            chart.ChartAreas[0].AxisX.LabelStyle.Angle = 0;
                            cListBoxDate.Visible = false;
                            if (getLineStatus == "Scatter Line") { ScatterLine(); }
                            if (getLineStatus == "Normal Distribution") { NormalDistribution(); Frequency(); }
                            if (getLineStatus == "Combined Scatter Line") { CombineScatterLine(count); if (rBtnDate.Checked == true) { cListBoxDate.Visible = true; } else { cListBoxDate.Visible = false; } }
                            if (getLineStatus == "Combined Normal Distribution") { CombineNomralDistribution(count); if (rBtnDate.Checked == true) { cListBoxDate.Visible = true; } else { cListBoxDate.Visible = false; } }                                                                     
                            chart.GetToolTipText += new EventHandler<ToolTipEventArgs>(chart_GetToolTipText);
                            chart.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
                            chart.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
                        }
                        else { MessageBox.Show("Please select the item!"); }
                    }
                }
             
            }
        }
        public void AddTitle()
        {
            string timestamp = DateTime.Now.ToLocalTime().ToString("yyyyMMddhhmmss");
            chart.Titles.Add(getHeader + "\r\n" + getLineStatus + "\r\n" + timestamp);
        }//Add title
        private void cListBoxStation_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if(openstatus == true)
            {
                if(getLineStatus == "Combined Scatter Line" || getLineStatus == "Combined Normal Distribution")
                {
                    string selectStation = string.Empty;
                    for (int i = 0; i < cListBoxStation.Items.Count; i++)
                    {
                        if (cListBoxStation.GetSelected(i)){selectStation = cListBoxStation.Items[i].ToString();}
                    }
                    if (selectStation != string.Empty)
                    {
                        if (getStation.IndexOf(selectStation) > -1){getStation = getStation.Replace(selectStation, "");}
                        else {getStation += "," + selectStation;}
                    }
                    string[] stationtem = getStation.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                    //for(int i = 0; i < stationtem.Length; i++) { MessageBox.Show(stationtem[i]); }
                    if (stationtem.Length == 0){MessageBox.Show("Please select the station!");}
                    else
                    {
                        cListBoxDate.Visible = false;
                        cListBoxDate.Items.Clear();
                        for (int i = 0; i < stationtem.Length; i++)
                        {
                            stationtem[i] = "StationID = '" + stationtem[i] + "'";
                            DataRow[] dr = datatable.Select(stationtem[i]);
                            DataTable tem = dr.CopyToDataTable();
                            string datetem = string.Empty;
                            foreach (DataRow row in dr)
                                datetem = datetem + row[2].ToString().Trim().Substring(0, 8) + ",";
                            string[] datearraytem = RemoveSame(datetem.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray());
                            for (int j = 0; j < datearraytem.Length; j++)
                            {
                                cListBoxDate.Items.Add(datearraytem[j]);
                                int index = cListBoxDate.Items.IndexOf(datearraytem[j]);
                                cListBoxDate.SetItemChecked(index, true);
                            }
                            cListBoxDate.Visible = true;
                        }
                        string filter = stationtem[0];
                        if (stationtem.Length == 1) { filter = stationtem[0]; }
                        else
                        {
                            for (int i = 1; i < stationtem.Length; i++)
                            {
                                filter = filter + " or " + stationtem[i];
                            }
                        }
                        datatable.DefaultView.RowFilter = filter;
                    }
                }                                   
            }       
        }
        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            if (filename == null) { MessageBox.Show("Please open a .csv file!"); }
            else
            {
                if(cBoxScatter.Checked == false && cBoxNormal.Checked == false){MessageBox.Show("Please select the type of chart you want to save!"); } else
                {
                    FolderBrowserDialog sfd = new FolderBrowserDialog();
                    sfd.ShowNewFolderButton = true;
                    sfd.Description = "Please choose the saving path";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        savefoldertem = sfd.SelectedPath;
                        chart.Visible = false;
                        comboBox.Visible = false;
                        cbBoxstatus.Visible = false;
                        btnDraw.Visible = false;
                        chart.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
                        chart.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
                        Directory.CreateDirectory(savefoldertem + "\\" + "imagetem");
                        string timestamp = DateTime.Now.ToLocalTime().ToString("yyyyMMddhhmmss");
                        string[] zippathImage = new string[3];
                        zippathImage[0] = savefoldertem + "\\" + "[All]"+timestamp + ".zip";
                        zippathImage[1] = savefoldertem + "\\" + "[Scatter Line]"+timestamp + ".zip";
                        zippathImage[2] = savefoldertem + "\\" + "[Normal Distribution]"+ timestamp +".zip";
                        for (int i = 0; i < 3; i++)
                        {
                            if (Directory.Exists(zippathImage[i]))
                            {
                                DirectoryInfo tem = new DirectoryInfo(zippathImage[i]);
                                tem.Delete(true);
                            }
                        }
                        if (cBoxScatter.Checked == true)
                        {
                            drawScatter();
                            zippathImagetem = zippathImage[1];
                            chart.Visible = false;
                            //MessageBox.Show("Scatter Line chart is saved!");
                            if (cBoxNormal.Checked == true)
                            {
                                drawNormalDistribution();
                                zippathImagetem = zippathImage[0];
                                chart.Visible = false;
                                //MessageBox.Show("Normal Distribution Line chart is saved!");
                            }
                        }
                        else
                        {
                            if (cBoxNormal.Checked == true)
                            {
                                comboBox.Visible = false;
                                drawNormalDistribution();
                                zippathImagetem = zippathImage[2];
                                chart.Visible = false;
                                comboBox.Visible = true;
                                //MessageBox.Show("Normal Distribution Line chart is saved!");
                            }
                        }
                        ZipClass.ZipFile(savefoldertem + "\\" + "imagetem" + "\\", zippathImagetem, out string err);
                        DirectoryInfo di = new DirectoryInfo(savefoldertem + "\\" + "imagetem");
                        di.Delete(true);
                        btnDraw.Visible = true;
                        comboBox.Visible = true;
                        cbBoxstatus.Visible = true;
                        MessageBox.Show("Charts have been successfully saved!");
                    }
                }                
            }
        }        
        public void drawScatter()
        {
            for(int j = 0; j < comboBox.Items.Count; j++)
            {                
                comboBox.SelectedIndex = j;
                getHeader = comboBox.SelectedItem.ToString();
                getLineStatus = "Scatter Line";
                int count = FindColumn(getHeader);
                UpperValue = datatable.Rows[1][count + 6].ToString();
                LowerValue = datatable.Rows[0][count + 6].ToString();
                for (int i = 0; i < TestDatalen; i++) { TestData[i] = datatable.Rows[i + 2][count + 6].ToString(); }
                chart.Visible = true;
                testitemtem = getHeader;
                teststatustem = getLineStatus;
                chart.Titles.Clear();
                chart.ChartAreas[0].AxisX.CustomLabels.Clear();
                for (int i = 0; i < chart.Series.Count; i++) { chart.Series[i].Points.Clear(); }
                chart.Series.Clear();
                chart.Legends["Legend1"].CustomItems.Clear();
                AddTitle();
                var array = new double[TestDatalen];
                for (int i = 0; i < TestDatalen; i++) { array[i] = ConvertDouble(TestData[i]); }
                max = Math.Max(array.Max(), ConvertDouble(UpperValue));
                min = Math.Min(array.Min(), ConvertDouble(LowerValue));
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = 0;
                ScatterLine();
                string timestamp = DateTime.Now.ToLocalTime().ToString("yyyyMMddhhmmss");
                string pathImage = savefoldertem + "\\" + "imagetem" + "\\" + timestamp + "(" + testitemtem + ")" + "[" + teststatustem + "]" + ".png";
                //MessageBox.Show(pathImage);
                if (pathImage.Length >= 248) { MessageBox.Show("File name is too long!"); pathImage = savefoldertem + "\\" + "imagetem" + "\\" + "(" + testitemtem + ")" + "[" + teststatustem + "]" + ".png"; chart.SaveImage(pathImage, ChartImageFormat.Png); } else
                {
                    chart.SaveImage(pathImage, ChartImageFormat.Png);
                }            
            }
        }
        public void drawNormalDistribution()
        {
            for (int j = 0; j < comboBox.Items.Count; j++)
            {
                
                comboBox.SelectedIndex = j;
                getHeader = comboBox.SelectedItem.ToString();
                getLineStatus = "Normal Distribution";
                int count = FindColumn(getHeader);
                UpperValue = datatable.Rows[1][count + 6].ToString();
                LowerValue = datatable.Rows[0][count + 6].ToString();
                for (int i = 0; i < TestDatalen; i++) { TestData[i] = datatable.Rows[i + 2][count + 6].ToString(); }
                chart.Visible = true;
                testitemtem = getHeader;
                teststatustem = getLineStatus;
                chart.Titles.Clear();
                chart.ChartAreas[0].AxisX.CustomLabels.Clear();
                for (int i = 0; i < chart.Series.Count; i++) { chart.Series[i].Points.Clear(); }
                chart.Series.Clear();
                chart.Legends["Legend1"].CustomItems.Clear();
                AddTitle();
                var array = new double[TestDatalen];
                for (int i = 0; i < TestDatalen; i++) { array[i] = ConvertDouble(TestData[i]); }
                max = Math.Max(array.Max(), ConvertDouble(UpperValue));
                min = Math.Min(array.Min(), ConvertDouble(LowerValue));
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = 0;
                NormalDistribution();
                string timestamp = DateTime.Now.ToLocalTime().ToString("yyyyMMddhhmmss");
                string pathImage = savefoldertem + "\\" + "imagetem" + "\\" + timestamp + "(" + testitemtem + ")" + "[" + teststatustem + "]" + ".png";
                //MessageBox.Show(pathImage);
                if (pathImage.Length >= 248) { MessageBox.Show("File name is too long!"); pathImage = savefoldertem + "\\" + "imagetem" + "\\" + "(" + testitemtem + ")" + "[" + teststatustem + "]" + ".png"; chart.SaveImage(pathImage, ChartImageFormat.Png); }
                else
                {
                    chart.SaveImage(pathImage, ChartImageFormat.Png);
                }
            }
        }
        private void btnCheck_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(searchText.Text);
            getSearch = searchText.Text;
            string[] searchtmp = new string[1000];
            searchtmp = testStation.Concat(testDate).ToArray();
            searchtmp = searchtmp.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            if(getSearch == string.Empty) { MessageBox.Show("Please enter searching keywords!"); } else
            {
                if (searchtmp.Contains(getSearch))
                {
                    if (getSearch.Length == testStation[0].Length)
                    {
                        if (testStation.Contains(getSearch)){
                            cListBoxDate.Visible = false;
                            int index = cListBoxStation.FindString(getSearch);
                            for (int i = 0; i < cListBoxStation.Items.Count; i++)
                            {
                                if (i == index)
                                {
                                    cListBoxStation.SetSelected(i, true);
                                    cListBoxStation.SetItemChecked(i, true);
                                }
                                else
                                {
                                    cListBoxStation.SetSelected(i, true);
                                    cListBoxStation.SetItemChecked(i, false);
                                }
                            };
                            cListBoxStation.SetSelected(index, true);
                            cListBoxDate.Visible = true;
                        } else{ MessageBox.Show("Please enter right searching keywords!"); }
                    }
                    else if(getSearch.Length == testDate[0].Length)
                    {
                        if (testDate.Contains(getSearch))
                        {
                            int index = cListBoxDate.FindString(getSearch);
                            for (int i = 0; i < cListBoxDate.Items.Count; i++)
                            {
                                if (i == index)
                                {
                                    cListBoxDate.SetSelected(i, true);
                                    cListBoxDate.SetItemChecked(i, true);
                                }
                                else
                                {
                                    cListBoxDate.SetSelected(i, true);
                                    cListBoxDate.SetItemChecked(i, false);
                                }
                            };
                            cListBoxDate.SetSelected(index, true);
                        }
                        else { MessageBox.Show("Please enter right searching keywords!"); }
                    } else { MessageBox.Show("Please enter right searching keywords!"); }
                } else{MessageBox.Show("Please enter right searching keywords!");}
            }
            searchText.ResetText();
        }
        public int ConvertInt(string str)
        {
            int a1 = int.Parse(str);
            int a2;
            int.TryParse(str, out a2);
            return Convert.ToInt32(str);
        }//convert string to int
        public double ConvertDouble(string str)
        {
            double a1 = double.Parse(str);
            double a2;
            double.TryParse(str, out a2);
            return Convert.ToDouble(str);
        }//convert string to double
        private void rBtnStation_CheckedChanged(object sender, EventArgs e)
        {
            if(rBtnDate.Checked == true)
            {
                cListBoxDate.Visible = true;
            }
            if(rBtnStation.Checked == true)
            {
                cListBoxDate.Visible = false;
            }
        }
        private void chart_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                int i = e.HitTestResult.PointIndex;
                DataPoint dp = e.HitTestResult.Series.Points[i];
                e.Text = string.Format("{1:F3}", dp.XValue, dp.YValues[0]);
            }
        }
        private void cbBoxstatus_SelectedIndexChanged(object sender, EventArgs e)
        {            
            if(cbBoxstatus.SelectedIndex == 2||cbBoxstatus.SelectedIndex == 3)
            {
                rBtnDate.Visible = true;
                rBtnStation.Visible = true;
                cListBoxStation.Visible = true;
                cListBoxDate.Visible = true;
                btnCheck.Visible = true;
                searchText.Visible = true;
            }
            else
            {
                rBtnDate.Visible = false;
                rBtnStation.Visible = false;
                cListBoxStation.Visible = false;
                cListBoxDate.Visible = false;
                btnCheck.Visible = false;
                searchText.Visible = false;
            }
            getLineStatus = cbBoxstatus.SelectedItem.ToString();
        }
        public int FindColumn(string str)
        {
            int count = 0;
            for (int i = 0; i < TestHeaderlen; i++)
            {
                if (testHeader[i] == str)
                {
                    count = i;
                }
            }
            return count;
        }//find header count
        private void btnShow_Click(object sender, EventArgs e)
        {
            if(btnShow.Text == "Open DataViewer")
            {
                dataGridView.Visible = true;
                btnShow.Text = "Close DataViewer";
            }
            else
            {
                dataGridView.Visible = false;
                btnShow.Text = "Open DataViewer";
            }
        }
        public void NormalDistribution()
        {
            for (int i = 0; i < cListBoxStation.Items.Count; i++)
            {
                cListBoxStation.SetItemChecked(i, true);
            }
            for (int i = 0; i < cListBoxDate.Items.Count; i++)
            {
                cListBoxDate.SetItemChecked(i, true);
            }
            Series series5 = new Series();
            Series series6 = new Series();
            Series series7 = new Series();
            Series series8 = new Series();
            series5.ChartArea = "ChartArea1";
            series5.ChartType = SeriesChartType.Line;
            series5.Color = Color.Lime;
            series5.IsVisibleInLegend = false;
            series5.Legend = "Legend1";
            series5.Name = "UpperValue";
            series6.ChartArea = "ChartArea1";
            series6.ChartType = SeriesChartType.Line;
            series6.Color = Color.Lime;
            series6.IsVisibleInLegend = false;
            series6.Legend = "Legend1";
            series6.Name = "LowerValue";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = SeriesChartType.Spline;
            series7.Color = Color.Red;
            series7.IsVisibleInLegend = false;
            series7.Legend = "Legend1";
            series7.Name = "NormalDistribution";
            series8.ChartArea = "ChartArea1";
            series8.IsVisibleInLegend = false;
            series8.Legend = "Legend1";
            series8.Name = "Frequency";
            series8.YAxisType = AxisType.Secondary;
            chart.Series.Add(series5);
            chart.Series.Add(series6);
            chart.Series.Add(series7);
            chart.Series.Add(series8);
            chart.Series["UpperValue"].IsVisibleInLegend = true;
            chart.Series["LowerValue"].IsVisibleInLegend = true;
            chart.Series["NormalDistribution"].IsVisibleInLegend = true;
            chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
            double sum = 0;
            for (int i = 0; i < TestDatalen; i++)
            {
                sum = sum + ConvertDouble(TestData[i]);
            }
            avg = sum / TestDatalen;
            double summ = 0;
            for (int i = 0; i < TestDatalen; i++)
            {
                summ = summ + (ConvertDouble(TestData[i]) - avg) * (ConvertDouble(TestData[i]) - avg);
            }
            delta = Math.Sqrt(summ / TestDatalen);
            for (double i = min; i < max; i = i + (max - min) / TestDatalen)
            {
                chart.Series["NormalDistribution"].Points.AddXY(i, NormalDistributionFunction(i, avg, delta));
            }
            double NDmax = NormalDistributionFunction(avg, avg, delta);
            chart.Series["UpperValue"].Points.AddXY(ConvertDouble(UpperValue), 0);
            chart.Series["UpperValue"].Points.AddXY(ConvertDouble(UpperValue), NDmax);
            chart.Series["LowerValue"].Points.AddXY(ConvertDouble(LowerValue), 0);
            chart.Series["LowerValue"].Points.AddXY(ConvertDouble(LowerValue), NDmax);
            chart.Series["UpperValue"].LegendText = "Upper Value = " + UpperValue;
            chart.Series["LowerValue"].LegendText = "Lower Value = " + LowerValue;
            cp = (ConvertDouble(UpperValue) - ConvertDouble(LowerValue)) / 6 / delta;
            cpl = (avg - ConvertDouble(LowerValue)) / 3 / delta;
            cpu = (ConvertDouble(UpperValue) - avg) / 3 / delta;
            cpk = Math.Min(cpl, cpu);
            chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, "Average = " + Math.Round(avg, 4));
            chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, "Variance = " + Math.Round(delta * delta, 4));
            chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, "CP = " + Math.Round(cp, 4));
            chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, "CPL = " + Math.Round(cpl, 4));
            chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, "CPU = " + Math.Round(cpu, 4));
            chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, "CPK = " + Math.Round(cpk, 4));
            for(int i=0;i< chart.Legends["Legend1"].CustomItems.Count; i++)
            {
                chart.Legends["Legend1"].CustomItems[i].BorderColor = Color.Transparent;
            }
        }//draw normal distribution line
        public void ScatterLine()
        {
            for(int i = 0; i < cListBoxStation.Items.Count; i++)
            {
                cListBoxStation.SetItemChecked(i, true);
            }
            for(int i = 0; i < cListBoxDate.Items.Count; i++)
            {
                cListBoxDate.SetItemChecked(i, true);
            }
            Series series1 = new Series();
            Series series2 = new Series();
            Series series3 = new Series();
            Series series4 = new Series();
            series1.ChartArea = "ChartArea1";
            series1.ChartType = SeriesChartType.Line;
            series1.Color = Color.Blue;
            series1.IsVisibleInLegend = false;
            series1.Legend = "Legend1";
            series1.Name = "ScatterLine";
            series1.XValueType = ChartValueType.Double;
            series1.YValueType = ChartValueType.Double;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = SeriesChartType.Line;
            series2.Color = Color.Lime;
            series2.IsVisibleInLegend = false;
            series2.Legend = "Legend1";
            series2.Name = "UpperValue";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = SeriesChartType.Line;
            series3.Color = Color.Lime;
            series3.IsVisibleInLegend = false;
            series3.Legend = "Legend1";
            series3.Name = "LowerValue";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = SeriesChartType.Point;
            series4.Color = Color.Red;
            series4.IsVisibleInLegend = false;
            series4.Legend = "Legend1";
            series4.Name = "Scatter";
            chart.Series.Add(series1);
            chart.Series.Add(series2);
            chart.Series.Add(series3);
            chart.Series.Add(series4);
            chart.Series["UpperValue"].IsVisibleInLegend = true;
            chart.Series["LowerValue"].IsVisibleInLegend = true;
            chart.Series["UpperValue"].LegendText = "Upper Value = " + UpperValue;
            chart.Series["LowerValue"].LegendText = "Lower Value = " + LowerValue;
            chart.Series["ScatterLine"].IsVisibleInLegend = true;
            for (int i = 0; i < TestDatalen; i++)
            {
                chart.Series["UpperValue"].Points.AddXY(i, UpperValue);
                chart.Series["LowerValue"].Points.AddXY(i, LowerValue);
                chart.Series["ScatterLine"].Points.AddXY(i, TestData[i]);
                chart.Series["Scatter"].Points.AddXY(i, TestData[i]);
            }
            chart.ChartAreas[0].AxisY.IsStartedFromZero = false;
            //chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, "Maximum = " + Math.Round(max, 2));
            //chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, "Minimum = " + Math.Round(min, 2));
        }//draw scatter line
        private void button1_Click(object sender, EventArgs e)
        {
            if (filename == null) { MessageBox.Show("Please open a .csv file!"); }
            else
            {
                FolderBrowserDialog sfd = new FolderBrowserDialog();
                sfd.ShowNewFolderButton = true;
                sfd.Description = "Please choose the saving path";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string savefolder = sfd.SelectedPath;
                    string timestamp = DateTime.Now.ToLocalTime().ToString("yyyyMMddhhmmss");
                    string pathtmp = savefolder + "\\" + timestamp + ".csv";
                    using (StreamWriter writer = new StreamWriter(pathtmp))
                    {
                        WriteDataTable(datatable, writer, true);
                    }
                    MessageBox.Show("Csv is saved!");
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            addTemplate = true;
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "CSV|*.csv", ValidateNames = true })
            {

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    DataTable tmp = ReadCsv(ofd.FileName);
                    for(int i = 2; i < tmp.Rows.Count; i++)
                    {
                        tmp.Rows[i].Delete();
                    }
                    tmp.AcceptChanges();
                    datatable = tmp;
                    datatable.AcceptChanges();
                }
            }
            MessageBox.Show("Column name have been added!");
        }
        public double NormalDistributionFunction(double x, double avg, double delta)
        {
            return (1 / Math.Sqrt(2 * Math.PI) / delta) * Math.Exp((x - avg) * (avg - x) / 2 / delta / delta);
        }//calculate normal distribution
        public void Frequency()
        {
            chart.Series["Frequency"].IsVisibleInLegend = true;
            var array = new double[TestDatalen];
            for (int i = 0; i < TestDatalen; i++)
            {
                array[i] = ConvertDouble(TestData[i]);
            }
            int count = Convert.ToInt32(Math.Ceiling((max - min) / interval));
            var tem = new int[count];
            for (int i = 0; i < TestDatalen; i++)
            {
                int a = Convert.ToInt32((array[i] - min) / interval);
                tem[a] = tem[a] + 1;
            }
            for (int j = 0; j < count; j++)
            {
                chart.Series["Frequency"].Points.AddXY(min + interval * j, tem[j]);
            }
        }//draw frequency line
        public void Display(bool a)
        {
            cBoxScatter.Visible = a;
            cBoxNormal.Visible = a;
            btnSaveAll.Visible = a;
            comboBox.Visible = a;
            cbBoxstatus.Visible = a;
            cListBoxStation.Visible = a;
            cListBoxDate.Visible = a;
            searchText.Visible = a;
            btnCheck.Visible = a;
            btnDraw.Visible = a;
            rBtnDate.Visible = a;
            rBtnStation.Visible = a;
            btnShow.Visible = a;
            btnSave.Visible = a;
            btnClose.Visible = a;
            labelTotal.Visible = a;
            labelFail.Visible = a;
            button1.Visible = a;
        }//display item state
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (filename == null) { MessageBox.Show("Please open a .csv file!"); }
            else
            {
                btnShow.Text = "Open DataViewer";
                dataGridView.Visible = false;
                chart.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
                chart.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
                Display(false);
                chart.Visible = false;
                datatable.Columns.Clear();
                datatable.Clear();                
                cListBoxStation.Items.Clear();
                cListBoxDate.Items.Clear();
                comboBox.Items.Clear();
                cbBoxstatus.Items.Clear();
                openstatus = false;
                searchText.ResetText();
                filename = null;
                UpperValue = null;
                LowerValue = null;
                TestItem = new string[500];
                TestData = new string[500];
                testHeader = new string[500];
                testDate = new string[500];
                testDateFull = new string[500];
                testStation = new string[500];
                testitemtem = null;
                teststatustem = null;
                getHeader = null;
                getLineStatus = null;
                getStation = null;
                getSearch = string.Empty;
                zippathImagetem = null;
                addTemplate = false;
            }
        }
        private string[] RemoveSame(string[] str)
        {
            List<string> list = new List<string>();
            foreach (string item in str)
            {
                if (!list.Contains(item)) { list.Add(item); }
            }
            return list.ToArray().Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }//remove same component in string[]
        public void CombineScatterLine(int count)
        {
            Series series2 = new Series();
            series2.ChartArea = "ChartArea1";
            series2.ChartType = SeriesChartType.Line;
            series2.Color = Color.Lime;
            series2.IsVisibleInLegend = true;
            series2.Legend = "Legend1";
            series2.Name = "UpperValue";
            Series series3 = new Series();
            series3.ChartArea = "ChartArea1";
            series3.ChartType = SeriesChartType.Line;
            series3.Color = Color.Lime;
            series3.IsVisibleInLegend = true;
            series3.Legend = "Legend1";
            series3.Name = "LowerValue";
            chart.Series.Add(series2);
            chart.Series.Add(series3);
            chart.ChartAreas[0].AxisY.LabelStyle.Enabled = true;
            string[] getdate = new string[500];
            for (int i = 0; i < cListBoxDate.Items.Count; i++)
            {
                if (cListBoxDate.GetItemChecked(i))
                {
                    cListBoxDate.SetSelected(i, true);
                    getdate[i]=cListBoxDate.Text.ToString();
                }
            }
            getdate=getdate.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            string[] getdategg = new string[500];
            testDateFull = testDateFull.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            for (int i=0;i<getdate.Length;i++)
            {
                for(int j=0;j<testDateFull.Length;j++)
                {
                    if(testDateFull[j].Substring(0,8)==getdate[i])
                    {
                        getdategg[i] = testDateFull[j];
                    }
                }
            }
            getdategg = getdategg.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            //MessageBox.Show(getdate.Length.ToString());
            for (int i = 1; i < getdategg.Length+1; i++)
            {
                getdategg[i-1] = "StartTime = '	" + getdategg[i-1] + "'";
                DataRow[] dr = datatable.Select(getdategg[i-1]);
                DataTable tem = dr.CopyToDataTable();
                string datatem = string.Empty;
                foreach (DataRow row in dr)
                {
                    datatem = datatem + row[count + 6].ToString().Trim() + ",";

                }
                string[] dataarraytem = RemoveSame(datatem.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray());
                chart.Series.Add(i.ToString());
                chart.Series[i.ToString()].ChartType = SeriesChartType.Point;
                chart.Series[i.ToString()].Color = Color.Red;
                chart.Series[i.ToString()].MarkerStyle = MarkerStyle.Circle;
                chart.Series[i.ToString()].IsVisibleInLegend = false;
                foreach (DataRow row in dr)
                {
                    chart.Series[i.ToString()].Points.AddXY(i,row[count + 6]);
                }
                double sum = 0;
                double summ = 0;
                foreach (DataRow row in dr)
                {
                    sum = sum + ConvertDouble(row[count + 6].ToString()) / dr.Count();
                }
                foreach (DataRow row in dr)
                {
                    summ = summ + (ConvertDouble(row[count + 6].ToString()) - sum) * (ConvertDouble(row[count + 6].ToString()) - sum) / dr.Count();
                }
                chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, getdate[i - 1] + " avg: " + Math.Round(sum, 4).ToString());
                chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, getdate[i - 1] + " dev: " + Math.Round(summ, 4).ToString());
                for (int j = 0; j < chart.Legends["Legend1"].CustomItems.Count; j++)
                {
                    chart.Legends["Legend1"].CustomItems[j].BorderColor = Color.Transparent;
                }
                CustomLabel label = new CustomLabel();
                label.Text = getdate[i-1];
                label.FromPosition = -2;
                label.ToPosition = 2*(i+1);
                chart.ChartAreas[0].AxisX.CustomLabels.Add(label);
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            }
            for (int i = 0; i < getdate.Length+2; i++)
            {
                chart.Series["UpperValue"].Points.AddXY(i, UpperValue);
                chart.Series["LowerValue"].Points.AddXY(i, LowerValue);
            }
            chart.Series["UpperValue"].LegendText = "Upper Value = " + UpperValue;
            chart.Series["LowerValue"].LegendText = "Lower Value = " + LowerValue;
            chart.ChartAreas[0].AxisY.IsLabelAutoFit = true;
            chart.ChartAreas[0].AxisY.IsStartedFromZero = false;

        }//draw combined scatter line by date
        public void CombineNomralDistribution(int count)
        {
            Series series2 = new Series();
            series2.ChartArea = "ChartArea1";
            series2.ChartType = SeriesChartType.Line;
            series2.Color = Color.Lime;
            series2.IsVisibleInLegend = true;
            series2.Legend = "Legend1";
            series2.Name = "UpperValue";
            Series series3 = new Series();
            series3.ChartArea = "ChartArea1";
            series3.ChartType = SeriesChartType.Line;
            series3.Color = Color.Lime;
            series3.IsVisibleInLegend = true;
            series3.Legend = "Legend1";
            series3.Name = "LowerValue";
            chart.Series.Add(series2);
            chart.Series.Add(series3);
            chart.ChartAreas[0].AxisY.LabelStyle.Enabled = true;
            string[] getdate = new string[500];
            for (int i = 0; i < cListBoxDate.Items.Count; i++)
            {
                if (cListBoxDate.GetItemChecked(i))
                {
                    cListBoxDate.SetSelected(i, true);
                    getdate[i] = cListBoxDate.Text.ToString();
                }
            }
            getdate = getdate.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            string[] getdategg = new string[500];
            testDateFull = testDateFull.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            for (int i = 0; i < getdate.Length; i++)
            {
                for (int j = 0; j < testDateFull.Length; j++)
                {
                    if (testDateFull[j].Substring(0, 8) == getdate[i])
                    {
                        getdategg[i] = testDateFull[j];
                    }
                }
            }
            getdategg = getdategg.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            for (int i = 1; i < getdategg.Length+1; i++)
            {
                getdategg[i-1] = "StartTime = '	" + getdategg[i-1] + "'";
                DataRow[] dr = datatable.Select(getdategg[i-1]);
                DataTable tem = dr.CopyToDataTable();
                string datatem = string.Empty;
                foreach (DataRow row in dr)
                {
                    datatem = datatem + row[count + 6].ToString().Trim() + ",";

                }
                string[] dataarraytem = RemoveSame(datatem.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray());
                chart.Series.Add(i.ToString());
                chart.Series[i.ToString()].ChartType = SeriesChartType.Point;
                chart.Series[i.ToString()].Color = Color.Red;
                chart.Series[i.ToString()].MarkerStyle = MarkerStyle.Circle;
                chart.Series[i.ToString()].MarkerSize = 8;
                chart.Series[i.ToString()].IsVisibleInLegend = false;                
                chart.Series.Add((getdate.Length+i).ToString());
                chart.Series[(getdate.Length + i).ToString()].ChartType = SeriesChartType.Line;
                chart.Series[(getdate.Length + i).ToString()].Color = Color.Blue;
                chart.Series[(getdate.Length + i).ToString()].IsVisibleInLegend = false;
                double sum =0;
                double summ = 0;
                foreach (DataRow row in dr)
                {
                    sum = sum + ConvertDouble(row[count + 6].ToString())/dr.Count();
                }
                foreach(DataRow row in dr)
                {
                    summ = summ + (ConvertDouble(row[count + 6].ToString()) - sum) * (ConvertDouble(row[count + 6].ToString()) - sum) / dr.Count();
                }
                chart.Series[i.ToString()].Points.AddXY(i, sum);
                chart.Series[(getdate.Length + i).ToString()].Points.AddXY(i, sum - Math.Sqrt(summ));
                chart.Series[(getdate.Length + i).ToString()].Points.AddXY(i, sum + Math.Sqrt(summ));
                chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, getdate[i - 1] + " avg: " + Math.Round(sum, 4).ToString());
                chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, getdate[i - 1] + " dev: " + Math.Round(summ, 4).ToString());
                for (int j = 0; j < chart.Legends["Legend1"].CustomItems.Count; j++)
                {
                    chart.Legends["Legend1"].CustomItems[j].BorderColor = Color.Transparent;
                }
                CustomLabel label = new CustomLabel();
                label.Text = getdate[i-1];
                label.FromPosition = -2;
                label.ToPosition = 2 * (i + 1);
                chart.ChartAreas[0].AxisX.CustomLabels.Add(label);
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;                
            }
            for (int i = 0; i < getdate.Length + 2; i++)
            {
                chart.Series["UpperValue"].Points.AddXY(i, UpperValue);
                chart.Series["LowerValue"].Points.AddXY(i, LowerValue);
            }
            chart.Series["UpperValue"].LegendText = "Upper Value = " + UpperValue;
            chart.Series["LowerValue"].LegendText = "Lower Value = " + LowerValue;
            chart.ChartAreas[0].AxisY.IsLabelAutoFit = true;
            chart.ChartAreas[0].AxisY.IsStartedFromZero = false;
            chart.Legends[0].BackColor = Color.Transparent;
        }//draw combined normal distribution line by date
        public void RemoveListSame(ListBox a)
        {
            for (int i = 0; i < a.Items.Count; i++)
            {
                for (int j = i + 1; j < a.Items.Count; j++)
                {
                    if (a.Items[j].ToString() == a.Items[i].ToString())
                    {
                        a.Items.RemoveAt(j);
                    }
                }
            }
        }//Remove repeated items in ListBox
        public void CombineScatterLineStation(int count)
        {
            Series series2 = new Series();
            series2.ChartArea = "ChartArea1";
            series2.ChartType = SeriesChartType.Line;
            series2.Color = Color.Lime;
            series2.IsVisibleInLegend = true;
            series2.Legend = "Legend1";
            series2.Name = "UpperValue";
            Series series3 = new Series();
            series3.ChartArea = "ChartArea1";
            series3.ChartType = SeriesChartType.Line;
            series3.Color = Color.Lime;
            series3.IsVisibleInLegend = true;
            series3.Legend = "Legend1";
            series3.Name = "LowerValue";
            chart.Series.Add(series2);
            chart.Series.Add(series3);
            chart.ChartAreas[0].AxisY.LabelStyle.Enabled = true;
            string[] stationget = new string[500];
            for(int i = 0; i < cListBoxStation.Items.Count; i++)
            {
                if (cListBoxStation.GetItemChecked(i))
                {
                    stationget[i]=cListBoxStation.GetItemText(cListBoxStation.Items[i]);
                }
            }
            stationget = stationget.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            string[] getstationgg = new string[stationget.Length];
            for (int i = 1; i < stationget.Length + 1; i++)
            {
                getstationgg[i - 1] = "StationID = '" + stationget[i - 1] + "'";
                DataRow[] dr = datatable.Select(getstationgg[i - 1]);
                DataTable tem = dr.CopyToDataTable();
                string datatem = string.Empty;
                foreach (DataRow row in dr)
                {
                    datatem = datatem + row[count + 6].ToString().Trim() + ",";

                }
                string[] dataarraytem = RemoveSame(datatem.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray());
                chart.Series.Add(i.ToString());
                chart.Series[i.ToString()].ChartType = SeriesChartType.Point;
                chart.Series[i.ToString()].Color = Color.Red;
                chart.Series[i.ToString()].MarkerStyle = MarkerStyle.Circle;
                chart.Series[i.ToString()].IsVisibleInLegend = false;
                foreach (DataRow row in dr)
                {
                    chart.Series[i.ToString()].Points.AddXY(i, row[count + 6]);
                }
                double sum = 0;
                double summ = 0;
                foreach (DataRow row in dr)
                {
                    sum = sum + ConvertDouble(row[count + 6].ToString()) / dr.Count();
                }
                foreach (DataRow row in dr)
                {
                    summ = summ + (ConvertDouble(row[count + 6].ToString()) - sum) * (ConvertDouble(row[count + 6].ToString()) - sum) / dr.Count();
                }
                chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, stationget[i - 1] + " avg: " + Math.Round(sum, 4).ToString());
                chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, stationget[i - 1] + " dev: " + Math.Round(summ, 4).ToString());
                for (int j = 0; j < chart.Legends["Legend1"].CustomItems.Count; j++)
                {
                    chart.Legends["Legend1"].CustomItems[j].BorderColor = Color.Transparent;
                }
                CustomLabel label = new CustomLabel();
                label.Text = stationget[i - 1];
                label.FromPosition = -2;
                label.ToPosition = 2 * (i + 1);
                chart.ChartAreas[0].AxisX.CustomLabels.Add(label);
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            }
            for (int i = 0; i < stationget.Length + 2; i++)
            {
                chart.Series["UpperValue"].Points.AddXY(i, UpperValue);
                chart.Series["LowerValue"].Points.AddXY(i, LowerValue);
            }
            chart.Series["UpperValue"].LegendText = "Upper Value = " + UpperValue;
            chart.Series["LowerValue"].LegendText = "Lower Value = " + LowerValue;
            chart.ChartAreas[0].AxisY.IsLabelAutoFit = true;
            chart.ChartAreas[0].AxisY.IsStartedFromZero = false;
        }//draw combined scatter line by station
        public void CombineNormalDistributionStation(int count)
        {
            Series series2 = new Series();
            series2.ChartArea = "ChartArea1";
            series2.ChartType = SeriesChartType.Line;
            series2.Color = Color.Lime;
            series2.IsVisibleInLegend = true;
            series2.Legend = "Legend1";
            series2.Name = "UpperValue";
            Series series3 = new Series();
            series3.ChartArea = "ChartArea1";
            series3.ChartType = SeriesChartType.Line;
            series3.Color = Color.Lime;
            series3.IsVisibleInLegend = true;
            series3.Legend = "Legend1";
            series3.Name = "LowerValue";
            chart.Series.Add(series2);
            chart.Series.Add(series3);
            chart.ChartAreas[0].AxisY.LabelStyle.Enabled = true;
            string[] stationget = new string[500];
            for (int i = 0; i < cListBoxStation.Items.Count; i++)
            {
                if (cListBoxStation.GetItemChecked(i))
                {
                    stationget[i] = cListBoxStation.GetItemText(cListBoxStation.Items[i]);
                }
            }
            stationget = stationget.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            string[] getstationgg = new string[stationget.Length];
            string[] getdategg = new string[stationget.Length];
            for (int i = 1; i < stationget.Length + 1; i++)
            {
                getdategg[i - 1] = "StationID = '" + stationget[i - 1] + "'";
                DataRow[] dr = datatable.Select(getdategg[i - 1]);
                DataTable tem = dr.CopyToDataTable();
                string datatem = string.Empty;
                foreach (DataRow row in dr)
                {
                    datatem = datatem + row[count + 6].ToString().Trim() + ",";

                }
                string[] dataarraytem = RemoveSame(datatem.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray());
                chart.Series.Add(i.ToString());
                chart.Series[i.ToString()].ChartType = SeriesChartType.Point;
                chart.Series[i.ToString()].Color = Color.Red;
                chart.Series[i.ToString()].MarkerStyle = MarkerStyle.Circle;
                chart.Series[i.ToString()].MarkerSize = 8;
                chart.Series[i.ToString()].IsVisibleInLegend = false;
                chart.Series.Add((stationget.Length + i).ToString());
                chart.Series[(stationget.Length + i).ToString()].ChartType = SeriesChartType.Line;
                chart.Series[(stationget.Length + i).ToString()].Color = Color.Blue;
                chart.Series[(stationget.Length + i).ToString()].IsVisibleInLegend = false;
                double sum = 0;
                double summ = 0;
                foreach (DataRow row in dr)
                {
                    sum = sum + ConvertDouble(row[count + 6].ToString()) / dr.Count();
                }
                foreach (DataRow row in dr)
                {
                    summ = summ + (ConvertDouble(row[count + 6].ToString()) - sum) * (ConvertDouble(row[count + 6].ToString()) - sum) / dr.Count();
                }
                //MessageBox.Show(sum.ToString());
                chart.Series[i.ToString()].Points.AddXY(i, sum);
                chart.Series[(stationget.Length + i).ToString()].Points.AddXY(i, sum - Math.Sqrt(summ));
                chart.Series[(stationget.Length + i).ToString()].Points.AddXY(i, sum + Math.Sqrt(summ));                
                chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, stationget[i - 1] + " avg: " + Math.Round(sum, 4).ToString());
                chart.Legends["Legend1"].CustomItems.Add(Color.Transparent, stationget[i - 1] + " dev: " + Math.Round(summ, 4).ToString());
                for (int j = 0; j < chart.Legends["Legend1"].CustomItems.Count; j++)
                {
                    chart.Legends["Legend1"].CustomItems[j].BorderColor = Color.Transparent;
                }
                CustomLabel label = new CustomLabel();
                label.Text = stationget[i - 1];
                label.FromPosition = -2;
                label.ToPosition = 2 * (i + 1);
                chart.ChartAreas[0].AxisX.CustomLabels.Add(label);
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            }
            for (int i = 0; i < stationget.Length + 2; i++)
            {
                chart.Series["UpperValue"].Points.AddXY(i, UpperValue);
                chart.Series["LowerValue"].Points.AddXY(i, LowerValue);
            }
            chart.Series["UpperValue"].LegendText = "Upper Value = " + UpperValue;
            chart.Series["LowerValue"].LegendText = "Lower Value = " + LowerValue;
            chart.ChartAreas[0].AxisY.IsLabelAutoFit = true;
            chart.ChartAreas[0].AxisY.IsStartedFromZero = false;
        }//draw combined normal distribution line by station
        private static string QuoteValue(string value)
        {
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }
        public static void WriteDataTable(DataTable sourceTable, TextWriter writer, bool includeHeaders)
        {
            if (includeHeaders)
            {
                IEnumerable<String> headerValues = sourceTable.Columns
                    .OfType<DataColumn>()
                    .Select(column => QuoteValue(column.ColumnName));

                writer.WriteLine(String.Join(",", headerValues));
            }

            IEnumerable<String> items = null;

            foreach (DataRow row in sourceTable.Rows)
            {
                items = row.ItemArray.Select(o => QuoteValue(o?.ToString() ?? String.Empty));
                writer.WriteLine(String.Join(",", items));
            }

            writer.Flush();
        }
    }
    public class ZipClass
    {
        #region 加压
        /// <summary>
        /// 功能：压缩文件（暂时只压缩文件夹下一级目录中的文件，文件夹及其子级被忽略）
        /// </summary>
        /// <param name="dirPath"> 被压缩的文件夹夹路径 </param>
        /// <param name="zipFilePath"> 生成压缩文件的路径，为空则默认与被压缩文件夹同一级目录，名称为：文件夹名 +.zip</param>
        /// <param name="err"> 出错信息</param>
        /// <returns> 是否压缩成功 </returns>
        static public bool ZipFile(string dirPath, string zipFilePath, out string err)
        {
            err = "";
            if (dirPath == string.Empty)
            {
                err = "要压缩的文件夹不能为空！ ";
                return false;
            }
            if (!Directory.Exists(dirPath))
            {
                err = "要压缩的文件夹不存在！ ";
                return false;
            }
            //压缩文件名为空时使用文件夹名＋ zip
            if (zipFilePath == string.Empty)
            {
                if (dirPath.EndsWith("\\"))
                {
                    dirPath = dirPath.Substring(0, dirPath.Length - 1);
                }
                zipFilePath = dirPath + ".zip";
            }

            try
            {
                string[] filenames = Directory.GetFiles(dirPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    s.SetLevel(9);
                    byte[] buffer = new byte[4096];
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            return true;
        }

        #endregion
    }
    
}

