<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopProfissional.aspx.cs" Inherits="eVidaCredenciados.GenPops.PopProfissional" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    	function setProfissional(id) {
    		parent.locatorCallback(id);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
 <table width="90%">
        <tr>
            <td><asp:Label ID="lblConselho" runat="server" Text="Nº do Conselho:" /></td>
            <td colspan="3"><asp:TextBox ID="txtConselho" runat="server" Width="250px" /></td>
            <td rowspan="3"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" /></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblTipoConselho" runat="server" Text="Tipo Conselho:" /></td>
            <td><asp:DropDownList ID="dpdTipoConselho" runat="server" Width="150px" DataValueField="Key" DataTextField="Key" /></td>
			<td>UF:</td>
			<td><asp:DropDownList ID="dpdUfConselho" runat="server" Width="80px" DataValueField="Sigla" DataTextField="Sigla" /></td>
        </tr>
		<tr>
            <td><asp:Label ID="lblNome" runat="server" Text="Nome:" /></td>
            <td colspan="3"><asp:TextBox ID="txtNome" runat="server" Width="250px" /></td>
        </tr>
        <tr>
            <td colspan="5" align="center">
				<asp:Button ID="btnEmpty" runat="server" OnClick="btnEmpty_Click" Visible="false" Text="Incluir" />
                <asp:GridView ID="gdv" runat="server" Width="100%" CssClass="tabela"
                    DataKeyNames="Codigo" AutoGenerateColumns="false"
                    OnRowCommand="gdv_RowCommand" OnRowDataBound="gdv_RowDataBound"
					ItemType="eVidaGeneralLib.VO.Protheus.PProfissionalSaudeVO">
                    <Columns>
                        <asp:ButtonField ButtonType="Link" Text="Sel." CommandName="CmdSelecionar"  />
                        <asp:BoundField DataField="Nome" HeaderText="NOME" />
                        <asp:BoundField HeaderText="Codsig" ItemStyle-Width="150px" />
                    </Columns>
                </asp:GridView>

            </td>
        </tr>
    </table>
</asp:Content>
