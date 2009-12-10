using System;

namespace Log4netConfigConsulter {
    public class Constants {
        // Fields
        public const string msCONST_APPENDER_INTERFACE_NAME = "IAppender";
        public const string msCONST_LAYOUT_INTERFACE_NAME = "ILayout";
        public const string msCONST_LOG4NET_APPENDER_NAMESPACE_PATH = "log4net.Appender.";
        public const string msCONST_LOG4NET_ASSEMBLY_NAME = "log4net";
        public const string msCONST_LOG4NET_DEFAULT_LAYOUT = "log4net.Layout.PatternLayout";
        public const string msCONST_LOG4NET_DEFAULT_PATTERNLAYOUT = "%c	%p	%d{yyyy/MM/dd HH:mm:ss,fff}	%t	%X{rquid}-	%m%n";
        public const string msCONST_LOG4NET_NAMESPACE_PATH = "log4net.Layout.";
        public const string msCONST_NOLAYOUT_APPENDER = "AdoNetAppender";

        // Nested Types
        public class ArgInfoFieldName {
            // Fields
            public const string DataTypeField = "DataType";
            public const string DescriptionField = "Description";
            public const string EnumValuesField = "EnumValues";
            public const string IsTagNameField = "IsTagName";
            public const string NameField = "Name";
            public const string UITypeField = "UIType";
            public const string ValueAttriNameField = "ValueAttriName";
            public const string ValueField = "Value";
        }

        public class ArgName {
            // Fields
            public const string msCONST_LOG4NET_CONVPATTERN = "conversionPattern";
            public const string msCONST_LOG4NET_LAYOUT = "layout";
        }
    }
}
