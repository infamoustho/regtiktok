namespace WindowsFormsApp2
{
    partial class Form1
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSelectEmailFile = new System.Windows.Forms.Button();
            this.txtEmails = new System.Windows.Forms.TextBox();
            this.labelEmails = new System.Windows.Forms.Label();
            this.labelExpressVpn = new System.Windows.Forms.Label();
            this.txtExpressVpn = new System.Windows.Forms.TextBox();
            this.labelVpnProvider = new System.Windows.Forms.Label();
            this.cboVpnProvider = new System.Windows.Forms.ComboBox();
            this.processText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnStopAll = new System.Windows.Forms.Button();
            this.dgvDevices = new System.Windows.Forms.DataGridView();
            this.colDevice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLog = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnReloadDevices = new System.Windows.Forms.Button();
            this.grpGmailVip = new System.Windows.Forms.GroupBox();
            this.lblGmailVipApiKey = new System.Windows.Forms.Label();
            this.txtGmailVipApiKey = new System.Windows.Forms.TextBox();
            this.lblGmailVipProduct = new System.Windows.Forms.Label();
            this.cboGmailVipProduct = new System.Windows.Forms.ComboBox();
            this.btnLoadProducts = new System.Windows.Forms.Button();
            this.lblBuyAmount = new System.Windows.Forms.Label();
            this.numBuyAmount = new System.Windows.Forms.NumericUpDown();
            this.chkAutoBuy = new System.Windows.Forms.CheckBox();
            this.lblGmailVipBalance = new System.Windows.Forms.Label();
            this.btnCheckBalance = new System.Windows.Forms.Button();
            this.btnBuyNow = new System.Windows.Forms.Button();
            this.lblAutoBuyStatus = new System.Windows.Forms.Label();
            this.lblGmailVipProductDetail = new System.Windows.Forms.Label();
            this.toolTipGmailVip = new System.Windows.Forms.ToolTip();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.tabGmailVip = new System.Windows.Forms.TabPage();
            this.tabGoogleSheet = new System.Windows.Forms.TabPage();
            this.grpGoogleSheet = new System.Windows.Forms.GroupBox();
            this.chkAutoSendSheet = new System.Windows.Forms.CheckBox();
            this.lblGoogleSheetUrl = new System.Windows.Forms.Label();
            this.txtGoogleSheetUrl = new System.Windows.Forms.TextBox();
            this.btnTestGoogleSheet = new System.Windows.Forms.Button();
            this.btnSaveSheetUrl = new System.Windows.Forms.Button();
            this.lblSheetTestStatus = new System.Windows.Forms.Label();
            this.lblSheetGuide = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevices)).BeginInit();
            this.grpGmailVip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyAmount)).BeginInit();
            this.grpGoogleSheet.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabGmailVip.SuspendLayout();
            this.tabGoogleSheet.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.button1.CausesValidation = false;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "▶ Start";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSelectEmailFile
            // 
            this.btnSelectEmailFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            this.btnSelectEmailFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelectEmailFile.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(210)))), ((int)(((byte)(254)))));
            this.btnSelectEmailFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectEmailFile.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnSelectEmailFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.btnSelectEmailFile.Location = new System.Drawing.Point(12, 80);
            this.btnSelectEmailFile.Name = "btnSelectEmailFile";
            this.btnSelectEmailFile.Size = new System.Drawing.Size(88, 30);
            this.btnSelectEmailFile.TabIndex = 24;
            this.btnSelectEmailFile.Text = "📁 File TXT";
            this.btnSelectEmailFile.UseVisualStyleBackColor = false;
            this.btnSelectEmailFile.Click += new System.EventHandler(this.btnSelectEmailFile_Click);
            // 
            // txtEmails
            // 
            this.txtEmails.BackColor = System.Drawing.Color.White;
            this.txtEmails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmails.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.txtEmails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.txtEmails.Location = new System.Drawing.Point(470, 32);
            this.txtEmails.Multiline = true;
            this.txtEmails.Name = "txtEmails";
            this.txtEmails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEmails.Size = new System.Drawing.Size(252, 308);
            this.txtEmails.TabIndex = 10;
            // 
            // labelEmails
            // 
            this.labelEmails.AutoSize = true;
            this.labelEmails.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelEmails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.labelEmails.Location = new System.Drawing.Point(467, 12);
            this.labelEmails.Name = "labelEmails";
            this.labelEmails.Size = new System.Drawing.Size(182, 15);
            this.labelEmails.TabIndex = 8;
            this.labelEmails.Text = "📧 Danh sách Email (email|pass):";
            // 
            // labelVpnProvider
            // 
            this.labelVpnProvider.AutoSize = true;
            this.labelVpnProvider.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelVpnProvider.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.labelVpnProvider.Location = new System.Drawing.Point(205, 10);
            this.labelVpnProvider.Name = "labelVpnProvider";
            this.labelVpnProvider.Size = new System.Drawing.Size(65, 15);
            this.labelVpnProvider.TabIndex = 26;
            this.labelVpnProvider.Text = "🌐 Loại VPN:";
            // 
            // cboVpnProvider
            // 
            this.cboVpnProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVpnProvider.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.cboVpnProvider.FormattingEnabled = true;
            this.cboVpnProvider.Items.AddRange(new object[] {
            "ExpressVPN",
            "HMA VPN"});
            this.cboVpnProvider.Location = new System.Drawing.Point(276, 7);
            this.cboVpnProvider.Name = "cboVpnProvider";
            this.cboVpnProvider.Size = new System.Drawing.Size(179, 21);
            this.cboVpnProvider.TabIndex = 27;
            this.cboVpnProvider.SelectedIndexChanged += new System.EventHandler(this.cboVpnProvider_SelectedIndexChanged);
            // 
            // labelExpressVpn
            // 
            this.labelExpressVpn.AutoSize = true;
            this.labelExpressVpn.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelExpressVpn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.labelExpressVpn.Location = new System.Drawing.Point(205, 34);
            this.labelExpressVpn.Name = "labelExpressVpn";
            this.labelExpressVpn.Size = new System.Drawing.Size(170, 15);
            this.labelExpressVpn.TabIndex = 11;
            this.labelExpressVpn.Text = "🔒 ExpressVPN (email|pass):";
            // 
            // txtExpressVpn
            // 
            this.txtExpressVpn.BackColor = System.Drawing.Color.White;
            this.txtExpressVpn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtExpressVpn.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.txtExpressVpn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.txtExpressVpn.Location = new System.Drawing.Point(205, 52);
            this.txtExpressVpn.Multiline = true;
            this.txtExpressVpn.Name = "txtExpressVpn";
            this.txtExpressVpn.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExpressVpn.Size = new System.Drawing.Size(250, 90);
            this.txtExpressVpn.TabIndex = 12;
            this.txtExpressVpn.TextChanged += new System.EventHandler(this.txtExpressVpn_TextChanged);
            // 
            // processText
            // 
            this.processText.BackColor = System.Drawing.Color.White;
            this.processText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.processText.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.processText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.processText.Location = new System.Drawing.Point(12, 368);
            this.processText.Multiline = true;
            this.processText.Name = "processText";
            this.processText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.processText.Size = new System.Drawing.Size(710, 75);
            this.processText.TabIndex = 13;
            this.processText.TextChanged += new System.EventHandler(this.processText_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.label4.Location = new System.Drawing.Point(12, 348);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(229, 15);
            this.label4.TabIndex = 16;
            this.label4.Text = "📋 Nhật ký hoạt động (Live Process Log):";
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.btnStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(105, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(88, 30);
            this.btnStop.TabIndex = 17;
            this.btnStop.Text = "⏸ Pause";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnContinue
            // 
            this.btnContinue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnContinue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnContinue.FlatAppearance.BorderSize = 0;
            this.btnContinue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnContinue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnContinue.ForeColor = System.Drawing.Color.White;
            this.btnContinue.Location = new System.Drawing.Point(105, 46);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(88, 30);
            this.btnContinue.TabIndex = 18;
            this.btnContinue.Text = "⏯ Tiếp tục";
            this.btnContinue.UseVisualStyleBackColor = false;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // btnStopAll
            // 
            this.btnStopAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnStopAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStopAll.FlatAppearance.BorderSize = 0;
            this.btnStopAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnStopAll.ForeColor = System.Drawing.Color.White;
            this.btnStopAll.Location = new System.Drawing.Point(12, 46);
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.Size = new System.Drawing.Size(88, 30);
            this.btnStopAll.TabIndex = 19;
            this.btnStopAll.Text = "⏹ Stop";
            this.btnStopAll.UseVisualStyleBackColor = false;
            this.btnStopAll.Click += new System.EventHandler(this.btnStopAll_Click);
            // 
            // dgvDevices
            // 
            this.dgvDevices.AllowUserToAddRows = false;
            this.dgvDevices.AllowUserToDeleteRows = false;
            this.dgvDevices.AllowUserToResizeRows = false;
            this.dgvDevices.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.dgvDevices.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(233)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(233)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDevices.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDevices.ColumnHeadersHeight = 24;
            this.dgvDevices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvDevices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDevice,
            this.colLog});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(132)))), ((int)(((byte)(199)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDevices.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvDevices.EnableHeadersVisualStyles = false;
            this.dgvDevices.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dgvDevices.Location = new System.Drawing.Point(12, 148);
            this.dgvDevices.MultiSelect = false;
            this.dgvDevices.Name = "dgvDevices";
            this.dgvDevices.ReadOnly = true;
            this.dgvDevices.RowHeadersVisible = false;
            this.dgvDevices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDevices.Size = new System.Drawing.Size(443, 192);
            this.dgvDevices.TabIndex = 21;
            // 
            // colDevice
            // 
            this.colDevice.HeaderText = "Device";
            this.colDevice.Name = "colDevice";
            this.colDevice.ReadOnly = true;
            this.colDevice.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colDevice.Width = 145;
            // 
            // colLog
            // 
            this.colLog.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colLog.HeaderText = "Log";
            this.colLog.Name = "colLog";
            this.colLog.ReadOnly = true;
            this.colLog.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnReset.Location = new System.Drawing.Point(105, 80);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(88, 30);
            this.btnReset.TabIndex = 22;
            this.btnReset.Text = "🔄 Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnReloadDevices
            // 
            this.btnReloadDevices.BackColor = System.Drawing.Color.White;
            this.btnReloadDevices.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReloadDevices.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnReloadDevices.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReloadDevices.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnReloadDevices.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.btnReloadDevices.Location = new System.Drawing.Point(12, 114);
            this.btnReloadDevices.Name = "btnReloadDevices";
            this.btnReloadDevices.Size = new System.Drawing.Size(181, 28);
            this.btnReloadDevices.TabIndex = 23;
            this.btnReloadDevices.Text = "📲 Load Devices";
            this.btnReloadDevices.UseVisualStyleBackColor = false;
            this.btnReloadDevices.Click += new System.EventHandler(this.btnReloadDevices_Click);
            // 
            // grpGmailVip
            // 
            this.grpGmailVip.Controls.Add(this.lblGmailVipProductDetail);
            this.grpGmailVip.Controls.Add(this.lblAutoBuyStatus);
            this.grpGmailVip.Controls.Add(this.btnBuyNow);
            this.grpGmailVip.Controls.Add(this.btnCheckBalance);
            this.grpGmailVip.Controls.Add(this.lblGmailVipBalance);
            this.grpGmailVip.Controls.Add(this.chkAutoBuy);
            this.grpGmailVip.Controls.Add(this.numBuyAmount);
            this.grpGmailVip.Controls.Add(this.lblBuyAmount);
            this.grpGmailVip.Controls.Add(this.btnLoadProducts);
            this.grpGmailVip.Controls.Add(this.cboGmailVipProduct);
            this.grpGmailVip.Controls.Add(this.lblGmailVipProduct);
            this.grpGmailVip.Controls.Add(this.txtGmailVipApiKey);
            this.grpGmailVip.Controls.Add(this.lblGmailVipApiKey);
            this.grpGmailVip.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpGmailVip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpGmailVip.Location = new System.Drawing.Point(12, 8);
            this.grpGmailVip.Name = "grpGmailVip";
            this.grpGmailVip.Size = new System.Drawing.Size(712, 432);
            this.grpGmailVip.TabIndex = 25;
            this.grpGmailVip.TabStop = false;
            this.grpGmailVip.Text = "🛒 GmailVIP Auto Buy";
            // 
            // lblGmailVipApiKey
            // 
            this.lblGmailVipApiKey.AutoSize = true;
            this.lblGmailVipApiKey.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular);
            this.lblGmailVipApiKey.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblGmailVipApiKey.Location = new System.Drawing.Point(15, 25);
            this.lblGmailVipApiKey.Name = "lblGmailVipApiKey";
            this.lblGmailVipApiKey.Size = new System.Drawing.Size(51, 15);
            this.lblGmailVipApiKey.TabIndex = 0;
            this.lblGmailVipApiKey.Text = "API Key:";
            // 
            // txtGmailVipApiKey
            // 
            this.txtGmailVipApiKey.BackColor = System.Drawing.Color.White;
            this.txtGmailVipApiKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGmailVipApiKey.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.txtGmailVipApiKey.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.txtGmailVipApiKey.Location = new System.Drawing.Point(15, 45);
            this.txtGmailVipApiKey.Name = "txtGmailVipApiKey";
            this.txtGmailVipApiKey.Size = new System.Drawing.Size(682, 21);
            this.txtGmailVipApiKey.TabIndex = 1;
            this.txtGmailVipApiKey.TextChanged += new System.EventHandler(this.txtGmailVipApiKey_TextChanged);
            // 
            // lblGmailVipProduct
            // 
            this.lblGmailVipProduct.AutoSize = true;
            this.lblGmailVipProduct.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular);
            this.lblGmailVipProduct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblGmailVipProduct.Location = new System.Drawing.Point(15, 73);
            this.lblGmailVipProduct.Name = "lblGmailVipProduct";
            this.lblGmailVipProduct.Size = new System.Drawing.Size(83, 15);
            this.lblGmailVipProduct.TabIndex = 2;
            this.lblGmailVipProduct.Text = "Gói email mua:";
            // 
            // cboGmailVipProduct
            // 
            this.cboGmailVipProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGmailVipProduct.DropDownWidth = 720;
            this.cboGmailVipProduct.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.cboGmailVipProduct.FormattingEnabled = true;
            this.cboGmailVipProduct.Location = new System.Drawing.Point(15, 93);
            this.cboGmailVipProduct.Name = "cboGmailVipProduct";
            this.cboGmailVipProduct.Size = new System.Drawing.Size(565, 21);
            this.cboGmailVipProduct.TabIndex = 3;
            this.cboGmailVipProduct.SelectedIndexChanged += new System.EventHandler(this.cboGmailVipProduct_SelectedIndexChanged);
            // 
            // lblGmailVipProductDetail
            // 
            this.lblGmailVipProductDetail.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblGmailVipProductDetail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(132)))), ((int)(((byte)(199)))));
            this.lblGmailVipProductDetail.Location = new System.Drawing.Point(15, 120);
            this.lblGmailVipProductDetail.Name = "lblGmailVipProductDetail";
            this.lblGmailVipProductDetail.Size = new System.Drawing.Size(682, 18);
            this.lblGmailVipProductDetail.TabIndex = 12;
            this.lblGmailVipProductDetail.Text = "💵 Giá: -- đ  |  📦 Kho: --";
            // 
            // btnLoadProducts
            // 
            this.btnLoadProducts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnLoadProducts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadProducts.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnLoadProducts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadProducts.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnLoadProducts.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnLoadProducts.Location = new System.Drawing.Point(588, 91);
            this.btnLoadProducts.Name = "btnLoadProducts";
            this.btnLoadProducts.Size = new System.Drawing.Size(109, 26);
            this.btnLoadProducts.TabIndex = 4;
            this.btnLoadProducts.Text = "📋 Tải gói";
            this.btnLoadProducts.UseVisualStyleBackColor = false;
            this.btnLoadProducts.Click += new System.EventHandler(this.btnLoadProducts_Click);
            // 
            // lblBuyAmount
            // 
            this.lblBuyAmount.AutoSize = true;
            this.lblBuyAmount.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular);
            this.lblBuyAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblBuyAmount.Location = new System.Drawing.Point(15, 146);
            this.lblBuyAmount.Name = "lblBuyAmount";
            this.lblBuyAmount.Size = new System.Drawing.Size(49, 15);
            this.lblBuyAmount.TabIndex = 5;
            this.lblBuyAmount.Text = "SL mua:";
            // 
            // numBuyAmount
            // 
            this.numBuyAmount.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.numBuyAmount.Location = new System.Drawing.Point(70, 143);
            this.numBuyAmount.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            this.numBuyAmount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numBuyAmount.Name = "numBuyAmount";
            this.numBuyAmount.Size = new System.Drawing.Size(70, 23);
            this.numBuyAmount.TabIndex = 6;
            this.numBuyAmount.Value = new decimal(new int[] { 5, 0, 0, 0 });
            this.numBuyAmount.ValueChanged += new System.EventHandler(this.numBuyAmount_ValueChanged);
            // 
            // chkAutoBuy
            // 
            this.chkAutoBuy.AutoSize = true;
            this.chkAutoBuy.Checked = true;
            this.chkAutoBuy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoBuy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoBuy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.chkAutoBuy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.chkAutoBuy.Location = new System.Drawing.Point(165, 145);
            this.chkAutoBuy.Name = "chkAutoBuy";
            this.chkAutoBuy.Size = new System.Drawing.Size(167, 19);
            this.chkAutoBuy.TabIndex = 7;
            this.chkAutoBuy.Text = "⚡ Tự mua khi hết email";
            this.chkAutoBuy.UseVisualStyleBackColor = true;
            this.chkAutoBuy.CheckedChanged += new System.EventHandler(this.chkAutoBuy_CheckedChanged);
            // 
            // lblGmailVipBalance
            // 
            this.lblGmailVipBalance.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblGmailVipBalance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblGmailVipBalance.Location = new System.Drawing.Point(15, 175);
            this.lblGmailVipBalance.Name = "lblGmailVipBalance";
            this.lblGmailVipBalance.Size = new System.Drawing.Size(682, 22);
            this.lblGmailVipBalance.TabIndex = 8;
            this.lblGmailVipBalance.Text = "💰 Số dư: ---";
            // 
            // btnCheckBalance
            // 
            this.btnCheckBalance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            this.btnCheckBalance.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCheckBalance.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(210)))), ((int)(((byte)(254)))));
            this.btnCheckBalance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckBalance.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnCheckBalance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.btnCheckBalance.Location = new System.Drawing.Point(15, 202);
            this.btnCheckBalance.Name = "btnCheckBalance";
            this.btnCheckBalance.Size = new System.Drawing.Size(335, 30);
            this.btnCheckBalance.TabIndex = 9;
            this.btnCheckBalance.Text = "💳 Kiểm tra số dư";
            this.btnCheckBalance.UseVisualStyleBackColor = false;
            this.btnCheckBalance.Click += new System.EventHandler(this.btnCheckBalance_Click);
            // 
            // btnBuyNow
            // 
            this.btnBuyNow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.btnBuyNow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBuyNow.FlatAppearance.BorderSize = 0;
            this.btnBuyNow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuyNow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBuyNow.ForeColor = System.Drawing.Color.White;
            this.btnBuyNow.Location = new System.Drawing.Point(362, 202);
            this.btnBuyNow.Name = "btnBuyNow";
            this.btnBuyNow.Size = new System.Drawing.Size(335, 30);
            this.btnBuyNow.TabIndex = 10;
            this.btnBuyNow.Text = "🛒 Mua ngay";
            this.btnBuyNow.UseVisualStyleBackColor = false;
            this.btnBuyNow.Click += new System.EventHandler(this.btnBuyNow_Click);
            // 
            // lblAutoBuyStatus
            // 
            this.lblAutoBuyStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblAutoBuyStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblAutoBuyStatus.Location = new System.Drawing.Point(15, 240);
            this.lblAutoBuyStatus.Name = "lblAutoBuyStatus";
            this.lblAutoBuyStatus.Size = new System.Drawing.Size(682, 178);
            this.lblAutoBuyStatus.TabIndex = 11;
            this.lblAutoBuyStatus.Text = "💡 Cơ chế Tự Mua:\r\nKhi bật tính năng này, nếu danh sách email trong tool còn ít hơn hoặc hết, tool sẽ tự động gọi API GmailVIP mua thêm email và nạp vào hàng đợi để các thiết bị chạy liên tục không bị dừng.";
            // 
            // grpGoogleSheet
            // 
            this.grpGoogleSheet.Controls.Add(this.lblSheetGuide);
            this.grpGoogleSheet.Controls.Add(this.lblSheetTestStatus);
            this.grpGoogleSheet.Controls.Add(this.btnSaveSheetUrl);
            this.grpGoogleSheet.Controls.Add(this.btnTestGoogleSheet);
            this.grpGoogleSheet.Controls.Add(this.txtGoogleSheetUrl);
            this.grpGoogleSheet.Controls.Add(this.lblGoogleSheetUrl);
            this.grpGoogleSheet.Controls.Add(this.chkAutoSendSheet);
            this.grpGoogleSheet.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpGoogleSheet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpGoogleSheet.Location = new System.Drawing.Point(12, 8);
            this.grpGoogleSheet.Name = "grpGoogleSheet";
            this.grpGoogleSheet.Size = new System.Drawing.Size(712, 432);
            this.grpGoogleSheet.TabIndex = 26;
            this.grpGoogleSheet.TabStop = false;
            this.grpGoogleSheet.Text = "📊 Google Sheet Auto Save";
            // 
            // chkAutoSendSheet
            // 
            this.chkAutoSendSheet.AutoSize = true;
            this.chkAutoSendSheet.Checked = true;
            this.chkAutoSendSheet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSendSheet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkAutoSendSheet.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.chkAutoSendSheet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.chkAutoSendSheet.Location = new System.Drawing.Point(15, 22);
            this.chkAutoSendSheet.Name = "chkAutoSendSheet";
            this.chkAutoSendSheet.Size = new System.Drawing.Size(260, 19);
            this.chkAutoSendSheet.TabIndex = 0;
            this.chkAutoSendSheet.Text = "⚡ Tự động ghi tài khoản lên Google Sheet";
            this.chkAutoSendSheet.UseVisualStyleBackColor = true;
            // 
            // lblGoogleSheetUrl
            // 
            this.lblGoogleSheetUrl.AutoSize = true;
            this.lblGoogleSheetUrl.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular);
            this.lblGoogleSheetUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblGoogleSheetUrl.Location = new System.Drawing.Point(15, 48);
            this.lblGoogleSheetUrl.Name = "lblGoogleSheetUrl";
            this.lblGoogleSheetUrl.Size = new System.Drawing.Size(154, 15);
            this.lblGoogleSheetUrl.TabIndex = 1;
            this.lblGoogleSheetUrl.Text = "Google Apps Script Webhook URL:";
            // 
            // txtGoogleSheetUrl
            // 
            this.txtGoogleSheetUrl.BackColor = System.Drawing.Color.White;
            this.txtGoogleSheetUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGoogleSheetUrl.Font = new System.Drawing.Font("Consolas", 8F);
            this.txtGoogleSheetUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.txtGoogleSheetUrl.Location = new System.Drawing.Point(15, 68);
            this.txtGoogleSheetUrl.Multiline = true;
            this.txtGoogleSheetUrl.Name = "txtGoogleSheetUrl";
            this.txtGoogleSheetUrl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGoogleSheetUrl.Size = new System.Drawing.Size(682, 55);
            this.txtGoogleSheetUrl.TabIndex = 2;
            // 
            // btnTestGoogleSheet
            // 
            this.btnTestGoogleSheet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            this.btnTestGoogleSheet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestGoogleSheet.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(210)))), ((int)(((byte)(254)))));
            this.btnTestGoogleSheet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestGoogleSheet.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnTestGoogleSheet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.btnTestGoogleSheet.Location = new System.Drawing.Point(15, 130);
            this.btnTestGoogleSheet.Name = "btnTestGoogleSheet";
            this.btnTestGoogleSheet.Size = new System.Drawing.Size(335, 30);
            this.btnTestGoogleSheet.TabIndex = 3;
            this.btnTestGoogleSheet.Text = "🧪 Gửi dòng Test";
            this.btnTestGoogleSheet.UseVisualStyleBackColor = false;
            this.btnTestGoogleSheet.Click += new System.EventHandler(this.btnTestGoogleSheet_Click);
            // 
            // btnSaveSheetUrl
            // 
            this.btnSaveSheetUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnSaveSheetUrl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveSheetUrl.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnSaveSheetUrl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveSheetUrl.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnSaveSheetUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnSaveSheetUrl.Location = new System.Drawing.Point(362, 130);
            this.btnSaveSheetUrl.Name = "btnSaveSheetUrl";
            this.btnSaveSheetUrl.Size = new System.Drawing.Size(335, 30);
            this.btnSaveSheetUrl.TabIndex = 4;
            this.btnSaveSheetUrl.Text = "💾 Lưu URL";
            this.btnSaveSheetUrl.UseVisualStyleBackColor = false;
            this.btnSaveSheetUrl.Click += new System.EventHandler(this.btnSaveSheetUrl_Click);
            // 
            // lblSheetTestStatus
            // 
            this.lblSheetTestStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSheetTestStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblSheetTestStatus.Location = new System.Drawing.Point(15, 166);
            this.lblSheetTestStatus.Name = "lblSheetTestStatus";
            this.lblSheetTestStatus.Size = new System.Drawing.Size(682, 25);
            this.lblSheetTestStatus.TabIndex = 5;
            this.lblSheetTestStatus.Text = "Chưa kết nối Webhook.";
            // 
            // lblSheetGuide
            // 
            this.lblSheetGuide.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSheetGuide.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblSheetGuide.Location = new System.Drawing.Point(15, 195);
            this.lblSheetGuide.Name = "lblSheetGuide";
            this.lblSheetGuide.Size = new System.Drawing.Size(682, 225);
            this.lblSheetGuide.TabIndex = 6;
            this.lblSheetGuide.Text = "📋 Cột dữ liệu tự động ghi lên Sheet:\r\n1. Email (Mail Google)\r\n2. Pass mail (Pass Google)\r\n3. User name (TikTok @username)\r\n4. Pass tiktok (Mật khẩu TikTok)\r\n5. 2FA (Secret Key 2-step)\r\n6. Acc Full (username|pass|2fa)\r\n7. Unlink (ok / no unlink)\r\n8. Thời gian tạo (yyyy-MM-dd HH:mm:ss)\r\n\r\n💡 Ghi chú:\r\n- Dữ liệu được gửi ngay sau bước Unlink email thành công.\r\n- Tool đồng thời luôn sao lưu đầy đủ vào file accounts_full.txt trong thư mục ứng dụng.";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabMain);
            this.tabControlMain.Controls.Add(this.tabGmailVip);
            this.tabControlMain.Controls.Add(this.tabGoogleSheet);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tabControlMain.ItemSize = new System.Drawing.Size(140, 28);
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(750, 495);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabMain
            // 
            this.tabMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tabMain.Controls.Add(this.button1);
            this.tabMain.Controls.Add(this.btnStop);
            this.tabMain.Controls.Add(this.btnStopAll);
            this.tabMain.Controls.Add(this.btnContinue);
            this.tabMain.Controls.Add(this.btnSelectEmailFile);
            this.tabMain.Controls.Add(this.btnReset);
            this.tabMain.Controls.Add(this.btnReloadDevices);
            this.tabMain.Controls.Add(this.labelVpnProvider);
            this.tabMain.Controls.Add(this.cboVpnProvider);
            this.tabMain.Controls.Add(this.labelExpressVpn);
            this.tabMain.Controls.Add(this.txtExpressVpn);
            this.tabMain.Controls.Add(this.labelEmails);
            this.tabMain.Controls.Add(this.txtEmails);
            this.tabMain.Controls.Add(this.dgvDevices);
            this.tabMain.Controls.Add(this.label4);
            this.tabMain.Controls.Add(this.processText);
            this.tabMain.Location = new System.Drawing.Point(4, 32);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain.Size = new System.Drawing.Size(742, 459);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "🎮 Điều khiển chính";
            // 
            // tabGmailVip
            // 
            this.tabGmailVip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tabGmailVip.Controls.Add(this.grpGmailVip);
            this.tabGmailVip.Location = new System.Drawing.Point(4, 32);
            this.tabGmailVip.Name = "tabGmailVip";
            this.tabGmailVip.Padding = new System.Windows.Forms.Padding(3);
            this.tabGmailVip.Size = new System.Drawing.Size(742, 459);
            this.tabGmailVip.TabIndex = 1;
            this.tabGmailVip.Text = "🛒 Mua mail (GmailVIP)";
            // 
            // tabGoogleSheet
            // 
            this.tabGoogleSheet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tabGoogleSheet.Controls.Add(this.grpGoogleSheet);
            this.tabGoogleSheet.Location = new System.Drawing.Point(4, 32);
            this.tabGoogleSheet.Name = "tabGoogleSheet";
            this.tabGoogleSheet.Padding = new System.Windows.Forms.Padding(3);
            this.tabGoogleSheet.Size = new System.Drawing.Size(742, 459);
            this.tabGoogleSheet.TabIndex = 2;
            this.tabGoogleSheet.Text = "📊 Cấu hình Google Sheet";
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(750, 495);
            this.Controls.Add(this.tabControlMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(760, 520);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TikTok Automation Pro - Multi-Device & 2FA Manager";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevices)).EndInit();
            this.grpGmailVip.ResumeLayout(false);
            this.grpGmailVip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBuyAmount)).EndInit();
            this.grpGoogleSheet.ResumeLayout(false);
            this.grpGoogleSheet.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabMain.PerformLayout();
            this.tabGmailVip.ResumeLayout(false);
            this.tabGoogleSheet.ResumeLayout(false);
            this.tabControlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSelectEmailFile;
        private System.Windows.Forms.Label labelEmails;
        private System.Windows.Forms.TextBox txtEmails;
        private System.Windows.Forms.Label labelExpressVpn;
        private System.Windows.Forms.TextBox txtExpressVpn;
        private System.Windows.Forms.Label labelVpnProvider;
        private System.Windows.Forms.ComboBox cboVpnProvider;
        private System.Windows.Forms.TextBox processText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Button btnStopAll;
        private System.Windows.Forms.DataGridView dgvDevices;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDevice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLog;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnReloadDevices;
        private System.Windows.Forms.GroupBox grpGmailVip;
        private System.Windows.Forms.Label lblGmailVipApiKey;
        private System.Windows.Forms.TextBox txtGmailVipApiKey;
        private System.Windows.Forms.Label lblGmailVipProduct;
        private System.Windows.Forms.ComboBox cboGmailVipProduct;
        private System.Windows.Forms.Button btnLoadProducts;
        private System.Windows.Forms.Label lblBuyAmount;
        private System.Windows.Forms.NumericUpDown numBuyAmount;
        private System.Windows.Forms.CheckBox chkAutoBuy;
        private System.Windows.Forms.Label lblGmailVipBalance;
        private System.Windows.Forms.Button btnCheckBalance;
        private System.Windows.Forms.Button btnBuyNow;
        private System.Windows.Forms.Label lblAutoBuyStatus;
        private System.Windows.Forms.Label lblGmailVipProductDetail;
        private System.Windows.Forms.ToolTip toolTipGmailVip;
        private System.Windows.Forms.GroupBox grpGoogleSheet;
        private System.Windows.Forms.CheckBox chkAutoSendSheet;
        private System.Windows.Forms.Label lblGoogleSheetUrl;
        private System.Windows.Forms.TextBox txtGoogleSheetUrl;
        private System.Windows.Forms.Button btnTestGoogleSheet;
        private System.Windows.Forms.Button btnSaveSheetUrl;
        private System.Windows.Forms.Label lblSheetTestStatus;
        private System.Windows.Forms.Label lblSheetGuide;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabMain;
        private System.Windows.Forms.TabPage tabGmailVip;
        private System.Windows.Forms.TabPage tabGoogleSheet;
    }
}

