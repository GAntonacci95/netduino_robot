namespace ComputerSide
{
    partial class NUI
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NUI));
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.treeLog = new System.Windows.Forms.TreeView();
            this.lbltemperatura = new System.Windows.Forms.Label();
            this.lblT = new System.Windows.Forms.Label();
            this.gbInput = new System.Windows.Forms.GroupBox();
            this.btnAlza = new System.Windows.Forms.Button();
            this.btnFerma = new System.Windows.Forms.Button();
            this.btnBuzzer = new System.Windows.Forms.Button();
            this.btnAbbassa = new System.Windows.Forms.Button();
            this.btnChiudi = new System.Windows.Forms.Button();
            this.btnApri = new System.Windows.Forms.Button();
            this.pbRight = new System.Windows.Forms.PictureBox();
            this.pbUp = new System.Windows.Forms.PictureBox();
            this.pbDown = new System.Windows.Forms.PictureBox();
            this.pbLeft = new System.Windows.Forms.PictureBox();
            this.gbNetduino = new System.Windows.Forms.GroupBox();
            this.pbRDx = new System.Windows.Forms.PictureBox();
            this.pbRSx = new System.Windows.Forms.PictureBox();
            this.pbNUI = new System.Windows.Forms.PictureBox();
            this.gbLog.SuspendLayout();
            this.gbInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeft)).BeginInit();
            this.gbNetduino.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRDx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRSx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNUI)).BeginInit();
            this.SuspendLayout();
            // 
            // gbLog
            // 
            this.gbLog.BackColor = System.Drawing.Color.Transparent;
            this.gbLog.Controls.Add(this.treeLog);
            this.gbLog.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.gbLog.Location = new System.Drawing.Point(794, 12);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(301, 611);
            this.gbLog.TabIndex = 1;
            this.gbLog.TabStop = false;
            this.gbLog.Text = "Log";
            // 
            // treeLog
            // 
            this.treeLog.BackColor = System.Drawing.Color.DimGray;
            this.treeLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeLog.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.treeLog.Location = new System.Drawing.Point(6, 19);
            this.treeLog.Name = "treeLog";
            this.treeLog.Size = new System.Drawing.Size(287, 580);
            this.treeLog.TabIndex = 0;
            // 
            // lbltemperatura
            // 
            this.lbltemperatura.AutoSize = true;
            this.lbltemperatura.Location = new System.Drawing.Point(685, 32);
            this.lbltemperatura.Name = "lbltemperatura";
            this.lbltemperatura.Size = new System.Drawing.Size(0, 13);
            this.lbltemperatura.TabIndex = 1;
            // 
            // lblT
            // 
            this.lblT.AutoSize = true;
            this.lblT.Location = new System.Drawing.Point(609, 32);
            this.lblT.Name = "lblT";
            this.lblT.Size = new System.Drawing.Size(70, 13);
            this.lblT.TabIndex = 0;
            this.lblT.Text = "Temperatura:";
            // 
            // gbInput
            // 
            this.gbInput.BackColor = System.Drawing.Color.Transparent;
            this.gbInput.Controls.Add(this.btnAlza);
            this.gbInput.Controls.Add(this.btnFerma);
            this.gbInput.Controls.Add(this.btnBuzzer);
            this.gbInput.Controls.Add(this.btnAbbassa);
            this.gbInput.Controls.Add(this.btnChiudi);
            this.gbInput.Controls.Add(this.btnApri);
            this.gbInput.Controls.Add(this.pbRight);
            this.gbInput.Controls.Add(this.pbUp);
            this.gbInput.Controls.Add(this.pbDown);
            this.gbInput.Controls.Add(this.pbLeft);
            this.gbInput.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.gbInput.Location = new System.Drawing.Point(12, 475);
            this.gbInput.Name = "gbInput";
            this.gbInput.Size = new System.Drawing.Size(776, 148);
            this.gbInput.TabIndex = 1;
            this.gbInput.TabStop = false;
            this.gbInput.Text = "Input";
            // 
            // btnAlza
            // 
            this.btnAlza.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAlza.Location = new System.Drawing.Point(249, 19);
            this.btnAlza.Name = "btnAlza";
            this.btnAlza.Size = new System.Drawing.Size(55, 55);
            this.btnAlza.TabIndex = 12;
            this.btnAlza.UseVisualStyleBackColor = true;
            // 
            // btnFerma
            // 
            this.btnFerma.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFerma.Location = new System.Drawing.Point(97, 82);
            this.btnFerma.Name = "btnFerma";
            this.btnFerma.Size = new System.Drawing.Size(55, 55);
            this.btnFerma.TabIndex = 11;
            this.btnFerma.UseVisualStyleBackColor = true;
            // 
            // btnBuzzer
            // 
            this.btnBuzzer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBuzzer.Location = new System.Drawing.Point(97, 19);
            this.btnBuzzer.Name = "btnBuzzer";
            this.btnBuzzer.Size = new System.Drawing.Size(55, 55);
            this.btnBuzzer.TabIndex = 10;
            this.btnBuzzer.UseVisualStyleBackColor = true;
            // 
            // btnAbbassa
            // 
            this.btnAbbassa.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbbassa.Location = new System.Drawing.Point(249, 82);
            this.btnAbbassa.Name = "btnAbbassa";
            this.btnAbbassa.Size = new System.Drawing.Size(55, 55);
            this.btnAbbassa.TabIndex = 9;
            this.btnAbbassa.UseVisualStyleBackColor = true;
            // 
            // btnChiudi
            // 
            this.btnChiudi.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnChiudi.Location = new System.Drawing.Point(173, 82);
            this.btnChiudi.Name = "btnChiudi";
            this.btnChiudi.Size = new System.Drawing.Size(55, 55);
            this.btnChiudi.TabIndex = 7;
            this.btnChiudi.UseVisualStyleBackColor = true;
            // 
            // btnApri
            // 
            this.btnApri.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnApri.Location = new System.Drawing.Point(173, 19);
            this.btnApri.Name = "btnApri";
            this.btnApri.Size = new System.Drawing.Size(55, 55);
            this.btnApri.TabIndex = 6;
            this.btnApri.UseVisualStyleBackColor = true;
            // 
            // pbRight
            // 
            this.pbRight.BackColor = System.Drawing.Color.Transparent;
            this.pbRight.Location = new System.Drawing.Point(601, 80);
            this.pbRight.Name = "pbRight";
            this.pbRight.Size = new System.Drawing.Size(55, 56);
            this.pbRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbRight.TabIndex = 4;
            this.pbRight.TabStop = false;
            // 
            // pbUp
            // 
            this.pbUp.BackColor = System.Drawing.Color.Transparent;
            this.pbUp.Location = new System.Drawing.Point(540, 18);
            this.pbUp.Name = "pbUp";
            this.pbUp.Size = new System.Drawing.Size(55, 56);
            this.pbUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbUp.TabIndex = 1;
            this.pbUp.TabStop = false;
            // 
            // pbDown
            // 
            this.pbDown.BackColor = System.Drawing.Color.Transparent;
            this.pbDown.ImageLocation = "";
            this.pbDown.Location = new System.Drawing.Point(540, 80);
            this.pbDown.Name = "pbDown";
            this.pbDown.Size = new System.Drawing.Size(55, 56);
            this.pbDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbDown.TabIndex = 3;
            this.pbDown.TabStop = false;
            // 
            // pbLeft
            // 
            this.pbLeft.BackColor = System.Drawing.Color.Transparent;
            this.pbLeft.Location = new System.Drawing.Point(479, 80);
            this.pbLeft.Name = "pbLeft";
            this.pbLeft.Size = new System.Drawing.Size(55, 56);
            this.pbLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLeft.TabIndex = 2;
            this.pbLeft.TabStop = false;
            // 
            // gbNetduino
            // 
            this.gbNetduino.BackColor = System.Drawing.Color.Transparent;
            this.gbNetduino.Controls.Add(this.pbRDx);
            this.gbNetduino.Controls.Add(this.pbRSx);
            this.gbNetduino.Controls.Add(this.lbltemperatura);
            this.gbNetduino.Controls.Add(this.lblT);
            this.gbNetduino.Controls.Add(this.pbNUI);
            this.gbNetduino.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.gbNetduino.Location = new System.Drawing.Point(12, 12);
            this.gbNetduino.Name = "gbNetduino";
            this.gbNetduino.Size = new System.Drawing.Size(776, 457);
            this.gbNetduino.TabIndex = 0;
            this.gbNetduino.TabStop = false;
            this.gbNetduino.Text = "Netduino";
            // 
            // pbRDx
            // 
            this.pbRDx.Location = new System.Drawing.Point(17, 341);
            this.pbRDx.Name = "pbRDx";
            this.pbRDx.Size = new System.Drawing.Size(100, 100);
            this.pbRDx.TabIndex = 5;
            this.pbRDx.TabStop = false;
            // 
            // pbRSx
            // 
            this.pbRSx.Location = new System.Drawing.Point(17, 32);
            this.pbRSx.Name = "pbRSx";
            this.pbRSx.Size = new System.Drawing.Size(100, 100);
            this.pbRSx.TabIndex = 4;
            this.pbRSx.TabStop = false;
            // 
            // pbNUI
            // 
            this.pbNUI.Location = new System.Drawing.Point(97, 65);
            this.pbNUI.Name = "pbNUI";
            this.pbNUI.Size = new System.Drawing.Size(659, 327);
            this.pbNUI.TabIndex = 3;
            this.pbNUI.TabStop = false;
            // 
            // NUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1099, 649);
            this.Controls.Add(this.gbNetduino);
            this.Controls.Add(this.gbInput);
            this.Controls.Add(this.gbLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "NUI";
            this.Text = "Netduino User Interface";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NUI_FormClosing);
            this.Load += new System.EventHandler(this.NUI_Load);
            this.Shown += new System.EventHandler(this.NUI_Shown);
            this.gbLog.ResumeLayout(false);
            this.gbInput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeft)).EndInit();
            this.gbNetduino.ResumeLayout(false);
            this.gbNetduino.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRDx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRSx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNUI)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.GroupBox gbInput;
        private System.Windows.Forms.PictureBox pbRight;
        private System.Windows.Forms.PictureBox pbUp;
        private System.Windows.Forms.PictureBox pbDown;
        private System.Windows.Forms.PictureBox pbLeft;
        private System.Windows.Forms.Label lbltemperatura;
        private System.Windows.Forms.Label lblT;
        private System.Windows.Forms.TreeView treeLog;
        private System.Windows.Forms.Button btnAbbassa;
        private System.Windows.Forms.Button btnChiudi;
        private System.Windows.Forms.Button btnApri;
        private System.Windows.Forms.Button btnBuzzer;
        private System.Windows.Forms.Button btnFerma;
        private System.Windows.Forms.Button btnAlza;
        private System.Windows.Forms.GroupBox gbNetduino;
        private System.Windows.Forms.PictureBox pbRDx;
        private System.Windows.Forms.PictureBox pbRSx;
        private System.Windows.Forms.PictureBox pbNUI;
    }
}

