<%@ Page Title="Protocolo de Fatura" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormProtocoloFatura.aspx.cs" Inherits="eVidaIntranet.Forms.FormProtocoloFatura" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        var POP_CREDENCIADO = 0;

        function ConfigControlsCustom() {
            createLocator(650, 550, dlgOpen, null, dlgCallback);

            $('#<%= txtValorApresentado.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtValorProcessado.ClientID %>').priceFormat(formatPreco);
            $('#<%= txtValorGlosa.ClientID %>').priceFormat(formatPreco);
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
            if (handler.tipo == POP_CREDENCIADO)
                $("#<%: hidCodCredenciado.ClientID%>").val(response);

            executeLocatorHandlerPost(handler);
        }

        function openPopCredenciado() {
            defaultOpenPop(POP_CREDENCIADO, $("#<%: btnBuscarCredenciado.ClientID %>")[0], 0, 0, "Buscar credenciado");
            return false;
        }

        function goVoltar() {
            window.location = 'BuscaProtocoloFatura.aspx';
            return false;
        }
        function goNovo() {
            if (confirm("Deseja ir para a tela de criar nova fatura? Se houver dados não salvos nesta tela serão perdidos"))
                window.location = 'FormProtocoloFaturaNova.aspx';
            return false;
        }
        function openPdf() {
            try {
                var id = '<%= Request["ID"] %>';
                openReport(RELATORIO_PROTOCOLO_FATURA_CAPA, 'ID=' + id);
            } catch (e) {
                alert(e.message);
            }
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
<table width="100%" style="padding:20px">
	<tr>
		<td>
			<asp:HiddenField ID="hidProtocolo" runat="server" />
			<h2>EDIÇÃO</h2>
			<table id="tbDados" class="tbBorda" border="1">
                <tr>
                    <td colspan="2"><b>Protocolo<br /><asp:Literal ID="litProtocolo" runat="server" /></b></td>
                    <td><b>Natureza:</b><br />
                        <asp:Literal ID="litNatureza" runat="server" />
                    </td>
                </tr>
				<tr>
					<td>
						<asp:HiddenField ID="hidCodCredenciado" runat="server" />
						<b>CPF/CNPJ*</b>
						<asp:ImageButton ID="btnBuscarCredenciado" runat="server" ImageUrl="~/img/lupa.gif" 
							OnClientClick="return openPopCredenciado()" OnClick="btnBuscarCredenciado_Click" /><br />
						<asp:Label ID="txtCpfCnpj" runat="server" Width="180px" />				
					</td>
					<td colspan="2">
						<b>Razão Social *</b><br />
						<asp:Label ID="txtRazaoSocial" runat="server" Width="400px"/>&nbsp;
					</td>
				</tr>
                <tr>
                    <td colspan="3">
                        <b>Analista Responsável</b>						
						<asp:Label ID="lblAnalista" runat="server" Width="400px" />				
                    </td>
                </tr>
				<tr>
                    <td><b>Doc. Fiscal</b><br />
                        <asp:TextBox ID="txtDocFiscal" runat="server" Width="200px" MaxLength="200" />
                    </td>
                    <td><b>Data de Finalização:</b><br />
                        <asp:TextBox ID="txtDataFinalizacao" runat="server" CssClass="calendario" Width="100px" MaxLength="10" /></td>
                    <td><b>Data de Expedição:</b><br />
                        <asp:TextBox ID="txtDataExpedicao" runat="server" CssClass="calendario" Width="100px" MaxLength="10" /></td>

                </tr>
                <tr>
                    <td><b>Data de Emissão:</b><br />
                        <asp:TextBox ID="txtDataEmissao" runat="server" CssClass="calendario" Width="100px" MaxLength="10" /></td>                    
					<td><b>Data de Entrada *</b><br />
						<asp:TextBox ID="txtDataEntrada" runat="server" CssClass="calendario" Width="100px" MaxLength="10" /></td>                    
                    <td><b>Vencimento</b><br />
                        <asp:TextBox ID="txtVencimento" runat="server" CssClass="calendario" Width="80px" MaxLength="10" />
                    </td>
				</tr>
                <tr>
                    <td><b>Valor Apresentado:</b><br />
                        <asp:TextBox ID="txtValorApresentado" runat="server" Width="200px" MaxLength="20" />
                    </td>
                    <td><b>Valor Processado:</b><br />
                        <asp:TextBox ID="txtValorProcessado" runat="server" Width="200px" MaxLength="20" />
                    </td>
                    <td><b>Glosa:</b><br />
                        <asp:TextBox ID="txtValorGlosa" runat="server" Width="200px" MaxLength="20" />
                    </td>
                </tr>
                <tr>

                </tr>
                <tr>
                    <td><b>Situação</b><br />
                        <asp:DropDownList ID="dpdSituacao" runat="server" />
                        <asp:Label ID="lblMotivo" runat="server" Visible="false" />
                    </td>
                    <td colspan="2"><b>Unidade:</b><br />
                        <asp:Literal ID="litUnidade" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <b>Pendência</b><br />
                        <asp:DropDownList ID="dpdPendencia" runat="server" DataValueField="Id" DataTextField="Nome" Width="450px" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="text-align:center">
                        <asp:Button ID="btnAssumir" runat="server" Text="Assumir responsabilidade" OnClick="btnAssumir_Click" />
                        <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" Visible="false"/>
                        <asp:Button ID="btnVoltar" runat="server" Text="Ir para tela de Busca" OnClientClick="return goVoltar()" />
                        <asp:Button ID="btnCapa" runat="server" Text="Capa de Lote" OnClientClick="return openPdf()" Visible="false" />
                        <asp:Button ID="btnNova" runat="server" Text="Novo Protocolo" OnClientClick="return goNovo()" />
                    </td>
                </tr>
			</table>
			<br />
        </td>
    </tr>
</table>
</asp:Content>
