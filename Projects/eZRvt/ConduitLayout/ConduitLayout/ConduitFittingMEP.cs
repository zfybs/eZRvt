using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using RevitStd;
using RevitStd.MEP;

namespace eZRvt.ConduitLayout
{
    /// <summary>
    /// 用来进行线管到配电箱的连接
    /// </summary>
    public class ConduitFittingMEP
    {
        #region ---   Properties

        /// <summary>
        /// 配电箱
        /// </summary>
        private readonly MEPElectricalEquipment _cabinet;

        /// <summary>
        /// 线管
        /// </summary>
        private readonly MEPConduit _conduit;

        private readonly Document _doc;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cabinet">配电箱</param>
        /// <param name="conduit">线管</param>
        public ConduitFittingMEP(MEPElectricalEquipment cabinet, Conduit conduit)
        {
            _cabinet = cabinet;
            _doc = cabinet.MepInstance.Document;
            _conduit = new MEPConduit(conduit);
        }

        #region ---   Connect 线管与电气设备的连接


        /// <summary>
        /// 将线管连接到设备中可用的第一个连接件上
        /// </summary>
        /// <returns></returns>
        public FamilyInstance Connect(Transaction docTrans)
        {
            var cabConnectors = _cabinet.GetConnetors();

            if (!cabConnectors.Any()) throw new InvalidOperationException("电气设备中未能找到有效的连接件。");

            Connector cabConnector = cabConnectors.First();

            FamilyInstance elbow = Connect(docTrans, cabConnector);

            return elbow;
        }


