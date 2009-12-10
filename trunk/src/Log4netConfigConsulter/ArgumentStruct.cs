using System;
using System.Xml;
using System.Collections;

namespace Log4netConfigConsulter
{
	public class ArgumentStruct
	{
		// Methods
		public ArgumentStruct()
		{
			_moChildArguments = new ArrayList();
			_moParameterXml = null;
			Name = string.Empty;
			Description = string.Empty;
			DataType = string.Empty;
			Value = string.Empty;
			ValueAttriName = string.Empty;
			IsTagName = false;
			UIType = UIControlType.SingleLineTextBox;
			ParentArgument = null;
		}


		public void AddChildArgument(ArgumentStruct oChildArgument)
		{
			oChildArgument.ParentArgument = this;
			_moChildArguments.Add(oChildArgument);
		}

 
		public ArgumentStruct Find(string sArgName)
		{
			ArgumentStruct oArg = null;
			if (Name.ToLower().Trim() == sArgName.ToLower().Trim())
			{
				return this;
			}
			if (_moChildArguments.Count > 0)
			{
				foreach (object oChildArg in _moChildArguments)
				{
					oArg = ((ArgumentStruct)oChildArg).Find(sArgName);
				}
			}
			return oArg;
		}

		// Properties
		public ArgumentStruct[] ChildArguments
		{
			get
			{
				if (_moChildArguments.Count == 0)
				{
					return null;
				}
				return (ArgumentStruct[]) _moChildArguments.ToArray(typeof(ArgumentStruct));
			}
		}
 
		/// <summary>
		/// Get parameters for some special and complex appender to use.
		/// </summary>
		public XmlNodeList ParameterXml
		{
			get { return _moParameterXml; }
			set { _moParameterXml = value; }
		}
		// Fields
		private ArrayList _moChildArguments;
		private XmlNodeList _moParameterXml;
		public string DataType;
		public string Description;
		public string[] EnumValues;
		public string Name;
		public ArgumentStruct ParentArgument;
		public UIControlType UIType;
		public string Value;
		public bool IsTagName;
		public string ValueAttriName;
	}
}