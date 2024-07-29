﻿
namespace PowerSet.Models
{
    partial class MainForm
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.SplitLayout = new System.Windows.Forms.SplitContainer();
			this.Chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.label3 = new System.Windows.Forms.Label();
			this.CloseBtn = new System.Windows.Forms.Button();
			this.HiddenChart = new System.Windows.Forms.Button();
			this.XAxisMax_Val = new System.Windows.Forms.NumericUpDown();
			this.XAxisMax_Lab = new System.Windows.Forms.Label();
			this.YAxisMax_Lab = new System.Windows.Forms.Label();
			this.YAxisMargin_Lab = new System.Windows.Forms.Label();
			this.XAxisMargin_Lab = new System.Windows.Forms.Label();
			this.XAxisMargin_Val = new System.Windows.Forms.NumericUpDown();
			this.YAxisMax_Val = new System.Windows.Forms.NumericUpDown();
			this.YAxisMargin_Val = new System.Windows.Forms.NumericUpDown();
			this.Start_Btn = new System.Windows.Forms.Button();
			this.End_Btn = new System.Windows.Forms.Button();
			this.Set_Btn = new System.Windows.Forms.Button();
			this.History_Btn = new System.Windows.Forms.Button();
			this.Saven_Btn = new System.Windows.Forms.Button();
			this.BTN = new System.Windows.Forms.Button();
			this.ChartHight_Val = new System.Windows.Forms.NumericUpDown();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.KEndProcess_Num = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.KTubeNum_Label = new System.Windows.Forms.Label();
			this.KI_Val = new System.Windows.Forms.Label();
			this.KCurrentI_Lab = new System.Windows.Forms.Label();
			this.KI_Lab = new System.Windows.Forms.Label();
			this.KTube_Val = new System.Windows.Forms.TextBox();
			this.KParamSetTable = new System.Windows.Forms.DataGridView();
			this.KAddProcessBtn = new System.Windows.Forms.Button();
			this.KStartProcess_Num = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.SplitLayout)).BeginInit();
			this.SplitLayout.Panel1.SuspendLayout();
			this.SplitLayout.Panel2.SuspendLayout();
			this.SplitLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Chart)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.XAxisMax_Val)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.XAxisMargin_Val)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.YAxisMax_Val)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.YAxisMargin_Val)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ChartHight_Val)).BeginInit();
			this.tableLayoutPanel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.KEndProcess_Num)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.KParamSetTable)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.KStartProcess_Num)).BeginInit();
			this.SuspendLayout();
			// 
			// SplitLayout
			// 
			this.SplitLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SplitLayout.IsSplitterFixed = true;
			this.SplitLayout.Location = new System.Drawing.Point(0, 0);
			this.SplitLayout.Margin = new System.Windows.Forms.Padding(0);
			this.SplitLayout.Name = "SplitLayout";
			this.SplitLayout.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// SplitLayout.Panel1
			// 
			this.SplitLayout.Panel1.Controls.Add(this.Chart);
			this.SplitLayout.Panel1.Margin = new System.Windows.Forms.Padding(3);
			// 
			// SplitLayout.Panel2
			// 
			this.SplitLayout.Panel2.Controls.Add(this.tableLayoutPanel1);
			this.SplitLayout.Size = new System.Drawing.Size(1460, 1100);
			this.SplitLayout.SplitterDistance = 80;
			this.SplitLayout.TabIndex = 0;
			// 
			// Chart
			// 
			this.Chart.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Chart.Location = new System.Drawing.Point(0, 0);
			this.Chart.Name = "Chart";
			this.Chart.Size = new System.Drawing.Size(1460, 80);
			this.Chart.TabIndex = 1;
			this.Chart.Text = "Chart";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1460, 1016);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.tableLayoutPanel2.ColumnCount = 10;
			this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 2);
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel2.Controls.Add(this.label3, 4, 0);
			this.tableLayoutPanel2.Controls.Add(this.CloseBtn, 6, 0);
			this.tableLayoutPanel2.Controls.Add(this.HiddenChart, 6, 1);
			this.tableLayoutPanel2.Controls.Add(this.XAxisMax_Val, 3, 0);
			this.tableLayoutPanel2.Controls.Add(this.XAxisMax_Lab, 2, 0);
			this.tableLayoutPanel2.Controls.Add(this.YAxisMax_Lab, 2, 1);
			this.tableLayoutPanel2.Controls.Add(this.YAxisMargin_Lab, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.XAxisMargin_Lab, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.XAxisMargin_Val, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.YAxisMax_Val, 3, 1);
			this.tableLayoutPanel2.Controls.Add(this.YAxisMargin_Val, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.Start_Btn, 9, 0);
			this.tableLayoutPanel2.Controls.Add(this.End_Btn, 9, 1);
			this.tableLayoutPanel2.Controls.Add(this.Set_Btn, 8, 0);
			this.tableLayoutPanel2.Controls.Add(this.History_Btn, 8, 1);
			this.tableLayoutPanel2.Controls.Add(this.Saven_Btn, 7, 0);
			this.tableLayoutPanel2.Controls.Add(this.BTN, 7, 1);
			this.tableLayoutPanel2.Controls.Add(this.ChartHight_Val, 5, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(1454, 94);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label3.Location = new System.Drawing.Point(584, 1);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(138, 45);
			this.label3.TabIndex = 14;
			this.label3.Text = "图表高度：";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// CloseBtn
			// 
			this.CloseBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CloseBtn.BackColor = System.Drawing.Color.Transparent;
			this.CloseBtn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CloseBtn.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.CloseBtn.ForeColor = System.Drawing.Color.Red;
			this.CloseBtn.Location = new System.Drawing.Point(874, 4);
			this.CloseBtn.Name = "CloseBtn";
			this.CloseBtn.Size = new System.Drawing.Size(138, 39);
			this.CloseBtn.TabIndex = 0;
			this.CloseBtn.Text = "关闭程序";
			this.CloseBtn.UseVisualStyleBackColor = false;
			this.CloseBtn.Click += new System.EventHandler(this.CloseSysEvent);
			// 
			// HiddenChart
			// 
			this.HiddenChart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.HiddenChart.BackColor = System.Drawing.Color.Transparent;
			this.HiddenChart.Dock = System.Windows.Forms.DockStyle.Fill;
			this.HiddenChart.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.HiddenChart.ForeColor = System.Drawing.Color.DarkOrange;
			this.HiddenChart.Location = new System.Drawing.Point(874, 50);
			this.HiddenChart.Name = "HiddenChart";
			this.HiddenChart.Size = new System.Drawing.Size(138, 40);
			this.HiddenChart.TabIndex = 2;
			this.HiddenChart.Text = "隐藏折线图";
			this.HiddenChart.UseVisualStyleBackColor = false;
			this.HiddenChart.Click += new System.EventHandler(this.HiddenChartClick);
			// 
			// XAxisMax_Val
			// 
			this.XAxisMax_Val.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.XAxisMax_Val.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.XAxisMax_Val.Location = new System.Drawing.Point(439, 9);
			this.XAxisMax_Val.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
			this.XAxisMax_Val.Name = "XAxisMax_Val";
			this.XAxisMax_Val.Size = new System.Drawing.Size(138, 29);
			this.XAxisMax_Val.TabIndex = 5;
			this.XAxisMax_Val.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// XAxisMax_Lab
			// 
			this.XAxisMax_Lab.AutoSize = true;
			this.XAxisMax_Lab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.XAxisMax_Lab.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.XAxisMax_Lab.Location = new System.Drawing.Point(294, 1);
			this.XAxisMax_Lab.Name = "XAxisMax_Lab";
			this.XAxisMax_Lab.Size = new System.Drawing.Size(138, 45);
			this.XAxisMax_Lab.TabIndex = 3;
			this.XAxisMax_Lab.Text = "X轴最大值：";
			this.XAxisMax_Lab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// YAxisMax_Lab
			// 
			this.YAxisMax_Lab.AutoSize = true;
			this.YAxisMax_Lab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.YAxisMax_Lab.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.YAxisMax_Lab.Location = new System.Drawing.Point(294, 47);
			this.YAxisMax_Lab.Name = "YAxisMax_Lab";
			this.YAxisMax_Lab.Size = new System.Drawing.Size(138, 46);
			this.YAxisMax_Lab.TabIndex = 2;
			this.YAxisMax_Lab.Text = "Y轴最大值：";
			this.YAxisMax_Lab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// YAxisMargin_Lab
			// 
			this.YAxisMargin_Lab.AutoSize = true;
			this.YAxisMargin_Lab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.YAxisMargin_Lab.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.YAxisMargin_Lab.Location = new System.Drawing.Point(4, 47);
			this.YAxisMargin_Lab.Name = "YAxisMargin_Lab";
			this.YAxisMargin_Lab.Size = new System.Drawing.Size(138, 46);
			this.YAxisMargin_Lab.TabIndex = 1;
			this.YAxisMargin_Lab.Text = "Y轴间距：";
			this.YAxisMargin_Lab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// XAxisMargin_Lab
			// 
			this.XAxisMargin_Lab.AutoSize = true;
			this.XAxisMargin_Lab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.XAxisMargin_Lab.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.XAxisMargin_Lab.Location = new System.Drawing.Point(4, 1);
			this.XAxisMargin_Lab.Name = "XAxisMargin_Lab";
			this.XAxisMargin_Lab.Size = new System.Drawing.Size(138, 45);
			this.XAxisMargin_Lab.TabIndex = 0;
			this.XAxisMargin_Lab.Text = "X轴间距：";
			this.XAxisMargin_Lab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// XAxisMargin_Val
			// 
			this.XAxisMargin_Val.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.XAxisMargin_Val.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.XAxisMargin_Val.Location = new System.Drawing.Point(149, 9);
			this.XAxisMargin_Val.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
			this.XAxisMargin_Val.Name = "XAxisMargin_Val";
			this.XAxisMargin_Val.Size = new System.Drawing.Size(138, 29);
			this.XAxisMargin_Val.TabIndex = 4;
			this.XAxisMargin_Val.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// YAxisMax_Val
			// 
			this.YAxisMax_Val.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.YAxisMax_Val.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.YAxisMax_Val.Location = new System.Drawing.Point(439, 55);
			this.YAxisMax_Val.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
			this.YAxisMax_Val.Name = "YAxisMax_Val";
			this.YAxisMax_Val.Size = new System.Drawing.Size(138, 29);
			this.YAxisMax_Val.TabIndex = 6;
			this.YAxisMax_Val.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// YAxisMargin_Val
			// 
			this.YAxisMargin_Val.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.YAxisMargin_Val.DecimalPlaces = 1;
			this.YAxisMargin_Val.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.YAxisMargin_Val.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.YAxisMargin_Val.Location = new System.Drawing.Point(149, 55);
			this.YAxisMargin_Val.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
			this.YAxisMargin_Val.Name = "YAxisMargin_Val";
			this.YAxisMargin_Val.Size = new System.Drawing.Size(138, 29);
			this.YAxisMargin_Val.TabIndex = 7;
			this.YAxisMargin_Val.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Start_Btn
			// 
			this.Start_Btn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Start_Btn.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Start_Btn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Start_Btn.Location = new System.Drawing.Point(1309, 4);
			this.Start_Btn.Name = "Start_Btn";
			this.Start_Btn.Size = new System.Drawing.Size(141, 39);
			this.Start_Btn.TabIndex = 8;
			this.Start_Btn.Text = "开始";
			this.Start_Btn.UseVisualStyleBackColor = true;
			// 
			// End_Btn
			// 
			this.End_Btn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.End_Btn.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.End_Btn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.End_Btn.Location = new System.Drawing.Point(1309, 50);
			this.End_Btn.Name = "End_Btn";
			this.End_Btn.Size = new System.Drawing.Size(141, 40);
			this.End_Btn.TabIndex = 9;
			this.End_Btn.Text = "结束";
			this.End_Btn.UseVisualStyleBackColor = true;
			// 
			// Set_Btn
			// 
			this.Set_Btn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Set_Btn.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Set_Btn.Location = new System.Drawing.Point(1164, 4);
			this.Set_Btn.Name = "Set_Btn";
			this.Set_Btn.Size = new System.Drawing.Size(138, 39);
			this.Set_Btn.TabIndex = 10;
			this.Set_Btn.Text = "设置";
			this.Set_Btn.UseVisualStyleBackColor = true;
			// 
			// History_Btn
			// 
			this.History_Btn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.History_Btn.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.History_Btn.Location = new System.Drawing.Point(1164, 50);
			this.History_Btn.Name = "History_Btn";
			this.History_Btn.Size = new System.Drawing.Size(138, 40);
			this.History_Btn.TabIndex = 11;
			this.History_Btn.Text = "历史记录";
			this.History_Btn.UseVisualStyleBackColor = true;
			// 
			// Saven_Btn
			// 
			this.Saven_Btn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Saven_Btn.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Saven_Btn.Location = new System.Drawing.Point(1019, 4);
			this.Saven_Btn.Name = "Saven_Btn";
			this.Saven_Btn.Size = new System.Drawing.Size(138, 39);
			this.Saven_Btn.TabIndex = 12;
			this.Saven_Btn.Text = "保存";
			this.Saven_Btn.UseVisualStyleBackColor = true;
			// 
			// BTN
			// 
			this.BTN.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BTN.Enabled = false;
			this.BTN.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.BTN.Location = new System.Drawing.Point(1019, 50);
			this.BTN.Name = "BTN";
			this.BTN.Size = new System.Drawing.Size(138, 40);
			this.BTN.TabIndex = 13;
			this.BTN.UseVisualStyleBackColor = true;
			// 
			// ChartHight_Val
			// 
			this.ChartHight_Val.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.ChartHight_Val.Location = new System.Drawing.Point(729, 7);
			this.ChartHight_Val.Maximum = new decimal(new int[] {
            85,
            0,
            0,
            0});
			this.ChartHight_Val.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.ChartHight_Val.Name = "ChartHight_Val";
			this.ChartHight_Val.Size = new System.Drawing.Size(138, 33);
			this.ChartHight_Val.TabIndex = 15;
			this.ChartHight_Val.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.ChartHight_Val.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.ChartHight_Val.ValueChanged += new System.EventHandler(this.ChartHight_Val_Changed);
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.tableLayoutPanel3.ColumnCount = 7;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
			this.tableLayoutPanel3.Controls.Add(this.KEndProcess_Num, 5, 1);
			this.tableLayoutPanel3.Controls.Add(this.label2, 3, 1);
			this.tableLayoutPanel3.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.KTubeNum_Label, 3, 0);
			this.tableLayoutPanel3.Controls.Add(this.KI_Val, 2, 0);
			this.tableLayoutPanel3.Controls.Add(this.KCurrentI_Lab, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.KI_Lab, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.KTube_Val, 4, 0);
			this.tableLayoutPanel3.Controls.Add(this.KParamSetTable, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this.KAddProcessBtn, 6, 0);
			this.tableLayoutPanel3.Controls.Add(this.KStartProcess_Num, 2, 1);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 103);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 3;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(724, 452);
			this.tableLayoutPanel3.TabIndex = 1;
			// 
			// KEndProcess_Num
			// 
			this.KEndProcess_Num.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.KEndProcess_Num.Location = new System.Drawing.Point(519, 45);
			this.KEndProcess_Num.Name = "KEndProcess_Num";
			this.KEndProcess_Num.Size = new System.Drawing.Size(96, 33);
			this.KEndProcess_Num.TabIndex = 10;
			this.KEndProcess_Num.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.SystemColors.Control;
			this.tableLayoutPanel3.SetColumnSpan(this.label2, 2);
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label2.Location = new System.Drawing.Point(310, 42);
			this.label2.Margin = new System.Windows.Forms.Padding(0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(205, 40);
			this.label2.TabIndex = 8;
			this.label2.Text = "结束周期";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.SystemColors.Control;
			this.tableLayoutPanel3.SetColumnSpan(this.label1, 2);
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label1.Location = new System.Drawing.Point(1, 42);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(205, 40);
			this.label1.TabIndex = 7;
			this.label1.Text = "开始周期";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// KTubeNum_Label
			// 
			this.KTubeNum_Label.AutoSize = true;
			this.KTubeNum_Label.BackColor = System.Drawing.SystemColors.Control;
			this.KTubeNum_Label.Dock = System.Windows.Forms.DockStyle.Fill;
			this.KTubeNum_Label.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.KTubeNum_Label.Location = new System.Drawing.Point(310, 1);
			this.KTubeNum_Label.Margin = new System.Windows.Forms.Padding(0);
			this.KTubeNum_Label.Name = "KTubeNum_Label";
			this.KTubeNum_Label.Size = new System.Drawing.Size(102, 40);
			this.KTubeNum_Label.TabIndex = 3;
			this.KTubeNum_Label.Text = "管号";
			this.KTubeNum_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// KI_Val
			// 
			this.KI_Val.AutoSize = true;
			this.KI_Val.BackColor = System.Drawing.SystemColors.Control;
			this.KI_Val.Dock = System.Windows.Forms.DockStyle.Fill;
			this.KI_Val.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.KI_Val.Location = new System.Drawing.Point(207, 1);
			this.KI_Val.Margin = new System.Windows.Forms.Padding(0);
			this.KI_Val.Name = "KI_Val";
			this.KI_Val.Size = new System.Drawing.Size(102, 40);
			this.KI_Val.TabIndex = 2;
			this.KI_Val.Text = "0";
			this.KI_Val.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// KCurrentI_Lab
			// 
			this.KCurrentI_Lab.AutoSize = true;
			this.KCurrentI_Lab.BackColor = System.Drawing.SystemColors.Control;
			this.KCurrentI_Lab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.KCurrentI_Lab.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.KCurrentI_Lab.Location = new System.Drawing.Point(104, 1);
			this.KCurrentI_Lab.Margin = new System.Windows.Forms.Padding(0);
			this.KCurrentI_Lab.Name = "KCurrentI_Lab";
			this.KCurrentI_Lab.Size = new System.Drawing.Size(102, 40);
			this.KCurrentI_Lab.TabIndex = 1;
			this.KCurrentI_Lab.Text = "电流(mA)";
			this.KCurrentI_Lab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// KI_Lab
			// 
			this.KI_Lab.AutoSize = true;
			this.KI_Lab.BackColor = System.Drawing.Color.LightGray;
			this.KI_Lab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.KI_Lab.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.KI_Lab.Location = new System.Drawing.Point(1, 1);
			this.KI_Lab.Margin = new System.Windows.Forms.Padding(0);
			this.KI_Lab.Name = "KI_Lab";
			this.KI_Lab.Size = new System.Drawing.Size(102, 40);
			this.KI_Lab.TabIndex = 0;
			this.KI_Lab.Text = "K　电源";
			this.KI_Lab.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// KTube_Val
			// 
			this.KTube_Val.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel3.SetColumnSpan(this.KTube_Val, 2);
			this.KTube_Val.Location = new System.Drawing.Point(413, 1);
			this.KTube_Val.Margin = new System.Windows.Forms.Padding(0);
			this.KTube_Val.MaxLength = 12;
			this.KTube_Val.Multiline = true;
			this.KTube_Val.Name = "KTube_Val";
			this.KTube_Val.Size = new System.Drawing.Size(205, 40);
			this.KTube_Val.TabIndex = 4;
			this.KTube_Val.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// KParamSetTable
			// 
			this.KParamSetTable.AllowUserToAddRows = false;
			this.KParamSetTable.AllowUserToDeleteRows = false;
			this.KParamSetTable.BackgroundColor = System.Drawing.Color.Gainsboro;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.KParamSetTable.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.KParamSetTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.tableLayoutPanel3.SetColumnSpan(this.KParamSetTable, 7);
			this.KParamSetTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.KParamSetTable.Location = new System.Drawing.Point(1, 83);
			this.KParamSetTable.Margin = new System.Windows.Forms.Padding(0);
			this.KParamSetTable.Name = "KParamSetTable";
			this.KParamSetTable.RowHeadersVisible = false;
			this.KParamSetTable.RowTemplate.Height = 23;
			this.KParamSetTable.Size = new System.Drawing.Size(722, 368);
			this.KParamSetTable.TabIndex = 5;
			// 
			// KAddProcessBtn
			// 
			this.KAddProcessBtn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.KAddProcessBtn.Location = new System.Drawing.Point(619, 1);
			this.KAddProcessBtn.Margin = new System.Windows.Forms.Padding(0);
			this.KAddProcessBtn.Name = "KAddProcessBtn";
			this.tableLayoutPanel3.SetRowSpan(this.KAddProcessBtn, 2);
			this.KAddProcessBtn.Size = new System.Drawing.Size(104, 81);
			this.KAddProcessBtn.TabIndex = 6;
			this.KAddProcessBtn.Text = "添加周期";
			this.KAddProcessBtn.UseVisualStyleBackColor = true;
			this.KAddProcessBtn.Click += new System.EventHandler(this.KAddProcessBtn_Click);
			// 
			// KStartProcess_Num
			// 
			this.KStartProcess_Num.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.KStartProcess_Num.Location = new System.Drawing.Point(210, 45);
			this.KStartProcess_Num.Name = "KStartProcess_Num";
			this.KStartProcess_Num.Size = new System.Drawing.Size(96, 33);
			this.KStartProcess_Num.TabIndex = 9;
			this.KStartProcess_Num.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1460, 1100);
			this.ControlBox = false;
			this.Controls.Add(this.SplitLayout);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MainForm";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.SplitLayout.Panel1.ResumeLayout(false);
			this.SplitLayout.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.SplitLayout)).EndInit();
			this.SplitLayout.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Chart)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.XAxisMax_Val)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.XAxisMargin_Val)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.YAxisMax_Val)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.YAxisMargin_Val)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ChartHight_Val)).EndInit();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.KEndProcess_Num)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.KParamSetTable)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.KStartProcess_Num)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer SplitLayout;
        private System.Windows.Forms.Button CloseBtn;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart;
        private System.Windows.Forms.Button HiddenChart;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label XAxisMargin_Lab;
        private System.Windows.Forms.Label YAxisMargin_Lab;
        private System.Windows.Forms.Label XAxisMax_Lab;
        private System.Windows.Forms.Label YAxisMax_Lab;
        private System.Windows.Forms.NumericUpDown XAxisMax_Val;
        private System.Windows.Forms.NumericUpDown XAxisMargin_Val;
        private System.Windows.Forms.NumericUpDown YAxisMax_Val;
        private System.Windows.Forms.NumericUpDown YAxisMargin_Val;
        private System.Windows.Forms.Button Start_Btn;
        private System.Windows.Forms.Button End_Btn;
        private System.Windows.Forms.Button Set_Btn;
        private System.Windows.Forms.Button History_Btn;
        private System.Windows.Forms.Button Saven_Btn;
        private System.Windows.Forms.Button BTN;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label KI_Lab;
        private System.Windows.Forms.Label KCurrentI_Lab;
        private System.Windows.Forms.Label KI_Val;
        private System.Windows.Forms.Label KTubeNum_Label;
        private System.Windows.Forms.TextBox KTube_Val;
        private System.Windows.Forms.DataGridView KParamSetTable;
        private System.Windows.Forms.Button KAddProcessBtn;
        private System.Windows.Forms.NumericUpDown KEndProcess_Num;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown KStartProcess_Num;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown ChartHight_Val;
	}
}