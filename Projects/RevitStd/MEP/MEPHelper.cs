using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Autodesk.Revit.DB;

namespace RevitStd.MEP
{
    public static class MEPHelper
    {
        #region ---   判断 Connector 对象是否可以用来进行 线管 到 配电箱 的连接件

        /// <summary> 判断 Connector 对象是否可以用来进行 线管 到 配电箱 的连接件 </summary>
        public static List<Connector> FilterConnectors(IEnumerable<Connector> connectorSet)
        {
            return connectorSet.Where(IsConnector).ToList();
        }

        /// <summary> 判断 Connector 对象是否可以用来进行 线管 到 配电箱 的连接件 </summary>
        public static List<Connector> FilterConnectors(ConnectorSet connectorSet)
        {
            return connectorSet.Cast<Connector>().Where(IsConnector).ToList();
        }

        /// <summary> 判断 Connector 对象是否可以用来进行 线管 到 配电箱 的连接件 </summary>
        public static bool IsConnector(Connector connector)
        {
            if (connector.Domain != Domain.DomainCableTrayConduit)
            {
                return false;
            }

            if (!connector.IsMovable)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
