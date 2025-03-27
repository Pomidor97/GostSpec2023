using System.Collections.Generic;
using Autodesk.Revit.DB;
using GostSpec2023.Constants;
using GostSpec2023.Utils;

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

                    string srcName    = RevitParamUtils.GetParamAsString(pipeType, GostSpecConstants.SRC_Naimen);
                    var typTruby   = RevitParamUtils.GetParamAsDouble(pipe, GostSpecConstants.SRC_TypTruby, true);
                    var pipeOut    = RevitParamUtils.GetParamAsDouble(pipe, "Внешний диаметр");
                    var pipeIn     = RevitParamUtils.GetParamAsDouble(pipe, "Внутренний диаметр");
                    string pipeSize   = RevitParamUtils.GetParamAsString(pipe, "Размер");

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

                    RevitParamUtils.SetParam(pipe, GostSpecConstants.S_Name, targetName);
                    RevitParamUtils.SetParam(pipe, GostSpecConstants.S_Marka,   RevitParamUtils.GetParamAsString(pipeType, GostSpecConstants.SRC_Marka));
                    RevitParamUtils.SetParam(pipe, GostSpecConstants.S_KodIzd,  RevitParamUtils.GetParamAsString(pipeType, GostSpecConstants.SRC_KodIzd));
                    RevitParamUtils.SetParam(pipe, GostSpecConstants.S_Zavod,   RevitParamUtils.GetParamAsString(pipeType, GostSpecConstants.SRC_Zavod));
                    RevitParamUtils.SetParam(pipe, GostSpecConstants.S_EdIzm,   RevitParamUtils.GetParamAsString(pipeType, GostSpecConstants.SRC_EdIzm));
                    RevitParamUtils.SetParam(pipe, GostSpecConstants.S_Primech, RevitParamUtils.GetParamAsString(pipe, GostSpecConstants.SRC_Primech));

                    double lengthInternal = RevitParamUtils.GetParamAsDouble(pipe, "Длина");
                    double lengthMM       = UnitUtils.ConvertFromInternalUnits(lengthInternal, DisplayUnitType.DUT_MILLIMETERS);
                    double finalCount     = (lengthMM * zapas) / 1000.0;
                    RevitParamUtils.SetParam(pipe, GostSpecConstants.S_Count, finalCount);
                }
                catch
                {
                    // Игнорировать ошибки, как в оригинале
                }
            }
        }
    }
}
