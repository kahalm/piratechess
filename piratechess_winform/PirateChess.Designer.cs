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
            labelChapter = new Label();
            textBoxChapter = new TextBox();
            label4 = new Label();
            textBoxCurLines = new TextBox();
            buttonFirstTenLines = new Button();
            label5 = new Label();
            textBoxCumulativeLines = new TextBox();
            buttonSavePNG = new Button();
            buttonLogin = new Button();
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
            labelExtraDelayMin = new Label();
            numericExtraDelayMin = new NumericUpDown();
            labelExtraDelayMax = new Label();
            numericExtraDelayMax = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)numericExtraDelayMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericExtraDelayMax).BeginInit();
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
            // labelChapter
            //
            labelChapter.AutoSize = true;
            labelChapter.Location = new Point(676, 362);
            labelChapter.Name = "labelChapter";
            labelChapter.Size = new Size(59, 20);
            labelChapter.TabIndex = 13;
            labelChapter.Text = "chapter";
            // 
            // textBoxChapter
            // 
            textBoxChapter.Enabled = false;
            textBoxChapter.Location = new Point(760, 359);
            textBoxChapter.Name = "textBoxChapter";
            textBoxChapter.Size = new Size(125, 27);
            textBoxChapter.TabIndex = 12;
            textBoxChapter.Text = "0";
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
            labelBearer.Location = new Point(46, 40);
            labelBearer.Name = "labelBearer";
            labelBearer.Size = new Size(52, 20);
            labelBearer.TabIndex = 32;
            labelBearer.Text = "Bearer";
            // 
            // textBoxBearer
            // 
            textBoxBearer.Location = new Point(126, 37);
            textBoxBearer.Name = "textBoxBearer";
            textBoxBearer.Size = new Size(229, 27);
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
            groupBoxSettings.Size = new Size(305, 235);
            groupBoxSettings.TabIndex = 38;
            groupBoxSettings.TabStop = false;
            groupBoxSettings.Text = "Settings";
            groupBoxSettings.Controls.Add(radioButtonFirstKeyMove);
            groupBoxSettings.Controls.Add(radioButtonAllKeyMoves);
            groupBoxSettings.Controls.Add(radioButtonNoTrainingMove);
            groupBoxSettings.Controls.Add(checkBoxAddMoveEmptyChapters);
            groupBoxSettings.Controls.Add(labelExtraDelayMin);
            groupBoxSettings.Controls.Add(numericExtraDelayMin);
            groupBoxSettings.Controls.Add(labelExtraDelayMax);
            groupBoxSettings.Controls.Add(numericExtraDelayMax);
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
            // labelExtraDelayMin
            //
            labelExtraDelayMin.AutoSize = true;
            labelExtraDelayMin.Location = new Point(12, 158);
            labelExtraDelayMin.Name = "labelExtraDelayMin";
            labelExtraDelayMin.TabIndex = 4;
            labelExtraDelayMin.Text = "Extra delay min (ms)";
            //
            // numericExtraDelayMin
            //
            numericExtraDelayMin.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            numericExtraDelayMin.Location = new Point(185, 155);
            numericExtraDelayMin.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numericExtraDelayMin.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            numericExtraDelayMin.Name = "numericExtraDelayMin";
            numericExtraDelayMin.Size = new Size(105, 27);
            numericExtraDelayMin.TabIndex = 5;
            numericExtraDelayMin.TextAlign = HorizontalAlignment.Right;
            numericExtraDelayMin.ThousandsSeparator = true;
            numericExtraDelayMin.ValueChanged += NumericExtraDelayMin_ValueChanged;
            //
            // labelExtraDelayMax
            //
            labelExtraDelayMax.AutoSize = true;
            labelExtraDelayMax.Location = new Point(12, 196);
            labelExtraDelayMax.Name = "labelExtraDelayMax";
            labelExtraDelayMax.TabIndex = 6;
            labelExtraDelayMax.Text = "Extra delay max (ms)";
            //
            // numericExtraDelayMax
            //
            numericExtraDelayMax.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            numericExtraDelayMax.Location = new Point(185, 193);
            numericExtraDelayMax.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
            numericExtraDelayMax.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            numericExtraDelayMax.Name = "numericExtraDelayMax";
            numericExtraDelayMax.Size = new Size(105, 27);
            numericExtraDelayMax.TabIndex = 7;
            numericExtraDelayMax.TextAlign = HorizontalAlignment.Right;
            numericExtraDelayMax.ThousandsSeparator = true;
            numericExtraDelayMax.ValueChanged += NumericExtraDelayMax_ValueChanged;
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
            Controls.Add(buttonLogin);
            Controls.Add(buttonSavePNG);
            Controls.Add(label5);
            Controls.Add(textBoxCumulativeLines);
            Controls.Add(buttonFirstTenLines);
            Controls.Add(label4);
            Controls.Add(textBoxCurLines);
            Controls.Add(labelCourse);
            Controls.Add(textBoxCourse);
            Controls.Add(labelChapter);
            Controls.Add(textBoxChapter);
            Controls.Add(textBoxPGN);
            Controls.Add(textBoxLog);
            Controls.Add(labelElapsed);
            Controls.Add(groupBoxSettings);
            Controls.Add(buttonParseAll);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "PirateChess";
            Text = "Piratechess";
            Load += PirateChess_Load;
            ((System.ComponentModel.ISupportInitialize)numericExtraDelayMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericExtraDelayMax).EndInit();
            groupBoxSettings.ResumeLayout(false);
            groupBoxSettings.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonParseAll;
        private TextBox textBoxPGN;
        private Label labelChapter;
        private TextBox textBoxChapter;
        private Label label4;
        private TextBox textBoxCurLines;
        private Button buttonFirstTenLines;
        private Label label5;
        private TextBox textBoxCumulativeLines;
        private Button buttonSavePNG;
        private Button buttonLogin;
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
        private Label labelExtraDelayMin;
        private NumericUpDown numericExtraDelayMin;
        private Label labelExtraDelayMax;
        private NumericUpDown numericExtraDelayMax;
    }
}
