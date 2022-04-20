<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopUploadClassico.aspx.cs" Inherits="eVidaIntranet.GenPops.PopUploadClassico" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		function afterUpload(newName, original) {
			parent.onAfterUpload(newName, original);
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<asp:HiddenField ID="hidRnd" runat="server" />
    <table width="90%" cellspacing="10px" style="vertical-align:top;">
        <tr>
		    <td colspan="4"><h2 class="componentheading">Envio de Arquivo (método clássico)</h2></td>
	    </tr>
		<tr>
			<td colspan="2">
				Selecione o arquivo e depois clique em enviar
			</td>
		</tr>
        <tr>
            <td>Arquivo:</td>
            <td><asp:FileUpload ID="upFile" runat="server" ToolTip="Selecione o arquivo" AllowMultiple="false" Width="90%" /></td>
        </tr>
		<tr>
			<td colspan="2"><asp:Button ID="btnEnviar" runat="server" Text="Enviar Arquivo" OnClick="btnEnviar_Click" /></td>
		</tr>
		<tr>
			<td colspan="2">
				<b>Tipos suportados:</b> <%: GetUploadFilter() %><br />				
				<b>Tamanho máximo:</b> <%: GetUploadSize()/1024 %>MB
			</td>
		</tr>
	</table>
</asp:Content>
