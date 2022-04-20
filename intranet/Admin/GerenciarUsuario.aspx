<%@ Page Title="Usuários" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="GerenciarUsuario.aspx.cs" Inherits="eVidaIntranet.Admin.GerenciarUsuario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="650px"  cellspacing="10px" style="vertical-align:top">
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
		<td style="width:100px; text-align: right">Perfil:</td>
		<td align="left"><asp:DropDownList ID="dpdPerfil" runat="server" Width="220px" DataValueField="Key" DataTextField="Value" /></td>
	</tr>
    <tr>
        <td style="width:100px; text-align: right">Regional:</td>
		<td align="left"><asp:DropDownList ID="dpdRegional" runat="server" Width="220px" DataValueField="Key" DataTextField="Value" /></td>
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
			    <asp:GridView ID="gdvUsuarios" runat="server" AutoGenerateColumns="false" 
				    AllowSorting="false" CssClass="tabela" Width="100%" EmptyDataText="Nenhum registro encontrado" HorizontalAlign="Center" >
				    <Columns>
					    <asp:BoundField HeaderText="Nome" DataField="nm_usuario" SortExpression="nm_usuario" ItemStyle-Width="300px" />
					    <asp:BoundField HeaderText="Login" DataField="cd_usuario" SortExpression="cd_usuario" ItemStyle-Width="70px" />
					    <asp:BoundField HeaderText="E-mail" DataField="ds_email" SortExpression="ds_email" />
                        <asp:HyperLinkField HeaderText="" DataNavigateUrlFields="cd_usuario" DataNavigateUrlFormatString="editarUsuario.aspx?id={0}"
						    Text="&lt;img src='../img/ico_editar.gif' alt='Editar Usuário' border='0'/&gt;" />
				    </Columns>
			    </asp:GridView>
		    </td>
	    </tr>
    </table>
</asp:Content>