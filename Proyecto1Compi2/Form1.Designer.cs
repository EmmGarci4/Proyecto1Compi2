namespace Proyecto1Compi2
{
	partial class Form1
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.Bt_EjecutarLup = new System.Windows.Forms.ToolStripButton();
			this.Btn_leerXml = new System.Windows.Forms.ToolStripButton();
			this.Btn_LimpiarDB = new System.Windows.Forms.ToolStripButton();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			Form1.textBox1 = new System.Windows.Forms.TextBox();
			this.toolStrip1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Bt_EjecutarLup,
            this.Btn_leerXml,
            this.Btn_LimpiarDB});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.toolStrip1.Size = new System.Drawing.Size(800, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// Bt_EjecutarLup
			// 
			this.Bt_EjecutarLup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.Bt_EjecutarLup.Image = ((System.Drawing.Image)(resources.GetObject("Bt_EjecutarLup.Image")));
			this.Bt_EjecutarLup.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.Bt_EjecutarLup.Name = "Bt_EjecutarLup";
			this.Bt_EjecutarLup.Size = new System.Drawing.Size(76, 22);
			this.Bt_EjecutarLup.Text = "Ejecutar Lup";
			this.Bt_EjecutarLup.Click += new System.EventHandler(this.Bt_EjecutarLup_Click_1);
			// 
			// Btn_leerXml
			// 
			this.Btn_leerXml.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.Btn_leerXml.Image = ((System.Drawing.Image)(resources.GetObject("Btn_leerXml.Image")));
			this.Btn_leerXml.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.Btn_leerXml.Name = "Btn_leerXml";
			this.Btn_leerXml.Size = new System.Drawing.Size(93, 22);
			this.Btn_leerXml.Text = "Ejecutar Chison";
			this.Btn_leerXml.Click += new System.EventHandler(this.Btn_leerXml_Click);
			// 
			// Btn_LimpiarDB
			// 
			this.Btn_LimpiarDB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.Btn_LimpiarDB.Image = ((System.Drawing.Image)(resources.GetObject("Btn_LimpiarDB.Image")));
			this.Btn_LimpiarDB.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.Btn_LimpiarDB.Name = "Btn_LimpiarDB";
			this.Btn_LimpiarDB.Size = new System.Drawing.Size(69, 22);
			this.Btn_LimpiarDB.Text = "Limpiar DB";
			this.Btn_LimpiarDB.Click += new System.EventHandler(this.Btn_LimpiarDB_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.richTextBox1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(Form1.textBox1, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 425);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// richTextBox1
			// 
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Font = new System.Drawing.Font("Consolas", 11F);
			this.richTextBox1.Location = new System.Drawing.Point(3, 3);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(794, 206);
			this.richTextBox1.TabIndex = 2;
			this.richTextBox1.Text = "";
			// 
			// textBox1
			// 
			Form1.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			Form1.textBox1.Location = new System.Drawing.Point(3, 215);
			Form1.textBox1.Multiline = true;
			Form1.textBox1.Name = "textBox1";
			Form1.textBox1.Size = new System.Drawing.Size(794, 207);
			Form1.textBox1.TabIndex = 3;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.toolStrip1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.ToolStripButton Btn_leerXml;
		private System.Windows.Forms.ToolStripButton Bt_EjecutarLup;
		private System.Windows.Forms.ToolStripButton Btn_LimpiarDB;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private static System.Windows.Forms.TextBox textBox1;
	}
}

