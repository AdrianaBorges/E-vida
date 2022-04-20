using eVidaGeneralLib.VO.Protheus;

namespace eVidaGeneralLib.BO.Extensions
{
    public static class PUsuarioVoExtencion
    {
        public static bool PodeValidarNomeECpfDoUsuario(this PUsuarioVO thisObj)
        {
            var resp = thisObj?.Nomusr != null && thisObj.Cpfusr != null;
            return resp;

        }

        public static bool PodeValidarNomeERgDoUsuario(this PUsuarioVO thisObj)
        {
            var resp = thisObj?.Nomusr != null && thisObj.Drgusr != null;
            return resp;

        }

        public static string NomeDoUsuarioNoProtheus(this PUsuarioVO thisObj)
        {
            return thisObj.Nomusr.Trim();
        }

        public static string CpfDoUsuarioNoProtheus(this PUsuarioVO thisObj)
        {
            return thisObj.Cpfusr.Trim();

        }
        public static string RgDoUsuarioNoProtheus(this PUsuarioVO thisObj)
        {
            return thisObj.Drgusr.Trim();

        }

    }
}
