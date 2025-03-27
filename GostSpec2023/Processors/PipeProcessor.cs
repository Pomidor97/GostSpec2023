using System.Collections.Generic;
using Autodesk.Revit.DB;
using GostSpec2023.Constants;

namespace GostSpec2023.Processors
{
    public class PipeProcessor : ICategoryProcessor
    {
        public void ProcessElements(Document doc, IList<Element> elements, double zapas)
        {
            foreach (var pipe in elements)
            {
                try
                {
                    var pipeType = doc.GetElement(pipe.GetTypeId());

                    string srcName    = RevitParamUtils.RevitParamUtils.GetParamAsString(pipeType, GostSpecConstants.SRC_Naimen);
                    var typTruby   = RevitParamUtils.RevitParamUtils.GetParamAsDouble(pipe, GostSpecConstants.SRC_TypTruby, true);
                    var pipeOut    = RevitParamUtils.RevitParamUtils.GetParamAsDouble(pipe, "Внешний диаметр");
                    var pipeIn     = RevitParamUtils.RevitParamUtils.GetParamAsDouble(pipe, "Внутренний диаметр");
                    string pipeSize   = RevitParamUtils.RevitParamUtils.GetParamAsString(pipe, "Размер");

                    double thickness  = UnitUtils.ConvertFromInternalUnits((pipeOut - pipeIn) / 2.0, DisplayUnitType.DUT_MILLIMETERS);
                    pipeOut           = UnitUtils.ConvertFromInternalUnits(pipeOut, DisplayUnitType.DUT_MILLIMETERS);
                    pipeIn            = UnitUtils.ConvertFromInternalUnits(pipeIn, DisplayUnitType.DUT_MILLIMETERS);

                    string targetName = typTruby switch
                    {
                        1 => $"{srcName}, {pipeSize}х{thickness:0.#}",
                        2 => $"{srcName}, Ø{pipeOut:0.#}х{thickness:0.#}",
                        3 => $"{srcName}, {pipeSize}",
                        _ => "ТАКОЙ ТИП ТРУБЫ НЕ СУЩЕСТВУЕТ. Доступно 1-3"
                    };

                    RevitParamUtils.RevitParamUtils.SetParam(pipe, GostSpecConstants.S_Name, targetName);
                    RevitParamUtils.RevitParamUtils.SetParam(pipe, GostSpecConstants.S_Marka,   RevitParamUtils.RevitParamUtils.GetParamAsString(pipeType, GostSpecConstants.SRC_Marka));
                    RevitParamUtils.RevitParamUtils.SetParam(pipe, GostSpecConstants.S_KodIzd,  RevitParamUtils.RevitParamUtils.GetParamAsString(pipeType, GostSpecConstants.SRC_KodIzd));
                    RevitParamUtils.RevitParamUtils.SetParam(pipe, GostSpecConstants.S_Zavod,   RevitParamUtils.RevitParamUtils.GetParamAsString(pipeType, GostSpecConstants.SRC_Zavod));
                    RevitParamUtils.RevitParamUtils.SetParam(pipe, GostSpecConstants.S_EdIzm,   RevitParamUtils.RevitParamUtils.GetParamAsString(pipeType, GostSpecConstants.SRC_EdIzm));
                    RevitParamUtils.RevitParamUtils.SetParam(pipe, GostSpecConstants.S_Primech, RevitParamUtils.RevitParamUtils.GetParamAsString(pipe, GostSpecConstants.SRC_Primech));

                    double lengthInternal = RevitParamUtils.RevitParamUtils.GetParamAsDouble(pipe, "Длина");
                    double lengthMM       = UnitUtils.ConvertFromInternalUnits(lengthInternal, DisplayUnitType.DUT_MILLIMETERS);
                    double finalCount     = (lengthMM * zapas) / 1000.0;
                    RevitParamUtils.RevitParamUtils.SetParam(pipe, GostSpecConstants.S_Count, finalCount);
                }
                catch
                {
                    // Игнорировать ошибки, как в оригинале
                }
            }
        }
    }
}
