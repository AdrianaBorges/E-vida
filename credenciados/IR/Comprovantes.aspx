<%@ Page Title="Comprovantes IR" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="Comprovantes.aspx.cs" Inherits="eVidaCredenciados.IR.Comprovantes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		function openIr(tipo) {
			window.open('Comprovantes.aspx?DW_FILE=' + tipo);
			return false;
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="850px" cellspacing="10px">
		<tr>
			<td style="width:300px"><b>Comprovante de rendimentos e retenções:</b></td>
			<td><asp:LinkButton ID="lnkIr" runat="server" Text="Baixar" OnClientClick="return openIr(1111)"   /></td>
		</tr>
		<tr>
			<td></td>
		</tr>
	</table>
	
</asp:Content>
