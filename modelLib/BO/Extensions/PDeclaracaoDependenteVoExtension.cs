using System;
using System.Runtime.InteropServices.WindowsRuntime;
using eVidaGeneralLib.VO.Adesao;
using eVidaGeneralLib.VO.Protheus;

namespace eVidaGeneralLib.BO.Extensions
{
    public static class PDeclaracaoDependenteVoExtension
    {
   
        public static bool DependenteDaDeclaracaoOuUsuarioDuplicado(this PDeclaracaoDependenteVO thisObj, PUsuarioVO thisUsuario)
        {
            if (thisObj == null || thisUsuario == null)
                return false;

            if (thisObj.Nome.Trim() 
                .Equals(thisUsuario.Nomusr?.Trim(),
                    StringComparison.InvariantCultureIgnoreCase))
            {
                if (!DocCpfDuplicado(thisObj, thisUsuario))
                    return false;

                if (!DocRgDuplicado(thisObj, thisUsuario))
                    return false;
            }

            return true;

        }

        private static bool DocRgDuplicado(PDeclaracaoDependenteVO thisObj, PUsuarioVO thisUsuario)
        {
            if (!string.IsNullOrEmpty(thisObj.Rg?.Trim()) && !string.IsNullOrEmpty(thisUsuario.Drgusr?.Trim()))
            {
                if (thisObj.Rg 
                    .Equals(thisUsuario.Drgusr,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool DocCpfDuplicado(PDeclaracaoDependenteVO thisObj, PUsuarioVO thisUsuario)
        {
            if (!string.IsNullOrEmpty(thisObj.Cpf?.Trim()) && !string.IsNullOrEmpty(thisUsuario.Cpfusr?.Trim()))
            {
                if (thisObj.Cpf 
                    .Equals(thisUsuario.Cpfusr,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }


        public static bool DependenteDaDeclaracaoOuUsuarioDuplicado(this PDeclaracaoDependenteVO thisObj, PVidaVO thisVida)
        {
            if (thisObj == null || thisVida == null)
                return false;

            if (thisObj.Nome.Trim()
                .Equals(thisVida.Nomusr?.Trim(),
                    StringComparison.InvariantCultureIgnoreCase))
            {
                if (!DocCnsDuplicado(thisObj, thisVida))
                    return false;

            }

            return true;

        }

        private static bool DocCnsDuplicado(PDeclaracaoDependenteVO thisObj, PVidaVO thisVida)
        {
            if (!string.IsNullOrEmpty(thisObj.Cns?.Trim()) && !string.IsNullOrEmpty(thisVida.Nrcrna?.Trim()))
            {
                if (thisObj.Cns
                    .Equals(thisVida.Nrcrna,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
