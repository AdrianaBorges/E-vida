<%@ Page Title="Serviços utilizados por Prestador" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioServicoPrestador.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioServicoPrestador" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
	var POP_SERVICO = 1;
	function ConfigControlsCustom() {
		createLocator(600, 600, dlgOpen, null, dlgCallback);
	}

	function dlgOpen(handler, ev, ui) {
		var src = "";
		switch (handler.tipo) {
			case POP_SERVICO: src = '../GenPops/PopServico.aspx?SHOWT=false'; break;
		}
		setLocatorUrl(src);
	}

	function dlgCallback(handler, response) {
		switch (handler.tipo) {
			case POP_SERVICO:
				$("#" + handler.id).val(response);
				closeLocator();
				__doPostBack(handler.btn.name, '');
				break;
		}
	}

	function openPopServico(btn) {
		openPop(POP_SERVICO, btn, '<%=hidCodServico.ClientID%>');
		return false;
	}

	function openPop(tipo, obj, id, row) {
		var handler = new LocatorHandler(tipo, id, row, obj);
		var titulo = "";
		switch (tipo) {
			case POP_SERVICO: titulo = "Serviços"; break;
		}

		openLocator(titulo, handler);
		return false;
	}

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	
	<table width="1100px" cellspacing="10px">
		<tr>
			<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
		</tr>
		<tr>
			<td><b>Período:</b></td>
				<td align="left"><asp:TextBox ID="txtInicio" runat="server" Width="120px" MaxLength="10" CssClass="calendario"  />
             a <asp:TextBox ID="txtFim" runat="server" Width="120px" MaxLength="10" CssClass="calendario" />
			</td>
			<td><b>Tipo:</b></td>           
			<td><asp:RadioButtonList ID="rblTipoRelatorio" runat="server">
                <asp:ListItem Value="A" Text="Analítico" />
                <asp:ListItem Value="S" Text="Sintético" Selected="True" />
                </asp:RadioButtonList></td>
		</tr>
		<tr>
			<td><b>Regionais:</b></td>
			<td colspan="3">
				<asp:CheckBoxList ID="chkRegional" runat="server" DataValueField="Codigo" DataTextField="Descricao" RepeatColumns="3" Width="1000px" BorderStyle="Solid" />
			</td>
		</tr>
		<tr>
			<td><b>Serviços:</b></td>
			<td align="left" colspan="3">
				<asp:UpdatePanel ID="updPanelServicos" runat="server" UpdateMode="Conditional">
					<ContentTemplate>

					<asp:GridView ID="gdvServicos" runat="server" Width="500px" CssClass="tabela"
						AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false"
						OnRowCommand="gdvServicos_RowCommand">
						<Columns>
							<asp:TemplateField>
								<ItemTemplate>
									<asp:ImageButton ID="btnRemover" runat="server" ImageUrl="~/img/remove.png" CommandName="RemoverServico" CommandArgument='<%# Container.DataItemIndex %>' />
								</ItemTemplate>
							</asp:TemplateField>
							<asp:BoundField DataField="cd_mascara" HeaderText="SERVICO" />
							<asp:BoundField DataField="nm_servico" HeaderText="NOME" />
						</Columns>
					</asp:GridView>
					<asp:HiddenField ID="hidCodServico" runat="server" />
					<asp:Button ID="btnAdicionarServico" runat="server" OnClientClick="return openPopServico(this)" Text="Adicionar Servico" OnClick="btnAdicionarServico_Click" />
					<asp:Button ID="btnLimparServico" runat="server" Text="Limpar Serviços" OnClick="btnLimparServico_Click" Visible="false" />
				
					</ContentTemplate>
				</asp:UpdatePanel>
			</td>
		</tr>
		<tr>		
			<td colspan="4" style="text-align:center">
				<asp:UpdatePanel ID="pnlBotao" runat="server" UpdateMode="Conditional">
					<ContentTemplate>
						<asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
						<asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
					</ContentTemplate>
				</asp:UpdatePanel>
			</td>
		</tr>
    </table>
    <table>
		<tr style="height:300px">
			<td colspan="4">
				<asp:UpdatePanel ID="pnlGrid" runat="server" UpdateMode="Conditional">
					<ContentTemplate>
						<asp:Label ID="lblCount" runat="server" Visible="false" />
						<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
							AllowSorting="false" CssClass="tabela" Width="1500px" OnSorting="gdvRelatorio_Sorting" 
							AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
							<Columns>
								<asp:BoundField HeaderText="DATA REF" DataField="dt_ano_mes_ref" DataFormatString="{0:dd/MM/yyyy}"/>
								<asp:BoundField HeaderText="CODIGO DO SERVIÇO" DataField="cd_mascara"/>
								<asp:BoundField HeaderText="NOME SERVIÇO" DataField="ds_servico" />
								<asp:BoundField HeaderText="CNPJ" DataField="nr_cnpj_cpf" />
								<asp:BoundField HeaderText="CREDENCIADO" DataField="nm_razao_social" />
								<asp:BoundField HeaderText="SITUAÇÃO CREDENCIADO" DataField="st_credenciado" />
								<asp:BoundField HeaderText="EMPREGADOS" DataField="qtd_empregados" DataFormatString="{0:0}" />
								<asp:BoundField HeaderText="ATENDIMENTOS" DataField="qtd_atendimentos" DataFormatString="{0:0}" />
								<asp:BoundField HeaderText="PLANO" DataField="ds_plano" />
								<asp:BoundField HeaderText="BENEFICIÁRIO" DataField="nm_beneficiario" />
								<asp:BoundField HeaderText="CODIGO MUNICIPIO" DataField="cd_municipio" />
								<asp:BoundField HeaderText="MUNICIPIO" DataField="ds_municipio" />
								<asp:BoundField HeaderText="UF" DataField="cd_uf" />
								<asp:BoundField HeaderText="SITUAÇÃO BENEFICIÁRIO" DataField="cd_situacao_benef" />
								<asp:BoundField HeaderText="SISTEMA ATENDIMENTO" DataField="tp_sistema_atend" />
								<asp:BoundField HeaderText="NRO ATENDIMENTO" DataField="nr_atendimento" />
								<asp:BoundField HeaderText="VALOR CALCULADO" DataField="vl_calculado" DataFormatString="{0:C}" />
								<asp:BoundField HeaderText="VALOR APRESENTADO" DataField="vl_apresentado" DataFormatString="{0:C}" />
							</Columns>
						</asp:GridView>
					</ContentTemplate>
				</asp:UpdatePanel>
			</td>
		</tr>

	</table>
</asp:Content>
