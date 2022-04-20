<%@ Page Title="Autorização Provisória" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormAutorizacaoProvisoria.aspx.cs" Inherits="eVidaIntranet.Forms.FormAutorizacaoProvisoria" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		var POP_BENEFICIARIO = 0;
		function ConfigControlsCustom() {
			createLocator(650, 550, dlgOpen, null, dlgCallback);
		}

		function dlgOpen(handler, ev, ui) {
			var src = "";
			switch (handler.tipo) {
				case POP_BENEFICIARIO: src = '../GenPops/PopBeneficiario.aspx?'; break;
			}
			src += '&ID=' + handler.id;
			setLocatorUrl(src);
		}

		function dlgCallback(handler, response) {

		    var dados_beneficiario = response.split("|");
		    var codint = dados_beneficiario[0];
		    var codemp = dados_beneficiario[1];
		    var matric = dados_beneficiario[2];
		    var tipreg = dados_beneficiario[3];

		    $("#<%: hidCodint.ClientID%>").val(codint);
		    $("#<%: hidCodemp.ClientID%>").val(codemp);
		    $("#<%: hidMatric.ClientID%>").val(matric);
		    $("#<%: hidTipreg.ClientID%>").val(tipreg);

			executeLocatorHandlerPost(handler);
		}

		function openPopBeneficiario() {
			defaultOpenPop(POP_BENEFICIARIO, $("#<%: btnBuscarBeneficiario.ClientID %>")[0], 0, 0, "Buscar beneficiário");
			return false;
		}

		function openPdf() {
			try {
				var id = $('#<%: hidProtocolo.ClientID %>').val();
				openReport(RELATORIO_AUTORIZACAO_PROVISORIA, 'ID=' + id);
			} catch (e) {
				alert(e.message);
			}
			return false;
		}

		function goVoltar() {
			window.location = 'BuscaAutorizacaoProvisoria.aspx';
		}

		function confirmGerar() {
			return confirm("Deseja realmente gerar a autorização? Após geração não será permitido alteração de dados. Se houver algum ajuste realizado deve ser salvo antes de gerar!");
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<table width="100%" style="padding:20px">
		<tr>
			<td>

			<asp:HiddenField ID="hidProtocolo" runat="server" />
			<table id="tbCabecalho" class="tbBorda" border="1">
				<tr>
					<td>
						<asp:Label ID="lblProtocolo" runat="server" Text="Protocolo" Font-Bold="true" /><br />
						<asp:Label ID="txtProtocolo" runat="server" Width="150px"/>&nbsp;
					</td>
					<td>
						<asp:Label ID="lblDataSolicitacao" runat="server" Text="Data de Solicitação" Font-Bold="true" /><br />
						<asp:Label ID="txtDataSolicitacao" runat="server" Width="150px"/>&nbsp;
					</td>
				</tr>
				<tr>
					<td colspan="2"><b>Situação:</b><br />
						<asp:Label ID="lblSituacao" runat="server" /><asp:Label ID="lblMotivo" runat="server" />
					</td>
				</tr>
			</table>

			<h2>1 - Dados do Beneficiário</h2>
			<table id="tbTitular" border="1">
				<tr>
					<td>
						<asp:HiddenField ID="hidCodint" runat="server" />
                        <asp:HiddenField ID="hidCodemp" runat="server" />
                        <asp:HiddenField ID="hidMatric" runat="server" />
                        <asp:HiddenField ID="hidTipreg" runat="server" />

						<asp:Label ID="lblNumCartao" runat="server" Text="Número do Cartão" Font-Bold="true" /> *
						<asp:ImageButton ID="btnBuscarBeneficiario" runat="server" ImageUrl="~/img/lupa.gif" 
							OnClientClick="return openPopBeneficiario()" OnClick="btnBuscarBeneficiario_Click" /><br />
						<asp:TextBox ID="txtNumCartao" runat="server" Width="180px" OnTextChanged="txtNumCartao_TextChanged" AutoPostBack="true"/>				
					</td>
					<td>
						<asp:Label ID="lblNomeDependente" runat="server" Text="Nome do Beneficiário" Font-Bold="true" /><br />
						<asp:Label ID="txtNomeDependente" runat="server" Width="400px"/>&nbsp;
					</td>
					<td><b>Validade *</b><br />
						<asp:TextBox ID="txtValidade" runat="server" CssClass="calendario" Width="100px" MaxLength="10" /></td>
				</tr>
				<tr>
					<td>
						<asp:Label ID="lblPlano" runat="server" Text="Plano Vinculado" Font-Bold="true" /><br />
						<asp:Label ID="txtPlano" runat="server" Width="190px"/>&nbsp;
					</td>
					<td>
						<asp:Label ID="lblNomeTitular" runat="server" Text="Nome Titular" Font-Bold="true" /><br />
						<asp:Label ID="txtNomeTitular" runat="server" Width="400px"/>&nbsp;
					</td>            		
					<td>
						<asp:Label ID="lblMatricula" runat="server" Text="Matrícula Titular" Font-Bold="true" /><br />
						<asp:Label ID="txtMatricula" runat="server" />
					</td>
				</tr>
			</table>
			<br />

			<h2>2 - DADOS DA SOLICITACAO</h2>
			<table id="tbSolicitacao" class="tbBorda" border="1">
				<tr>
					<td colspan="3">
						<asp:Label ID="lblLocal" runat="server" Text="Local *" Font-Bold="true" /><br />
						<asp:DropDownList ID="dpdLocal" runat="server" Width="400px"
							 OnSelectedIndexChanged="dpdLocal_SelectedIndexChanged" AutoPostBack="true" />
					</td>
					<td><b>Plantão Social:</b><br />
						<asp:Label ID="lblPlantaoSocial" runat="server" Width="200px" /></td>
				</tr>
				<tr>
					<td colspan="2" rowspan="2"><b>Autorização de Atendimento  para Coirmãs?</b>
						<asp:DropDownList ID="dpdReciprocidade" runat="server" Width="120px"
							OnSelectedIndexChanged="dpdReciprocidade_SelectedIndexChanged" AutoPostBack="true" >
							<asp:ListItem Text="SELECIONE" Value="" />
							<asp:ListItem Text="SIM" Value="S" />
							<asp:ListItem Text="NÃO" Value="N" />
						</asp:DropDownList>
					</td>
					<td colspan="2">
						<b>Cobertura:</b>
						<asp:CheckBoxList ID="chkCobertura" runat="server" RepeatDirection="Horizontal" Enabled="false">
							<asp:ListItem Value="AMB" Text="AMB" />
							<asp:ListItem Value="OBS" Text="OBS" />
							<asp:ListItem Value="ODO" Text="ODO" />
							<asp:ListItem Value="UrE" Text="Urgência e Emergência" />
						</asp:CheckBoxList>
					</td>			
				</tr>
				<tr>
					<td colspan="2">
						<b>Abrangência:</b>
						<asp:RadioButtonList ID="rbAbrangencia" runat="server" RepeatDirection="Horizontal" Enabled="false">
							<asp:ListItem Value="N" Text="NACIONAL" />
							<asp:ListItem Value="R" Text="REGIONAL" />
						</asp:RadioButtonList>
					</td>
				</tr>
			</table>
			<h2>3 - PROCEDIMENTOS AUTORIZADOS</h2>
			<asp:CheckBoxList ID="chkAssistencias" runat="server" />
				
			<h2>4 - OBSERVAÇÕES</h2>
			<asp:TextBox ID="txtObs" runat="server" TextMode="MultiLine" Rows="5" Width="95%" />
			</td>
		</tr>
		<tr>
			<td style="text-align:center"><br />
				<asp:Button ID="btnVoltar" runat="server" Text="Voltar" OnClientClick="return goVoltar()" />
				<asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" Height="30px" OnClientClick="return openPdf();" Visible="false" />
				<asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
				<asp:Button ID="btnGerar" runat="server" Text="Gerar Autorização de Atendimento" OnClick="btnGerar_Click" Visible="false" OnClientClick="return confirmGerar()" />
			</td>
		</tr>
	</table>
</asp:Content>
