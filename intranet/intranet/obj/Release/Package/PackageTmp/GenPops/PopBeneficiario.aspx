<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopBeneficiario.aspx.cs" Inherits="eVidaIntranet.GenPops.PopBeneficiario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function setBeneficiario(cd_beneficiario) {
        	if (parent.$locatorConfig) {
        	    parent.locatorCallback(cd_beneficiario);
        	} else {
        	    parent.setBeneficiario(cd_beneficiario);
        	}
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="500px">
        <tr>
            <td><asp:Label ID="lblCartao" runat="server" Text="Cartão identificação:" /></td>
            <td><asp:TextBox ID="txtCartao" runat="server" Width="250px" /></td>
            <td rowspan="2"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" /></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblNome" runat="server" Text="Nome beneficiário:" /></td>
            <td><asp:TextBox ID="txtNome" runat="server" Width="250px" /></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:GridView ID="gdv" runat="server" Width="100%" CssClass="tabela"
                    DataKeyNames="CD_BENEFICIARIO" AutoGenerateColumns="false"
                    OnRowCommand="gdv_RowCommand">
                    <Columns>
                        <asp:ButtonField ButtonType="Link" Text="Selecionar" CommandName="CmdSelecionar"  />
                        <asp:BoundField DataField="BA1_MATANT" HeaderText="CARTÃO" />
                        <asp:BoundField DataField="BA1_NOMUSR" HeaderText="NOME" />
                    </Columns>
                </asp:GridView>

            </td>
        </tr>
    </table>
</asp:Content>
