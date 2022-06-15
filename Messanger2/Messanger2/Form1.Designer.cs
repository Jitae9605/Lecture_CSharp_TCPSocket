
namespace Messanger2
{
	partial class Form1
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.tsbtnOption = new System.Windows.Forms.ToolStripDropDownButton();
			this.설정ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.닫기ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tsbtnConn = new System.Windows.Forms.ToolStripButton();
			this.tsbtnDisconn = new System.Windows.Forms.ToolStripButton();
			this.plOption = new System.Windows.Forms.Panel();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.cbServer = new System.Windows.Forms.CheckBox();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.txtId = new System.Windows.Forms.TextBox();
			this.txtIp = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.btnSender = new System.Windows.Forms.Button();
			this.rtbText = new System.Windows.Forms.RichTextBox();
			this.toolStrip1.SuspendLayout();
			this.plOption.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbtnOption,
            this.tsbtnConn,
            this.tsbtnDisconn});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(357, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// tsbtnOption
			// 
			this.tsbtnOption.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnOption.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.설정ToolStripMenuItem,
            this.toolStripSeparator1,
            this.닫기ToolStripMenuItem});
			this.tsbtnOption.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnOption.Image")));
			this.tsbtnOption.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnOption.Name = "tsbtnOption";
			this.tsbtnOption.Size = new System.Drawing.Size(29, 22);
			this.tsbtnOption.Text = "toolStripDropDownButton1";
			this.tsbtnOption.ToolTipText = "환경설정";
			// 
			// 설정ToolStripMenuItem
			// 
			this.설정ToolStripMenuItem.Name = "설정ToolStripMenuItem";
			this.설정ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.설정ToolStripMenuItem.Text = "설정";
			this.설정ToolStripMenuItem.Click += new System.EventHandler(this.설정ToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(95, 6);
			// 
			// 닫기ToolStripMenuItem
			// 
			this.닫기ToolStripMenuItem.Name = "닫기ToolStripMenuItem";
			this.닫기ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.닫기ToolStripMenuItem.Text = "닫기";
			this.닫기ToolStripMenuItem.Click += new System.EventHandler(this.닫기ToolStripMenuItem_Click);
			// 
			// tsbtnConn
			// 
			this.tsbtnConn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnConn.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnConn.Image")));
			this.tsbtnConn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnConn.Name = "tsbtnConn";
			this.tsbtnConn.Size = new System.Drawing.Size(23, 22);
			this.tsbtnConn.Text = "연결";
			this.tsbtnConn.Click += new System.EventHandler(this.tsbtnConn_Click);
			// 
			// tsbtnDisconn
			// 
			this.tsbtnDisconn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbtnDisconn.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnDisconn.Image")));
			this.tsbtnDisconn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbtnDisconn.Name = "tsbtnDisconn";
			this.tsbtnDisconn.Size = new System.Drawing.Size(23, 22);
			this.tsbtnDisconn.Text = "연결끊기";
			this.tsbtnDisconn.Click += new System.EventHandler(this.tsbtnDisconn_Click);
			// 
			// plOption
			// 
			this.plOption.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.plOption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.plOption.Controls.Add(this.btnClose);
			this.plOption.Controls.Add(this.btnSave);
			this.plOption.Controls.Add(this.cbServer);
			this.plOption.Controls.Add(this.txtPort);
			this.plOption.Controls.Add(this.txtId);
			this.plOption.Controls.Add(this.txtIp);
			this.plOption.Controls.Add(this.label4);
			this.plOption.Controls.Add(this.label3);
			this.plOption.Controls.Add(this.label2);
			this.plOption.Location = new System.Drawing.Point(16, 57);
			this.plOption.Name = "plOption";
			this.plOption.Size = new System.Drawing.Size(327, 210);
			this.plOption.TabIndex = 1;
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(174, 140);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(80, 40);
			this.btnClose.TabIndex = 5;
			this.btnClose.Text = "닫기";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(71, 140);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(80, 40);
			this.btnSave.TabIndex = 5;
			this.btnSave.Text = "설정";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// cbServer
			// 
			this.cbServer.AutoSize = true;
			this.cbServer.Location = new System.Drawing.Point(32, 118);
			this.cbServer.Name = "cbServer";
			this.cbServer.Size = new System.Drawing.Size(72, 16);
			this.cbServer.TabIndex = 4;
			this.cbServer.Text = "서버실행";
			this.cbServer.UseVisualStyleBackColor = true;
			this.cbServer.CheckedChanged += new System.EventHandler(this.cbServer_CheckedChanged);
			// 
			// txtPort
			// 
			this.txtPort.BackColor = System.Drawing.SystemColors.Control;
			this.txtPort.Location = new System.Drawing.Point(97, 91);
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(192, 21);
			this.txtPort.TabIndex = 3;
			// 
			// txtId
			// 
			this.txtId.Location = new System.Drawing.Point(97, 64);
			this.txtId.Name = "txtId";
			this.txtId.Size = new System.Drawing.Size(192, 21);
			this.txtId.TabIndex = 3;
			// 
			// txtIp
			// 
			this.txtIp.Location = new System.Drawing.Point(97, 37);
			this.txtIp.Name = "txtIp";
			this.txtIp.Size = new System.Drawing.Size(192, 21);
			this.txtIp.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(30, 94);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(38, 12);
			this.label4.TabIndex = 2;
			this.label4.Text = "PORT";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(30, 67);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(16, 12);
			this.label3.TabIndex = 2;
			this.label3.Text = "ID";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(30, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(16, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "IP";
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.LightSkyBlue;
			this.panel2.Controls.Add(this.panel3);
			this.panel2.Controls.Add(this.btnSender);
			this.panel2.Location = new System.Drawing.Point(0, 332);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(357, 82);
			this.panel2.TabIndex = 6;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.SystemColors.Control;
			this.panel3.Controls.Add(this.txtMessage);
			this.panel3.Location = new System.Drawing.Point(13, 12);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(214, 54);
			this.panel3.TabIndex = 7;
			// 
			// txtMessage
			// 
			this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtMessage.Location = new System.Drawing.Point(13, 12);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(189, 27);
			this.txtMessage.TabIndex = 0;
			this.txtMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMessage_KeyPress);
			// 
			// btnSender
			// 
			this.btnSender.Location = new System.Drawing.Point(233, 12);
			this.btnSender.Name = "btnSender";
			this.btnSender.Size = new System.Drawing.Size(110, 54);
			this.btnSender.TabIndex = 5;
			this.btnSender.Text = "보내기";
			this.btnSender.UseVisualStyleBackColor = true;
			this.btnSender.Click += new System.EventHandler(this.btnSender_Click);
			// 
			// rtbText
			// 
			this.rtbText.Location = new System.Drawing.Point(0, 28);
			this.rtbText.Name = "rtbText";
			this.rtbText.Size = new System.Drawing.Size(357, 298);
			this.rtbText.TabIndex = 7;
			this.rtbText.Text = "";
			//this.rtbText.TextChanged += new System.EventHandler(this.rtbText_TextChanged);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(357, 413);
			this.Controls.Add(this.plOption);
			this.Controls.Add(this.rtbText);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.toolStrip1);
			this.Name = "Form1";
			this.Text = "1:1 채팅";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.plOption.ResumeLayout(false);
			this.plOption.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripDropDownButton tsbtnOption;
		private System.Windows.Forms.ToolStripButton tsbtnConn;
		private System.Windows.Forms.ToolStripButton tsbtnDisconn;
		private System.Windows.Forms.ToolStripMenuItem 설정ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 닫기ToolStripMenuItem;
		private System.Windows.Forms.Panel plOption;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.CheckBox cbServer;
		private System.Windows.Forms.TextBox txtPort;
		private System.Windows.Forms.TextBox txtId;
		private System.Windows.Forms.TextBox txtIp;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.Button btnSender;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.RichTextBox rtbText;
	}
}

