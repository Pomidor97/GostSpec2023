using Autodesk.Revit.DB;


namespace GostSpec2023.RevitParamUtils
{
    public class RevitParamUtils
    {
        public static string GetParamAsString(Element elem, string paramName)
        {
            if (elem == null) return string.Empty;
            var p = elem.LookupParameter(paramName);
            return p != null ? p.AsString() : string.Empty;
        }
        
        public static double GetParamAsDouble(Element elem, string paramName, bool typeParameter = false)
        {
            if (elem == null) return 0.0;

            Parameter p = elem.LookupParameter(paramName);

            if (p == null || typeParameter)
            {
                var type = elem.Document.GetElement(elem.GetTypeId());
                p = type?.LookupParameter(paramName);
            }

            return p != null ? p.AsDouble() : 0.0;
        }

        
        public static void SetParam(Element elem, string paramName, string value)
        {
            if (elem == null) return;
            var p = elem.LookupParameter(paramName);
            if (p != null && !p.IsReadOnly)
            {
                p.Set(value);
            }
        }
        
        public static void SetParam(Element elem, string paramName, double value)
        {
            if (elem == null) return;
            var p = elem.LookupParameter(paramName);
            if (p != null && !p.IsReadOnly)
            {
                p.Set(value);
            }
        }
    }
}