using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections;

namespace Log4netConfigConsulter
{
	/// <summary>
	/// AppenderConsulter ªººK­n´y­z¡C
	/// </summary>
	public class AppenderConsulter
	{
		protected ArrayList _mMyArguments;
		protected XmlDocument _moXmlDoc;
		protected XmlDocument _moInfoXmlDoc;
		protected string msAppenderDesc;
		protected const string msCONST_AppenderInfoDir = "AppenderInfo\\";
		private const string msCONST_NAME_SPACE = "BSConfigConsulter.AppenderConsulter";
		private const string msCONST_EXTENSION_FILENAME = ".XML";

		protected AppenderConsulter(){}

		protected void InitConsulter(string AppenderName)
		{
			string sFilePath = Path.Combine(
				new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, 
				msCONST_AppenderInfoDir + AppenderName + msCONST_EXTENSION_FILENAME
				);

			System.Diagnostics.Debug.Write(sFilePath, GetType().Name);
			_moInfoXmlDoc = new XmlDocument();
			_mMyArguments = new ArrayList();
			if (File.Exists(sFilePath))
			{
				_moInfoXmlDoc.Load(sFilePath);
				InitMyArgument();
			} 
			else
			{
				msAppenderDesc = "Use default basic consulter.";
			}

			// Some appender is too special to use common layout setting. (eg: AdoNetAppender)
			if (Constants.msCONST_NOLAYOUT_APPENDER.ToLower() != AppenderName.ToLower())
			{
				AddLayoutArgument();
			}
		}

		protected virtual void AddLayoutArgument()
		{
			ArgumentStruct oArg = new ArgumentStruct();
			oArg.Name = Constants.ArgName.msCONST_LOG4NET_LAYOUT;
			oArg.DataType = typeof(string).Name.ToLower();
			oArg.Value = Constants.msCONST_LOG4NET_DEFAULT_LAYOUT;
			oArg.Description = "Assign layout pattern";
			oArg.UIType = UIControlType.DropDownList;
			oArg.EnumValues = EnumerateAllLayouts();
			_mMyArguments.Add(oArg);

			oArg = new ArgumentStruct();
			oArg.DataType = typeof(string).Name.ToLower();
			oArg.Name = Constants.ArgName.msCONST_LOG4NET_CONVPATTERN;
			oArg.Value = Constants.msCONST_LOG4NET_DEFAULT_PATTERNLAYOUT;
			oArg.UIType = UIControlType.SingleLineTextBox;
			oArg.Description = "Assign regular expression for this pattern layout.";
			((ArgumentStruct) _mMyArguments[_mMyArguments.Count - 1]).AddChildArgument(oArg);
		}

		private static string[] EnumerateAllLayouts()
		{
			ArrayList oLayoutLst = new ArrayList();
			Type[] AllTypesInLog4net = Assembly.LoadWithPartialName(Constants.msCONST_LOG4NET_ASSEMBLY_NAME).GetTypes();
			foreach (Type otmpType in AllTypesInLog4net)
			{
				if ((otmpType.GetInterface(Constants.msCONST_LAYOUT_INTERFACE_NAME) != null) && 
					!otmpType.IsAbstract)
				{
					oLayoutLst.Add(otmpType.FullName);
				}
			}
			return (string[]) oLayoutLst.ToArray(typeof(string));
		}

		protected virtual void GenerateConfigXMLDoc()
		{
			_moXmlDoc = new XmlDocument();
			XmlElement oRootNode = _moXmlDoc.CreateElement("Root");
			foreach (ArgumentStruct oArg in Arguments)
			{
				if (UIControlType.ParameterGrid == oArg.UIType)
				{
					System.Diagnostics.Debug.WriteLine("oArg.ParameterXml.Count = " + oArg.ParameterXml.Count);
					foreach (XmlNode tmpNode in oArg.ParameterXml)
					{
						XmlNode tmpNodeClone = _moXmlDoc.CreateNode(tmpNode.NodeType, tmpNode.Name, tmpNode.NamespaceURI);
						tmpNodeClone.InnerXml = tmpNode.InnerXml;
						oRootNode.AppendChild(tmpNodeClone);
					}
				}
				else
				{
					RecursiveFromArguments(oRootNode, oArg);
				}
			}
			_moXmlDoc.AppendChild(oRootNode);
		}

		public static AppenderConsulter GetAppender(string AppenderName)
		{
			AppenderConsulter oConsulter = new AppenderConsulter();
			oConsulter.InitConsulter(AppenderName);
			return oConsulter;
		}

