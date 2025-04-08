namespace piratechess_Winform
{
    partial class PirateChess
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PirateChess));
            buttonParseAll = new Button();
            textBoxPGN = new TextBox();
            labelDurchlauf = new Label();
            textBoxDurchlauf = new TextBox();
            label4 = new Label();
            textBoxCurLines = new TextBox();
            buttonFirstTenLines = new Button();
            label5 = new Label();
            textBoxCumulativeLines = new TextBox();
            buttonSavePNG = new Button();
            textBoxPwd = new TextBox();
            labelPwd = new Label();
            labelEmail = new Label();
            textBoxEmail = new TextBox();
            buttonLogin = new Button();
            radioButtonLogin = new RadioButton();
            radioButtonBearer = new RadioButton();
            buttonLoadChapters = new Button();
            comboBoxChapters = new ComboBox();
            SuspendLayout();
            // 
            // buttonParseAll
            // 
            buttonParseAll.Enabled = false;
            buttonParseAll.Location = new Point(62, 555);
            buttonParseAll.Name = "buttonParseAll";
            buttonParseAll.Size = new Size(293, 29);
            buttonParseAll.TabIndex = 0;
            buttonParseAll.Text = "complete course";
            buttonParseAll.UseVisualStyleBackColor = true;
            buttonParseAll.Click += Button1_Click;
            // 
            // textBoxPGN
            // 
            textBoxPGN.Location = new Point(676, 56);
            textBoxPGN.Multiline = true;
            textBoxPGN.Name = "textBoxPGN";
            textBoxPGN.Size = new Size(309, 268);
            textBoxPGN.TabIndex = 1;
            // 
            // labelDurchlauf
            // 
            labelDurchlauf.AutoSize = true;
            labelDurchlauf.Location = new Point(46, 269);
            labelDurchlauf.Name = "labelDurchlauf";
            labelDurchlauf.Size = new Size(59, 20);
            labelDurchlauf.TabIndex = 13;
            labelDurchlauf.Text = "chapter";
            // 
            // textBoxDurchlauf
            // 
            textBoxDurchlauf.Enabled = false;
            textBoxDurchlauf.Location = new Point(126, 269);
            textBoxDurchlauf.Name = "textBoxDurchlauf";
            textBoxDurchlauf.Size = new Size(125, 27);
            textBoxDurchlauf.TabIndex = 12;
            textBoxDurchlauf.Text = "0";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(46, 304);
            label4.Name = "label4";
            label4.Size = new Size(39, 20);
            label4.TabIndex = 16;
            label4.Text = "lines";
            // 
            // textBoxCurLines
            // 
            textBoxCurLines.AccessibleRole = AccessibleRole.None;
            textBoxCurLines.Enabled = false;
            textBoxCurLines.Location = new Point(126, 304);
            textBoxCurLines.Name = "textBoxCurLines";
            textBoxCurLines.Size = new Size(125, 27);
            textBoxCurLines.TabIndex = 15;
            textBoxCurLines.Text = "0";
            // 
            // buttonFirstTenLines
            // 
            buttonFirstTenLines.Enabled = false;
            buttonFirstTenLines.Location = new Point(62, 519);
            buttonFirstTenLines.Name = "buttonFirstTenLines";
            buttonFirstTenLines.Size = new Size(293, 29);
            buttonFirstTenLines.TabIndex = 17;
            buttonFirstTenLines.Text = "First 10 lines";
            buttonFirstTenLines.UseVisualStyleBackColor = true;
            buttonFirstTenLines.Click += ButtonFirstTenLines_Click_1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(46, 337);
            label5.Name = "label5";
            label5.Size = new Size(74, 20);
            label5.TabIndex = 19;
            label5.Text = "total lines";
            // 
            // textBoxCumulativeLines
            // 
            textBoxCumulativeLines.AccessibleRole = AccessibleRole.None;
            textBoxCumulativeLines.Enabled = false;
            textBoxCumulativeLines.Location = new Point(126, 337);
            textBoxCumulativeLines.Name = "textBoxCumulativeLines";
            textBoxCumulativeLines.Size = new Size(125, 27);
            textBoxCumulativeLines.TabIndex = 18;
            textBoxCumulativeLines.Text = "0";
            // 
            // buttonSavePNG
            // 
            buttonSavePNG.Location = new Point(799, 380);
            buttonSavePNG.Name = "buttonSavePNG";
            buttonSavePNG.Size = new Size(94, 29);
            buttonSavePNG.TabIndex = 20;
            buttonSavePNG.Text = "Save";
            buttonSavePNG.UseVisualStyleBackColor = true;
            buttonSavePNG.Click += ButtonSavePNG_Click;
            // 
            // textBoxPwd
            // 
            textBoxPwd.Location = new Point(126, 177);
            textBoxPwd.Name = "textBoxPwd";
            textBoxPwd.Size = new Size(125, 27);
            textBoxPwd.TabIndex = 24;
            // 
            // labelPwd
            // 
            labelPwd.AutoSize = true;
            labelPwd.Location = new Point(46, 177);
            labelPwd.Name = "labelPwd";
            labelPwd.Size = new Size(70, 20);
            labelPwd.TabIndex = 23;
            labelPwd.Text = "Password";
            // 
            // labelEmail
            // 
            labelEmail.AutoSize = true;
            labelEmail.Location = new Point(46, 141);
            labelEmail.Name = "labelEmail";
            labelEmail.Size = new Size(46, 20);
            labelEmail.TabIndex = 22;
            labelEmail.Text = "Email";
            // 
            // textBoxEmail
            // 
            textBoxEmail.Location = new Point(126, 141);
            textBoxEmail.Name = "textBoxEmail";
            textBoxEmail.Size = new Size(125, 27);
            textBoxEmail.TabIndex = 21;
            // 
            // buttonLogin
            // 
            buttonLogin.Location = new Point(62, 423);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(293, 29);
            buttonLogin.TabIndex = 25;
            buttonLogin.Text = "Login";
            buttonLogin.UseVisualStyleBackColor = true;
            buttonLogin.Click += ButtonLogin_Click;
            // 
            // radioButtonLogin
            // 
            radioButtonLogin.AutoSize = true;
            radioButtonLogin.Checked = true;
            radioButtonLogin.Location = new Point(62, 37);
            radioButtonLogin.Margin = new Padding(3, 4, 3, 4);
            radioButtonLogin.Name = "radioButtonLogin";
            radioButtonLogin.Size = new Size(67, 24);
            radioButtonLogin.TabIndex = 26;
            radioButtonLogin.TabStop = true;
            radioButtonLogin.Text = "Login";
            radioButtonLogin.UseVisualStyleBackColor = true;
            // 
            // radioButtonBearer
            // 
            radioButtonBearer.AutoSize = true;
            radioButtonBearer.Location = new Point(176, 37);
            radioButtonBearer.Margin = new Padding(3, 4, 3, 4);
            radioButtonBearer.Name = "radioButtonBearer";
            radioButtonBearer.Size = new Size(73, 24);
            radioButtonBearer.TabIndex = 27;
            radioButtonBearer.Text = "Bearer";
            radioButtonBearer.UseVisualStyleBackColor = true;
            radioButtonBearer.CheckedChanged += RadioButtonBearer_CheckedChanged;
            // 
            // buttonLoadChapters
            // 
            buttonLoadChapters.Enabled = false;
            buttonLoadChapters.Location = new Point(62, 457);
            buttonLoadChapters.Name = "buttonLoadChapters";
            buttonLoadChapters.Size = new Size(293, 29);
            buttonLoadChapters.TabIndex = 28;
            buttonLoadChapters.Text = "fill Chapters";
            buttonLoadChapters.UseVisualStyleBackColor = true;
            buttonLoadChapters.Click += ButtonLoadChapters_Click;
            // 
            // comboBoxChapters
            // 
            comboBoxChapters.FormattingEnabled = true;
            comboBoxChapters.Location = new Point(46, 221);
            comboBoxChapters.Margin = new Padding(3, 4, 3, 4);
            comboBoxChapters.Name = "comboBoxChapters";
            comboBoxChapters.Size = new Size(499, 28);
            comboBoxChapters.TabIndex = 30;
            comboBoxChapters.SelectedIndexChanged += ComboBoxChapters_SelectedIndexChanged;
            // 
            // PirateChess
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1293, 617);
            Controls.Add(comboBoxChapters);
            Controls.Add(buttonLoadChapters);
            Controls.Add(radioButtonBearer);
            Controls.Add(radioButtonLogin);
            Controls.Add(buttonLogin);
            Controls.Add(textBoxPwd);
            Controls.Add(labelPwd);
            Controls.Add(labelEmail);
            Controls.Add(textBoxEmail);
            Controls.Add(buttonSavePNG);
            Controls.Add(label5);
            Controls.Add(textBoxCumulativeLines);
            Controls.Add(buttonFirstTenLines);
            Controls.Add(label4);
            Controls.Add(textBoxCurLines);
            Controls.Add(labelDurchlauf);
            Controls.Add(textBoxDurchlauf);
            Controls.Add(textBoxPGN);
            Controls.Add(buttonParseAll);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "PirateChess";
            Text = "Piratechess";
            Load += PirateChess_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonParseAll;
        private TextBox textBoxPGN;
        private Label labelDurchlauf;
        private TextBox textBoxDurchlauf;
        private Label label4;
        private TextBox textBoxCurLines;
        private Button buttonFirstTenLines;
        private Label label5;
        private TextBox textBoxCumulativeLines;
        private Button buttonSavePNG;
        private TextBox textBoxPwd;
        private Label labelPwd;
        private Label labelEmail;
        private TextBox textBoxEmail;
        private Button buttonLogin;
        private RadioButton radioButtonLogin;
        private RadioButton radioButtonBearer;
        private Button buttonLoadChapters;
        private ComboBox comboBoxChapters;
    }
}
