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
            SavePODToDbButton = new Button();
            ServiceStartButton = new Button();
            ServiceStopButton = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel = new ToolStripStatusLabel();
            SavePOHToDbButton = new Button();
            Button = new Button();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // SavePODToDbButton
            // 
            SavePODToDbButton.Location = new Point(14, 115);
            SavePODToDbButton.Margin = new Padding(3, 4, 3, 4);
            SavePODToDbButton.Name = "SavePODToDbButton";
            SavePODToDbButton.Size = new Size(157, 31);
            SavePODToDbButton.TabIndex = 0;
            SavePODToDbButton.Text = "Save POD to DB";
            SavePODToDbButton.UseVisualStyleBackColor = true;
            SavePODToDbButton.Click += SavePODToDbButton_Click;
            // 
            // ServiceStartButton
            // 
            ServiceStartButton.Location = new Point(14, 16);
            ServiceStartButton.Margin = new Padding(3, 4, 3, 4);
            ServiceStartButton.Name = "ServiceStartButton";
            ServiceStartButton.Size = new Size(157, 31);
            ServiceStartButton.TabIndex = 1;
            ServiceStartButton.Text = "Service Start";
            ServiceStartButton.UseVisualStyleBackColor = true;
            ServiceStartButton.Click += ServiceStartButton_Click;
            // 
            // ServiceStopButton
            // 
            ServiceStopButton.Location = new Point(177, 16);
            ServiceStopButton.Margin = new Padding(3, 4, 3, 4);
            ServiceStopButton.Name = "ServiceStopButton";
            ServiceStopButton.Size = new Size(157, 31);
            ServiceStopButton.TabIndex = 2;
            ServiceStopButton.Text = "Service Stop";
            ServiceStopButton.UseVisualStyleBackColor = true;
            ServiceStopButton.Click += ServiceStopButton_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
            statusStrip1.Location = new Point(0, 574);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(914, 26);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(266, 20);
            toolStripStatusLabel.Text = "Status messages will be displayed here";
            // 
            // SavePOHToDbButton
            // 
            SavePOHToDbButton.Location = new Point(14, 166);
            SavePOHToDbButton.Name = "SavePOHToDbButton";
            SavePOHToDbButton.Size = new Size(157, 29);
            SavePOHToDbButton.TabIndex = 4;
            SavePOHToDbButton.Text = "Save POH to DB";
            SavePOHToDbButton.UseVisualStyleBackColor = true;
            SavePOHToDbButton.Click += SavePOHToDbButton_Click;
            // 
            // Button
            // 
            Button.Location = new Point(14, 243);
            Button.Name = "Button";
            Button.Size = new Size(157, 29);
            Button.TabIndex = 5;
            Button.Text = "Create POS XMLs";
            Button.UseVisualStyleBackColor = true;
            Button.Click += CreatePOSXMLsButton_ClickAsync;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(Button);
            Controls.Add(SavePOHToDbButton);
            Controls.Add(statusStrip1);
            Controls.Add(ServiceStopButton);
            Controls.Add(ServiceStartButton);
            Controls.Add(SavePODToDbButton);
            Margin = new Padding(3, 4, 3, 4);
            Name = "MainForm";
            Text = "MainForm";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button SavePODToDbButton;
        private Button ServiceStartButton;
        private Button ServiceStopButton;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel;
        private Button SavePOHToDbButton;
        private Button Button;
    }
}