		public virtual string GetAppenderDesc()
		{
			return msAppenderDesc;
		}

		public virtual string GetConfigXML()
		{
			GenerateConfigXMLDoc();
			return _moXmlDoc.FirstChild.InnerXml;
		}

		protected virtual void IdentifyDefault(XmlNode oNode)
		{
			string ArgName = oNode.Name.ToLower().Trim();
			string ArgValue = string.Empty;
			if (ArgName == "param")
			{
				ArgName = oNode.Attributes["name"].Value;
				ArgValue = oNode.Attributes["value"].Value;
			}
			else
			{
				if (oNode.Attributes.Count > 0) ArgValue = oNode.Attributes[0].Value;
			}
			ArgumentStruct oArg = SearchUpdateArg(ArgName, ArgValue);
			if (null != oArg)
			{
				if (UIControlType.ParameterGrid == oArg.UIType)
				{
					oArg.ParameterXml = oNode.ParentNode.SelectNodes("parameter");
				}
			}
		}

		protected virtual void IdentifyDefault(out XmlNode oNode, ArgumentStruct oArg)
		{
			XmlAttribute oAttri = null;
			if (!oArg.IsTagName)
			{
				oNode = _moXmlDoc.CreateNode(XmlNodeType.Element, "param", string.Empty);
				oAttri = _moXmlDoc.CreateAttribute("name");
				oAttri.Value = oArg.Name;
				oNode.Attributes.Append(oAttri);
				oAttri = _moXmlDoc.CreateAttribute("value");
			} 
			else
			{
				oNode = _moXmlDoc.CreateNode(XmlNodeType.Element, oArg.Name, string.Empty);
				if (string.Empty != oArg.ValueAttriName)
				{
					oAttri = _moXmlDoc.CreateAttribute(oArg.ValueAttriName);
				}
			}
			if (null != oAttri) 
			{
				oAttri.Value = oArg.Value;
				oNode.Attributes.Append(oAttri);
			}
		}

		protected virtual bool IdentifyLayoutConversationPattern(XmlNode oNode)
		{
			bool bFound = true;
			string sArgName = oNode.Name.ToLower().Trim();
			if (Constants.ArgName.msCONST_LOG4NET_LAYOUT.ToLower() == sArgName)
			{
				SearchUpdateArg(sArgName, oNode.Attributes["type"].Value);
			} 
			else if (Constants.ArgName.msCONST_LOG4NET_CONVPATTERN.ToLower() == sArgName) 
			{
				SearchUpdateArg(sArgName, oNode.Attributes["value"].Value);
			} 
			else 
			{
				bFound = false;
			}
			return bFound;
		}

		protected virtual bool IdentifyLayoutConversationPattern(out XmlNode oNode, ArgumentStruct oArg)
		{
			XmlAttribute attribute1;
			bool flag1 = true;
			switch (oArg.Name.ToLower().Trim())
			{
				case "layout":
					oNode = _moXmlDoc.CreateNode(XmlNodeType.Element, oArg.Name, string.Empty);
					attribute1 = _moXmlDoc.CreateAttribute("type");
					attribute1.Value = oArg.Value;
					oNode.Attributes.Append(attribute1);
					return flag1;

				case "conversionpattern":
					oNode = _moXmlDoc.CreateNode(XmlNodeType.Element, oArg.Name, string.Empty);
					attribute1 = _moXmlDoc.CreateAttribute("value");
					attribute1.Value = oArg.Value;
					oNode.Attributes.Append(attribute1);
					return flag1;
			}
			oNode = null;
			return false;
		}

		protected virtual void IdentifyRuleForArgument(out XmlNode oNode, ArgumentStruct oArg)
		{
			if (!IdentifyLayoutConversationPattern(out oNode, oArg))
			{
				IdentifyDefault(out oNode, oArg);
			}
		}

		protected virtual void IdentifyRuleForXml(XmlNode oNode)
		{
			if (!IdentifyLayoutConversationPattern(oNode))
			{
				IdentifyDefault(oNode);
			}
		}

		protected virtual void InitMyArgument()
		{
			msAppenderDesc = _moInfoXmlDoc.SelectSingleNode("//AppenderDesc").InnerText;
			foreach (XmlNode oArgNode in _moInfoXmlDoc.DocumentElement.SelectNodes("//Arguments/Argument"))
			{
				_mMyArguments.Add(GetArgumentInfo(oArgNode));
			}
		}

