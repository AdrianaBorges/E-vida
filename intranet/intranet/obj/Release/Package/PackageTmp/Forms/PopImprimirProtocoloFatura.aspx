<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopImprimirProtocoloFatura.aspx.cs" Inherits="eVidaIntranet.Forms.PopImprimirProtocoloFatura" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        window.print();
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table border="1" class="tbBorda" style="width:290px; font-family: Arial">
        <tr>
            <td><b>
                PROTOCOLO Nº <asp:Literal ID="litProtocolo" runat="server" /><br />
                NATUREZA: <asp:Literal ID="litNatureza" runat="server" /> <br />
                CPF/CNPJ: <asp:Literal ID="litCpfCnpj" runat="server" /> <br />
                RAZÃO SOCIAL: <asp:Literal ID="litRazaoSocial" runat="server" /> <br />
				DATA DE ENTRADA: <asp:Literal ID="litDataEntrada" runat="server" /> <br />
				USUÁRIO DO PROTOCOLO: <asp:Literal ID="litUsuarioProtocolo" runat="server" />
                </b></td>
        </tr>
    </table>
</asp:Content>
