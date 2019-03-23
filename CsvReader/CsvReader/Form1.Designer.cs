namespace CsvReader
{
    partial class CsvReader
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CsvReader));
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnDraw = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.cbBoxstatus = new System.Windows.Forms.ComboBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.cListBoxStation = new System.Windows.Forms.CheckedListBox();
            this.cListBoxDate = new System.Windows.Forms.CheckedListBox();
            this.searchText = new System.Windows.Forms.RichTextBox();
            this.btnCheck = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cBoxScatter = new System.Windows.Forms.CheckBox();
            this.cBoxNormal = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.rBtnDate = new System.Windows.Forms.RadioButton();
            this.rBtnStation = new System.Windows.Forms.RadioButton();
            this.btnShow = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.labelTotal = new System.Windows.Forms.Label();
            this.labelFail = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(7, 42);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnDraw
            // 
            this.btnDraw.Location = new System.Drawing.Point(602, 72);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(75, 23);
            this.btnDraw.TabIndex = 1;
            this.btnDraw.Text = "Draw";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.Visible = false;
            this.btnDraw.Click += new System.EventHandler(this.btnDraw_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(256, 43);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // comboBox
            // 
            this.comboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox.BackColor = System.Drawing.SystemColors.Window;
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(6, 71);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(438, 21);
            this.comboBox.TabIndex = 5;
            this.comboBox.Visible = false;
            // 
            // cbBoxstatus
            // 
            this.cbBoxstatus.BackColor = System.Drawing.SystemColors.Window;
            this.cbBoxstatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBoxstatus.FormattingEnabled = true;
            this.cbBoxstatus.Location = new System.Drawing.Point(450, 72);
            this.cbBoxstatus.Name = "cbBoxstatus";
            this.cbBoxstatus.Size = new System.Drawing.Size(146, 21);
            this.cbBoxstatus.TabIndex = 6;
            this.cbBoxstatus.Visible = false;
            this.cbBoxstatus.SelectedIndexChanged += new System.EventHandler(this.cbBoxstatus_SelectedIndexChanged);
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(7, 179);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.Size = new System.Drawing.Size(664, 171);
            this.dataGridView.TabIndex = 7;
            this.dataGridView.Visible = false;
            // 
            // chart
            // 
            this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.IsStartedFromZero = false;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.ScrollBar.Size = 10D;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisY2.IsLabelAutoFit = false;
            chartArea1.AxisY2.MajorGrid.LineColor = System.Drawing.Color.Transparent;
            chartArea1.CursorX.Interval = 0D;
            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.CursorX.LineColor = System.Drawing.Color.Transparent;
            chartArea1.CursorY.Interval = 0D;
            chartArea1.CursorY.IsUserEnabled = true;
            chartArea1.CursorY.IsUserSelectionEnabled = true;
            chartArea1.CursorY.LineColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart.Legends.Add(legend1);
            this.chart.Location = new System.Drawing.Point(6, 179);
            this.chart.Margin = new System.Windows.Forms.Padding(0);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(665, 483);
            this.chart.TabIndex = 4;
            this.chart.Text = "chart1";
            this.chart.Visible = false;
            this.chart.GetToolTipText += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs>(this.chart_GetToolTipText);
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveAll.Location = new System.Drawing.Point(602, 43);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAll.TabIndex = 11;
            this.btnSaveAll.Text = "Save All";
            this.btnSaveAll.UseVisualStyleBackColor = true;
            this.btnSaveAll.Visible = false;
            this.btnSaveAll.Click += new System.EventHandler(this.btnSaveAll_Click);
            // 
            // cListBoxStation
            // 
            this.cListBoxStation.CheckOnClick = true;
            this.cListBoxStation.FormattingEnabled = true;
            this.cListBoxStation.Location = new System.Drawing.Point(105, 97);
            this.cListBoxStation.Name = "cListBoxStation";
            this.cListBoxStation.Size = new System.Drawing.Size(94, 79);
            this.cListBoxStation.TabIndex = 12;
            this.cListBoxStation.Visible = false;
            this.cListBoxStation.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.cListBoxStation_ItemCheck);
            // 
            // cListBoxDate
            // 
            this.cListBoxDate.CheckOnClick = true;
            this.cListBoxDate.Location = new System.Drawing.Point(205, 97);
            this.cListBoxDate.Name = "cListBoxDate";
            this.cListBoxDate.Size = new System.Drawing.Size(154, 79);
            this.cListBoxDate.TabIndex = 13;
            this.cListBoxDate.Visible = false;
            // 
            // searchText
            // 
            this.searchText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchText.ForeColor = System.Drawing.SystemColors.WindowText;
            this.searchText.Location = new System.Drawing.Point(365, 125);
            this.searchText.MaxLength = 15;
            this.searchText.Multiline = false;
            this.searchText.Name = "searchText";
            this.searchText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.searchText.Size = new System.Drawing.Size(231, 22);
            this.searchText.TabIndex = 14;
            this.searchText.Text = "";
            this.searchText.Visible = false;
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(602, 125);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(75, 23);
            this.btnCheck.TabIndex = 15;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Visible = false;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Broadway", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(199, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 31);
            this.label1.TabIndex = 19;
            this.label1.Text = "Smart Analysis Tool";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cBoxScatter
            // 
            this.cBoxScatter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cBoxScatter.AutoSize = true;
            this.cBoxScatter.BackColor = System.Drawing.Color.Transparent;
            this.cBoxScatter.Checked = true;
            this.cBoxScatter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBoxScatter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBoxScatter.Location = new System.Drawing.Point(362, 47);
            this.cBoxScatter.Name = "cBoxScatter";
            this.cBoxScatter.Size = new System.Drawing.Size(95, 17);
            this.cBoxScatter.TabIndex = 20;
            this.cBoxScatter.Text = "Scatter Line";
            this.cBoxScatter.UseVisualStyleBackColor = false;
            this.cBoxScatter.Visible = false;
            // 
            // cBoxNormal
            // 
            this.cBoxNormal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cBoxNormal.AutoSize = true;
            this.cBoxNormal.BackColor = System.Drawing.Color.Transparent;
            this.cBoxNormal.Checked = true;
            this.cBoxNormal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBoxNormal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBoxNormal.Location = new System.Drawing.Point(463, 47);
            this.cBoxNormal.Name = "cBoxNormal";
            this.cBoxNormal.Size = new System.Drawing.Size(133, 17);
            this.cBoxNormal.TabIndex = 21;
            this.cBoxNormal.Text = "Normal Distribution";
            this.cBoxNormal.UseVisualStyleBackColor = false;
            this.cBoxNormal.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(175, 43);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 22;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Visible = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // rBtnDate
            // 
            this.rBtnDate.AutoSize = true;
            this.rBtnDate.BackColor = System.Drawing.Color.Transparent;
            this.rBtnDate.Checked = true;
            this.rBtnDate.Location = new System.Drawing.Point(6, 97);
            this.rBtnDate.Margin = new System.Windows.Forms.Padding(2);
            this.rBtnDate.Name = "rBtnDate";
            this.rBtnDate.Size = new System.Drawing.Size(84, 17);
            this.rBtnDate.TabIndex = 23;
            this.rBtnDate.TabStop = true;
            this.rBtnDate.Text = "Sort by Date";
            this.rBtnDate.UseVisualStyleBackColor = false;
            this.rBtnDate.Visible = false;
            // 
            // rBtnStation
            // 
            this.rBtnStation.AutoSize = true;
            this.rBtnStation.BackColor = System.Drawing.Color.Transparent;
            this.rBtnStation.Location = new System.Drawing.Point(6, 118);
            this.rBtnStation.Margin = new System.Windows.Forms.Padding(2);
            this.rBtnStation.Name = "rBtnStation";
            this.rBtnStation.Size = new System.Drawing.Size(94, 17);
            this.rBtnStation.TabIndex = 24;
            this.rBtnStation.Text = "Sort by Station";
            this.rBtnStation.UseVisualStyleBackColor = false;
            this.rBtnStation.Visible = false;
            this.rBtnStation.CheckedChanged += new System.EventHandler(this.rBtnStation_CheckedChanged);
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(446, 153);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(109, 23);
            this.btnShow.TabIndex = 25;
            this.btnShow.Text = "Open DataViewer";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Visible = false;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.BackColor = System.Drawing.Color.Transparent;
            this.labelTotal.Location = new System.Drawing.Point(7, 147);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(62, 13);
            this.labelTotal.TabIndex = 27;
            this.labelTotal.Text = "Total Items:";
            this.labelTotal.Visible = false;
            // 
            // labelFail
            // 
            this.labelFail.AutoSize = true;
            this.labelFail.BackColor = System.Drawing.Color.Transparent;
            this.labelFail.Location = new System.Drawing.Point(8, 163);
            this.labelFail.Name = "labelFail";
            this.labelFail.Size = new System.Drawing.Size(56, 13);
            this.labelFail.TabIndex = 28;
            this.labelFail.Text = "Fail items: ";
            this.labelFail.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(365, 153);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 29;
            this.button1.Text = "Save as csv";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(88, 43);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(81, 23);
            this.button2.TabIndex = 30;
            this.button2.Text = "Add Template";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // CsvReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(684, 671);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelFail);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.btnShow);
            this.Controls.Add(this.rBtnStation);
            this.Controls.Add(this.rBtnDate);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.cBoxNormal);
            this.Controls.Add(this.cBoxScatter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.searchText);
            this.Controls.Add(this.cListBoxDate);
            this.Controls.Add(this.cListBoxStation);
            this.Controls.Add(this.btnSaveAll);
            this.Controls.Add(this.cbBoxstatus);
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnDraw);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.chart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CsvReader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Smart Analysis Tool";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnDraw;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.ComboBox cbBoxstatus;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.Button btnSaveAll;
        private System.Windows.Forms.CheckedListBox cListBoxStation;
        private System.Windows.Forms.CheckedListBox cListBoxDate;
        private System.Windows.Forms.RichTextBox searchText;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cBoxScatter;
        private System.Windows.Forms.CheckBox cBoxNormal;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.RadioButton rBtnDate;
        private System.Windows.Forms.RadioButton rBtnStation;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.Label labelFail;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