		private ArgumentStruct GetArgumentInfo(XmlNode oArgNode)
		{
			XmlAttribute oTempAttri;
			ArgumentStruct oArg = new ArgumentStruct();
			oTempAttri = oArgNode.Attributes[Constants.ArgInfoFieldName.NameField]; oArg.Name = (null == oTempAttri) ? string.Empty : oTempAttri.Value;
			oTempAttri = oArgNode.Attributes[Constants.ArgInfoFieldName.ValueField]; oArg.Value = (null == oTempAttri) ? string.Empty : oTempAttri.Value;
			oTempAttri = oArgNode.Attributes[Constants.ArgInfoFieldName.DataTypeField]; oArg.DataType = (null == oTempAttri) ? string.Empty : oTempAttri.Value;
			oTempAttri = oArgNode.Attributes[Constants.ArgInfoFieldName.DescriptionField]; oArg.Description = (null == oTempAttri) ? string.Empty : oTempAttri.Value;
			oTempAttri = oArgNode.Attributes[Constants.ArgInfoFieldName.EnumValuesField]; oArg.EnumValues = (null == oTempAttri) ? new string[] { string.Empty } : oTempAttri.Value.Split(';');
			oTempAttri = oArgNode.Attributes[Constants.ArgInfoFieldName.IsTagNameField]; oArg.IsTagName = (null == oTempAttri) ? false : bool.Parse(oTempAttri.Value);
			oTempAttri = oArgNode.Attributes[Constants.ArgInfoFieldName.ValueAttriNameField]; oArg.ValueAttriName = (null == oTempAttri) ? string.Empty : oTempAttri.Value;
			oTempAttri = oArgNode.Attributes[Constants.ArgInfoFieldName.UITypeField]; oArg.UIType = (null == oTempAttri) ? UIControlType.None : ParseUIControlTypeFromString(oTempAttri.Value);
			if (UIControlType.ParameterGrid == oArg.UIType)
			{
				oArg.ParameterXml = oArgNode.SelectNodes("//parameter");
			}
			else
			{
				foreach (XmlNode oChildArgNode in oArgNode.ChildNodes)
				{
					oArg.AddChildArgument(GetArgumentInfo(oChildArgNode));
				}
			}
			return oArg;
		}

		private UIControlType ParseUIControlTypeFromString(string UIString)
		{
			UIControlType oTemp = UIControlType.None;
			for (int i = 0 ; i < 100 ; i++)
			{
				try
				{
					oTemp = (UIControlType)i;
					if (oTemp.ToString() == UIString) break;
				}
				catch(InvalidCastException)
				{
					oTemp = UIControlType.None;
				}
			}
			return oTemp;
		}

		private void RecursiveFromArguments(XmlNode oCurrentNode, ArgumentStruct oArg)
		{
			if (UIControlType.ParameterGrid == oArg.UIType)
			{
				foreach (XmlNode oNode in oArg.ParameterXml)
				{
					oCurrentNode.AppendChild(oNode);
				}
			}
			else
			{
				XmlNode oArgNode;
				IdentifyRuleForArgument(out oArgNode, oArg);
				if (oArg.ChildArguments != null)
				{
					foreach (ArgumentStruct oArgChild in oArg.ChildArguments)
					{
						RecursiveFromArguments(oArgNode, oArgChild);
					}
				}
				oCurrentNode.AppendChild(oArgNode);
			}
		}

		public virtual void RestoreArgsFromXml(XmlNode oAppenderNode)
		{
			foreach (XmlNode oChildNode in oAppenderNode.ChildNodes)
			{
				if (oChildNode is XmlComment) continue;		// Ignore Comment in XML.

				IdentifyRuleForXml(oChildNode);
				if (oChildNode.ChildNodes.Count > 0)
				{
					RestoreArgsFromXml(oChildNode);
				}
			}
		}

		public virtual ArgumentStruct SearchUpdateArg(string sArgName, string UpdatedValue)
		{
			ArgumentStruct oFinalArg = null;
			foreach (object oTemp in _mMyArguments)
			{
				ArgumentStruct oArg = ((ArgumentStruct)oTemp).Find(sArgName);
				if (null != oArg)
				{
					oFinalArg = oArg;
					oArg.Value = UpdatedValue;
				}
			}
			return oFinalArg;
		}

		// Properties
		public virtual ArgumentStruct[] Arguments
		{
			get
			{
				if (_mMyArguments.Count == 0) return null;
				return (ArgumentStruct[]) _mMyArguments.ToArray(typeof(ArgumentStruct));
			}
			set
			{
				_mMyArguments.Clear();
				foreach (ArgumentStruct oArg in value)
				{
					_mMyArguments.Add(oArg);
				}
			}
		}
 
	}

}
