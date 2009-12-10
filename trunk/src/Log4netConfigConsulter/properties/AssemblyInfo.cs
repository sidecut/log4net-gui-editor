using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Log4net configuration information provider library.")]
#if(DEBUG)
[assembly: AssemblyDescription("This library offer the funtionality to retrieve log4net configuration definition information.(DEBUG build)")]
#else
[assembly: AssemblyDescription("This library offer the funtionality to retrieve log4net configuration definition information.(RELEASE build)")]
#endif
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("DOWILL Studio")]
[assembly: AssemblyProduct("Log4net GUI editor")]
[assembly: AssemblyCopyright("Copyright (C)  2009 all rights reserved")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("845bced5-4d33-473e-bc81-3d17b59039ac")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
#if(DEBUG)
[assembly: AssemblyVersion("0.2009.1211.0")]
[assembly: AssemblyFileVersion("0.2009.1211.0")]
#else
[assembly: AssemblyVersion("1.2009.1211.0")]
[assembly: AssemblyFileVersion("1.2009.1211.0")]
#endif
