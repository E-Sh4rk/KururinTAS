namespace KuruBot
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
            this.connect = new System.Windows.Forms.Button();
            this.dlMap = new System.Windows.Forms.Button();
            this.main_panel = new System.Windows.Forms.Panel();
            this.showGMap = new System.Windows.Forms.CheckBox();
            this.showPMap = new System.Windows.Forms.CheckBox();
            this.kuruComFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.showPosition = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.enterInputString = new System.Windows.Forms.Button();
            this.downloadInputs = new System.Windows.Forms.Button();
            this.inputsFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.savePos = new System.Windows.Forms.Button();
            this.restorePos = new System.Windows.Forms.Button();
            this.convertInputs = new System.Windows.Forms.Button();
            this.sendLastInputs = new System.Windows.Forms.Button();
            this.logLastMoves = new System.Windows.Forms.Button();
            this.saveLogFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveInputs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connect
            // 
            this.connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.connect.Location = new System.Drawing.Point(12, 426);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(75, 23);
            this.connect.TabIndex = 0;
            this.connect.Text = "Connect";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);
            // 
            // dlMap
            // 
            this.dlMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dlMap.Location = new System.Drawing.Point(93, 426);
            this.dlMap.Name = "dlMap";
            this.dlMap.Size = new System.Drawing.Size(103, 23);
            this.dlMap.TabIndex = 1;
            this.dlMap.Text = "Download Map";
            this.dlMap.UseVisualStyleBackColor = true;
            this.dlMap.Click += new System.EventHandler(this.dlMap_Click);
            // 
            // main_panel
            // 
            this.main_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main_panel.Location = new System.Drawing.Point(12, 12);
            this.main_panel.Name = "main_panel";
            this.main_panel.Size = new System.Drawing.Size(791, 366);
            this.main_panel.TabIndex = 2;
            // 
            // showGMap
            // 
            this.showGMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.showGMap.AutoSize = true;
            this.showGMap.Checked = true;
            this.showGMap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGMap.Location = new System.Drawing.Point(681, 426);
            this.showGMap.Name = "showGMap";
            this.showGMap.Size = new System.Drawing.Size(122, 17);
            this.showGMap.TabIndex = 3;
            this.showGMap.Text = "Show graphical map";
            this.showGMap.UseVisualStyleBackColor = true;
            this.showGMap.CheckedChanged += new System.EventHandler(this.showGPMap_CheckedChanged);
            // 
            // showPMap
            // 
            this.showPMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.showPMap.AutoSize = true;
            this.showPMap.Location = new System.Drawing.Point(560, 426);
            this.showPMap.Name = "showPMap";
            this.showPMap.Size = new System.Drawing.Size(117, 17);
            this.showPMap.TabIndex = 4;
            this.showPMap.Text = "Show physical map";
            this.showPMap.UseVisualStyleBackColor = true;
            this.showPMap.CheckedChanged += new System.EventHandler(this.showGPMap_CheckedChanged);
            // 
            // kuruComFileDialog
            // 
            this.kuruComFileDialog.FileName = "kuruCOM.lua";
            this.kuruComFileDialog.Filter = "LUA Script|*.lua";
            // 
            // showPosition
            // 
            this.showPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.showPosition.Location = new System.Drawing.Point(202, 426);
            this.showPosition.Name = "showPosition";
            this.showPosition.Size = new System.Drawing.Size(103, 23);
            this.showPosition.TabIndex = 5;
            this.showPosition.Text = "Download Position";
            this.showPosition.UseVisualStyleBackColor = true;
            this.showPosition.Click += new System.EventHandler(this.showPosition_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 389);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(317, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Use Ø/CTRL/ALT+Numpad to perform a move for the next frame.";
            // 
            // enterInputString
            // 
            this.enterInputString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.enterInputString.Location = new System.Drawing.Point(545, 384);
            this.enterInputString.Name = "enterInputString";
            this.enterInputString.Size = new System.Drawing.Size(76, 23);
            this.enterInputString.TabIndex = 7;
            this.enterInputString.Text = "Load inputs";
            this.enterInputString.UseVisualStyleBackColor = true;
            this.enterInputString.Click += new System.EventHandler(this.enterInputString_Click);
            // 
            // downloadInputs
            // 
            this.downloadInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.downloadInputs.Location = new System.Drawing.Point(311, 426);
            this.downloadInputs.Name = "downloadInputs";
            this.downloadInputs.Size = new System.Drawing.Size(123, 23);
            this.downloadInputs.TabIndex = 8;
            this.downloadInputs.Text = "Download next inputs";
            this.downloadInputs.UseVisualStyleBackColor = true;
            this.downloadInputs.Click += new System.EventHandler(this.downloadInputs_Click);
            // 
            // inputsFileDialog
            // 
            this.inputsFileDialog.FileName = "inputs.txt";
            this.inputsFileDialog.Filter = "Text file|*.txt";
            // 
            // savePos
            // 
            this.savePos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.savePos.Location = new System.Drawing.Point(469, 384);
            this.savePos.Name = "savePos";
            this.savePos.Size = new System.Drawing.Size(31, 23);
            this.savePos.TabIndex = 9;
            this.savePos.Text = "SP";
            this.savePos.UseVisualStyleBackColor = true;
            this.savePos.Click += new System.EventHandler(this.savePos_Click);
            // 
            // restorePos
            // 
            this.restorePos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.restorePos.Location = new System.Drawing.Point(508, 384);
            this.restorePos.Name = "restorePos";
            this.restorePos.Size = new System.Drawing.Size(31, 23);
            this.restorePos.TabIndex = 10;
            this.restorePos.Text = "RP";
            this.restorePos.UseVisualStyleBackColor = true;
            this.restorePos.Click += new System.EventHandler(this.restorePos_Click);
            // 
            // convertInputs
            // 
            this.convertInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.convertInputs.Location = new System.Drawing.Point(712, 384);
            this.convertInputs.Name = "convertInputs";
            this.convertInputs.Size = new System.Drawing.Size(91, 23);
            this.convertInputs.TabIndex = 11;
            this.convertInputs.Text = "Convert inputs";
            this.convertInputs.UseVisualStyleBackColor = true;
            this.convertInputs.Click += new System.EventHandler(this.convertInputs_Click);
            // 
            // sendLastInputs
            // 
            this.sendLastInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendLastInputs.Location = new System.Drawing.Point(440, 426);
            this.sendLastInputs.Name = "sendLastInputs";
            this.sendLastInputs.Size = new System.Drawing.Size(99, 23);
            this.sendLastInputs.TabIndex = 12;
            this.sendLastInputs.Text = "Send last inputs";
            this.sendLastInputs.UseVisualStyleBackColor = true;
            this.sendLastInputs.Click += new System.EventHandler(this.sendLastInputs_Click);
            // 
            // logLastMoves
            // 
            this.logLastMoves.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.logLastMoves.Location = new System.Drawing.Point(366, 384);
            this.logLastMoves.Name = "logLastMoves";
            this.logLastMoves.Size = new System.Drawing.Size(97, 23);
            this.logLastMoves.TabIndex = 13;
            this.logLastMoves.Text = "Log last moves";
            this.logLastMoves.UseVisualStyleBackColor = true;
            this.logLastMoves.Click += new System.EventHandler(this.logLastMoves_Click);
            // 
            // saveLogFileDialog
            // 
            this.saveLogFileDialog.Filter = "Text file|*.txt";
            // 
            // saveInputs
            // 
            this.saveInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveInputs.Location = new System.Drawing.Point(627, 384);
            this.saveInputs.Name = "saveInputs";
            this.saveInputs.Size = new System.Drawing.Size(79, 23);
            this.saveInputs.TabIndex = 14;
            this.saveInputs.Text = "Save inputs";
            this.saveInputs.UseVisualStyleBackColor = true;
            this.saveInputs.Click += new System.EventHandler(this.saveInputs_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 461);
            this.Controls.Add(this.saveInputs);
            this.Controls.Add(this.logLastMoves);
            this.Controls.Add(this.sendLastInputs);
            this.Controls.Add(this.convertInputs);
            this.Controls.Add(this.restorePos);
            this.Controls.Add(this.savePos);
            this.Controls.Add(this.downloadInputs);
            this.Controls.Add(this.enterInputString);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.showPosition);
            this.Controls.Add(this.showPMap);
            this.Controls.Add(this.showGMap);
            this.Controls.Add(this.main_panel);
            this.Controls.Add(this.dlMap);
            this.Controls.Add(this.connect);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "KuruBot";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.Button dlMap;
        private System.Windows.Forms.Panel main_panel;
        private System.Windows.Forms.CheckBox showGMap;
        private System.Windows.Forms.CheckBox showPMap;
        private System.Windows.Forms.OpenFileDialog kuruComFileDialog;
        private System.Windows.Forms.Button showPosition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button enterInputString;
        private System.Windows.Forms.Button downloadInputs;
        private System.Windows.Forms.OpenFileDialog inputsFileDialog;
        private System.Windows.Forms.Button savePos;
        private System.Windows.Forms.Button restorePos;
        private System.Windows.Forms.Button convertInputs;
        private System.Windows.Forms.Button sendLastInputs;
        private System.Windows.Forms.Button logLastMoves;
        private System.Windows.Forms.SaveFileDialog saveLogFileDialog;
        private System.Windows.Forms.Button saveInputs;
    }
}

