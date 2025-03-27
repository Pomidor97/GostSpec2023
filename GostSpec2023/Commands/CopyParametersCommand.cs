using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GostSpec2023.Constants;
using GostSpec2023.Services;

namespace GostSpec2023.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CopyParametersCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!ServiceLocator.IsRegistered<IParameterCopyService>())
            {
                ServiceLocator.Register<IParameterCopyService, ParameterCopyService>();
            }
            
            Document doc = commandData.Application.ActiveUIDocument.Document;
            var service = ServiceLocator.Resolve<IParameterCopyService>();

            using (Transaction t = new Transaction(doc, GostSpecConstants.Transaction_CopyParams))
            {
                t.Start();
                service.CopyParameters(doc);
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}