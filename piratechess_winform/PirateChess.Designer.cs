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
            checkedListBoxChapters = new CheckedListBox();
            buttonSelectAll = new Button();
            labelBearer = new Label();
            textBoxBearer = new TextBox();
            buttonSaveRestResponse = new Button();
            buttonLoadRestResponse = new Button();
            labelCourse = new Label();
            textBoxCourse = new TextBox();
            textBoxLog = new TextBox();
            labelElapsed = new Label();
            groupBoxSettings = new GroupBox();
            radioButtonFirstKeyMove = new RadioButton();
            radioButtonAllKeyMoves = new RadioButton();
            radioButtonNoTrainingMove = new RadioButton();
            checkBoxAddMoveEmptyChapters = new CheckBox();
            groupBoxSettings.SuspendLayout();
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
            textBoxPGN.Size = new Size(309, 238);
            textBoxPGN.TabIndex = 1;
            //
            // labelCourse
            //
            labelCourse.AutoSize = true;
            labelCourse.Location = new Point(676, 332);
            labelCourse.Name = "labelCourse";
            labelCourse.Size = new Size(49, 20);
            labelCourse.TabIndex = 36;
            labelCourse.Text = "course";
            //
            // textBoxCourse
            //
            textBoxCourse.Enabled = false;
            textBoxCourse.Location = new Point(760, 329);
            textBoxCourse.Name = "textBoxCourse";
            textBoxCourse.Size = new Size(125, 27);
            textBoxCourse.TabIndex = 37;
            textBoxCourse.Text = "0/0";
            //
            // labelDurchlauf
            //
            labelDurchlauf.AutoSize = true;
            labelDurchlauf.Location = new Point(676, 362);
            labelDurchlauf.Name = "labelDurchlauf";
            labelDurchlauf.Size = new Size(59, 20);
            labelDurchlauf.TabIndex = 13;
            labelDurchlauf.Text = "chapter";
            // 
            // textBoxDurchlauf
            // 
            textBoxDurchlauf.Enabled = false;
            textBoxDurchlauf.Location = new Point(760, 359);
            textBoxDurchlauf.Name = "textBoxDurchlauf";
            textBoxDurchlauf.Size = new Size(125, 27);
            textBoxDurchlauf.TabIndex = 12;
            textBoxDurchlauf.Text = "0";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(676, 392);
            label4.Name = "label4";
            label4.Size = new Size(39, 20);
            label4.TabIndex = 16;
            label4.Text = "lines";
            // 
            // textBoxCurLines
            // 
            textBoxCurLines.AccessibleRole = AccessibleRole.None;
            textBoxCurLines.Enabled = false;
            textBoxCurLines.Location = new Point(760, 389);
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
            label5.Location = new Point(676, 422);
            label5.Name = "label5";
            label5.Size = new Size(74, 20);
            label5.TabIndex = 19;
            label5.Text = "total lines";
            // 
            // textBoxCumulativeLines
            // 
            textBoxCumulativeLines.AccessibleRole = AccessibleRole.None;
            textBoxCumulativeLines.Enabled = false;
            textBoxCumulativeLines.Location = new Point(760, 419);
            textBoxCumulativeLines.Name = "textBoxCumulativeLines";
            textBoxCumulativeLines.Size = new Size(125, 27);
            textBoxCumulativeLines.TabIndex = 18;
            textBoxCumulativeLines.Text = "0";
            // 
            // buttonSavePNG
            // 
            buttonSavePNG.Location = new Point(799, 458);
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
            // checkedListBoxChapters
            //
            checkedListBoxChapters.FormattingEnabled = true;
            checkedListBoxChapters.Location = new Point(46, 221);
            checkedListBoxChapters.Margin = new Padding(3, 4, 3, 4);
            checkedListBoxChapters.Name = "checkedListBoxChapters";
            checkedListBoxChapters.Size = new Size(469, 160);
            checkedListBoxChapters.TabIndex = 30;
            checkedListBoxChapters.ItemCheck += CheckedListBoxChapters_ItemCheck;
            //
            // buttonSelectAll
            //
            buttonSelectAll.Location = new Point(520, 221);
            buttonSelectAll.Name = "buttonSelectAll";
            buttonSelectAll.Size = new Size(100, 29);
            buttonSelectAll.TabIndex = 35;
            buttonSelectAll.Text = "Select All";
            buttonSelectAll.UseVisualStyleBackColor = true;
            buttonSelectAll.Click += ButtonSelectAll_Click;
            // 
            // labelBearer
            // 
            labelBearer.AutoSize = true;
            labelBearer.Location = new Point(46, 104);
            labelBearer.Name = "labelBearer";
            labelBearer.Size = new Size(52, 20);
            labelBearer.TabIndex = 32;
            labelBearer.Text = "Bearer";
            // 
            // textBoxBearer
            // 
            textBoxBearer.Location = new Point(126, 104);
            textBoxBearer.Name = "textBoxBearer";
            textBoxBearer.Size = new Size(125, 27);
            textBoxBearer.TabIndex = 31;
            // 
            // buttonSaveRestResponse
            // 
            buttonSaveRestResponse.Location = new Point(799, 493);
            buttonSaveRestResponse.Name = "buttonSaveRestResponse";
            buttonSaveRestResponse.Size = new Size(156, 29);
            buttonSaveRestResponse.TabIndex = 33;
            buttonSaveRestResponse.Text = "Save Raw Respone";
            buttonSaveRestResponse.UseVisualStyleBackColor = true;
            buttonSaveRestResponse.Click += buttonSaveRestResponse_Click;
            // 
            // buttonLoadRestResponse
            // 
            buttonLoadRestResponse.Location = new Point(799, 528);
            buttonLoadRestResponse.Name = "buttonLoadRestResponse";
            buttonLoadRestResponse.Size = new Size(156, 29);
            buttonLoadRestResponse.TabIndex = 34;
            buttonLoadRestResponse.Text = "Load Raw Respone";
            buttonLoadRestResponse.UseVisualStyleBackColor = true;
            buttonLoadRestResponse.Click += buttonLoadRestResponse_Click;
            //
            // labelElapsed
            //
            labelElapsed.AutoSize = true;
            labelElapsed.Location = new Point(676, 298);
            labelElapsed.Name = "labelElapsed";
            labelElapsed.TabIndex = 41;
            labelElapsed.Text = "Elapsed: --:--:--";
            //
            // textBoxLog
            //
            textBoxLog.Location = new Point(676, 562);
            textBoxLog.Multiline = true;
            textBoxLog.Name = "textBoxLog";
            textBoxLog.ReadOnly = true;
            textBoxLog.ScrollBars = ScrollBars.Vertical;
            textBoxLog.Size = new Size(635, 80);
            textBoxLog.TabIndex = 40;
            //
            // groupBoxSettings
            //
            groupBoxSettings.Location = new Point(1008, 28);
            groupBoxSettings.Name = "groupBoxSettings";
            groupBoxSettings.Size = new Size(305, 155);
            groupBoxSettings.TabIndex = 38;
            groupBoxSettings.TabStop = false;
            groupBoxSettings.Text = "Settings";
            groupBoxSettings.Controls.Add(radioButtonFirstKeyMove);
            groupBoxSettings.Controls.Add(radioButtonAllKeyMoves);
            groupBoxSettings.Controls.Add(radioButtonNoTrainingMove);
            groupBoxSettings.Controls.Add(checkBoxAddMoveEmptyChapters);
            //
            // radioButtonFirstKeyMove
            //
            radioButtonFirstKeyMove.AutoSize = true;
            radioButtonFirstKeyMove.Checked = true;
            radioButtonFirstKeyMove.Location = new Point(12, 28);
            radioButtonFirstKeyMove.Name = "radioButtonFirstKeyMove";
            radioButtonFirstKeyMove.TabIndex = 0;
            radioButtonFirstKeyMove.TabStop = true;
            radioButtonFirstKeyMove.Text = "First key move as training move";
            radioButtonFirstKeyMove.UseVisualStyleBackColor = true;
            //
            // radioButtonAllKeyMoves
            //
            radioButtonAllKeyMoves.AutoSize = true;
            radioButtonAllKeyMoves.Location = new Point(12, 58);
            radioButtonAllKeyMoves.Name = "radioButtonAllKeyMoves";
            radioButtonAllKeyMoves.TabIndex = 1;
            radioButtonAllKeyMoves.Text = "All key moves as training moves";
            radioButtonAllKeyMoves.UseVisualStyleBackColor = true;
            //
            // radioButtonNoTrainingMove
            //
            radioButtonNoTrainingMove.AutoSize = true;
            radioButtonNoTrainingMove.Location = new Point(12, 88);
            radioButtonNoTrainingMove.Name = "radioButtonNoTrainingMove";
            radioButtonNoTrainingMove.TabIndex = 2;
            radioButtonNoTrainingMove.Text = "No training move";
            radioButtonNoTrainingMove.UseVisualStyleBackColor = true;
            //
            // checkBoxAddMoveEmptyChapters
            //
            checkBoxAddMoveEmptyChapters.AutoSize = true;
            checkBoxAddMoveEmptyChapters.Location = new Point(12, 122);
            checkBoxAddMoveEmptyChapters.Name = "checkBoxAddMoveEmptyChapters";
            checkBoxAddMoveEmptyChapters.TabIndex = 3;
            checkBoxAddMoveEmptyChapters.Text = "Add move to empty chapters";
            checkBoxAddMoveEmptyChapters.UseVisualStyleBackColor = true;
            //
            // PirateChess
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1331, 655);
            Controls.Add(buttonLoadRestResponse);
            Controls.Add(buttonSaveRestResponse);
            Controls.Add(labelBearer);
            Controls.Add(textBoxBearer);
            Controls.Add(checkedListBoxChapters);
            Controls.Add(buttonSelectAll);
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
            Controls.Add(labelCourse);
            Controls.Add(textBoxCourse);
            Controls.Add(labelDurchlauf);
            Controls.Add(textBoxDurchlauf);
            Controls.Add(textBoxPGN);
            Controls.Add(textBoxLog);
            Controls.Add(labelElapsed);
            Controls.Add(groupBoxSettings);
            Controls.Add(buttonParseAll);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "PirateChess";
            Text = "Piratechess";
            Load += PirateChess_Load;
            groupBoxSettings.ResumeLayout(false);
            groupBoxSettings.PerformLayout();
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
        private CheckedListBox checkedListBoxChapters;
        private Button buttonSelectAll;
        private Label labelBearer;
        private TextBox textBoxBearer;
        private Button buttonSaveRestResponse;
        private Button buttonLoadRestResponse;
        private Label labelCourse;
        private TextBox textBoxCourse;
        private TextBox textBoxLog;
        private Label labelElapsed;
        private GroupBox groupBoxSettings;
        private RadioButton radioButtonFirstKeyMove;
        private RadioButton radioButtonAllKeyMoves;
        private RadioButton radioButtonNoTrainingMove;
        private CheckBox checkBoxAddMoveEmptyChapters;
    }
}
