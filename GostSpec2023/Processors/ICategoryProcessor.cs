using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace GostSpec2023.Processors
{
    public interface ICategoryProcessor
    {
        void ProcessElements(Document doc, IList<Element> elements, double zapas);
    }
}