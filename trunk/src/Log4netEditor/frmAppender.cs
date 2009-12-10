using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using Log4netConfigConsulter;

namespace Log4netEditor
{
	/// <summary>
	/// frmAppender ªººK­n´y­z¡C
	/// </summary>
	public class frmAppender : Form
	{
		private bool bSkipScrollChangeEvent;
		private const string msCONST_DefaultAppender = "OutputDebugStringAppender";
		private const int mnCONST_SpacingInControls = 5;
		private AppenderConsulter moConsulter;
		private XmlDocument moXmlDoc;
		private string msAliasName;

		#region Form Controls
		private Button btnCancel;
		private Button btnSave;
		private IContainer components;
		private ComboBox ddlAppenderClasses;
		private GroupBox gpArgContainer;
		private Label lblAlias;
		private Label lblAppenderClass;
		private Label lblDesc;
		private Panel pnlArguments;
		private ToolTip toolTip;
		private TextBox txtAlias;
		private TextBox txtDesc;
		private System.Windows.Forms.GroupBox gbLayout;
		private System.Windows.Forms.Label lblLayoutType;
		private System.Windows.Forms.ComboBox ddlLayoutType;
		private System.Windows.Forms.TextBox txtConversionPattern;
		private System.Windows.Forms.Label lblCnsnPtn;
		private System.Windows.Forms.Label lblPtnPreview;
		private System.Windows.Forms.Label lblPreviewResult;
		private System.Windows.Forms.Label lblDemoString;
		private System.Windows.Forms.TextBox txtDemoString;
        private System.Windows.Forms.Button btnPatternHelp;
		#endregion

		public XmlDocument Current_Log4net_config_XmlDoc
		{
			get { return moXmlDoc; }
		}
		public frmAppender(XmlDocument log4net_config_XmlDoc)
		{
			bSkipScrollChangeEvent = true;
			moXmlDoc = log4net_config_XmlDoc;
			InitializeComponent();
			InitAppenderDropDownList();
		}

