namespace HeritabilityCalculator
{
    partial class HeritabilityCalculator
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
            this.TreeBrowse = new System.Windows.Forms.Button();
            this.TreePathText = new System.Windows.Forms.TextBox();
            this.BrowseTreeLabel = new System.Windows.Forms.Label();
            this.BrowseInputlabel = new System.Windows.Forms.Label();
            this.UserInputText = new System.Windows.Forms.TextBox();
            this.InputBrowse = new System.Windows.Forms.Button();
            this.MainHeader = new System.Windows.Forms.Label();
            this.Log = new System.Windows.Forms.RichTextBox();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.StartButton = new System.Windows.Forms.Button();
            this.CalculateProgressBar = new System.Windows.Forms.ProgressBar();
            this.ApplicationHelp = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // TreeBrowse
            // 
            this.TreeBrowse.Location = new System.Drawing.Point(647, 228);
            this.TreeBrowse.Name = "TreeBrowse";
            this.TreeBrowse.Size = new System.Drawing.Size(92, 32);
            this.TreeBrowse.TabIndex = 0;
            this.TreeBrowse.Text = "Browse";
            this.TreeBrowse.UseVisualStyleBackColor = true;
            this.TreeBrowse.Click += new System.EventHandler(this.TreeBrowse_Click);
            // 
            // TreePathText
            // 
            this.TreePathText.Location = new System.Drawing.Point(148, 229);
            this.TreePathText.Multiline = true;
            this.TreePathText.Name = "TreePathText";
            this.TreePathText.ReadOnly = true;
            this.TreePathText.Size = new System.Drawing.Size(500, 30);
            this.TreePathText.TabIndex = 1;
            // 
            // BrowseTreeLabel
            // 
            this.BrowseTreeLabel.AutoSize = true;
            this.BrowseTreeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BrowseTreeLabel.Location = new System.Drawing.Point(14, 230);
            this.BrowseTreeLabel.Name = "BrowseTreeLabel";
            this.BrowseTreeLabel.Size = new System.Drawing.Size(121, 25);
            this.BrowseTreeLabel.TabIndex = 2;
            this.BrowseTreeLabel.Text = "Newick Tree";
            // 
            // BrowseInputlabel
            // 
            this.BrowseInputlabel.AutoSize = true;
            this.BrowseInputlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BrowseInputlabel.Location = new System.Drawing.Point(17, 343);
            this.BrowseInputlabel.Name = "BrowseInputlabel";
            this.BrowseInputlabel.Size = new System.Drawing.Size(55, 25);
            this.BrowseInputlabel.TabIndex = 5;
            this.BrowseInputlabel.Text = "Input";
            // 
            // UserInputText
            // 
            this.UserInputText.Location = new System.Drawing.Point(149, 342);
            this.UserInputText.Multiline = true;
            this.UserInputText.Name = "UserInputText";
            this.UserInputText.ReadOnly = true;
            this.UserInputText.Size = new System.Drawing.Size(500, 30);
            this.UserInputText.TabIndex = 4;
            // 
            // InputBrowse
            // 
            this.InputBrowse.Location = new System.Drawing.Point(648, 341);
            this.InputBrowse.Name = "InputBrowse";
            this.InputBrowse.Size = new System.Drawing.Size(92, 32);
            this.InputBrowse.TabIndex = 3;
            this.InputBrowse.Text = "Browse";
            this.InputBrowse.UseVisualStyleBackColor = true;
            this.InputBrowse.Click += new System.EventHandler(this.InputBrowse_Click);
            // 
            // MainHeader
            // 
            this.MainHeader.AutoSize = true;
            this.MainHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainHeader.Location = new System.Drawing.Point(162, 22);
            this.MainHeader.Name = "MainHeader";
            this.MainHeader.Size = new System.Drawing.Size(498, 58);
            this.MainHeader.TabIndex = 6;
            this.MainHeader.Text = "Heritability Calculator";
            // 
            // Log
            // 
            this.Log.Location = new System.Drawing.Point(12, 601);
            this.Log.Name = "Log";
            this.Log.ReadOnly = true;
            this.Log.Size = new System.Drawing.Size(758, 140);
            this.Log.TabIndex = 7;
            this.Log.Text = "";
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoSize = true;
            this.DescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionLabel.Location = new System.Drawing.Point(49, 111);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(708, 48);
            this.DescriptionLabel.TabIndex = 8;
            this.DescriptionLabel.Text = "Welcome to Heritability Calculator, Please load all requierd data to begin calcul" +
    "ation.\r\nFor help press F1.";
            this.DescriptionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // StartButton
            // 
            this.StartButton.Enabled = false;
            this.StartButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartButton.Location = new System.Drawing.Point(316, 493);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(160, 73);
            this.StartButton.TabIndex = 9;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // CalculateProgressBar
            // 
            this.CalculateProgressBar.Location = new System.Drawing.Point(12, 417);
            this.CalculateProgressBar.Name = "CalculateProgressBar";
            this.CalculateProgressBar.Size = new System.Drawing.Size(758, 35);
            this.CalculateProgressBar.TabIndex = 10;
            this.CalculateProgressBar.Visible = false;
            // 
            // HeritabilityCalculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 753);
            this.Controls.Add(this.CalculateProgressBar);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.DescriptionLabel);
            this.Controls.Add(this.Log);
            this.Controls.Add(this.MainHeader);
            this.Controls.Add(this.BrowseInputlabel);
            this.Controls.Add(this.UserInputText);
            this.Controls.Add(this.InputBrowse);
            this.Controls.Add(this.BrowseTreeLabel);
            this.Controls.Add(this.TreePathText);
            this.Controls.Add(this.TreeBrowse);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "HeritabilityCalculator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Heritability Calculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TreeBrowse;
        private System.Windows.Forms.TextBox TreePathText;
        private System.Windows.Forms.Label BrowseTreeLabel;
        private System.Windows.Forms.Label BrowseInputlabel;
        private System.Windows.Forms.TextBox UserInputText;
        private System.Windows.Forms.Button InputBrowse;
        private System.Windows.Forms.Label MainHeader;
        private System.Windows.Forms.RichTextBox Log;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.ProgressBar CalculateProgressBar;
        private System.Windows.Forms.HelpProvider ApplicationHelp;
    }
}

