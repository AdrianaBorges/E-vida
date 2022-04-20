<%@ Page Title="Perfil x Módulo" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="PerfilModulo.aspx.cs" Inherits="eVidaIntranet.Admin.PerfilModulo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="650px" cellspacing="10px" style="min-height:300px; vertical-align:top">
	<tr>
		<td style="width:100px; text-align: right">Perfil:</td>
		<td align="left"><asp:DropDownList ID="dpdPerfil" runat="server" Width="220px" OnSelectedIndexChanged="dpdPerfil_SelectedIndexChanged" AutoPostBack="true" DataValueField="Key" DataTextField="Value" /></td>
	</tr>
    <tr>
        <td><br /></td>
	</tr>
    <tr>
		<td colspan="2">
			<asp:Repeater ID="rptCategoria" runat="server" 
				OnItemDataBound="rptCategoria_ItemDataBound">
				<ItemTemplate><br />
					<b><%# DataBinder.Eval(Container.DataItem, "Nome") %></b>
					<asp:CheckBoxList ID="chkModulo" runat="server" Width="900px" RepeatColumns="3"							
						DataValueField="Id" DataTextField="Nome" RepeatDirection="Horizontal"  RepeatLayout="Table" />
				</ItemTemplate>
				<SeparatorTemplate><hr /></SeparatorTemplate>
			</asp:Repeater>
				
		</td>
	</tr>
	<tr>
		<td colspan="2" style="text-align:center"><asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" /></td>
	</tr>
</table>
</asp:Content>