        /// <summary>
        /// 将线管连接到设备中的指定的那一个连接件上
        /// </summary>
        /// <param name="cabConnctor"> 配电箱中的要进行连接的连接件</param>
        /// <returns></returns>
        public FamilyInstance Connect(Transaction docTrans, Connector cabConnctor)
        {
            Connector conduitConnector = null;
            double nearestDist = double.MaxValue;

            // 首先找到线管中指向配电箱的那个connector
            foreach (Connector conduitConn in MEPHelper.FilterConnectors(_conduit.ConduitIns.ConnectorManager.Connectors))
            {
                // 判断准则： conduitConn 的方向与两点连线方向夹角小于90度，而且线管连接件的标高要小于设备连接件的标高
                // 在满足条件的线管连接件中，取其中到设备连接件的距离最短的那一个
                if (conduitConn.CoordinateSystem.BasisZ.Z > 0.000001 || conduitConn.Origin.Z >= cabConnctor.Origin.Z)
                {
                    continue;
                }

                double angle = conduitConn.CoordinateSystem.BasisZ.AngleTo(cabConnctor.Origin.Subtract(conduitConn.Origin));
                double dist = conduitConn.Origin.DistanceTo(cabConnctor.Origin);
                if (angle < Math.PI / 2 && dist < nearestDist)
                {
                    conduitConnector = conduitConn;
                }
            }

            // 开始连接
            if (conduitConnector == null) throw new NullReferenceException("    在线管中未找到合适的连接件来进行连接。\r\n" +
                "    有效的连接件的方向应该水平，低于电气设备中指定连接件的标高，而且指向设备。");

            FamilyInstance elbow = Connect(docTrans, cabConnctor, conduitConnector);
            return elbow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private FamilyInstance Connect(Transaction docTrans, Connector conCabinet, Connector conConduit)
        {
            // 找到设备的底面

            XYZ cabLocation = conCabinet.Origin;

            PlanarFace bottomFace = GeoHelper.GetBottomPlanarFace(_cabinet.MepInstance);
            double bottomZ = bottomFace.Origin.Z;

            // 根据线管的位置与方向，以及设备中连接件的位置，得到此线管发出的射线与设备连接件的某竖向平面的交点
            XYZ rayPoint = GetRayPoint(conConduit.CoordinateSystem, conCabinet.CoordinateSystem);

            XYZ verticalConduitTop = new XYZ(rayPoint.X, rayPoint.Y, cabLocation.Z);
            XYZ verticalConduitBottom = new XYZ(rayPoint.X, rayPoint.Y, bottomZ);


            // 判断是否能够成功地生成弯头
            // （因为如果不能成功生成，在API的代码中也不会报错，而只是在Transaction.Commit时弹出一个消息框）
            double? outDiamter = _conduit.GetConduitOuterDiameter();
            if (outDiamter == null) throw new NullReferenceException("在线管中未找到参数“外径”");

            // ------------------------------------------------
            // 要生成弯头所需要的最小的弯曲半径，如果弯头的半径大于此值，则不能成功生成弯头。

            // 在生成弯头时，两边的线管如果被截断，则其截断后的长度不能为负，也不能为零，至少还要剩3mm（2.5 mm都不行）。
            double minLengthOfVerticalConduit = UnitUtils.ConvertToInternalUnits(5, DisplayUnitType.DUT_MILLIMETERS);
            // 弯头对象除了圆弧段以外，其两端还各有一个线管段，其长度 = 线管外径。
            double allowableRadius = cabLocation.Z - conConduit.Origin.Z - outDiamter.Value - minLengthOfVerticalConduit;

            double allowableRatio = allowableRadius / outDiamter.Value;

            // ------------------------------------------------

            double? ratioInFamily = GetElbowRadiusRatio(docTrans, _conduit.ConduitIns);
            if (ratioInFamily == null) throw new NullReferenceException("在线管中没有找到匹配的弯头对象");

            if (allowableRatio < ratioInFamily)
            {
                throw new InvalidOperationException(message: "线管当前所使用的弯头中的弯曲半径值过大，不能正常地生成弯头。" +
                    "请换用其他的弯头，或者将将弯头中的实例参数“弯曲半径”由“管件外径 * " + ratioInFamily +
                                                            "”修改为“管件外径 * " + allowableRatio + "”或更小的值。");
            }

            // 生成设备内部的竖向线管
            Conduit cd = Conduit.Create(_doc, _conduit.ConduitIns.GetTypeId(), verticalConduitBottom, verticalConduitTop, ElementId.InvalidElementId);
            MEPConduit mepCd = new MEPConduit(cd);
            // 调整线管直径到与选择的线管直径相同
            double? diameter = _conduit.GetConduitDiameter();
            if (diameter == null) throw new NullReferenceException("在线管中未找到参数“直径（公称尺寸）”");
            mepCd.SetConduitDiameter(diameter.Value);

            // 
            Connector conn_vert = cd.ConnectorManager.Lookup(0);

            // 生成弯头
            FamilyInstance elbow = _doc.Create.NewElbowFitting(conn_vert, conConduit);

            return elbow;
        }

        #endregion

        /// <summary>
        /// 根据线管的位置与方向，以及设备中连接件的位置，得到此线管发出的射线与设备连接件的某竖向平面的交点
        /// </summary>
        /// <param name="conduit"></param>
        /// <param name="cabinet"></param>
        /// <returns></returns>
        private XYZ GetRayPoint(Transform conduit, Transform cabinet)
        {
            XYZ o1 = conduit.Origin;
            XYZ o2 = cabinet.Origin;

            XYZ v1 = conduit.BasisZ;
            XYZ v2 = o2.Subtract(o1);
            //
            double angle = v1.AngleTo(v2);
            double distDiagnal = o1.DistanceTo(o2);  // 直角三角形中斜边的长度
            //
            double dist = distDiagnal * Math.Cos(angle);
            //
            return o1 + v1.Normalize()*dist;
        }


        /// <summary>
        /// 获取线管所对应的弯头的族文档中，族参数“弯曲半径 = 管件外径 * 15” 后面的数值 15
        /// </summary>
        /// <param name="conduit"> 线管对象 </param>
        /// <returns>  </returns>
        private double? GetElbowRadiusRatio(Transaction docTrans, Conduit conduit)
        {
            ConduitType cdType = _doc.GetElement(conduit.GetTypeId()) as ConduitType;

            Parameter paElbow = cdType.get_Parameter(BuiltInParameter.RBS_CURVETYPE_DEFAULT_BEND_PARAM);
            if (paElbow == null) throw new NullReferenceException("在线管中没有找到匹配的弯头参数");

            ElementId elbowId = paElbow.AsElementId();

            FamilySymbol fs = _doc.GetElement(elbowId) as FamilySymbol;

            if (docTrans.GetStatus() == TransactionStatus.Started)
            {
                docTrans.Commit();
            }

            // 在_doc.EditFamily之前，必须要确保没有打开的事务
            Document famDoc;
            famDoc = _doc.EditFamily(fs.Family);

            if (null == famDoc || !famDoc.IsFamilyDocument) throw new NullReferenceException("无法编辑线管设定的弯头所对应的族文件");

            ElbowFamily mepElbow = new ElbowFamily(famDoc);

            string formula = mepElbow.GetFormula();  // 其值应该大致为 “管件外径 * 15”
            famDoc.Close(false);

            // 
            docTrans.Start("读取完弯头族文档中的参数后重新开启事务");
            try
            {
                string ratio = formula.Split(new char[] { '*' })[1];
                return double.Parse(ratio);
            }
            catch
            {
                throw new InvalidOperationException("弯头的族参数不是“管件外径 * 15”的形式");
            }
        }
    }

}