		private void ArrangeControls()
		{
			int nTop = 0;
			int nLeftSpace = (int) (pnlArguments.Width * 0.1);
			if (pnlArguments.Controls.Count > 0)
			{
				foreach (Control ArgControl in pnlArguments.Controls)
				{
					if (gbLayout != ArgControl)
					{
						ArgControl.Visible = true;
						ArgControl.Width = (int) (pnlArguments.Width * 0.8);
						ArgControl.Left = nLeftSpace;
					}
					ArgControl.Top = nTop;
					nTop += ArgControl.Height + 5;
				}
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			base.Close();
			base.Dispose();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			XmlNode oCurrentAppender = null;
			msAliasName = txtAlias.Text;

			#region Update arguments
			foreach (Control control1 in pnlArguments.Controls)
			{
				if (!(control1 is Label))
				{
					if (control1 is TextBox)
					{
						moConsulter.SearchUpdateArg((string) control1.Tag, ((TextBox) control1).Text);
					}
					else if (control1 is ComboBox)
					{
						moConsulter.SearchUpdateArg((string) control1.Tag, ((ComboBox) control1).Text);
					}
					else if (control1 is ParameterGrid)
					{
						moConsulter.SearchUpdateArg((string) control1.Tag, string.Empty).ParameterXml = ((ParameterGrid)control1).ParameterXmlNodes;
					}
				}
			}
			#endregion

			#region Update Layout & pattern
			moConsulter.SearchUpdateArg("layout", ddlLayoutType.Text);
			moConsulter.SearchUpdateArg("conversionpattern", txtConversionPattern.Text);
			#endregion

			#region Update & Save log4net.config for this appender
			foreach (XmlNode oAppender in moXmlDoc.SelectNodes("//appender"))
			{
				if (oAppender.Attributes["name"].Value == msAliasName)
				{
					oCurrentAppender = oAppender;
					break;
				}
			}
			try
			{
				if (oCurrentAppender == null)
				{
					oCurrentAppender = moXmlDoc.CreateElement("appender");
					XmlAttribute oAttri = moXmlDoc.CreateAttribute("name");
					oAttri.Value = msAliasName;
					oCurrentAppender.Attributes.Append(oAttri);
					oAttri = moXmlDoc.CreateAttribute("type");
					oAttri.Value = "log4net.Appender." + ddlAppenderClasses.Text;
					oCurrentAppender.Attributes.Append(oAttri);
					oCurrentAppender.InnerXml = moConsulter.GetConfigXML();
					moXmlDoc.DocumentElement.InsertBefore(oCurrentAppender, moXmlDoc.DocumentElement.FirstChild);
				}
				else
				{
					oCurrentAppender.Attributes["type"].Value = "log4net.Appender." + ddlAppenderClasses.Text;
					oCurrentAppender.InnerXml = moConsulter.GetConfigXML();
				}
			}
			catch (Exception oEX)
			{
				MessageBox.Show(this, oEX.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			#endregion
			DialogResult = DialogResult.OK;

			base.Close();
		}

		private void ddlAppenderClasses_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bSkipScrollChangeEvent)
			{
				bSkipScrollChangeEvent = false;
			}
			else
			{
				moConsulter = null;
				InitGroupArgument();
			}
		}

		public void EditExistedAppender(string sAlias)
		{
			XmlNode node1 = null;
			msAliasName = sAlias;
			foreach (XmlNode node2 in moXmlDoc.SelectNodes("//appender"))
			{
				if (node2.Attributes["name"].Value == msAliasName)
				{
					node1 = node2;
					break;
				}
			}
			if (node1 == null)
			{
				throw new ApplicationException("This Appender named '" + msAliasName + "' is not existed in this log4net config file.");
			}
			txtAlias.Text = msAliasName;
			txtAlias.ReadOnly = true;
			try
			{
				string[] textArray1 = node1.Attributes["type"].Value.Split(new char[] { '.' });
				bSkipScrollChangeEvent = true;
				ddlAppenderClasses.Text = textArray1[textArray1.Length - 1];
				moConsulter = AppenderConsulter.GetAppender(textArray1[textArray1.Length - 1]);
				if (null == moConsulter)
				{
					moConsulter = AppenderConsulter.GetAppender(msCONST_DefaultAppender);
				}
				moConsulter.RestoreArgsFromXml(node1);
			}
			catch (InvalidCastException)
			{
				moConsulter = null;
			}
			InitGroupArgument();
		}

		private void InitAppenderDropDownList()
		{
			ddlAppenderClasses.DataSource = Helper.GetAppenders();
		}

		private void InitGroupArgument()
		{
			if (moConsulter == null)
			{
				try
				{
					moConsulter = AppenderConsulter.GetAppender(ddlAppenderClasses.Text);
				}
				catch (InvalidCastException)
				{
					moConsulter = null;
				}
			}
			pnlArguments.Controls.Clear();
			if (moConsulter == null)
			{
				txtDesc.Text = "Default basic consulter can't work.";
				btnSave.Enabled = false;
			}
			else
			{
				txtDesc.Text = moConsulter.GetAppenderDesc();
			}
			btnSave.Enabled = true;
			ArgumentStruct[] structArray1 = moConsulter.Arguments;
			if (structArray1 != null)
			{
				foreach (ArgumentStruct struct1 in structArray1)
				{
					RecursiveGenerateControls(struct1);
				}
			}
			if (Constants.msCONST_NOLAYOUT_APPENDER.ToLower() != ddlAppenderClasses.Text.ToLower())
			{
				pnlArguments.Controls.Add(gbLayout);
			}
			ArrangeControls();
		}


		#region Code generated by Visual Studio 2003
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.lblAlias = new System.Windows.Forms.Label();
            this.lblAppenderClass = new System.Windows.Forms.Label();
            this.ddlAppenderClasses = new System.Windows.Forms.ComboBox();
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblDesc = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnPatternHelp = new System.Windows.Forms.Button();
            this.gpArgContainer = new System.Windows.Forms.GroupBox();
            this.pnlArguments = new System.Windows.Forms.Panel();
            this.gbLayout = new System.Windows.Forms.GroupBox();
            this.txtDemoString = new System.Windows.Forms.TextBox();
            this.lblDemoString = new System.Windows.Forms.Label();
            this.lblPreviewResult = new System.Windows.Forms.Label();
            this.lblPtnPreview = new System.Windows.Forms.Label();
            this.lblCnsnPtn = new System.Windows.Forms.Label();
            this.txtConversionPattern = new System.Windows.Forms.TextBox();
            this.ddlLayoutType = new System.Windows.Forms.ComboBox();
            this.lblLayoutType = new System.Windows.Forms.Label();
            this.gpArgContainer.SuspendLayout();
            this.pnlArguments.SuspendLayout();
            this.gbLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblAlias
            // 
            this.lblAlias.AutoSize = true;
            this.lblAlias.Location = new System.Drawing.Point(8, 48);
            this.lblAlias.Name = "lblAlias";
            this.lblAlias.Size = new System.Drawing.Size(47, 15);
            this.lblAlias.TabIndex = 0;
            this.lblAlias.Text = "Name :";
            // 
            // lblAppenderClass
            // 
            this.lblAppenderClass.AutoSize = true;
            this.lblAppenderClass.Location = new System.Drawing.Point(8, 104);
            this.lblAppenderClass.Name = "lblAppenderClass";
            this.lblAppenderClass.Size = new System.Drawing.Size(95, 15);
            this.lblAppenderClass.TabIndex = 1;
            this.lblAppenderClass.Text = "Appender Type :";
            // 
            // ddlAppenderClasses
            // 
            this.ddlAppenderClasses.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlAppenderClasses.Location = new System.Drawing.Point(40, 128);
            this.ddlAppenderClasses.Name = "ddlAppenderClasses";
            this.ddlAppenderClasses.Size = new System.Drawing.Size(368, 23);
            this.ddlAppenderClasses.TabIndex = 2;
            this.ddlAppenderClasses.SelectedIndexChanged += new System.EventHandler(this.ddlAppenderClasses_SelectedIndexChanged);
            // 
            // txtAlias
            // 
            this.txtAlias.Location = new System.Drawing.Point(40, 80);
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(368, 21);
            this.txtAlias.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(392, 552);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "&Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(312, 552);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblDesc
            // 
            this.lblDesc.AutoSize = true;
            this.lblDesc.Location = new System.Drawing.Point(8, 160);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(76, 15);
            this.lblDesc.TabIndex = 0;
            this.lblDesc.Text = "Description :";
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(40, 184);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.ReadOnly = true;
            this.txtDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDesc.Size = new System.Drawing.Size(424, 56);
            this.txtDesc.TabIndex = 7;
            // 
            // btnPatternHelp
            // 
            this.btnPatternHelp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPatternHelp.Location = new System.Drawing.Point(384, 8);
            this.btnPatternHelp.Name = "btnPatternHelp";
            this.btnPatternHelp.Size = new System.Drawing.Size(32, 23);
            this.btnPatternHelp.TabIndex = 8;
            this.btnPatternHelp.Text = "?";
            this.toolTip.SetToolTip(this.btnPatternHelp, "How to use?");
            this.btnPatternHelp.Click += new System.EventHandler(this.btnPatternHelp_Click);
            // 
            // gpArgContainer
            // 
            this.gpArgContainer.Controls.Add(this.pnlArguments);
            this.gpArgContainer.Location = new System.Drawing.Point(8, 248);
            this.gpArgContainer.Name = "gpArgContainer";
            this.gpArgContainer.Size = new System.Drawing.Size(456, 296);
            this.gpArgContainer.TabIndex = 8;
            this.gpArgContainer.TabStop = false;
            this.gpArgContainer.Text = "Arguments";
            // 
            // pnlArguments
            // 
            this.pnlArguments.AutoScroll = true;
            this.pnlArguments.Controls.Add(this.gbLayout);
            this.pnlArguments.Location = new System.Drawing.Point(8, 17);
            this.pnlArguments.Name = "pnlArguments";
            this.pnlArguments.Size = new System.Drawing.Size(442, 271);
            this.pnlArguments.TabIndex = 0;
            // 
            // gbLayout
            // 
            this.gbLayout.Controls.Add(this.btnPatternHelp);
            this.gbLayout.Controls.Add(this.txtDemoString);
            this.gbLayout.Controls.Add(this.lblDemoString);
            this.gbLayout.Controls.Add(this.lblPreviewResult);
            this.gbLayout.Controls.Add(this.lblPtnPreview);
            this.gbLayout.Controls.Add(this.lblCnsnPtn);
            this.gbLayout.Controls.Add(this.txtConversionPattern);
            this.gbLayout.Controls.Add(this.ddlLayoutType);
            this.gbLayout.Controls.Add(this.lblLayoutType);
            this.gbLayout.Location = new System.Drawing.Point(0, 0);
            this.gbLayout.Name = "gbLayout";
            this.gbLayout.Size = new System.Drawing.Size(424, 272);
            this.gbLayout.TabIndex = 1;
            this.gbLayout.TabStop = false;
            this.gbLayout.Text = "Layout";
            // 
            // txtDemoString
            // 
            this.txtDemoString.Location = new System.Drawing.Point(32, 168);
            this.txtDemoString.Name = "txtDemoString";
            this.txtDemoString.Size = new System.Drawing.Size(376, 21);
            this.txtDemoString.TabIndex = 7;
            this.txtDemoString.Text = "Demo info(You may change it)";
            this.txtDemoString.TextChanged += new System.EventHandler(this.txtDemoString_TextChanged);
            // 
            // lblDemoString
            // 
            this.lblDemoString.AutoSize = true;
            this.lblDemoString.Location = new System.Drawing.Point(24, 144);
            this.lblDemoString.Name = "lblDemoString";
            this.lblDemoString.Size = new System.Drawing.Size(82, 15);
            this.lblDemoString.TabIndex = 6;
            this.lblDemoString.Text = "Demo String :";
            // 
            // lblPreviewResult
            // 
            this.lblPreviewResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPreviewResult.Location = new System.Drawing.Point(32, 224);
            this.lblPreviewResult.Name = "lblPreviewResult";
            this.lblPreviewResult.Size = new System.Drawing.Size(376, 40);
            this.lblPreviewResult.TabIndex = 5;
            this.lblPreviewResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPtnPreview
            // 
            this.lblPtnPreview.AutoSize = true;
            this.lblPtnPreview.Location = new System.Drawing.Point(24, 200);
            this.lblPtnPreview.Name = "lblPtnPreview";
            this.lblPtnPreview.Size = new System.Drawing.Size(98, 15);
            this.lblPtnPreview.TabIndex = 4;
            this.lblPtnPreview.Text = "Pattern Preview :";
            // 
            // lblCnsnPtn
            // 
            this.lblCnsnPtn.AutoSize = true;
            this.lblCnsnPtn.Location = new System.Drawing.Point(24, 88);
            this.lblCnsnPtn.Name = "lblCnsnPtn";
            this.lblCnsnPtn.Size = new System.Drawing.Size(118, 15);
            this.lblCnsnPtn.TabIndex = 3;
            this.lblCnsnPtn.Text = "Conversion Pattern :";
            // 
            // txtConversionPattern
            // 
            this.txtConversionPattern.Location = new System.Drawing.Point(32, 112);
            this.txtConversionPattern.Name = "txtConversionPattern";
            this.txtConversionPattern.Size = new System.Drawing.Size(376, 21);
            this.txtConversionPattern.TabIndex = 2;
            this.txtConversionPattern.TextChanged += new System.EventHandler(this.txtConversionPattern_TextChanged);
            // 
            // ddlLayoutType
            // 
            this.ddlLayoutType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlLayoutType.Location = new System.Drawing.Point(32, 48);
            this.ddlLayoutType.Name = "ddlLayoutType";
            this.ddlLayoutType.Size = new System.Drawing.Size(376, 23);
            this.ddlLayoutType.TabIndex = 1;
            // 
            // lblLayoutType
            // 
            this.lblLayoutType.AutoSize = true;
            this.lblLayoutType.Location = new System.Drawing.Point(24, 24);
            this.lblLayoutType.Name = "lblLayoutType";
            this.lblLayoutType.Size = new System.Drawing.Size(39, 15);
            this.lblLayoutType.TabIndex = 0;
            this.lblLayoutType.Text = "Type :";
            // 
            // frmAppender
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(472, 584);
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.txtAlias);
            this.Controls.Add(this.lblAppenderClass);
            this.Controls.Add(this.lblAlias);
            this.Controls.Add(this.lblDesc);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.ddlAppenderClasses);
            this.Controls.Add(this.gpArgContainer);
            this.Font = new System.Drawing.Font("Arial", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAppender";
            this.Text = "Log4net Appender Editor";
            this.gpArgContainer.ResumeLayout(false);
            this.pnlArguments.ResumeLayout(false);
            this.gbLayout.ResumeLayout(false);
            this.gbLayout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

		private void RecursiveGenerateControls(ArgumentStruct oArg)
		{
			Control control1;
			switch (oArg.Name)
			{
				case "layout":
					foreach (string sEnumValue in oArg.EnumValues)
					{
						ddlLayoutType.Items.Add(sEnumValue);
					}
					ddlLayoutType.Text = oArg.Value;
					break;
				case "conversionPattern":
					txtConversionPattern.Text = oArg.Value;
					break;
				default:
					Label label1 = new Label();
					label1.Text = oArg.Name + " : ";
					label1.AutoSize = true;
					switch (oArg.UIType)
					{
						case UIControlType.MultiLineTextBox:
							control1 = new TextBox();
							((TextBox) control1).Multiline = true;
							((TextBox) control1).ScrollBars = ScrollBars.Both;
							((TextBox) control1).Text = oArg.Value;
							control1.Height *= 5;
							break;

						case UIControlType.DropDownList:
							control1 = new ComboBox();
							((ComboBox) control1).DropDownStyle = ComboBoxStyle.DropDownList;
							foreach (string text2 in oArg.EnumValues)
							{
								((ComboBox) control1).Items.Add(text2);
							}
							((ComboBox) control1).Text = oArg.Value;
							break;

						case UIControlType.ListBox:
							control1 = new ListBox();
							foreach (string text1 in oArg.EnumValues)
							{
								((ListBox) control1).Items.Add(text1);
							}
							((ListBox) control1).Text = oArg.Value;
							break;

						case UIControlType.ParameterGrid:
							control1 = new ParameterGrid();
							((ParameterGrid)control1).ParameterXmlNodes = oArg.ParameterXml;
							break;

						case UIControlType.None:
							control1 = new Label();
							break;

						default:
							control1 = new TextBox();
							((TextBox) control1).Text = oArg.Value;
							break;
					}
					control1.Tag = oArg.Name;
					toolTip.SetToolTip(control1, oArg.Description);
					pnlArguments.Controls.Add(label1);
					pnlArguments.Controls.Add(control1);
					break;
			}
			if (oArg.ChildArguments != null)
			{
				foreach (ArgumentStruct struct1 in oArg.ChildArguments)
				{
					RecursiveGenerateControls(struct1);
				}
			}
		}

		public new DialogResult ShowDialog()
		{
			return ShowDialog(null);
		}

		public new DialogResult ShowDialog(IWin32Window owner)
		{
			if (moConsulter == null)
			{
				InitGroupArgument();
			}
			return base.ShowDialog(owner);
		}

		public string CurrentAliasName
		{
			get
			{
				return msAliasName;
			}
		}
 
		private void txtConversionPattern_TextChanged(object sender, System.EventArgs e)
		{
			RenderPatternDemo();
		}

		private void txtDemoString_TextChanged(object sender, System.EventArgs e)
		{
            RenderPatternDemo();
		}

		private void RenderPatternDemo()
		{
			try
			{
				lblPreviewResult.Text = Helper.PreviewPattern(txtDemoString.Text, ddlLayoutType.Text, txtConversionPattern.Text, this).Replace("\t", "    ");		
			} 
			catch (Exception oEX)
			{
				MessageBox.Show(oEX.ToString(), oEX.Message);
			}
		}

		private void btnPatternHelp_Click(object sender, System.EventArgs e)
		{
			System.Diagnostics.Process.Start("IExplore.exe", "http://logging.apache.org/log4net/release/sdk/log4net.Layout.PatternLayout.html");
		}
	}
}
