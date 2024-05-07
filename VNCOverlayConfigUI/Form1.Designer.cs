namespace VNCOverlayConfigUI
{
    partial class Form1
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
            btnSave = new Button();
            btnStartService = new Button();
            txtPortNumber = new TextBox();
            btnStopService = new Button();
            lblStatus = new Label();
            btnRestartService = new Button();
            SuspendLayout();
            // 
            // btnSave
            // 
            btnSave.Location = new Point(118, 12);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 0;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += buttonSave_Click;
            // 
            // btnStartService
            // 
            btnStartService.Location = new Point(12, 41);
            btnStartService.Name = "btnStartService";
            btnStartService.Size = new Size(75, 23);
            btnStartService.TabIndex = 1;
            btnStartService.Text = "Start";
            btnStartService.UseVisualStyleBackColor = true;
            btnStartService.Click += buttonStartService_Click;
            // 
            // txtPortNumber
            // 
            txtPortNumber.Location = new Point(12, 12);
            txtPortNumber.Name = "txtPortNumber";
            txtPortNumber.Size = new Size(100, 23);
            txtPortNumber.TabIndex = 2;
            // 
            // btnStopService
            // 
            btnStopService.Location = new Point(93, 41);
            btnStopService.Name = "btnStopService";
            btnStopService.Size = new Size(75, 23);
            btnStopService.TabIndex = 3;
            btnStopService.Text = "Stop";
            btnStopService.UseVisualStyleBackColor = true;
            btnStopService.Click += buttonStopService_Click;
            // 
            // lblStatus
            // 
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Location = new Point(12, 67);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(237, 27);
            lblStatus.TabIndex = 4;
            // 
            // btnRestartService
            // 
            btnRestartService.Location = new Point(174, 41);
            btnRestartService.Name = "btnRestartService";
            btnRestartService.Size = new Size(75, 23);
            btnRestartService.TabIndex = 5;
            btnRestartService.Text = "Restart";
            btnRestartService.UseVisualStyleBackColor = true;
            btnRestartService.Click += buttonRestartService_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(264, 110);
            Controls.Add(btnRestartService);
            Controls.Add(lblStatus);
            Controls.Add(btnStopService);
            Controls.Add(txtPortNumber);
            Controls.Add(btnStartService);
            Controls.Add(btnSave);
            Name = "Form1";
            Text = "VNCOverlay Config";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSave;
        private Button btnStartService;
        private TextBox txtPortNumber;
        private Button btnStopService;
        private Label lblStatus;
        private Button btnRestartService;
    }
}
