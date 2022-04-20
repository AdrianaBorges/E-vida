<%@ Page Title="Protocolos do Canal Gestante" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaCanalGestante.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaCanalGestante" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        var POP_FINALIZAR = 1;        
        function ConfigControlsCustom() {
            $('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();

            createLocator(650, 550, dlgOpen, null, dlgCallback);
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_FINALIZAR: src = 'PopFinalizarCanalGestante.aspx?'; break;
            }
            src += '&ID=' + handler.id;
            setLocatorUrl(src);
        }
        function dlgCallback(handler, response) {
            executeLocatorHandlerPost(handler);
        }

        function openFinalizar(obj, id, row) {
            defaultOpenPop(POP_FINALIZAR, obj, id, row, "Finalizar Protocolo");
            return false;
        }

        function openEmail(obj, id, row) {
            try {
                defaultOpenPop(POP_EMAIL, obj, id, row, "Enviar email");
            } catch (e) {
                alert(e.message);
            }
            return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
    
	<table width="800px" cellpadding="10px" class="tabelaForm" style="margin-left:20px">
		<tr>
			<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
		</tr>
		<tr>
			<td>Protocolo:</td>
			<td><asp:TextBox ID="txtProtocolo" runat="server" Width="120px" MaxLength="10"  /></td>
            <td>Situação:</td>
            <td><asp:DropDownList ID="dpdSituacao" runat="server" Width="250px" /></td>
		</tr>
        <tr>
			<td>Nº do Cartão:</td>
			<td><asp:TextBox ID="txtCartao" runat="server" Width="220px" MaxLength="30"  /></td>
            <td>Controle:</td>
            <td><asp:DropDownList ID="dpdControle" runat="server" Width="250px" /></td>
        </tr>
		<tr>
			<td colspan="6" style="text-align:center">
				<asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
				<asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
			</td>
		</tr>
    </table>

    
	<table width="100%" >
		<tr style="height:300px">
			<td colspan="4">
				<asp:Label ID="lblCount" runat="server" />
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_PROTOCOLO"
					AllowSorting="false" CssClass="tabela" Width="1400px"
					OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand"
					PageSize="20" AllowPaging="true" OnPageIndexChanging="gdvRelatorio_PageIndexChanging">
                <Columns>
                    <asp:TemplateField HeaderText="Controle">
                        <ItemTemplate>
                            <asp:Image ID="imgControle" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
					<asp:BoundField HeaderText="Protocolo" DataField="CD_PROTOCOLO" DataFormatString="{0:000000000}" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="Data de Solicitacao" DataField="DT_SOLICITACAO" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField HeaderText="Data de Finalização" DataField="DT_FINALIZACAO" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField HeaderText="Cartão" DataField="CD_ALTERNATIVO" />
					<asp:BoundField HeaderText="Beneficiário" DataField="benef_NM_BENEFICIARIO" />
					<asp:BoundField HeaderText="OBS (esclarecimento adicional)" DataField="DS_PENDENCIA" />
                    <asp:BoundField HeaderText="Meio Resposta" DataField="TP_CONTATO" />
					<asp:BoundField HeaderText="Resposta" DataField="DS_RESPOSTA" />
                    <asp:TemplateField HeaderText="Situação">
                        <ItemTemplate>
                            <asp:Literal ID="litSituacao" runat="server" />
                        </ItemTemplate>
					</asp:TemplateField>
                    <asp:TemplateField HeaderText="Ações">
						<ItemStyle Wrap="false" />
                        <ItemTemplate>
                            <asp:LinkButton  ID="btnFinalizar" runat="server" CommandName="CmdFinalizar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openFinalizar") %>'>
                                <asp:Image ID="imgFinalizar" runat="server" ImageUrl="~/img/ok.jpg" Height="25px" AlternateText="Finalizar protocolo" ToolTip="Finalizar Protocolo" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
			</td>
		</tr>

	</table>
</asp:Content>