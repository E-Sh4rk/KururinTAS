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
            this.loadFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.backupPos = new System.Windows.Forms.Button();
            this.restorePos = new System.Windows.Forms.Button();
            this.convertInputs = new System.Windows.Forms.Button();
            this.sendLastInputs = new System.Windows.Forms.Button();
            this.logLastMoves = new System.Windows.Forms.Button();
            this.saveLogFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveInputs = new System.Windows.Forms.Button();
            this.loadPos = new System.Windows.Forms.Button();
            this.savePos = new System.Windows.Forms.Button();
            this.loadMap = new System.Windows.Forms.Button();
            this.saveMap = new System.Windows.Forms.Button();
            this.showCostMap = new System.Windows.Forms.CheckBox();
            this.buildSolver = new System.Windows.Forms.Button();
            this.computeCostMap = new System.Windows.Forms.Button();
            this.solve = new System.Windows.Forms.Button();
            this.convertInputsToBk2 = new System.Windows.Forms.Button();
            this.buildSolverAny = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.abort = new System.Windows.Forms.Button();
            this.drawOnMap = new System.Windows.Forms.CheckBox();
            this.setConstraints = new System.Windows.Forms.Button();
            this.setTarget = new System.Windows.Forms.Button();
            this.destroySolver = new System.Windows.Forms.Button();
            this.brushRadius = new System.Windows.Forms.NumericUpDown();
            this.loadConfig = new System.Windows.Forms.Button();
            this.saveConfig = new System.Windows.Forms.Button();
            this.saveIniDialog = new System.Windows.Forms.SaveFileDialog();
            this.loadIniDialog = new System.Windows.Forms.OpenFileDialog();
            this.computeCostMapNoWC = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.brushRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // connect
            // 
            this.connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.connect.Location = new System.Drawing.Point(12, 559);
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
            this.dlMap.Location = new System.Drawing.Point(93, 559);
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
            this.main_panel.Size = new System.Drawing.Size(791, 404);
            this.main_panel.TabIndex = 2;
            // 
            // showGMap
            // 
            this.showGMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.showGMap.AutoSize = true;
            this.showGMap.Checked = true;
            this.showGMap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGMap.Location = new System.Drawing.Point(681, 559);
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
            this.showPMap.Location = new System.Drawing.Point(560, 559);
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
            this.showPosition.Location = new System.Drawing.Point(202, 559);
            this.showPosition.Name = "showPosition";
            this.showPosition.Size = new System.Drawing.Size(103, 23);
            this.showPosition.TabIndex = 5;
            this.showPosition.Text = "Download Position";
            this.showPosition.UseVisualStyleBackColor = true;
            this.showPosition.Click += new System.EventHandler(this.showPosition_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 522);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Use Ø/CTRL/ALT+Numpad to perform a move.";
            // 
            // enterInputString
            // 
            this.enterInputString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.enterInputString.Location = new System.Drawing.Point(587, 517);
            this.enterInputString.Name = "enterInputString";
            this.enterInputString.Size = new System.Drawing.Size(29, 23);
            this.enterInputString.TabIndex = 7;
            this.enterInputString.Text = "LI";
            this.enterInputString.UseVisualStyleBackColor = true;
            this.enterInputString.Click += new System.EventHandler(this.enterInputString_Click);
            // 
            // downloadInputs
            // 
            this.downloadInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.downloadInputs.Location = new System.Drawing.Point(311, 559);
            this.downloadInputs.Name = "downloadInputs";
            this.downloadInputs.Size = new System.Drawing.Size(123, 23);
            this.downloadInputs.TabIndex = 8;
            this.downloadInputs.Text = "Download next inputs";
            this.downloadInputs.UseVisualStyleBackColor = true;
            this.downloadInputs.Click += new System.EventHandler(this.downloadInputs_Click);
            // 
            // loadFileDialog
            // 
            this.loadFileDialog.Filter = "Text file|*.txt";
            // 
            // backupPos
            // 
            this.backupPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.backupPos.Location = new System.Drawing.Point(480, 517);
            this.backupPos.Name = "backupPos";
            this.backupPos.Size = new System.Drawing.Size(31, 23);
            this.backupPos.TabIndex = 9;
            this.backupPos.Text = "BP";
            this.backupPos.UseVisualStyleBackColor = true;
            this.backupPos.Click += new System.EventHandler(this.bkpPos_Click);
            // 
            // restorePos
            // 
            this.restorePos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.restorePos.Location = new System.Drawing.Point(443, 517);
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
            this.convertInputs.Location = new System.Drawing.Point(671, 517);
            this.convertInputs.Name = "convertInputs";
            this.convertInputs.Size = new System.Drawing.Size(64, 23);
            this.convertInputs.TabIndex = 11;
            this.convertInputs.Text = "From bk2";
            this.convertInputs.UseVisualStyleBackColor = true;
            this.convertInputs.Click += new System.EventHandler(this.convertInputsFromBk2_Click);
            // 
            // sendLastInputs
            // 
            this.sendLastInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendLastInputs.Location = new System.Drawing.Point(440, 559);
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
            this.logLastMoves.Location = new System.Drawing.Point(260, 517);
            this.logLastMoves.Name = "logLastMoves";
            this.logLastMoves.Size = new System.Drawing.Size(86, 23);
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
            this.saveInputs.Location = new System.Drawing.Point(622, 517);
            this.saveInputs.Name = "saveInputs";
            this.saveInputs.Size = new System.Drawing.Size(31, 23);
            this.saveInputs.TabIndex = 14;
            this.saveInputs.Text = "SI";
            this.saveInputs.UseVisualStyleBackColor = true;
            this.saveInputs.Click += new System.EventHandler(this.saveInputs_Click);
            // 
            // loadPos
            // 
            this.loadPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadPos.Location = new System.Drawing.Point(517, 517);
            this.loadPos.Name = "loadPos";
            this.loadPos.Size = new System.Drawing.Size(29, 23);
            this.loadPos.TabIndex = 15;
            this.loadPos.Text = "LP";
            this.loadPos.UseVisualStyleBackColor = true;
            this.loadPos.Click += new System.EventHandler(this.loadPos_Click);
            // 
            // savePos
            // 
            this.savePos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.savePos.Location = new System.Drawing.Point(552, 517);
            this.savePos.Name = "savePos";
            this.savePos.Size = new System.Drawing.Size(29, 23);
            this.savePos.TabIndex = 16;
            this.savePos.Text = "SP";
            this.savePos.UseVisualStyleBackColor = true;
            this.savePos.Click += new System.EventHandler(this.savePos_Click);
            // 
            // loadMap
            // 
            this.loadMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadMap.Location = new System.Drawing.Point(369, 517);
            this.loadMap.Name = "loadMap";
            this.loadMap.Size = new System.Drawing.Size(31, 23);
            this.loadMap.TabIndex = 17;
            this.loadMap.Text = "LM";
            this.loadMap.UseVisualStyleBackColor = true;
            this.loadMap.Click += new System.EventHandler(this.loadMap_Click);
            // 
            // saveMap
            // 
            this.saveMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveMap.Location = new System.Drawing.Point(406, 517);
            this.saveMap.Name = "saveMap";
            this.saveMap.Size = new System.Drawing.Size(31, 23);
            this.saveMap.TabIndex = 18;
            this.saveMap.Text = "SM";
            this.saveMap.UseVisualStyleBackColor = true;
            this.saveMap.Click += new System.EventHandler(this.saveMap_Click);
            // 
            // showCostMap
            // 
            this.showCostMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.showCostMap.AutoSize = true;
            this.showCostMap.Location = new System.Drawing.Point(704, 484);
            this.showCostMap.Name = "showCostMap";
            this.showCostMap.Size = new System.Drawing.Size(99, 17);
            this.showCostMap.TabIndex = 19;
            this.showCostMap.Text = "Show cost map";
            this.showCostMap.UseVisualStyleBackColor = true;
            this.showCostMap.CheckedChanged += new System.EventHandler(this.showGPMap_CheckedChanged);
            // 
            // buildSolver
            // 
            this.buildSolver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buildSolver.Location = new System.Drawing.Point(106, 480);
            this.buildSolver.Name = "buildSolver";
            this.buildSolver.Size = new System.Drawing.Size(138, 23);
            this.buildSolver.TabIndex = 20;
            this.buildSolver.Text = "Build solver (legal ending)";
            this.buildSolver.UseVisualStyleBackColor = true;
            this.buildSolver.Click += new System.EventHandler(this.buildSolver_Click);
            // 
            // computeCostMap
            // 
            this.computeCostMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.computeCostMap.Location = new System.Drawing.Point(394, 480);
            this.computeCostMap.Name = "computeCostMap";
            this.computeCostMap.Size = new System.Drawing.Size(80, 23);
            this.computeCostMap.TabIndex = 21;
            this.computeCostMap.Text = "Cost map";
            this.computeCostMap.UseVisualStyleBackColor = true;
            this.computeCostMap.Click += new System.EventHandler(this.computeCostMap_Click);
            // 
            // solve
            // 
            this.solve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.solve.Location = new System.Drawing.Point(615, 480);
            this.solve.Name = "solve";
            this.solve.Size = new System.Drawing.Size(62, 23);
            this.solve.TabIndex = 22;
            this.solve.Text = "Solve";
            this.solve.UseVisualStyleBackColor = true;
            this.solve.Click += new System.EventHandler(this.solve_Click);
            // 
            // convertInputsToBk2
            // 
            this.convertInputsToBk2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.convertInputsToBk2.Location = new System.Drawing.Point(739, 517);
            this.convertInputsToBk2.Name = "convertInputsToBk2";
            this.convertInputsToBk2.Size = new System.Drawing.Size(64, 23);
            this.convertInputsToBk2.TabIndex = 23;
            this.convertInputsToBk2.Text = "To bk2";
            this.convertInputsToBk2.UseVisualStyleBackColor = true;
            this.convertInputsToBk2.Click += new System.EventHandler(this.convertInputsToBk2_Click);
            // 
            // buildSolverAny
            // 
            this.buildSolverAny.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buildSolverAny.Location = new System.Drawing.Point(250, 480);
            this.buildSolverAny.Name = "buildSolverAny";
            this.buildSolverAny.Size = new System.Drawing.Size(138, 23);
            this.buildSolverAny.TabIndex = 24;
            this.buildSolverAny.Text = "Build solver (any ending)";
            this.buildSolverAny.UseVisualStyleBackColor = true;
            this.buildSolverAny.Click += new System.EventHandler(this.buildSolverAny_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(187, 422);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(616, 23);
            this.progressBar.TabIndex = 25;
            // 
            // abort
            // 
            this.abort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.abort.Enabled = false;
            this.abort.Location = new System.Drawing.Point(12, 451);
            this.abort.Name = "abort";
            this.abort.Size = new System.Drawing.Size(88, 23);
            this.abort.TabIndex = 26;
            this.abort.Text = "Abort";
            this.abort.UseVisualStyleBackColor = true;
            this.abort.Click += new System.EventHandler(this.abort_Click);
            // 
            // drawOnMap
            // 
            this.drawOnMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.drawOnMap.Appearance = System.Windows.Forms.Appearance.Button;
            this.drawOnMap.AutoSize = true;
            this.drawOnMap.Location = new System.Drawing.Point(250, 451);
            this.drawOnMap.Name = "drawOnMap";
            this.drawOnMap.Size = new System.Drawing.Size(42, 23);
            this.drawOnMap.TabIndex = 27;
            this.drawOnMap.Text = "Draw";
            this.drawOnMap.UseVisualStyleBackColor = true;
            this.drawOnMap.CheckedChanged += new System.EventHandler(this.drawOnMap_CheckedChanged);
            // 
            // setConstraints
            // 
            this.setConstraints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.setConstraints.Location = new System.Drawing.Point(394, 451);
            this.setConstraints.Name = "setConstraints";
            this.setConstraints.Size = new System.Drawing.Size(215, 23);
            this.setConstraints.TabIndex = 28;
            this.setConstraints.Text = "Set constraints to current drawing";
            this.setConstraints.UseVisualStyleBackColor = true;
            this.setConstraints.Click += new System.EventHandler(this.setConstraints_Click);
            // 
            // setTarget
            // 
            this.setTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.setTarget.Location = new System.Drawing.Point(615, 451);
            this.setTarget.Name = "setTarget";
            this.setTarget.Size = new System.Drawing.Size(188, 23);
            this.setTarget.TabIndex = 29;
            this.setTarget.Text = "Set target to current drawing";
            this.setTarget.UseVisualStyleBackColor = true;
            this.setTarget.Click += new System.EventHandler(this.setTarget_Click);
            // 
            // destroySolver
            // 
            this.destroySolver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.destroySolver.Location = new System.Drawing.Point(12, 480);
            this.destroySolver.Name = "destroySolver";
            this.destroySolver.Size = new System.Drawing.Size(88, 23);
            this.destroySolver.TabIndex = 30;
            this.destroySolver.Text = "Destroy solver";
            this.destroySolver.UseVisualStyleBackColor = true;
            this.destroySolver.Click += new System.EventHandler(this.destroySolver_Click);
            // 
            // brushRadius
            // 
            this.brushRadius.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.brushRadius.Location = new System.Drawing.Point(298, 454);
            this.brushRadius.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.brushRadius.Name = "brushRadius";
            this.brushRadius.Size = new System.Drawing.Size(90, 20);
            this.brushRadius.TabIndex = 31;
            this.brushRadius.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.brushRadius.ValueChanged += new System.EventHandler(this.brushRadius_ValueChanged);
            // 
            // loadConfig
            // 
            this.loadConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.loadConfig.Location = new System.Drawing.Point(12, 422);
            this.loadConfig.Name = "loadConfig";
            this.loadConfig.Size = new System.Drawing.Size(88, 23);
            this.loadConfig.TabIndex = 32;
            this.loadConfig.Text = "Load config";
            this.loadConfig.UseVisualStyleBackColor = true;
            this.loadConfig.Click += new System.EventHandler(this.loadConfig_Click);
            // 
            // saveConfig
            // 
            this.saveConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveConfig.Location = new System.Drawing.Point(106, 422);
            this.saveConfig.Name = "saveConfig";
            this.saveConfig.Size = new System.Drawing.Size(75, 23);
            this.saveConfig.TabIndex = 33;
            this.saveConfig.Text = "Save config";
            this.saveConfig.UseVisualStyleBackColor = true;
            this.saveConfig.Click += new System.EventHandler(this.saveConfig_Click);
            // 
            // saveIniDialog
            // 
            this.saveIniDialog.Filter = "INI File|*.ini";
            this.saveIniDialog.Title = "Save INI";
            // 
            // loadIniDialog
            // 
            this.loadIniDialog.Filter = "INI File|*.ini";
            // 
            // computeCostMapNoWC
            // 
            this.computeCostMapNoWC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.computeCostMapNoWC.Location = new System.Drawing.Point(480, 480);
            this.computeCostMapNoWC.Name = "computeCostMapNoWC";
            this.computeCostMapNoWC.Size = new System.Drawing.Size(129, 23);
            this.computeCostMapNoWC.TabIndex = 34;
            this.computeCostMapNoWC.Text = "Cost map (no wall clip)";
            this.computeCostMapNoWC.UseVisualStyleBackColor = true;
            this.computeCostMapNoWC.Click += new System.EventHandler(this.computeCostMapNoWC_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 594);
            this.Controls.Add(this.computeCostMapNoWC);
            this.Controls.Add(this.saveConfig);
            this.Controls.Add(this.loadConfig);
            this.Controls.Add(this.brushRadius);
            this.Controls.Add(this.destroySolver);
            this.Controls.Add(this.setTarget);
            this.Controls.Add(this.setConstraints);
            this.Controls.Add(this.drawOnMap);
            this.Controls.Add(this.abort);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buildSolverAny);
            this.Controls.Add(this.convertInputsToBk2);
            this.Controls.Add(this.solve);
            this.Controls.Add(this.computeCostMap);
            this.Controls.Add(this.buildSolver);
            this.Controls.Add(this.showCostMap);
            this.Controls.Add(this.saveMap);
            this.Controls.Add(this.loadMap);
            this.Controls.Add(this.savePos);
            this.Controls.Add(this.loadPos);
            this.Controls.Add(this.saveInputs);
            this.Controls.Add(this.logLastMoves);
            this.Controls.Add(this.sendLastInputs);
            this.Controls.Add(this.convertInputs);
            this.Controls.Add(this.restorePos);
            this.Controls.Add(this.backupPos);
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
            this.MinimumSize = new System.Drawing.Size(831, 39);
            this.Name = "Form1";
            this.Text = "KuruBot 1.2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.brushRadius)).EndInit();
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
        private System.Windows.Forms.OpenFileDialog loadFileDialog;
        private System.Windows.Forms.Button backupPos;
        private System.Windows.Forms.Button restorePos;
        private System.Windows.Forms.Button convertInputs;
        private System.Windows.Forms.Button sendLastInputs;
        private System.Windows.Forms.Button logLastMoves;
        private System.Windows.Forms.SaveFileDialog saveLogFileDialog;
        private System.Windows.Forms.Button saveInputs;
        private System.Windows.Forms.Button loadPos;
        private System.Windows.Forms.Button savePos;
        private System.Windows.Forms.Button loadMap;
        private System.Windows.Forms.Button saveMap;
        private System.Windows.Forms.CheckBox showCostMap;
        private System.Windows.Forms.Button buildSolver;
        private System.Windows.Forms.Button computeCostMap;
        private System.Windows.Forms.Button solve;
        private System.Windows.Forms.Button convertInputsToBk2;
        private System.Windows.Forms.Button buildSolverAny;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button abort;
        private System.Windows.Forms.CheckBox drawOnMap;
        private System.Windows.Forms.Button setConstraints;
        private System.Windows.Forms.Button setTarget;
        private System.Windows.Forms.Button destroySolver;
        private System.Windows.Forms.NumericUpDown brushRadius;
        private System.Windows.Forms.Button loadConfig;
        private System.Windows.Forms.Button saveConfig;
        private System.Windows.Forms.SaveFileDialog saveIniDialog;
        private System.Windows.Forms.OpenFileDialog loadIniDialog;
        private System.Windows.Forms.Button computeCostMapNoWC;
    }
}

