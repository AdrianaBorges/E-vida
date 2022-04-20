using eVidaGeneralLib.VO.Protheus;

namespace eVidaGeneralLib.BO.Extensions
{
    public static class PVidaVoExtension
    {
        public static bool PodeValidarNomeECarteiraNacionalDeSaude(this PVidaVO thisObj)
        {
            var resp = thisObj?.Nomusr != null && thisObj.Nrcrna != null;
            return resp;

        }

        public static bool PodeValidarNomeECpfDaVidaNoProtheus(this PVidaVO thisObj)
        {
            var resp = thisObj?.Nomusr != null && thisObj.Cpfusr != null;
            return resp;

        }

        public static bool PodeValidarNomeERgDaVidaNoProtheus(this PVidaVO thisObj)
        {
            var resp = thisObj?.Nomusr != null && thisObj.Drgusr != null;
            return resp;

        }

        public static string NomeDoDependenteNoProtheus(this PVidaVO thisObj)
        {
            return thisObj.Nomusr.Trim();
        }

        public static string CarteiraNacionalDeSaudeDoDependenteNoProtheus(this PVidaVO thisObj)
        {
            return thisObj.Nrcrna.Trim();
        }

        public static string CpfDaVidaNoProtheus(this PVidaVO thisObj)
        {
            return thisObj.Cpfusr.Trim();

        }
        public static string RgDaVidaNoProtheus(this PVidaVO thisObj)
        {
            return thisObj.Drgusr.Trim();

        }
    }
}
