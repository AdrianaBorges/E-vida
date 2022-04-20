﻿<%@ Page Title="Protocolo de Fatura" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormProtocoloFaturaNova.aspx.cs" Inherits="eVidaIntranet.Forms.FormProtocoloFaturaNova" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
	<script type="text/javascript">
		var POP_CREDENCIADO = 0;
		function ConfigControlsCustom() {
			createLocator(650, 550, dlgOpen, null, dlgCallback);
		}

		function dlgOpen(handler, ev, ui) {
			var src = "";
			switch (handler.tipo) {
			    case POP_CREDENCIADO: src = '../GenPops/PopCredenciado.aspx?hospital=true'; break;
			}
			src += '&ID=' + handler.id;
			setLocatorUrl(src);
		}
		function dlgCallback(handler, response) {
			$("#<%: hidCodCredenciado.ClientID%>").val(response);
			executeLocatorHandlerPost(handler);
		}

		function openPopCredenciado() {
		    defaultOpenPop(POP_CREDENCIADO, $("#<%: btnBuscarCredenciado.ClientID %>")[0], 0, 0, "Buscar credenciado");
			return false;
		}
	    function openImprimir() {
	        var id = $('#<%: hidProtocolo.ClientID %>').val();
	        window.open('PopImprimirProtocoloFatura.aspx?ID=' + id);
	        return false;
		}

		function confirmGerar() {
			return confirm("Deseja realmente gerar o protocolo com estes dados?");
		}
		function goVoltar() {
		    window.location = 'BuscaProtocoloFatura.aspx';
		    return false;
		}
		function goNovo() {
		    //if (confirm("Deseja ir para a tela de criar nova fatura? Se houver dados não salvos nesta tela serão perdidos"))
		        window.location = 'FormProtocoloFaturaNova.aspx';
		    return false;
		}
		function goEdit() {
		    var id = $('#<%: hidProtocolo.ClientID %>').val();
		    window.location = 'FormProtocoloFatura.aspx?ID=' + id;
		    return false;
		}
	</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
<table width="100%" style="padding:20px">
	<tr>
		<td>
			<asp:HiddenField ID="hidProtocolo" runat="server" />
			<h2>FILTROS</h2>
			<table id="tbDados" class="tbBorda" border="1">
				<tr>
					<td>
						<asp:HiddenField ID="hidCodCredenciado" runat="server" />
						<b>CPF/CNPJ*</b>
						<asp:ImageButton ID="btnBuscarCredenciado" runat="server" ImageUrl="~/img/lupa.gif" 
							OnClientClick="return openPopCredenciado()" OnClick="btnBuscarCredenciado_Click" /><br />
						<asp:Label ID="txtCpfCnpj" runat="server" Width="180px" />				
					</td>
					<td>
						<b>Razão Social *</b><br />
						<asp:Label ID="txtRazaoSocial" runat="server" Width="400px"/>&nbsp;
					</td>
				</tr>
				<tr>
					<td><b>Data de Entrada *</b><br />
						<asp:TextBox ID="txtDataEntrada" runat="server" CssClass="calendario" Width="100px" MaxLength="10" /></td>
                    <td><b>Doc. Fiscal</b><br />
                        <asp:TextBox ID="txtDocFiscal" runat="server" Width="200px" MaxLength="200" />
                    </td>
				</tr>
                <tr>
                    <td><b>Valor Apresentado:</b><br />
                        <asp:TextBox ID="txtValorApresentado" runat="server" Width="200px" MaxLength="20" />
                    </td>
                    <td><b>Data de Emissão:</b><br />
                        <asp:TextBox ID="txtDataEmissao" runat="server" CssClass="calendario" Width="100px" MaxLength="10" /></td>
                </tr>
                <tr>
                    <td><b>Data de Vencimento:</b><br />
                        <asp:TextBox ID="txtDataVencimento" runat="server" CssClass="calendario" Width="100px" MaxLength="10" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:center">
                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar PEG" OnClick="btnBuscar_Click" />
                        <asp:Button ID="btnImprimir" runat="server" Text="Imprimir Protocolo" Visible="false" OnClientClick="return openImprimir();" />
                        <asp:Button ID="btnVoltar" runat="server" Text="Ir para tela de Busca" OnClientClick="return goVoltar()" />
                        <asp:Button ID="btnEditar" runat="server" Text="Ir para Edição" OnClientClick="return goEdit()" Visible="false" />
                        <asp:Button ID="btnNova" runat="server" Text="Limpar Filtros" OnClientClick="return goNovo()" />
                    </td>
                </tr>

                <tr>
                    <td colspan="2" align="center">
                        <asp:GridView ID="gdvProtocolo" runat="server" Width="100%" CssClass="tabela"
                            DataKeyNames="BCI_CODPEG, BCI_CODRDA" AutoGenerateColumns="false" OnRowDataBound="gdvProtocolo_RowDataBound"
                            OnRowCommand="gdvProtocolo_RowCommand">
                            <Columns>
                                <asp:ButtonField ButtonType="Link" Text="Salvar" CommandName="CmdSalvar"  />
                                <asp:BoundField DataField="BCI_CODPEG" HeaderText="PROTOCOLO" />
                                <asp:BoundField DataField="BCI_CGCRDA" HeaderText="CPF/CNPJ" />
                                <asp:BoundField DataField="BCI_NOMRDA" HeaderText="RAZÃO SOCIAL" />
                                <asp:BoundField DataField="BCI_YREDOC" HeaderText="DATA DE ENTRADA" />
                                <asp:BoundField DataField="BCI_YDOC" HeaderText="DOC. FISCAL" />
                                <asp:BoundField DataField="BCI_YVLDOC" HeaderText="VALOR APRESENTADO" />
                                <asp:BoundField DataField="BCI_YEMISS" HeaderText="DATA DE EMISSÃO" />
                                <asp:BoundField DataField="BCI_YVEDOC" HeaderText="DATA DE VENCIMENTO" />
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>

			</table>
			<br />
        </td>
    </tr>
</table>
</asp:Content>
