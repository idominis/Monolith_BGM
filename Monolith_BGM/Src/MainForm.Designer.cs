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
            groupBox3 = new GroupBox();
            label4 = new Label();
            saveToDbRadioButtonOff = new RadioButton();
            saveToDbRadioButtonOn = new RadioButton();
            groupBox4 = new GroupBox();
            sendDataRangeButton = new Button();
            CreateDataRangePOS = new Button();
            dateTimePicker2 = new DateTimePicker();
            dateTimePicker1 = new DateTimePicker();
            label5 = new Label();
            createXmlDbRadioButtonOff = new RadioButton();
            createXmlDbRadioButtonOn = new RadioButton();
            richTextBoxLogs = new RichTextBox();
            label6 = new Label();
            exitAppButton = new Button();
            statusStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // SavePODToDbButton
            // 
            SavePODToDbButton.Location = new Point(16, 35);
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
            statusStrip1.Location = new Point(0, 481);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(925, 22);
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
            SavePOHToDbButton.Location = new Point(16, 73);
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
            Button.Location = new Point(16, 34);
            Button.Margin = new Padding(3, 2, 3, 2);
            Button.Name = "Button";
            Button.Size = new Size(137, 22);
            Button.TabIndex = 5;
            Button.Text = "Create All POS XMLs";
            Button.UseVisualStyleBackColor = true;
            Button.Click += CreatePOSXMLsButton_ClickAsync;
            // 
            // comboBoxStartDate
            // 
            comboBoxStartDate.FormattingEnabled = true;
            comboBoxStartDate.Location = new Point(25, 43);
            comboBoxStartDate.Margin = new Padding(3, 2, 3, 2);
            comboBoxStartDate.Name = "comboBoxStartDate";
            comboBoxStartDate.Size = new Size(104, 23);
            comboBoxStartDate.TabIndex = 6;
            comboBoxStartDate.Text = "Start Date Pick";
            // 
            // comboBoxEndDate
            // 
            comboBoxEndDate.FormattingEnabled = true;
            comboBoxEndDate.Location = new Point(134, 43);
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
            groupBox1.Location = new Point(322, 41);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(270, 128);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "XMLs Interval Creation";
            // 
            // sendXmlButton
            // 
            sendXmlButton.Location = new Point(134, 70);
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
            generateXmlButton.Location = new Point(25, 70);
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
            label2.Location = new Point(134, 26);
            label2.Name = "label2";
            label2.Size = new Size(54, 15);
            label2.TabIndex = 9;
            label2.Text = "End Date";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 26);
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
            groupBox2.Location = new Point(647, 41);
            groupBox2.Margin = new Padding(3, 2, 3, 2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 2, 3, 2);
            groupBox2.Size = new Size(256, 128);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Set AutoSend All Created XMLs";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(46, 60);
            label3.Name = "label3";
            label3.Size = new Size(81, 15);
            label3.TabIndex = 3;
            label3.Text = "Last Date Sent";
            // 
            // autoSendTextBox
            // 
            autoSendTextBox.Location = new Point(46, 85);
            autoSendTextBox.Margin = new Padding(3, 2, 3, 2);
            autoSendTextBox.Name = "autoSendTextBox";
            autoSendTextBox.Size = new Size(150, 23);
            autoSendTextBox.TabIndex = 2;
            // 
            // radioButtonOff
            // 
            radioButtonOff.AutoSize = true;
            radioButtonOff.Location = new Point(93, 36);
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
            // groupBox3
            // 
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(saveToDbRadioButtonOff);
            groupBox3.Controls.Add(saveToDbRadioButtonOn);
            groupBox3.Controls.Add(SavePODToDbButton);
            groupBox3.Controls.Add(SavePOHToDbButton);
            groupBox3.Location = new Point(12, 41);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(280, 128);
            groupBox3.TabIndex = 10;
            groupBox3.TabStop = false;
            groupBox3.Text = "Save XMLs to DB";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(192, 37);
            label4.Name = "label4";
            label4.Size = new Size(33, 15);
            label4.TabIndex = 5;
            label4.Text = "Auto";
            // 
            // saveToDbRadioButtonOff
            // 
            saveToDbRadioButtonOff.AutoSize = true;
            saveToDbRadioButtonOff.Location = new Point(221, 56);
            saveToDbRadioButtonOff.Margin = new Padding(3, 2, 3, 2);
            saveToDbRadioButtonOff.Name = "saveToDbRadioButtonOff";
            saveToDbRadioButtonOff.Size = new Size(42, 19);
            saveToDbRadioButtonOff.TabIndex = 4;
            saveToDbRadioButtonOff.TabStop = true;
            saveToDbRadioButtonOff.Text = "Off";
            saveToDbRadioButtonOff.UseMnemonic = false;
            saveToDbRadioButtonOff.UseVisualStyleBackColor = true;
            saveToDbRadioButtonOff.CheckedChanged += SaveToDbRadioButtonOff_CheckedChanged;
            // 
            // saveToDbRadioButtonOn
            // 
            saveToDbRadioButtonOn.AutoSize = true;
            saveToDbRadioButtonOn.Location = new Point(174, 56);
            saveToDbRadioButtonOn.Margin = new Padding(3, 2, 3, 2);
            saveToDbRadioButtonOn.Name = "saveToDbRadioButtonOn";
            saveToDbRadioButtonOn.Size = new Size(41, 19);
            saveToDbRadioButtonOn.TabIndex = 4;
            saveToDbRadioButtonOn.TabStop = true;
            saveToDbRadioButtonOn.Text = "On";
            saveToDbRadioButtonOn.UseVisualStyleBackColor = true;
            saveToDbRadioButtonOn.CheckedChanged += SaveToDbRadioButtonOn_CheckedChanged;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(sendDataRangeButton);
            groupBox4.Controls.Add(CreateDataRangePOS);
            groupBox4.Controls.Add(dateTimePicker2);
            groupBox4.Controls.Add(dateTimePicker1);
            groupBox4.Controls.Add(label5);
            groupBox4.Controls.Add(createXmlDbRadioButtonOff);
            groupBox4.Controls.Add(Button);
            groupBox4.Controls.Add(createXmlDbRadioButtonOn);
            groupBox4.Location = new Point(12, 185);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(810, 74);
            groupBox4.TabIndex = 11;
            groupBox4.TabStop = false;
            groupBox4.Text = "Create XMLs from DB";
            // 
            // sendDataRangeButton
            // 
            sendDataRangeButton.Location = new Point(682, 32);
            sendDataRangeButton.Name = "sendDataRangeButton";
            sendDataRangeButton.Size = new Size(122, 23);
            sendDataRangeButton.TabIndex = 10;
            sendDataRangeButton.Text = "Send DataRange POS XMLs";
            sendDataRangeButton.UseVisualStyleBackColor = true;
            sendDataRangeButton.Click += SendDataRangeButton_Click;
            // 
            // CreateDataRangePOS
            // 
            CreateDataRangePOS.Location = new Point(507, 33);
            CreateDataRangePOS.Name = "CreateDataRangePOS";
            CreateDataRangePOS.Size = new Size(169, 23);
            CreateDataRangePOS.TabIndex = 9;
            CreateDataRangePOS.Text = "Create DataRange POS XMLs";
            CreateDataRangePOS.UseVisualStyleBackColor = true;
            CreateDataRangePOS.Click += CreateDataRangePOS_Click;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Format = DateTimePickerFormat.Short;
            dateTimePicker2.Location = new Point(390, 32);
            dateTimePicker2.MinDate = new DateTime(2011, 4, 1, 0, 0, 0, 0);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(99, 23);
            dateTimePicker2.TabIndex = 8;
            dateTimePicker2.Value = new DateTime(2012, 2, 2, 5, 55, 0, 0);
            dateTimePicker2.ValueChanged += DateTimePicker2_ValueChanged;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Location = new Point(276, 32);
            dateTimePicker1.MinDate = new DateTime(2011, 3, 31, 0, 0, 0, 0);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(95, 23);
            dateTimePicker1.TabIndex = 7;
            dateTimePicker1.Value = new DateTime(2011, 4, 1, 5, 53, 0, 0);
            dateTimePicker1.ValueChanged += DateTimePicker1_ValueChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(182, 18);
            label5.Name = "label5";
            label5.Size = new Size(33, 15);
            label5.TabIndex = 6;
            label5.Text = "Auto";
            // 
            // createXmlDbRadioButtonOff
            // 
            createXmlDbRadioButtonOff.AutoSize = true;
            createXmlDbRadioButtonOff.Location = new Point(211, 38);
            createXmlDbRadioButtonOff.Margin = new Padding(3, 2, 3, 2);
            createXmlDbRadioButtonOff.Name = "createXmlDbRadioButtonOff";
            createXmlDbRadioButtonOff.Size = new Size(42, 19);
            createXmlDbRadioButtonOff.TabIndex = 5;
            createXmlDbRadioButtonOff.TabStop = true;
            createXmlDbRadioButtonOff.Text = "Off";
            createXmlDbRadioButtonOff.UseMnemonic = false;
            createXmlDbRadioButtonOff.UseVisualStyleBackColor = true;
            createXmlDbRadioButtonOff.CheckedChanged += CreateXmlDbRadioButtonOff_CheckedChanged;
            // 
            // createXmlDbRadioButtonOn
            // 
            createXmlDbRadioButtonOn.AutoSize = true;
            createXmlDbRadioButtonOn.Location = new Point(164, 38);
            createXmlDbRadioButtonOn.Margin = new Padding(3, 2, 3, 2);
            createXmlDbRadioButtonOn.Name = "createXmlDbRadioButtonOn";
            createXmlDbRadioButtonOn.Size = new Size(41, 19);
            createXmlDbRadioButtonOn.TabIndex = 6;
            createXmlDbRadioButtonOn.TabStop = true;
            createXmlDbRadioButtonOn.Text = "On";
            createXmlDbRadioButtonOn.UseVisualStyleBackColor = true;
            createXmlDbRadioButtonOn.CheckedChanged += CreateXmlDbRadioButtonOn_CheckedChanged;
            // 
            // richTextBoxLogs
            // 
            richTextBoxLogs.Location = new Point(12, 288);
            richTextBoxLogs.Name = "richTextBoxLogs";
            richTextBoxLogs.ReadOnly = true;
            richTextBoxLogs.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBoxLogs.Size = new Size(891, 190);
            richTextBoxLogs.TabIndex = 12;
            richTextBoxLogs.Text = "";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 270);
            label6.Name = "label6";
            label6.Size = new Size(75, 15);
            label6.TabIndex = 13;
            label6.Text = "Info Window";
            // 
            // exitAppButton
            // 
            exitAppButton.BackColor = Color.Red;
            exitAppButton.ForeColor = SystemColors.ButtonHighlight;
            exitAppButton.Location = new Point(828, 185);
            exitAppButton.Name = "exitAppButton";
            exitAppButton.Size = new Size(75, 74);
            exitAppButton.TabIndex = 14;
            exitAppButton.Text = "Exit App";
            exitAppButton.UseVisualStyleBackColor = false;
            exitAppButton.Click += exitAppButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(925, 503);
            Controls.Add(exitAppButton);
            Controls.Add(label6);
            Controls.Add(richTextBoxLogs);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(statusStrip1);
            Controls.Add(ServiceStopButton);
            Controls.Add(ServiceStartButton);
            Name = "MainForm";
            Text = "MainForm";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
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
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private RadioButton saveToDbRadioButtonOff;
        private RadioButton saveToDbRadioButtonOn;
        private Label label4;
        private Label label5;
        private RadioButton createXmlDbRadioButtonOff;
        private RadioButton createXmlDbRadioButtonOn;
        private RichTextBox richTextBoxLogs;
        private Label label6;
        private DateTimePicker dateTimePicker2;
        private DateTimePicker dateTimePicker1;
        private Button CreateDataRangePOS;
        private Button exitAppButton;
        private Button sendDataRangeButton;
    }
}
