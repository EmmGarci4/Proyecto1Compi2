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
			this.Btn_EjecutarSql = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.Btn_leerXml = new System.Windows.Forms.ToolStripButton();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.Btn_Probar = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Btn_EjecutarSql,
            this.toolStripButton2,
            this.Btn_leerXml,
            this.Btn_Probar});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.toolStrip1.Size = new System.Drawing.Size(800, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// Btn_EjecutarSql
			// 
			this.Btn_EjecutarSql.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.Btn_EjecutarSql.Image = ((System.Drawing.Image)(resources.GetObject("Btn_EjecutarSql.Image")));
			this.Btn_EjecutarSql.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.Btn_EjecutarSql.Name = "Btn_EjecutarSql";
			this.Btn_EjecutarSql.Size = new System.Drawing.Size(77, 22);
			this.Btn_EjecutarSql.Text = "Ejecutar SQL";
			this.Btn_EjecutarSql.Click += new System.EventHandler(this.ToolStripButton1_Click);
			// 
			// toolStripButton2
			// 
			this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
			this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new System.Drawing.Size(67, 22);
			this.toolStripButton2.Text = "GenerarDB";
			this.toolStripButton2.Click += new System.EventHandler(this.ToolStripButton2_Click);
			// 
			// Btn_leerXml
			// 
			this.Btn_leerXml.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.Btn_leerXml.Image = ((System.Drawing.Image)(resources.GetObject("Btn_leerXml.Image")));
			this.Btn_leerXml.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.Btn_leerXml.Name = "Btn_leerXml";
			this.Btn_leerXml.Size = new System.Drawing.Size(54, 22);
			this.Btn_leerXml.Text = "LeerXml";
			this.Btn_leerXml.Click += new System.EventHandler(this.Btn_leerXml_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.textBox1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.textBox2, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 425);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// textBox1
			// 
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox1.Location = new System.Drawing.Point(3, 3);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(794, 206);
			this.textBox1.TabIndex = 0;
			// 
			// textBox2
			// 
			this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox2.Location = new System.Drawing.Point(3, 215);
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(794, 207);
			this.textBox2.TabIndex = 1;
			// 
			// Btn_Probar
			// 
			this.Btn_Probar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.Btn_Probar.Image = ((System.Drawing.Image)(resources.GetObject("Btn_Probar.Image")));
			this.Btn_Probar.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.Btn_Probar.Name = "Btn_Probar";
			this.Btn_Probar.Size = new System.Drawing.Size(46, 22);
			this.Btn_Probar.Text = "Probar";
			this.Btn_Probar.Click += new System.EventHandler(this.Btn_Probar_Click);
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
		private System.Windows.Forms.ToolStripButton Btn_EjecutarSql;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.ToolStripButton Btn_leerXml;
		private System.Windows.Forms.ToolStripButton Btn_Probar;
	}
}

