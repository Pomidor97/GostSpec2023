using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using GostSpec2023.Constants;
using GostSpec2023.Processors;

namespace GostSpec2023.Services
{
    public class ParameterCopyService : IParameterCopyService
    {
        private readonly Dictionary<BuiltInCategory, ICategoryProcessor> _processors;

        public ParameterCopyService()
        {
            _processors = new Dictionary<BuiltInCategory, ICategoryProcessor>
            {
                { BuiltInCategory.OST_PipeCurves, new PipeProcessor() }
                // Добавим другие процессоры позже
            };
        }

        public void CopyParameters(Document doc)
        {
            double zapas = GetGlobalZapas(doc);

            foreach (var pair in _processors)
            {
                var elements = new FilteredElementCollector(doc)
                    .OfCategory(pair.Key)
                    .WhereElementIsNotElementType()
                    .ToElements();

                pair.Value.ProcessElements(doc, elements, zapas);
            }
        }

        private double GetGlobalZapas(Document doc)
        {
            var param = new FilteredElementCollector(doc)
                .OfClass(typeof(GlobalParameter))
                .Cast<GlobalParameter>()
                .FirstOrDefault(gp => gp.Name == GostSpecConstants.GlobalParam_Zapas);

            if (param?.GetValue() is DoubleParameterValue val)
                return val.Value;

            return 1.0; // по умолчанию
        }
    }
}