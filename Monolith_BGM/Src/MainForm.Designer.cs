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
            comboBoxStartDate = new ComboBox();
            comboBoxEndDate = new ComboBox();
            groupBox1 = new GroupBox();
            sendXmlButton = new Button();
            generateXmlButton = new Button();
            label2 = new Label();
            label1 = new Label();
            groupBox2 = new GroupBox();
            label3 = new Label();
            autoSendTextBox = new TextBox();
            radioButtonOff = new RadioButton();
            radioButtonOn = new RadioButton();
            statusStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // SavePODToDbButton
            // 
            SavePODToDbButton.Location = new Point(12, 86);
            SavePODToDbButton.Name = "SavePODToDbButton";
            SavePODToDbButton.Size = new Size(137, 23);
            SavePODToDbButton.TabIndex = 0;
            SavePODToDbButton.Text = "Save POD to DB";
            SavePODToDbButton.UseVisualStyleBackColor = true;
            SavePODToDbButton.Click += SavePODToDbButton_Click;
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
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(209, 17);
            toolStripStatusLabel.Text = "Status messages will be displayed here";
            // 
            // SavePOHToDbButton
            // 
            SavePOHToDbButton.Location = new Point(12, 124);
            SavePOHToDbButton.Margin = new Padding(3, 2, 3, 2);
            SavePOHToDbButton.Name = "SavePOHToDbButton";
            SavePOHToDbButton.Size = new Size(137, 22);
            SavePOHToDbButton.TabIndex = 4;
            SavePOHToDbButton.Text = "Save POH to DB";
            SavePOHToDbButton.UseVisualStyleBackColor = true;
            SavePOHToDbButton.Click += SavePOHToDbButton_Click;
            // 
            // Button
            // 
            Button.Location = new Point(12, 182);
            Button.Margin = new Padding(3, 2, 3, 2);
            Button.Name = "Button";
            Button.Size = new Size(137, 22);
            Button.TabIndex = 5;
            Button.Text = "Create POS XMLs";
            Button.UseVisualStyleBackColor = true;
            Button.Click += CreatePOSXMLsButton_ClickAsync;
            // 
            // comboBoxStartDate
            // 
            comboBoxStartDate.FormattingEnabled = true;
            comboBoxStartDate.Location = new Point(25, 49);
            comboBoxStartDate.Margin = new Padding(3, 2, 3, 2);
            comboBoxStartDate.Name = "comboBoxStartDate";
            comboBoxStartDate.Size = new Size(104, 23);
            comboBoxStartDate.TabIndex = 6;
            comboBoxStartDate.Text = "Start Date Pick";
            // 
            // comboBoxEndDate
            // 
            comboBoxEndDate.FormattingEnabled = true;
            comboBoxEndDate.Location = new Point(134, 49);
            comboBoxEndDate.Margin = new Padding(3, 2, 3, 2);
            comboBoxEndDate.Name = "comboBoxEndDate";
            comboBoxEndDate.Size = new Size(104, 23);
            comboBoxEndDate.TabIndex = 7;
            comboBoxEndDate.Text = "End Date Pick";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(sendXmlButton);
            groupBox1.Controls.Add(generateXmlButton);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(comboBoxStartDate);
            groupBox1.Controls.Add(comboBoxEndDate);
            groupBox1.Location = new Point(368, 106);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(267, 140);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "XMLs Interval Creation";
            // 
            // sendXmlButton
            // 
            sendXmlButton.Location = new Point(134, 76);
            sendXmlButton.Margin = new Padding(3, 2, 3, 2);
            sendXmlButton.Name = "sendXmlButton";
            sendXmlButton.Size = new Size(103, 42);
            sendXmlButton.TabIndex = 11;
            sendXmlButton.Text = "Send XML";
            sendXmlButton.UseVisualStyleBackColor = true;
            sendXmlButton.Click += SendXmlDateGeneratedButton_Click;
            // 
            // generateXmlButton
            // 
            generateXmlButton.Location = new Point(25, 76);
            generateXmlButton.Margin = new Padding(3, 2, 3, 2);
            generateXmlButton.Name = "generateXmlButton";
            generateXmlButton.Size = new Size(103, 42);
            generateXmlButton.TabIndex = 10;
            generateXmlButton.Text = "Create XML";
            generateXmlButton.UseVisualStyleBackColor = true;
            generateXmlButton.Click += GenerateXmlByDateButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(134, 32);
            label2.Name = "label2";
            label2.Size = new Size(54, 15);
            label2.TabIndex = 9;
            label2.Text = "End Date";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 32);
            label1.Name = "label1";
            label1.Size = new Size(58, 15);
            label1.TabIndex = 8;
            label1.Text = "Start Date";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(autoSendTextBox);
            groupBox2.Controls.Add(radioButtonOff);
            groupBox2.Controls.Add(radioButtonOn);
            groupBox2.Location = new Point(368, 262);
            groupBox2.Margin = new Padding(3, 2, 3, 2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 2, 3, 2);
            groupBox2.Size = new Size(267, 146);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Set AutoSend generated XMLs";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(46, 75);
            label3.Name = "label3";
            label3.Size = new Size(81, 15);
            label3.TabIndex = 3;
            label3.Text = "Last Date Sent";
            // 
            // autoSendTextBox
            // 
            autoSendTextBox.Location = new Point(46, 100);
            autoSendTextBox.Margin = new Padding(3, 2, 3, 2);
            autoSendTextBox.Name = "autoSendTextBox";
            autoSendTextBox.Size = new Size(150, 23);
            autoSendTextBox.TabIndex = 2;
            // 
            // radioButtonOff
            // 
            radioButtonOff.AutoSize = true;
            radioButtonOff.Location = new Point(153, 36);
            radioButtonOff.Margin = new Padding(3, 2, 3, 2);
            radioButtonOff.Name = "radioButtonOff";
            radioButtonOff.Size = new Size(42, 19);
            radioButtonOff.TabIndex = 1;
            radioButtonOff.TabStop = true;
            radioButtonOff.Text = "Off";
            radioButtonOff.UseMnemonic = false;
            radioButtonOff.UseVisualStyleBackColor = true;
            radioButtonOff.CheckedChanged += RadioButtonOff_CheckedChanged;
            // 
            // radioButtonOn
            // 
            radioButtonOn.AutoSize = true;
            radioButtonOn.Location = new Point(46, 36);
            radioButtonOn.Margin = new Padding(3, 2, 3, 2);
            radioButtonOn.Name = "radioButtonOn";
            radioButtonOn.Size = new Size(41, 19);
            radioButtonOn.TabIndex = 0;
            radioButtonOn.TabStop = true;
            radioButtonOn.Text = "On";
            radioButtonOn.UseVisualStyleBackColor = true;
            radioButtonOn.CheckedChanged += RadioButtonOn_CheckedChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(Button);
            Controls.Add(SavePOHToDbButton);
            Controls.Add(statusStrip1);
            Controls.Add(ServiceStopButton);
            Controls.Add(ServiceStartButton);
            Controls.Add(SavePODToDbButton);
            Name = "MainForm";
            Text = "MainForm";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
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
        private ComboBox comboBoxStartDate;
        private ComboBox comboBoxEndDate;
        private GroupBox groupBox1;
        private Label label2;
        private Label label1;
        private Button generateXmlButton;
        private GroupBox groupBox2;
        private Button sendXmlButton;
        private TextBox autoSendTextBox;
        private RadioButton radioButtonOff;
        private RadioButton radioButtonOn;
        private Label label3;
    }
}
