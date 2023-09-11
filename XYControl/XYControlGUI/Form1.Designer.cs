
namespace XYControlGUI
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.portNameList = new System.Windows.Forms.ComboBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.StartButton = new System.Windows.Forms.Button();
            this.COMSendButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.CLITextBox = new System.Windows.Forms.TextBox();
            this.baudRateList = new System.Windows.Forms.ComboBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(426, 369);
            this.panel1.TabIndex = 0;
            // 
            // portNameList
            // 
            this.portNameList.FormattingEnabled = true;
            this.portNameList.Location = new System.Drawing.Point(12, 409);
            this.portNameList.Name = "portNameList";
            this.portNameList.Size = new System.Drawing.Size(99, 21);
            this.portNameList.TabIndex = 1;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(201, 409);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 2;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(282, 409);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 23);
            this.StartButton.TabIndex = 3;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            // 
            // COMSendButton
            // 
            this.COMSendButton.Location = new System.Drawing.Point(363, 438);
            this.COMSendButton.Name = "COMSendButton";
            this.COMSendButton.Size = new System.Drawing.Size(75, 23);
            this.COMSendButton.TabIndex = 4;
            this.COMSendButton.Text = "Send";
            this.COMSendButton.UseVisualStyleBackColor = true;
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(363, 409);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(75, 23);
            this.StopButton.TabIndex = 5;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            // 
            // CLITextBox
            // 
            this.CLITextBox.Location = new System.Drawing.Point(12, 440);
            this.CLITextBox.Name = "CLITextBox";
            this.CLITextBox.Size = new System.Drawing.Size(345, 20);
            this.CLITextBox.TabIndex = 7;
            // 
            // baudRateList
            // 
            this.baudRateList.FormattingEnabled = true;
            this.baudRateList.Location = new System.Drawing.Point(117, 409);
            this.baudRateList.Name = "baudRateList";
            this.baudRateList.Size = new System.Drawing.Size(78, 21);
            this.baudRateList.TabIndex = 9;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(46, 385);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(49, 20);
            this.numericUpDown1.TabIndex = 10;
            this.numericUpDown1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 387);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Dim:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(101, 387);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "cm";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(211, 385);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            -2147483648});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown2.TabIndex = 13;
            this.numericUpDown2.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(311, 385);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDown3.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            -2147483648});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(46, 20);
            this.numericUpDown3.TabIndex = 14;
            this.numericUpDown3.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(155, 387);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "pen down";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(265, 387);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "pen up";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(362, 385);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "Pen Test";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 467);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDown3);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.baudRateList);
            this.Controls.Add(this.CLITextBox);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.COMSendButton);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.portNameList);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox portNameList;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button COMSendButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.TextBox CLITextBox;
        private System.Windows.Forms.ComboBox baudRateList;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
    }
}

