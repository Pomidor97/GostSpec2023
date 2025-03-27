using Autodesk.Revit.DB;

namespace GostSpec2023.Services
{
    public interface IParameterCopyService
    {
        void CopyParameters(Document doc);
    }
}
