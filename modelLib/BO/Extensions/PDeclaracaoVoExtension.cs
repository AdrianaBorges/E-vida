using eVidaGeneralLib.VO.Adesao;
using eVidaGeneralLib.VO.Protheus;
using System;

namespace eVidaGeneralLib.BO.Extensions
{
    // Titular da declaração
    public static class PDeclaracaoVoExtension
    {
        public static bool TitularDaDeclaracaoOuUsuarioDuplicado(this PDeclaracaoVO thisObj, PUsuarioVO thisUsuario)
        {
            if (thisObj == null || thisUsuario == null)
                return false;

            if (thisObj.Titular.Nome.Trim()
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

        private static bool DocRgDuplicado(PDeclaracaoVO thisObj, PUsuarioVO thisUsuario)
        {
            if (!string.IsNullOrEmpty(thisObj.Titular.Rg?.Trim()) && !string.IsNullOrEmpty(thisUsuario.Drgusr?.Trim()))
            {
                if (thisObj.Titular.Rg
                    .Equals(thisUsuario.Drgusr,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool DocCpfDuplicado(PDeclaracaoVO thisObj, PUsuarioVO thisUsuario)
        {
            if (!string.IsNullOrEmpty(thisObj.Titular.Cpf?.Trim()) && !string.IsNullOrEmpty(thisUsuario.Cpfusr?.Trim()))
            {
                if (thisObj.Titular.Cpf 
                    .Equals(thisUsuario.Cpfusr,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }


        public static bool TitularDaDeclaracaoOuUsuarioDuplicado(this PDeclaracaoVO thisObj, PVidaVO thisVida)
        {
            if (thisObj == null || thisVida == null)
                return false;

            if (thisObj.Titular.Nome.Trim()
                .Equals(thisVida.Nomusr?.Trim(),
                    StringComparison.InvariantCultureIgnoreCase))
            {
                if (!DocCnsDuplicado(thisObj, thisVida))
                    return false;

            }

            return true;

        }
        private static bool DocCnsDuplicado(PDeclaracaoVO thisObj, PVidaVO thisVida)
        {
            if (!string.IsNullOrEmpty(thisObj.Titular.Cns?.Trim()) && !string.IsNullOrEmpty(thisVida.Nrcrna?.Trim()))
            {
                if (thisObj.Titular.Cns
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
