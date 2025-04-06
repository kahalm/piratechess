namespace piratechess
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
            textBoxBearer = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            textBoxUid = new TextBox();
            textBoxOid = new TextBox();
            textBoxLid = new TextBox();
            textBoxBid = new TextBox();
            labelLid = new Label();
            labelBid = new Label();
            labelDurchlauf = new Label();
            textBoxDurchlauf = new TextBox();
            buttonTestdaten = new Button();
            label4 = new Label();
            textBoxCurLines = new TextBox();
            buttonFirstTenLines = new Button();
            SuspendLayout();
            // 
            // buttonParseAll
            // 
            buttonParseAll.Location = new Point(62, 554);
            buttonParseAll.Name = "buttonParseAll";
            buttonParseAll.Size = new Size(292, 29);
            buttonParseAll.TabIndex = 0;
            buttonParseAll.Text = "Generiere kompletten Kurs";
            buttonParseAll.UseVisualStyleBackColor = true;
            buttonParseAll.Click += Button1_Click;
            // 
            // textBoxPGN
            // 
            textBoxPGN.Location = new Point(799, 93);
            textBoxPGN.Multiline = true;
            textBoxPGN.Name = "textBoxPGN";
            textBoxPGN.Size = new Size(309, 268);
            textBoxPGN.TabIndex = 1;
            // 
            // textBoxBearer
            // 
            textBoxBearer.Location = new Point(130, 30);
            textBoxBearer.Name = "textBoxBearer";
            textBoxBearer.Size = new Size(125, 27);
            textBoxBearer.TabIndex = 2;
            textBoxBearer.TextChanged += textBoxBearer_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(50, 30);
            label1.Name = "label1";
            label1.Size = new Size(52, 20);
            label1.TabIndex = 3;
            label1.Text = "bearer";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(50, 65);
            label2.Name = "label2";
            label2.Size = new Size(30, 20);
            label2.TabIndex = 4;
            label2.Text = "uid";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(50, 135);
            label3.Name = "label3";
            label3.Size = new Size(31, 20);
            label3.TabIndex = 5;
            label3.Text = "oid";
            // 
            // textBoxUid
            // 
            textBoxUid.Location = new Point(130, 65);
            textBoxUid.Name = "textBoxUid";
            textBoxUid.Size = new Size(125, 27);
            textBoxUid.TabIndex = 6;
            // 
            // textBoxOid
            // 
            textBoxOid.Enabled = false;
            textBoxOid.Location = new Point(130, 135);
            textBoxOid.Name = "textBoxOid";
            textBoxOid.Size = new Size(125, 27);
            textBoxOid.TabIndex = 7;
            // 
            // textBoxLid
            // 
            textBoxLid.Enabled = false;
            textBoxLid.Location = new Point(130, 170);
            textBoxLid.Name = "textBoxLid";
            textBoxLid.Size = new Size(125, 27);
            textBoxLid.TabIndex = 8;
            // 
            // textBoxBid
            // 
            textBoxBid.Location = new Point(130, 100);
            textBoxBid.Name = "textBoxBid";
            textBoxBid.Size = new Size(125, 27);
            textBoxBid.TabIndex = 9;
            // 
            // labelLid
            // 
            labelLid.AutoSize = true;
            labelLid.Location = new Point(50, 170);
            labelLid.Name = "labelLid";
            labelLid.Size = new Size(26, 20);
            labelLid.TabIndex = 10;
            labelLid.Text = "lid";
            // 
            // labelBid
            // 
            labelBid.AutoSize = true;
            labelBid.Location = new Point(50, 100);
            labelBid.Name = "labelBid";
            labelBid.Size = new Size(31, 20);
            labelBid.TabIndex = 11;
            labelBid.Text = "bid";
            // 
            // labelDurchlauf
            // 
            labelDurchlauf.AutoSize = true;
            labelDurchlauf.Location = new Point(50, 205);
            labelDurchlauf.Name = "labelDurchlauf";
            labelDurchlauf.Size = new Size(71, 20);
            labelDurchlauf.TabIndex = 13;
            labelDurchlauf.Text = "durchlauf";
            // 
            // textBoxDurchlauf
            // 
            textBoxDurchlauf.Enabled = false;
            textBoxDurchlauf.Location = new Point(130, 205);
            textBoxDurchlauf.Name = "textBoxDurchlauf";
            textBoxDurchlauf.Size = new Size(125, 27);
            textBoxDurchlauf.TabIndex = 12;
            textBoxDurchlauf.Text = "0";
            // 
            // buttonTestdaten
            // 
            buttonTestdaten.Location = new Point(62, 509);
            buttonTestdaten.Name = "buttonTestdaten";
            buttonTestdaten.Size = new Size(292, 29);
            buttonTestdaten.TabIndex = 14;
            buttonTestdaten.Text = "Testdaten";
            buttonTestdaten.UseVisualStyleBackColor = true;
            buttonTestdaten.Click += ButtonTestdaten_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(50, 240);
            label4.Name = "label4";
            label4.Size = new Size(74, 20);
            label4.TabIndex = 16;
            label4.Text = "curr. Lines";
            // 
            // textBoxCurLines
            // 
            textBoxCurLines.AccessibleRole = AccessibleRole.None;
            textBoxCurLines.Enabled = false;
            textBoxCurLines.Location = new Point(130, 240);
            textBoxCurLines.Name = "textBoxCurLines";
            textBoxCurLines.Size = new Size(125, 27);
            textBoxCurLines.TabIndex = 15;
            textBoxCurLines.Text = "0";
            // 
            // buttonFirstTenLines
            // 
            buttonFirstTenLines.Location = new Point(62, 462);
            buttonFirstTenLines.Name = "buttonFirstTenLines";
            buttonFirstTenLines.Size = new Size(292, 29);
            buttonFirstTenLines.TabIndex = 17;
            buttonFirstTenLines.Text = "Ersten 10 Lines";
            buttonFirstTenLines.UseVisualStyleBackColor = true;
            // 
            // PirateChess
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1293, 617);
            Controls.Add(buttonFirstTenLines);
            Controls.Add(label4);
            Controls.Add(textBoxCurLines);
            Controls.Add(buttonTestdaten);
            Controls.Add(labelDurchlauf);
            Controls.Add(textBoxDurchlauf);
            Controls.Add(labelBid);
            Controls.Add(labelLid);
            Controls.Add(textBoxBid);
            Controls.Add(textBoxLid);
            Controls.Add(textBoxOid);
            Controls.Add(textBoxUid);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBoxBearer);
            Controls.Add(textBoxPGN);
            Controls.Add(buttonParseAll);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "PirateChess";
            Text = "Piratechess";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonParseAll;
        private TextBox textBoxPGN;
        private TextBox textBoxBearer;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox textBoxUid;
        private TextBox textBoxOid;
        private TextBox textBoxLid;
        private TextBox textBoxBid;
        private Label labelLid;
        private Label labelBid;
        private Label labelDurchlauf;
        private TextBox textBoxDurchlauf;
        private Button buttonTestdaten;
        private Label label4;
        private TextBox textBoxCurLines;
        private Button buttonFirstTenLines;
    }
}
