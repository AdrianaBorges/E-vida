<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopAutorizacaoChanger.aspx.cs" Inherits="eVidaIntranet.Forms.PopAutorizacaoChanger" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function setSolicitacao(id) {
        	parent.locatorCallback(id);
        }
		function checkNeg(id) {
			$("#<%:hidNeg.ClientID%>").val(id);
		}
    	function openPdfNeg(id) {
    		openReport(RELATORIO_NEGATIVA, 'ID=' + id, true);
    		return false;
    	}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<table width="500px" id="tbSolDoc" runat="server" visible="false">
        <tr>
            <td>Solicitação de documentos adicionais para Autorização <asp:Literal ID="litProtocoloSolDoc" runat="server" />.<br /></td>
        </tr>
        <tr>
            <td>Informe a mensagem a ser colocada no e-mail de solicitação abaixo:</td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtConteudoSolDoc" runat="server" Width="450px" TextMode="MultiLine" Rows="5" /></td>
        </tr>
        <tr>
            <td align="center"><br /><asp:Button ID="btnEnviarSolDoc" runat="server" Text="Enviar solicitção" OnClick="btnEnviar_Click" /></td>
        </tr>
    </table>
	<table width="500px" id="tbCancel" runat="server" visible="false">
        <tr>
            <td>Cancelar Autorização <asp:Literal ID="litProtocoloCancel" runat="server" />.<br /></td>
        </tr>
        <tr>
            <td>Informe o motivo de cancelamento:</td>
        </tr>
        <tr>
            <td><asp:TextBox ID="txtCancel" runat="server" Width="480px" /></td>
        </tr>
        <tr>
            <td align="center"><br /><asp:Button ID="btnCancel" runat="server" Text="Cancelar solicitação" OnClick="btnEnviar_Click" /></td>
        </tr>
    </table>
	<table width="500px" id="tbNegar" runat="server" visible="false">
        <tr>
            <td>Negar Autorização <asp:Literal ID="litProtocoloNegar" runat="server" />.<br /></td>
        </tr>
        <tr id="trNegBenef" runat="server" visible="false">
            <td>Selecione a negativa associada ao beneficiário: <asp:Label ID="lblBeneficiario" runat="server" Font-Bold="true" /></td>
        </tr>
        <tr id="trNegCred" runat="server" visible="false">
            <td>Negar autorização ao credenciado <asp:Label ID="lblCredenciado" runat="server" Font-Bold="true" /></td>
        </tr>
        <tr>
            <td>
				<asp:HiddenField ID="hidNeg" runat="server" />
				<asp:GridView ID="gdvNegativa" runat="server" AutoGenerateColumns="false" DataKeyNames="CodSolicitacao"
				AllowSorting="false" CssClass="gridView" Width="100%" ItemType="eVidaGeneralLib.VO.FormNegativaVO" >
                <Columns>
					<asp:TemplateField>
						<ItemTemplate>
							<input type="radio" name="negar" onclick='<%# "checkNeg(" + Item.CodSolicitacao + ")" %>' />
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField HeaderText="Protocolo" DataField="CodSolicitacao" DataFormatString="{0:000000000}" />
					<asp:BoundField HeaderText="Data criação" DataField="DataCriacao" DataFormatString="{0:dd/MM/yyyy}" />
					<asp:BoundField HeaderText="Situação" DataField="Status" />
					<asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdfNeg("+ Item.CodSolicitacao + ");" %>' AlternateText="Gerar PDF" ToolTip="Gerar PDF" />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center"><br /><asp:Button ID="btnNegar" runat="server" Text="Negar solicitação" OnClick="btnEnviar_Click" /></td>
        </tr>
    </table>
</asp:Content>
