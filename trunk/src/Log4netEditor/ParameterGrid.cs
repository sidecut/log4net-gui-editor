using System;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Log4netEditor {
    public class ParameterGrid : UserControl {
        // Fields
        private Container components = null;
        private DataGridTextBoxColumn conversionPattern;
        private DataGridTextBoxColumn dbType;
        private DataGrid dgParameter;
        private DataGridTextBoxColumn layout;
        private DataGridTextBoxColumn parameterName;
        private DataGridTextBoxColumn size;
        private DataGridTableStyle tsParameters;

        // Methods
        public ParameterGrid() {
            this.InitializeComponent();
            dsADOParameters.ParametersDataTable table = new dsADOParameters.ParametersDataTable();
            this.dgParameter.DataSource = table;
        }

        private XmlNode CreateParamNode(string NodeName, string ValueName, string Value, XmlDocument oDoc) {
            XmlNode node = oDoc.CreateNode(XmlNodeType.Element, NodeName, string.Empty);
            XmlAttribute attribute = oDoc.CreateAttribute(ValueName);
            attribute.Value = Value;
            node.Attributes.Append(attribute);
            return node;
        }

        protected override void Dispose(bool disposing) {
            if (disposing && (this.components != null)) {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            this.dgParameter = new DataGrid();
            this.tsParameters = new DataGridTableStyle();
            this.parameterName = new DataGridTextBoxColumn();
            this.dbType = new DataGridTextBoxColumn();
            this.layout = new DataGridTextBoxColumn();
            this.conversionPattern = new DataGridTextBoxColumn();
            this.size = new DataGridTextBoxColumn();
            this.dgParameter.BeginInit();
            base.SuspendLayout();
            this.dgParameter.CaptionText = "Parameters";
            this.dgParameter.DataMember = "";
            this.dgParameter.HeaderForeColor = SystemColors.ControlText;
            this.dgParameter.Location = new Point(0, 0);
            this.dgParameter.Name = "dgParameter";
            this.dgParameter.Size = new Size(0x200, 0x108);
            this.dgParameter.TabIndex = 0;
            this.dgParameter.TableStyles.AddRange(new DataGridTableStyle[] { this.tsParameters });
            this.tsParameters.DataGrid = this.dgParameter;
            this.tsParameters.GridColumnStyles.AddRange(new DataGridColumnStyle[] { this.parameterName, this.dbType, this.layout, this.conversionPattern, this.size });
            this.tsParameters.HeaderForeColor = SystemColors.ControlText;
            this.tsParameters.MappingName = "Parameters";
            this.parameterName.Format = "";
            this.parameterName.FormatInfo = null;
            this.parameterName.HeaderText = "Parameter Name";
            this.parameterName.MappingName = "parameterName";
            this.parameterName.Width = 0x4b;
            this.dbType.Format = "";
            this.dbType.FormatInfo = null;
            this.dbType.HeaderText = "DBType";
            this.dbType.MappingName = "dbType";
            this.dbType.Width = 0x4b;
            this.layout.Format = "";
            this.layout.FormatInfo = null;
            this.layout.HeaderText = "Layout";
            this.layout.MappingName = "layout";
            this.layout.Width = 0x4b;
            this.conversionPattern.Format = "";
            this.conversionPattern.FormatInfo = null;
            this.conversionPattern.HeaderText = "Conversion Pattern";
            this.conversionPattern.MappingName = "conversionPattern";
            this.conversionPattern.Width = 0x4b;
            this.size.Format = "";
            this.size.FormatInfo = null;
            this.size.HeaderText = "size";
            this.size.MappingName = "size";
            this.size.Width = 0x4b;
            base.Controls.Add(this.dgParameter);
            this.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0x88);
            base.Name = "ParameterGrid";
            base.Size = new Size(520, 0x110);
            base.Resize += new EventHandler(this.ParameterGrid_Resize);
            this.dgParameter.EndInit();
            base.ResumeLayout(false);
        }

        private void ParameterGrid_Resize(object sender, EventArgs e) {
            this.dgParameter.Width = base.Width;
            this.dgParameter.Height = base.Height;
        }

        // Properties
        public XmlNodeList ParameterXmlNodes {
            get {
                XmlDocument oDoc = new XmlDocument();
                XmlNode node = oDoc.CreateNode(XmlNodeType.Element, "parameters", string.Empty);
                IEnumerator enumerator = ((dsADOParameters.ParametersDataTable)this.dgParameter.DataSource).GetEnumerator();
                try {
                    while (enumerator.MoveNext()) {
                        dsADOParameters.ParametersRow row = (dsADOParameters.ParametersRow)enumerator.Current;
                        XmlNode newChild = oDoc.CreateNode(XmlNodeType.Element, "parameter", string.Empty);
                        node.AppendChild(newChild);
                        newChild.AppendChild(this.CreateParamNode("parameterName", "value", row.parameterName, oDoc));
                        newChild.AppendChild(this.CreateParamNode("dbType", "value", row.dbType, oDoc));
                        if (row.size != 0) {
                            newChild.AppendChild(this.CreateParamNode("size", "value", ((int)row.size).ToString(), oDoc));
                        }
                        XmlNode node2 = this.CreateParamNode("layout", "type", row.layout, oDoc);
                        newChild.AppendChild(node2);
                        if ((row.conversionPattern != null) && (string.Empty != row.conversionPattern)) {
                            node2.AppendChild(this.CreateParamNode("conversionPattern", "value", row.conversionPattern, oDoc));
                        }
                    }
                }
                finally {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null) {
                        disposable.Dispose();
                    }
                }
                return node.SelectNodes("//parameter");
            }
            set {
                if (value != null) {
                    dsADOParameters.ParametersDataTable table = (dsADOParameters.ParametersDataTable)this.dgParameter.DataSource;
                    table.Clear();
                    try {
                        IEnumerator enumerator = value.GetEnumerator();
                        try {
                            while (enumerator.MoveNext()) {
                                XmlNode node = (XmlNode)enumerator.Current;
                                dsADOParameters.ParametersRow row = table.NewParametersRow();
                                row.parameterName = node.SelectSingleNode("parameterName").Attributes["value"].Value;
                                row.dbType = node.SelectSingleNode("dbType").Attributes["value"].Value;
                                row.layout = node.SelectSingleNode("layout").Attributes["type"].Value;
                                XmlNode node2 = node.SelectSingleNode("size");
                                if (node2 != null) {
                                    row.size = int.Parse(node2.Attributes["value"].Value);
                                }
                                node2 = node.SelectSingleNode("layout/conversionPattern");
                                if (node2 != null) {
                                    row.conversionPattern = node2.Attributes["value"].Value;
                                }
                                table.AddParametersRow(row);
                            }
                        }
                        finally {
                            IDisposable disposable = enumerator as IDisposable;
                            if (disposable != null) {
                                disposable.Dispose();
                            }
                        }
                    }
                    catch (Exception) {
                    }
                }
            }
        }
    }
}
