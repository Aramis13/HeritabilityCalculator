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
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TreeBrowse
            // 
            this.TreeBrowse.Location = new System.Drawing.Point(508, 185);
            this.TreeBrowse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TreeBrowse.Name = "TreeBrowse";
            this.TreeBrowse.Size = new System.Drawing.Size(69, 26);
            this.TreeBrowse.TabIndex = 0;
            this.TreeBrowse.Text = "Browse";
            this.TreeBrowse.UseVisualStyleBackColor = true;
            this.TreeBrowse.Click += new System.EventHandler(this.TreeBrowse_Click);
            // 
            // TreePathText
            // 
            this.TreePathText.Location = new System.Drawing.Point(137, 186);
            this.TreePathText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TreePathText.Multiline = true;
            this.TreePathText.Name = "TreePathText";
            this.TreePathText.ReadOnly = true;
            this.TreePathText.Size = new System.Drawing.Size(373, 25);
            this.TreePathText.TabIndex = 1;
            // 
            // BrowseTreeLabel
            // 
            this.BrowseTreeLabel.AutoSize = true;
            this.BrowseTreeLabel.Font = new System.Drawing.Font("Guttman Yad", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.BrowseTreeLabel.Location = new System.Drawing.Point(9, 185);
            this.BrowseTreeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.BrowseTreeLabel.Name = "BrowseTreeLabel";
            this.BrowseTreeLabel.Size = new System.Drawing.Size(124, 26);
            this.BrowseTreeLabel.TabIndex = 2;
            this.BrowseTreeLabel.Text = "Newick Tree";
            // 
            // BrowseInputlabel
            // 
            this.BrowseInputlabel.AutoSize = true;
            this.BrowseInputlabel.Font = new System.Drawing.Font("Guttman Yad", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.BrowseInputlabel.Location = new System.Drawing.Point(48, 273);
            this.BrowseInputlabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.BrowseInputlabel.Name = "BrowseInputlabel";
            this.BrowseInputlabel.Size = new System.Drawing.Size(55, 26);
            this.BrowseInputlabel.TabIndex = 5;
            this.BrowseInputlabel.Text = "Input";
            // 
            // UserInputText
            // 
            this.UserInputText.Location = new System.Drawing.Point(137, 278);
            this.UserInputText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.UserInputText.Multiline = true;
            this.UserInputText.Name = "UserInputText";
            this.UserInputText.ReadOnly = true;
            this.UserInputText.Size = new System.Drawing.Size(371, 25);
            this.UserInputText.TabIndex = 4;
            // 
            // InputBrowse
            // 
            this.InputBrowse.Location = new System.Drawing.Point(506, 277);
            this.InputBrowse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.InputBrowse.Name = "InputBrowse";
            this.InputBrowse.Size = new System.Drawing.Size(69, 26);
            this.InputBrowse.TabIndex = 3;
            this.InputBrowse.Text = "Browse";
            this.InputBrowse.UseVisualStyleBackColor = true;
            this.InputBrowse.Click += new System.EventHandler(this.InputBrowse_Click);
            // 
            // MainHeader
            // 
            this.MainHeader.AutoSize = true;
            this.MainHeader.Font = new System.Drawing.Font("Guttman Yad", 30F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.MainHeader.Location = new System.Drawing.Point(42, 9);
            this.MainHeader.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MainHeader.Name = "MainHeader";
            this.MainHeader.Size = new System.Drawing.Size(512, 65);
            this.MainHeader.TabIndex = 6;
            this.MainHeader.Text = "Heritability Calculator";
            // 
            // Log
            // 
            this.Log.Location = new System.Drawing.Point(11, 434);
            this.Log.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Log.Name = "Log";
            this.Log.ReadOnly = true;
            this.Log.Size = new System.Drawing.Size(570, 114);
            this.Log.TabIndex = 7;
            this.Log.Text = "";
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoSize = true;
            this.DescriptionLabel.Font = new System.Drawing.Font("Guttman Yad", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.DescriptionLabel.Location = new System.Drawing.Point(98, 88);
            this.DescriptionLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(423, 75);
            this.DescriptionLabel.TabIndex = 8;
            this.DescriptionLabel.Text = "Welcome to Heritability Calculator.\r\nPlease load all requierd data to begin calcu" +
    "lation.\r\nFor help press F1.";
            this.DescriptionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // StartButton
            // 
            this.StartButton.Enabled = false;
            this.StartButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartButton.Location = new System.Drawing.Point(232, 343);
            this.StartButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(120, 59);
            this.StartButton.TabIndex = 9;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // CalculateProgressBar
            // 
            this.CalculateProgressBar.Location = new System.Drawing.Point(10, 233);
            this.CalculateProgressBar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CalculateProgressBar.Name = "CalculateProgressBar";
            this.CalculateProgressBar.Size = new System.Drawing.Size(568, 24);
            this.CalculateProgressBar.TabIndex = 10;
            this.CalculateProgressBar.Visible = false;
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.AutoSize = true;
            this.ProgressLabel.Font = new System.Drawing.Font("Guttman Yad", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.ProgressLabel.Location = new System.Drawing.Point(227, 198);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(0, 26);
            this.ProgressLabel.TabIndex = 11;
            this.ProgressLabel.Visible = false;
            // 
            // HeritabilityCalculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(586, 551);
            this.Controls.Add(this.ProgressLabel);
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
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
        private System.Windows.Forms.Label ProgressLabel;
    }
}

