<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopCredenciado.aspx.cs" Inherits="eVidaBeneficiarios.GenPops.PopCredenciado" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    	function setCredenciado(id) {
    		parent.locatorCallback(id);
    	}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="90%">
        <tr>
            <td><asp:Label ID="lblCpfCnpj" runat="server" Text="CPF/CNPJ:" /></td>
            <td><asp:TextBox ID="txtCpfCnpj" runat="server" Width="250px" /></td>
            <td rowspan="2"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" /></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblDescricao" runat="server" Text="Razão Social:" /></td>
            <td><asp:TextBox ID="txtRazaoSocial" runat="server" Width="250px" /></td>
        </tr>
        <tr>
            <td colspan="3" align="center">
				<asp:Button ID="btnEmpty" runat="server" OnClick="btnEmpty_Click" Visible="false" Text="Incluir" />
                <asp:GridView ID="gdv" runat="server" Width="100%" CssClass="tabela"
                    DataKeyNames="BAU_CODIGO" AutoGenerateColumns="false"
                    OnRowCommand="gdv_RowCommand">
                    <Columns>
                        <asp:ButtonField ButtonType="Link" Text="Selecionar" CommandName="CmdSelecionar"  />
                        <asp:BoundField DataField="BAU_CODIGO" HeaderText="CÓDIGO" />
                        <asp:BoundField DataField="BAU_CPFCGC" HeaderText="CPF/CNPJ" />
                        <asp:BoundField DataField="BAU_NOME" HeaderText="RAZÃO SOCIAL" />
                    </Columns>
                </asp:GridView>

            </td>
        </tr>
    </table>
</asp:Content>
