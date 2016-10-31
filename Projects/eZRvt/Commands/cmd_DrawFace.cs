using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DllActivator;
using eZRvt.FaceWall;


namespace eZRvt.Commands
{

    /// <summary> 绘制面层 </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class cmd_DrawFaceWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //
            DllActivator_eZRvt dat = new DllActivator_eZRvt();
            dat.ActivateReferences();
            //
            MpFaceOptions mpf = MpFaceOptions.UniqueObject(commandData.Application);
            mpf.Show();
            return Result.Succeeded;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    internal class cmd_DrawForm // : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //
            DllActivator_eZRvt dat = new DllActivator_eZRvt();
            dat.ActivateReferences();
            //

            Document massFamilyDoc = commandData.Application.ActiveUIDocument.Document;

            using (Transaction trans = new Transaction(massFamilyDoc, "d"))
            {
                try
                {
                    trans.Start();
                    // 在体量族文档中创建 拉伸 Form
                    CreateExtrusionForm(massFamilyDoc);

                    trans.Commit();
                    return Result.Succeeded;
                }
                catch (Exception ex)
                {
                    trans.RollBack();
                    message = ex.Message;
                    return Result.Failed;
                }
            }
        }

        private Form CreateExtrusionForm(Autodesk.Revit.DB.Document massFamilyDocument)
        {
            Form extrusionForm = null;

            // Create one profile
            ReferenceArray ref_ar = new ReferenceArray();

            XYZ ptA = new XYZ(10, 10, 0);
            XYZ ptB = new XYZ(90, 10, 0);
            ModelCurve modelcurve = MakeLine(massFamilyDocument, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(90, 10, 0);
            ptB = new XYZ(10, 90, 0);
            modelcurve = MakeLine(massFamilyDocument, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            ptA = new XYZ(10, 90, 0);
            ptB = new XYZ(10, 10, 0);
            modelcurve = MakeLine(massFamilyDocument, ptA, ptB);
            ref_ar.Append(modelcurve.GeometryCurve.Reference);

            // The extrusion form direction
            XYZ direction = new XYZ(0, 0, 50);

            extrusionForm = massFamilyDocument.FamilyCreate.NewExtrusionForm(true, ref_ar, direction);

            int profileCount = extrusionForm.ProfileCount;

            return extrusionForm;
        }
        public ModelCurve MakeLine(Document doc, XYZ ptA, XYZ ptB)
        {
            Autodesk.Revit.ApplicationServices.Application app = doc.Application;
            // Create plane by the points
            Line line = Line.CreateBound(ptA, ptB);
            XYZ norm = ptA.CrossProduct(ptB);
            if (norm.IsZeroLength()) norm = XYZ.BasisZ;
            Plane plane = app.Create.NewPlane(norm, ptB);
            SketchPlane skplane = SketchPlane.Create(doc, plane);
            // Create line here
            ModelCurve modelcurve = doc.FamilyCreate.NewModelCurve(line, skplane);
            return modelcurve;
        }

    }

}