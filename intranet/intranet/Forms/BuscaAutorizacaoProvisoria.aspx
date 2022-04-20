<%@ Page Title="Autorizações Provisórias" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaAutorizacaoProvisoria.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaAutorizacaoProvisoria" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		var POP_NEGAR = 0;

		function ConfigControlsCustom() {
			$('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();
			$('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

			createLocator(650, 550, dlgOpen, null, defaultDlgCallback);
		}

		function dlgOpen(handler, ev, ui) {
			var src = "";
			switch (handler.tipo) {
				case POP_NEGAR: src = 'PopAutorizacaoProvChanger.aspx?TIPO=NEGAR'; break;
			}
			src += '&ID=' + handler.id;
			setLocatorUrl(src);
		}
		
		function openPdf(id) {
			try {
				openReport(RELATORIO_AUTORIZACAO_PROVISORIA, 'ID=' + id);
			} catch (e) {
				alert(e.message);
			}
			return false;
		}
		function openEdit(btn, id) {
			window.location = "FormAutorizacaoProvisoria.aspx?id=" + id;
			return false;
		}
		function goNova(id) {
			window.location = 'FormAutorizacaoProvisoria.aspx';
			return false;
		}
		function openNegar(obj, id, row) {
			defaultOpenPop(POP_NEGAR, obj, id, row, "Negar Solicitação");
			return false;
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

    
	<table width="100%" cellpadding="10px">
		<tr>
			<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
		</tr>
		<tr>
			<td style="width:100px; text-align: right">Protocolo:</td>
			<td align="left"><asp:TextBox ID="txtProtocolo" runat="server" Width="120px" MaxLength="10"  /></td>
			<td style="width:200px; text-align: right">Matrícula:</td>
			<td align="left"><asp:TextBox ID="txtMatricula" runat="server" Width="120px" MaxLength="20"  /></td>
		</tr>
		<tr>
			<td style="width:150px; text-align: right">Situação: </td>
			<td align="left"><asp:DropDownList ID="dpdSituacao" runat="server" Width="220px" /></td>        
		</tr>
		<tr>
			<td colspan="6" style="text-align:center">
				<asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
				<asp:Button ID="btnNovo" runat="server" Text="Nova Solicitação" OnClientClick="return goNova();" />
				<asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
			</td>
		</tr>
    </table>
	<table width="100%" >
		<tr style="height:300px">
			<td colspan="4">
				<asp:Label ID="lblCount" runat="server" />
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_SOLICITACAO"
					AllowSorting="false" CssClass="tabela" Width="1250px"
					OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand"
					PageSize="20" AllowPaging="true" OnPageIndexChanging="gdvRelatorio_PageIndexChanging">
                <Columns>
					<asp:BoundField HeaderText="Protocolo" DataField="cd_solicitacao" DataFormatString="{0:000000000}" ItemStyle-HorizontalAlign="Right" />
					<asp:BoundField HeaderText="Beneficiário" DataField="benef_NM_BENEFICIARIO" />
					<asp:BoundField HeaderText="Plano" DataField="plano_DS_PLANO" />
					<asp:BoundField HeaderText="Validade" DataField="DT_FIM_VIGENCIA" DataFormatString="{0:dd/MM/yyyy}" />
					<asp:TemplateField HeaderText="Local">
						<ItemTemplate>
							<%# Eval("DS_MUNICIPIO") + " - " + Eval("SG_UF") %>
						</ItemTemplate>
					</asp:TemplateField>
                    <asp:BoundField HeaderText="Usuario criação" DataField="criador_cd_usuario" />
                    <asp:BoundField HeaderText="Data Criação" DataField="DT_CRIACAO" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ItemStyle-HorizontalAlign="Center" />					
					<asp:BoundField HeaderText="Situação" DataField="CD_STATUS" />
					<asp:TemplateField HeaderText="Ações">
						<ItemStyle Wrap="false" />
                        <ItemTemplate>
							<asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="20px" OnClientClick='<%# CreateJsFunctionGrid(Container, "openEdit") %>' />
                            <asp:LinkButton  ID="btnAprovar" runat="server" CommandName="CmdAprovar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' >
                                <asp:Image ID="imgAprovar" runat="server" ImageUrl="~/img/ok.jpg" Height="25px" AlternateText="Gerar Autorização de Atendimento" ToolTip="Gerar Autorização de Atendimento" />
                            </asp:LinkButton>
                            <asp:LinkButton  ID="btnNegar" runat="server" CommandName="CmdNegar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openNegar") %>'>
                                <asp:Image ID="imgNegar" runat="server" ImageUrl="~/img/cancel.png" Height="25px" AlternateText="Negar/Cancelar Solicitação" ToolTip="Negar/Cancelar Solicitação" />
                            </asp:LinkButton>
							<asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick='<%# "return openPdf("+ Eval("cd_solicitacao") + ");" %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
			</td>
		</tr>

	</table>
</asp:Content>
