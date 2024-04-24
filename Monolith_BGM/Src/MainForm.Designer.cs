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
            ServiceStartButton = new Button();
            ServiceStopButton = new Button();
            SuspendLayout();
            // 
            // SaveToDbButton
            // 
            SaveToDbButton.Location = new Point(12, 150);
            SaveToDbButton.Name = "SaveToDbButton";
            SaveToDbButton.Size = new Size(137, 23);
            SaveToDbButton.TabIndex = 0;
            SaveToDbButton.Text = "Save XMLs to DB";
            SaveToDbButton.UseVisualStyleBackColor = true;
            SaveToDbButton.Click += SaveToDbButton_Click;
            // 
            // ServiceStartButton
            // 
            ServiceStartButton.Location = new Point(12, 12);
            ServiceStartButton.Name = "ServiceStartButton";
            ServiceStartButton.Size = new Size(137, 23);
            ServiceStartButton.TabIndex = 1;
            ServiceStartButton.Text = "Service Start";
            ServiceStartButton.UseVisualStyleBackColor = true;
            ServiceStartButton.Click += ServiceStartButton_Click;
            // 
            // ServiceStopButton
            // 
            ServiceStopButton.Location = new Point(155, 12);
            ServiceStopButton.Name = "ServiceStopButton";
            ServiceStopButton.Size = new Size(137, 23);
            ServiceStopButton.TabIndex = 2;
            ServiceStopButton.Text = "Service Stop";
            ServiceStopButton.UseVisualStyleBackColor = true;
            ServiceStopButton.Click += ServiceStopButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ServiceStopButton);
            Controls.Add(ServiceStartButton);
            Controls.Add(SaveToDbButton);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
        }

        #endregion

        private Button SaveToDbButton;
        private Button ServiceStartButton;
        private Button ServiceStopButton;
    }
}
