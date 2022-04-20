<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopResumoPermUsuario.aspx.cs" Inherits="eVidaIntranet.Admin.PopResumoPermUsuario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <b>Esta é apenas uma compilação das permissões de acordo com os perfis associados ao usuário.</b><br />
    <b>Para incluir ou remover permissões, deve-se alterar os perfis associados ou as permissões de perfis.</b><br /><br />
    <asp:Repeater ID="rptCategoria" runat="server" 
		OnItemDataBound="rptCategoria_ItemDataBound">
		<ItemTemplate><br />
			<b><%# DataBinder.Eval(Container.DataItem, "Nome") %></b>
			<asp:DataList ID="chkModulo" runat="server" RepeatColumns="2" Width="550px" 
				RepeatDirection="Horizontal"  RepeatLayout="Table" Enabled="false">
                <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "Nome") %></ItemTemplate>
                <ItemStyle Width="50%" />
			</asp:DataList>
		</ItemTemplate>
		<SeparatorTemplate><hr /></SeparatorTemplate>
	</asp:Repeater>
</asp:Content>
