using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using eZstd;
using eZstd.Miscellaneous;

namespace RevitStd.Tests_Templates
{
    internal class Test_EditFamily
    {

        private Result RefreshExcavationSoil(ExternalCommandData commandData)
        {
            UIApplication uiApp = commandData.Application;

            Document doc = uiApp.ActiveUIDocument.Document;
            Autodesk.Revit.DB.View view = uiApp.ActiveUIDocument.ActiveGraphicalView;
            //

            //

            // ExternalDefinition familyDefinition = null;
            DefinitionGroup defGroup = RvtTools.GetOldWDefinitionGroup(uiApp.Application);
            ExternalDefinition ExDef = defGroup.Definitions.get_Item("某共享参数名") as ExternalDefinition;

            //
            Family someFamily = null;
            Document famDoc = doc.EditFamily(someFamily);

            try
            {
                EditFamily(famDoc, ExDef);
            }
            catch (Exception ex)
            {
                DebugUtils.ShowDebugCatch(ex, "修改族参数名称");
            }

            // 将族加载到项目文档中
            Family fam = famDoc.LoadFamily(doc, UIDocument.GetRevitUIFamilyLoadOptions());
            famDoc.Close(false);


            return Result.Succeeded;
        }

        private void EditFamily(Document famDoc, ExternalDefinition ExDef)
        {

            View view = famDoc.FindElement(typeof(View), targetName: "视图 1") as View;
            if (view == null)
            {
                throw new InvalidOperationException("view is null! ");
            }
            // 对于开挖土体族，其与模型土体不同的地方在于，它还是
            using (Transaction tranFam = new Transaction(famDoc))
            {
                try
                {
                    tranFam.SetName("对族文档进行修改");
                    tranFam.Start();
                    // ---------------------------------------------------------------------------------------------------------

                    // 添加参数
                    FamilyManager FM = famDoc.FamilyManager;

                    //’在进行参数读写之前，首先需要判断当前族类型是否存在，如果不存在，读写族参数都是不可行的
                    if (FM.CurrentType == null)
                    {
                        FM.NewType("CurrentType"); // 随便取个名字即可，后期会将族中的第一个族类型名称统一进行修改。
                    }

                    // 
                    FamilyParameter Para_Depth;
                    Para_Depth = FM.get_Parameter("Depth");


                    FM.RemoveParameter(Para_Depth);

                    Para_Depth = FM.AddParameter(ExDef, BuiltInParameterGroup.PG_GEOMETRY, isInstance: true);

                    //' give initial values
                    // FM.Set(Para_Depth, depth); // 这里不知为何为给出报错：InvalidOperationException:There is no current type.
                    Extrusion extru = famDoc.FindElement(typeof(Extrusion)) as Extrusion;

                    // 添加标注
                    PlanarFace TopFace = GeoHelper.FindFace(extru, new XYZ(0, 0, 1));
                    PlanarFace BotFace = GeoHelper.FindFace(extru, new XYZ(0, 0, -1));

                    // make an array of references
                    ReferenceArray refArray = new ReferenceArray();
                    refArray.Append(TopFace.Reference);
                    refArray.Append(BotFace.Reference);
                    // define a demension line
                    var a = GeoHelper.FindFace(extru, new XYZ(0, 0, 1)).Origin;
                    Line DimLine = Line.CreateBound(TopFace.Origin, BotFace.Origin);
                    // create a dimension
                    Dimension DimDepth = famDoc.FamilyCreate.NewDimension(view, DimLine, refArray);

                    // 将深度参数与其拉伸实体的深度值关联起来
                    DimDepth.FamilyLabel = Para_Depth;

                    // famDoc.Close();
                    tranFam.Commit();
                }
                catch (Exception ex)
                {
                    // Utils.ShowDebugCatch(ex, $"事务“{tranFam.GetName()}” 出错！");
                    tranFam.RollBack();
                    throw new InvalidOperationException($"事务“{tranFam.GetName()}”出错", ex);
                }
            }
        }


    }
}
