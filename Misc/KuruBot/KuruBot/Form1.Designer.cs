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
            this.SuspendLayout();
            // 
            // connect
            // 
            this.connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.connect.Location = new System.Drawing.Point(12, 415);
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
            this.dlMap.Location = new System.Drawing.Point(93, 415);
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
            this.main_panel.Size = new System.Drawing.Size(776, 386);
            this.main_panel.TabIndex = 2;
            // 
            // showGMap
            // 
            this.showGMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.showGMap.AutoSize = true;
            this.showGMap.Checked = true;
            this.showGMap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGMap.Location = new System.Drawing.Point(666, 415);
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
            this.showPMap.Location = new System.Drawing.Point(532, 415);
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
            this.showPosition.Location = new System.Drawing.Point(202, 415);
            this.showPosition.Name = "showPosition";
            this.showPosition.Size = new System.Drawing.Size(103, 23);
            this.showPosition.TabIndex = 5;
            this.showPosition.Text = "Show Position";
            this.showPosition.UseVisualStyleBackColor = true;
            this.showPosition.Click += new System.EventHandler(this.showPosition_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.showPosition);
            this.Controls.Add(this.showPMap);
            this.Controls.Add(this.showGMap);
            this.Controls.Add(this.main_panel);
            this.Controls.Add(this.dlMap);
            this.Controls.Add(this.connect);
            this.Name = "Form1";
            this.Text = "KuruBot";
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
    }
}

