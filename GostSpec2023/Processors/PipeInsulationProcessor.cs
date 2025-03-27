using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using GostSpec2023.Constants;
using GostSpec2023.Processors;
using GostSpec2023.Utils;
using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;

namespace GostSpec2023.Processors
{
    public class PipeInsulationProcessor : ICategoryProcessor
    {
        public void ProcessElements(Document doc, IList<Element> elements, double zapas)
        {
            foreach (Element izolyacya in elements)
            {
                try
                {
                    // Проверка: хост — труба
                    var hostPipe = doc.GetElement((izolyacya as PipeInsulation)?.HostElementId) as Pipe;
                    if (hostPipe == null)
                    {
                        RevitParamUtils.SetParam(izolyacya, GostSpecConstants.S_Name, "Исключить");
                        continue;
                    }

                    var type = doc.GetElement(izolyacya.GetTypeId());

                    // Параметры
                    string srcName = RevitParamUtils.GetParamAsString(izolyacya, GostSpecConstants.SRC_Naimen);
                    string typIzol = RevitParamUtils.GetParamAsString(type, GostSpecConstants.SRC_TypIzol);
                    string pipeSize = RevitParamUtils.GetParamAsString(izolyacya, "Размер", true); // возьмётся у хоста

                    double tolshina = RevitParamUtils.GetParamAsDouble(izolyacya, "Толщина изоляции");
                    tolshina = UnitUtils.ConvertFromInternalUnits(tolshina, DisplayUnitType.DUT_MILLIMETERS);

                    // Формирование имени
                    string trgName = srcName;

                    if (typIzol.Contains("1"))
                    {
                        trgName += $" {tolshina}мм, для труб {pipeSize}";
                    }
                    else if (typIzol.Contains("2"))
                    {
                        double izolSize = RevitParamUtils.GetParamAsDouble(izolyacya, GostSpecConstants.SRC_IzolSize);
                        trgName += $" {tolshina}мм, диаметром {izolSize:0.#}мм";
                    }




                    RevitParamUtils.SetParam(izolyacya, GostSpecConstants.S_Name, trgName);
                    RevitParamUtils.SetParam(izolyacya, GostSpecConstants.S_Marka, RevitParamUtils.GetParamAsString(izolyacya, GostSpecConstants.SRC_Marka));
                    RevitParamUtils.SetParam(izolyacya, GostSpecConstants.S_KodIzd, RevitParamUtils.GetParamAsString(izolyacya, GostSpecConstants.SRC_KodIzd));
                    RevitParamUtils.SetParam(izolyacya, GostSpecConstants.S_Zavod, RevitParamUtils.GetParamAsString(izolyacya, GostSpecConstants.SRC_Zavod));
                    RevitParamUtils.SetParam(izolyacya, GostSpecConstants.S_EdIzm, RevitParamUtils.GetParamAsString(izolyacya, GostSpecConstants.SRC_EdIzm));
                    RevitParamUtils.SetParam(izolyacya, GostSpecConstants.S_Primech, RevitParamUtils.GetParamAsString(izolyacya, GostSpecConstants.SRC_Primech));

                    // Расчёт количества
                    if (typIzol.Contains("Д"))
                    {
                        double len = RevitParamUtils.GetParamAsDouble(izolyacya, "Длина");
                        len = UnitUtils.ConvertFromInternalUnits(len, DisplayUnitType.DUT_MILLIMETERS);
                        RevitParamUtils.SetParam(izolyacya, GostSpecConstants.S_Count, len * zapas / 1000);
                    }
                    else if (typIzol.Contains("П"))
                    {
                        double area = RevitParamUtils.GetParamAsDouble(izolyacya, "Площадь");
                        area = UnitUtils.ConvertFromInternalUnits(area, DisplayUnitType.DUT_SQUARE_METERS);
                        RevitParamUtils.SetParam(izolyacya, GostSpecConstants.S_Count, area * zapas);
                    }
                    else if (typIzol.Contains("О"))
                    {
                        double area = RevitParamUtils.GetParamAsDouble(izolyacya, "Площадь");
                        area = UnitUtils.ConvertFromInternalUnits(area, DisplayUnitType.DUT_SQUARE_METERS);
                        RevitParamUtils.SetParam(izolyacya, GostSpecConstants.S_Count, zapas * area * tolshina / 1000);
                    }
                    else if (string.IsNullOrWhiteSpace(typIzol))
                    {
                        RevitParamUtils.SetParam(izolyacya, GostSpecConstants.S_Count, -999999);
                    }
                }
                catch
                {
                    // Тихо игнорируем ошибки, как в оригинале
                }
            }
        }
    }
}
