<%@ Page Title="Template de E-mail" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="TemplateEmail.aspx.cs" Inherits="eVidaIntranet.Admin.TemplateEmail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
 <table width="850px" cellspacing="10px">
	    <tr>
            <td style="width:200px; text-align: right">ID:</td>
            <td><asp:Literal ID="ltId" runat="server"/></td>
	    </tr>
        <tr>
		    <td style="text-align: right">Nome:</td>
		    <td align="left"><asp:TextBox ID="txtNome" runat="server" Width="220px" MaxLength="50" /></td>
	    </tr>
        <tr>
		    <td style="text-align: right">Tipo:</td>
		    <td align="left"><asp:DropDownList ID="dpdTipo" runat="server" Width="320px" AutoPostBack="true" OnSelectedIndexChanged="dpdTipo_SelectedIndexChanged" /></td>
	    </tr>
        <tr>
		    <td style="text-align: right">Texto:</td>
		    <td align="left">
                Podem ser utilizadas as seguintes TAGs para substituição no texto:<br />
                <asp:Repeater ID="rptTag" runat="server">
                    <ItemTemplate>$$<%# Container.DataItem %>$$</ItemTemplate>
                    <SeparatorTemplate> , </SeparatorTemplate>
                </asp:Repeater>
                <br />
                <asp:TextBox ID="txtTexto" runat="server" Width="650px" TextMode="MultiLine" Height="200px" Enabled="false" /></td>
	    </tr>
        <tr>
		<td colspan="2" style="text-align:center"><asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
		</td>
	</tr>
</table>
</asp:Content>
