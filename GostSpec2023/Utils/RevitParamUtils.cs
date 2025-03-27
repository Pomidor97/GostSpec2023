using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;

namespace GostSpec2023.Utils
{
    public class RevitParamUtils
    {
        public static string GetParamAsString(Element elem, string paramName, bool typeParameter = false)
        {
            if (elem == null) return string.Empty;

            // 1. Попробуем найти обычный параметр
            var p = elem.LookupParameter(paramName);

            // 2. Если не найден или явно задано typeParameter — ищем у типа
            if ((p == null || typeParameter) && elem.GetTypeId() != ElementId.InvalidElementId)
            {
                var type = elem.Document.GetElement(elem.GetTypeId());
                p = type?.LookupParameter(paramName);
            }

            // 3. Попробуем найти по BuiltInParameter, если явно известный
            if (p == null)
            {
                if (TryGetBuiltInParameter(elem, paramName, out p)) { }
            }

            // 4. Если элемент — изоляция, пробуем у хоста
            if (p == null && TryGetFromHost(elem, paramName, out p)) { }

            return p != null ? p.AsString() ?? p.AsValueString() : string.Empty;
        }

        public static double GetParamAsDouble(Element elem, string paramName, bool typeParameter = false)
        {
            if (elem == null) return 0.0;

            Parameter p = elem.LookupParameter(paramName);

            if ((p == null || typeParameter) && elem.GetTypeId() != ElementId.InvalidElementId)
            {
                var type = elem.Document.GetElement(elem.GetTypeId());
                p = type?.LookupParameter(paramName);
            }

            // Попробуем как Double
            if (p != null && p.StorageType == StorageType.Double)
            {
                return p.AsDouble();
            }

            // Если параметр строковый, пробуем распарсить значение
            if (p != null && (p.StorageType == StorageType.String || p.StorageType == StorageType.None))
            {
                string value = p.AsString() ?? p.AsValueString();
                if (!string.IsNullOrEmpty(value))
                {
                    value = value.Replace("мм", "").Replace(",", ".").Trim();

                    if (double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double result))
                    {
                        return result;
                    }
                }
            }

            return 0.0;
        }

        private static bool TryGetBuiltInParameter(Element elem, string paramName, out Parameter p)
        {
            p = null;

            // Добавь сюда известные BuiltInParameter-переменные по имени
            if (paramName == "Размер" || paramName == "Размер трубы")
            {
                p = elem.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
                if (p == null)
                    p = elem.get_Parameter(BuiltInParameter.RBS_CALCULATED_SIZE);
            }

            return p != null;
        }

        private static bool TryGetFromHost(Element elem, string paramName, out Parameter p)
        {
            p = null;
            Element host = null;

            if (elem is PipeInsulation pipeIns)
            {
                host = elem.Document.GetElement(pipeIns.HostElementId);
            }
            else if (elem is DuctInsulation ductIns)
            {
                host = elem.Document.GetElement(ductIns.HostElementId);
            }

            if (host == null) return false;

            p = host.LookupParameter(paramName);

            if (p == null && host.GetTypeId() != ElementId.InvalidElementId)
            {
                var hostType = elem.Document.GetElement(host.GetTypeId());
                p = hostType?.LookupParameter(paramName);
            }

            return p != null;
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
