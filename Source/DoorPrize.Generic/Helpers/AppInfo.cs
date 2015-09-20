using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Text;

namespace DoorPrize.Generic
{
    public class AppInfo
    {
        public AppInfo(Assembly assembly)
        {
            Contract.Requires(assembly != null);

            Product = GetEntryProduct(assembly);
            Version = assembly.GetName().Version;
        }

        private Version Version { get; set; }
        private string Product { get; set; }

        public string GetTitle(string extraInfo = null)
        {
            var sb = new StringBuilder();

            sb.Append(Product);

            if (!string.IsNullOrWhiteSpace(extraInfo))
            {
                sb.Append(" (");
                sb.Append(extraInfo);
                sb.Append(')');
            }

            sb.Append(" v");
            sb.Append(Version.Major);
            sb.Append('.');
            sb.Append(Version.Minor);

            if ((Version.Build != 0) || (Version.Revision != 0))
            {
                sb.Append('.');
                sb.Append(Version.Build);
            }

            if (Version.Revision == 0) 
                return sb.ToNewString();

            sb.Append('.');
            sb.Append(Version.Revision);

            return sb.ToNewString();
        }

        private static string GetEntryProduct(Assembly assembly)
        {
            return assembly.GetAttribute<AssemblyProductAttribute>().Product;
        }
    }
}
