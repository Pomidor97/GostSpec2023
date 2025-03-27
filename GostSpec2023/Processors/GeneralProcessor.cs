using Autodesk.Revit.DB;
using GostSpec2023.Constants;
using GostSpec2023.Utils;
using System.Collections.Generic;

namespace GostSpec2023.Processors
{
    public class GeneralProcessor : ICategoryProcessor
    {
        public void ProcessElements(Document doc, IList<Element> elements, double zapas)
        {
            foreach (Element element in elements)
            {
                RevitParamUtils.SetParam(element, GostSpecConstants.S_Name,
                    RevitParamUtils.GetParamAsString(element, GostSpecConstants.SRC_Naimen));

                RevitParamUtils.SetParam(element, GostSpecConstants.S_Marka,
                    RevitParamUtils.GetParamAsString(element, GostSpecConstants.SRC_Marka));

                RevitParamUtils.SetParam(element, GostSpecConstants.S_KodIzd,
                    RevitParamUtils.GetParamAsString(element, GostSpecConstants.SRC_KodIzd));

                RevitParamUtils.SetParam(element, GostSpecConstants.S_Zavod,
                    RevitParamUtils.GetParamAsString(element, GostSpecConstants.SRC_Zavod));

                RevitParamUtils.SetParam(element, GostSpecConstants.S_EdIzm,
                    RevitParamUtils.GetParamAsString(element, GostSpecConstants.SRC_EdIzm));

                RevitParamUtils.SetParam(element, GostSpecConstants.S_Primech,
                    RevitParamUtils.GetParamAsString(element, GostSpecConstants.SRC_Primech));

                RevitParamUtils.SetParam(element, GostSpecConstants.S_Mass,
                    RevitParamUtils.GetParamAsString(element, GostSpecConstants.SRC_Mass));

                RevitParamUtils.SetParam(element, GostSpecConstants.S_Count, 1.0);
            }
        }
    }
}