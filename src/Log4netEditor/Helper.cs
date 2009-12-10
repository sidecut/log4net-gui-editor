using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
using log4net.Core;
using log4net.Layout;
using Log4netConfigConsulter;

namespace Log4netEditor
{
	public class Helper
	{
		public static ArrayList GetAppenders()
		{
			ArrayList AppenderList = new ArrayList();
			Type[] tempArray = Assembly.LoadWithPartialName(Constants.msCONST_LOG4NET_ASSEMBLY_NAME).GetTypes();
			foreach (Type oType in tempArray)
			{
				if ((oType.GetInterface(Constants.msCONST_APPENDER_INTERFACE_NAME) != null) && !oType.IsAbstract)
				{
					AppenderList.Add(oType.Name);
				}
			}
			return AppenderList;
		}

		public static string PreviewPattern(string InfoData, string LayoutType, string ConversionPattern, Object Host)
		{
			string sResult = string.Empty;
			StringWriter oWriter = new StringWriter();
			LoggingEvent oEvent = new LoggingEvent(Host.GetType(), null, "Logger Name", Level.All, InfoData, null);
            ILayout oLayer = (ILayout)Assembly.LoadWithPartialName(Constants.msCONST_LOG4NET_ASSEMBLY_NAME).CreateInstance(LayoutType);
			if (null == oLayer)
			{
				sResult = "This pattern layout is not supported. No preview work.";
			}
			else
			{
				if (oLayer.GetType() == typeof(PatternLayout))
				{
					((PatternLayout)oLayer).ConversionPattern = ConversionPattern;
					((PatternLayout)oLayer).ActivateOptions();
				}
				oLayer.Format(oWriter, oEvent);
				sResult = oWriter.GetStringBuilder().ToString();
			}
			return sResult;
		}
	}
}
