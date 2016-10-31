using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using RevitStd.MEP;

namespace eZRvt.ConduitLayout
{
    /// <summary>
    /// 包含有 MEPModel 的族实例，比如 配电箱 等，其中都有 ConnectorManager 等连接件的信息。
    /// </summary>
    public class MEPElectricalEquipment
    {

        public readonly FamilyInstance MepInstance;

        /// <summary>
        /// 
        /// </summary>
        public readonly MEPModel MepModel;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mepInstance"></param>
        public MEPElectricalEquipment(FamilyInstance mepInstance)
        {
            if (mepInstance.MEPModel == null)
            {
                throw new ArgumentException("This familyInstance does not represent a MEP model.");
            }
            MepInstance = mepInstance;
            MepModel = mepInstance.MEPModel;
        }


        public List<ConnectorElement> GetConnetorElements()
        {

            return null;
        }

        public List<Connector> GetConnetors()
        {
            return MEPHelper.FilterConnectors(MepModel.ConnectorManager.Connectors);
        }

    }
}
