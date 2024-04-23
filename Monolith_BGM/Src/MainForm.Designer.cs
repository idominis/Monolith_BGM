namespace Monolith_BGM
{
    partial class MainForm
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
            SaveToDbButton = new Button();
            SuspendLayout();
            // 
            // SaveToDbButton
            // 
            SaveToDbButton.Location = new Point(55, 53);
            SaveToDbButton.Margin = new Padding(3, 4, 3, 4);
            SaveToDbButton.Name = "SaveToDbButton";
            SaveToDbButton.Size = new Size(157, 31);
            SaveToDbButton.TabIndex = 0;
            SaveToDbButton.Text = "Save to XMLs DB";
            SaveToDbButton.UseVisualStyleBackColor = true;
            SaveToDbButton.Click += SaveToDbButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(SaveToDbButton);
            Margin = new Padding(3, 4, 3, 4);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
        }

        #endregion

        private Button SaveToDbButton;
    }
}
