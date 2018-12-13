namespace TestFlow
{
    partial class FormUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUI));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Variant", 2, 2);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Variable", 2, 2);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Testmodes", 2, 2);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("测试环境", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("测试序列", 0, 1);
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.menuStripTest = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTest = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparatorA = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewMain = new System.Windows.Forms.TreeView();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageTree = new System.Windows.Forms.TabPage();
            this.tabPageConfiguration = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabControlRun = new System.Windows.Forms.TabControl();
            this.tabPageParameters = new System.Windows.Forms.TabPage();
            this.tabPageNotes = new System.Windows.Forms.TabPage();
            this.tabPageLoopSetUp = new System.Windows.Forms.TabPage();
            this.menuStripTest.SuspendLayout();
            this.toolStripTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabPageTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabControlRun.SuspendLayout();
            this.tabPageParameters.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripMain
            // 
            this.statusStripMain.Location = new System.Drawing.Point(0, 766);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(1417, 22);
            this.statusStripMain.TabIndex = 0;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // menuStripTest
            // 
            this.menuStripTest.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.windowsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripTest.Location = new System.Drawing.Point(0, 0);
            this.menuStripTest.Name = "menuStripTest";
            this.menuStripTest.Size = new System.Drawing.Size(1417, 25);
            this.menuStripTest.TabIndex = 1;
            this.menuStripTest.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(66, 21);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(59, 21);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(73, 21);
            this.windowsToolStripMenuItem.Text = "Windows";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // toolStripTest
            // 
            this.toolStripTest.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNew,
            this.toolStripButtonOpen,
            this.toolStripButtonSave,
            this.toolStripSeparatorA});
            this.toolStripTest.Location = new System.Drawing.Point(0, 25);
            this.toolStripTest.Name = "toolStripTest";
            this.toolStripTest.Size = new System.Drawing.Size(1417, 25);
            this.toolStripTest.TabIndex = 2;
            this.toolStripTest.Text = "toolStrip1";
            // 
            // toolStripButtonNew
            // 
            this.toolStripButtonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonNew.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonNew.Image")));
            this.toolStripButtonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNew.Name = "toolStripButtonNew";
            this.toolStripButtonNew.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonNew.Text = "toolStripButton1";
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpen.Image")));
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOpen.Text = "toolStripButton1";
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSave.Text = "toolStripButton1";
            // 
            // toolStripSeparatorA
            // 
            this.toolStripSeparatorA.Name = "toolStripSeparatorA";
            this.toolStripSeparatorA.Size = new System.Drawing.Size(6, 25);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 50);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControlMain);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControlRun);
            this.splitContainer1.Size = new System.Drawing.Size(1417, 716);
            this.splitContainer1.SplitterDistance = 435;
            this.splitContainer1.SplitterWidth = 7;
            this.splitContainer1.TabIndex = 12;
            // 
            // treeViewMain
            // 
            this.treeViewMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.treeViewMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewMain.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeViewMain.ForeColor = System.Drawing.Color.Black;
            this.treeViewMain.HideSelection = false;
            this.treeViewMain.Location = new System.Drawing.Point(3, 3);
            this.treeViewMain.Name = "treeViewMain";
            treeNode1.ImageIndex = 2;
            treeNode1.Name = "Variant";
            treeNode1.NodeFont = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            treeNode1.SelectedImageIndex = 2;
            treeNode1.Text = "Variant";
            treeNode2.ImageIndex = 2;
            treeNode2.Name = "Variable";
            treeNode2.NodeFont = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            treeNode2.SelectedImageIndex = 2;
            treeNode2.Text = "Variable";
            treeNode3.ImageIndex = 2;
            treeNode3.Name = "Testmodes";
            treeNode3.NodeFont = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            treeNode3.SelectedImageIndex = 2;
            treeNode3.Text = "Testmodes";
            treeNode4.ImageIndex = 0;
            treeNode4.Name = "Definitions";
            treeNode4.NodeFont = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            treeNode4.SelectedImageIndex = 0;
            treeNode4.Text = "测试环境";
            treeNode5.ImageIndex = 0;
            treeNode5.Name = "TestFlow";
            treeNode5.NodeFont = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            treeNode5.SelectedImageIndex = 1;
            treeNode5.Text = "测试序列";
            this.treeViewMain.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5});
            this.treeViewMain.ShowLines = false;
            this.treeViewMain.Size = new System.Drawing.Size(419, 682);
            this.treeViewMain.TabIndex = 2;
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageTree);
            this.tabControlMain.Controls.Add(this.tabPageConfiguration);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(433, 714);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabPageTree
            // 
            this.tabPageTree.Controls.Add(this.treeViewMain);
            this.tabPageTree.Location = new System.Drawing.Point(4, 22);
            this.tabPageTree.Name = "tabPageTree";
            this.tabPageTree.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTree.Size = new System.Drawing.Size(425, 688);
            this.tabPageTree.TabIndex = 0;
            this.tabPageTree.Text = "TreeView";
            this.tabPageTree.UseVisualStyleBackColor = true;
            // 
            // tabPageConfiguration
            // 
            this.tabPageConfiguration.Location = new System.Drawing.Point(4, 22);
            this.tabPageConfiguration.Name = "tabPageConfiguration";
            this.tabPageConfiguration.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfiguration.Size = new System.Drawing.Size(425, 688);
            this.tabPageConfiguration.TabIndex = 1;
            this.tabPageConfiguration.Text = "Tester Configuration";
            this.tabPageConfiguration.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(959, 682);
            this.dataGridView1.TabIndex = 0;
            // 
            // tabControlRun
            // 
            this.tabControlRun.Controls.Add(this.tabPageParameters);
            this.tabControlRun.Controls.Add(this.tabPageNotes);
            this.tabControlRun.Controls.Add(this.tabPageLoopSetUp);
            this.tabControlRun.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlRun.Location = new System.Drawing.Point(0, 0);
            this.tabControlRun.Name = "tabControlRun";
            this.tabControlRun.SelectedIndex = 0;
            this.tabControlRun.Size = new System.Drawing.Size(973, 714);
            this.tabControlRun.TabIndex = 0;
            // 
            // tabPageParameters
            // 
            this.tabPageParameters.Controls.Add(this.dataGridView1);
            this.tabPageParameters.Location = new System.Drawing.Point(4, 22);
            this.tabPageParameters.Name = "tabPageParameters";
            this.tabPageParameters.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageParameters.Size = new System.Drawing.Size(965, 688);
            this.tabPageParameters.TabIndex = 0;
            this.tabPageParameters.Text = "Parameters";
            this.tabPageParameters.UseVisualStyleBackColor = true;
            // 
            // tabPageNotes
            // 
            this.tabPageNotes.Location = new System.Drawing.Point(4, 22);
            this.tabPageNotes.Name = "tabPageNotes";
            this.tabPageNotes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNotes.Size = new System.Drawing.Size(965, 688);
            this.tabPageNotes.TabIndex = 1;
            this.tabPageNotes.Text = "Notes";
            this.tabPageNotes.UseVisualStyleBackColor = true;
            // 
            // tabPageLoopSetUp
            // 
            this.tabPageLoopSetUp.Location = new System.Drawing.Point(4, 22);
            this.tabPageLoopSetUp.Name = "tabPageLoopSetUp";
            this.tabPageLoopSetUp.Size = new System.Drawing.Size(965, 688);
            this.tabPageLoopSetUp.TabIndex = 2;
            this.tabPageLoopSetUp.Text = "Loop Setup";
            this.tabPageLoopSetUp.UseVisualStyleBackColor = true;
            // 
            // FormUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1417, 788);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStripTest);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.menuStripTest);
            this.MainMenuStrip = this.menuStripTest;
            this.Name = "FormUI";
            this.Text = "通用测试平台";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormUI_Load);
            this.menuStripTest.ResumeLayout(false);
            this.menuStripTest.PerformLayout();
            this.toolStripTest.ResumeLayout(false);
            this.toolStripTest.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageTree.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabControlRun.ResumeLayout(false);
            this.tabPageParameters.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.MenuStrip menuStripTest;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStripTest;
        private System.Windows.Forms.ToolStripButton toolStripButtonNew;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorA;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeViewMain;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageTree;
        private System.Windows.Forms.TabPage tabPageConfiguration;
        private System.Windows.Forms.TabControl tabControlRun;
        private System.Windows.Forms.TabPage tabPageParameters;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage tabPageNotes;
        private System.Windows.Forms.TabPage tabPageLoopSetUp;
    }
}

