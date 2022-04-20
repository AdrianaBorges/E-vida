<%@ Page Title="Usuarios SCL" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscarUsuariosScl.aspx.cs" Inherits="eVidaIntranet.Admin.BuscarUsuariosScl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="850px" cellspacing="5px" style="vertical-align:top">
    <tr>
		<td colspan="2"><h2 class="componentheading">Filtros</h2></td>
	</tr>
	<tr>
		<td style="width:100px; text-align: right">Login:</td>
		<td align="left"><asp:TextBox ID="txtLogin" runat="server" Width="220px" /></td>
	</tr>
    <tr>
		<td style="width:100px; text-align: right">Nome:</td>
		<td align="left"><asp:TextBox ID="txtNome" runat="server" Width="320px" /></td>
	</tr>
    <tr>
        <td><br /></td>
	</tr>
    <tr>
		<td colspan="2" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
            <asp:Button ID="btnNovo" runat="server" Text="Novo Usuário" OnClick="btnNovo_Click" />
		</td>
	</tr>
    </table>
    <table width="100%">
	    <tr style="height:300px; vertical-align: top">
		    <td>
			    <asp:GridView ID="gdvUsuarios" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_USUARIO"
                    AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging"
				    AllowSorting="false" CssClass="gridView" Width="100%" EmptyDataText="Nenhum registro encontrado" HorizontalAlign="Center">
				    <Columns>
					    <asp:BoundField HeaderText="Login" DataField="cd_usuario" ItemStyle-Width="100px" />
					    <asp:BoundField HeaderText="Nome" DataField="nm_usuario" ItemStyle-Width="500px" />
					    <asp:BoundField HeaderText="Data de Cadastro" DataField="dt_cadastro" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="100px" />
                        <asp:BoundField HeaderText="Ativo" DataField="id_ativo" ItemStyle-Width="50px" />
                        <asp:HyperLinkField HeaderText="" DataNavigateUrlFields="cd_usuario" DataNavigateUrlFormatString="alterarSenhaUsuarioScl.aspx?login={0}"
						    Text="&lt;img src='../img/ico_editar.gif' alt='Alterar Senha' border='0'/&gt;" />
				    </Columns>
			    </asp:GridView>
		    </td>
	    </tr>
    </table>
</asp:Content>
