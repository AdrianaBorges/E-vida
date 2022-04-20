<%@ Page Title="Carta Positiva CRA" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormCartaPosCra.aspx.cs" Inherits="eVidaIntranet.Forms.FormCartaPosCra" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        
        var POP_CREDENCIADO = 3;
        function ConfigControlsCustom() {
            createLocator(650, 550, dlgOpen, null, dlgCallback);
        }
        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_CREDENCIADO: src = '../GenPops/PopCredenciado.aspx?enableEmpty=true'; break;
            }
            setLocatorUrl(src);
        }

        function dlgCallback(handler, response) {
            switch (handler.tipo) {
                case POP_CREDENCIADO:
                    defaultDlgCallback(handler, response);
                    break;
            }
        }
        function openPopCred(btnLoc) {
            var handler = new LocatorHandler(POP_CREDENCIADO, '<%: hidCredenciado.ClientID %>', -1, btnLoc);
            openLocator("Credenciado", handler);
            return false;
        }
        function openCartaPdf(id) {
            openReport(RELATORIO_CARTA_POSITIVA_CRA, "ID=" + id, true);
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo" style="margin-left:20px">
        
        <h2>1 - Dados da solicitação</h2>
        <table cellpadding="10px" class="tabelaFormBorder" width="100%" >
            <tr><td><b>Identificador:</b></td><td><asp:Label ID="lblId" runat="server" /></td>
                <td><b>Situação:</b></td>
                <td><asp:Label ID="lblSituacao" runat="server" /></td></tr>
            <tr>
                <td><b>Tipo de Carta:</b></td>
                <td colspan="3"><asp:RadioButtonList ID="rblTipo" runat="server" RepeatDirection="Horizontal" CellPadding="10" CssClass="tabelaNoBorda" >
                    <asp:ListItem Text="EM CARÊNCIA" Value="1" />
                    <asp:ListItem Text="ISENTO DE CARÊNCIA" Value="2" />
                    </asp:RadioButtonList></td>
            </tr>
            <tr>
                <td><b>Protocolo Registrado CRA</b></td>
                <td colspan="3"><asp:TextBox ID="txtProtocoloCra" runat="server" MaxLength="100" Width="300px" /></td>
            </tr>
            <tr>
                <td><b>Criado por:</b></td>
                <td style="min-width:250px"><asp:Label ID="lblCriadoPor" runat="server" /></td>
                <td><b>Alterado por:</b></td>
                <td style="min-width:250px"><asp:Label ID="lblAlteradoPor" runat="server" /></td>
            </tr>
        </table>
        <h2>2 - Dados do Beneficiário</h2>
	    <table id="tbTitular" border="1">
            <tr>
                <td>
				    <b>Cartão Titular *</b><br />
				    <asp:TextBox ID="txtCartaoTitular" runat="server" OnTextChanged="txtCartao_TextChanged" AutoPostBack="true" Width="160px"/>
                </td>
                <td>
				    <asp:Label ID="lblDependente" runat="server" Text="Nome do Beneficiário *" Font-Bold="true" /><br />
				    <asp:DropDownList ID="dpdBeneficiario" runat="server" Width="400px"
                        DataValueField="Cdusuario" DataTextField="Nomusr" OnSelectedIndexChanged="dpdBeneficiario_SelectedIndexChanged" AutoPostBack="true" />
                </td>                    
            </tr>
            <tr>
                <td><b>CPF:</b><br />
                    <asp:Label ID="lblCpf" runat="server" />
                </td>
                <td><b>Cartão:</b><br />
                    <asp:Label ID="lblCartao" runat="server" />
                </td>
            </tr>
            <tr>
                <td><b>Data de Nascimento:</b><br />
                    <asp:Label ID="lblDataNascimento" runat="server" />
                </td>
                <td><b>Filiação:</b><br />
                    <asp:Label ID="lblFiliacao" runat="server" />
                </td>
            </tr>
            <tr>
                <td><b>Plano:</b><br />
                    <asp:HiddenField ID="hidPlano" runat="server" />
                    <asp:Label ID="lblPlano" runat="server" />
                </td>
                <td><b>Vigência do Plano:</b><br />
                    <asp:Label ID="lblVigencia" runat="server" />
                </td>
            </tr>
        </table>
        <br />
        <h2>3 - Dados do Credenciado</h2>
	    <table id="tbCredenciado" class="tbBorda" border="1">
            <tr>
				<td style="width:350px">
					<asp:HiddenField ID="hidCredenciado" runat="server" />
                    <asp:Label ID="lblCpfCnpj" runat="server" Text="CPF/CNPJ *" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtCpfCnpj" runat="server" Width="200px" AutoPostBack="true" OnTextChanged="txtCpfCnpj_TextChanged"/>
					<asp:ImageButton ID="btnLocCred" runat="server" ImageUrl="~/img/lupa.gif" OnClientClick="return openPopCred(this)" OnClick="btnLocCred_Click" />
					<asp:ImageButton ID="btnClrCred" runat="server" ImageUrl="~/img/remove.png" OnClick="btnClrCred_Click" />
                </td>
			    <td colspan="2">
				    <asp:Label ID="lblRazaoSocial" runat="server" Text="Nome/Razão Social" Font-Bold="true" /><br />
				    <asp:Label ID="txtRazaoSocial" runat="server" />
                </td>
			</tr>
            <tr>
                <td colspan="2"><b>Contato *</b><br />
                    <asp:TextBox ID="txtContatoCred" runat="server" Width="300px" MaxLength="100" />
                </td>
                <td><b>Data</b><br />
                    <asp:Label ID="lblDataSolicitacao" runat="server" />
                </td>
            </tr>
        </table>
        <br />
        <div style="text-align:center">
                    <br /><br /><br /><br />      
                    ____________________________________<br />
            <asp:Label ID="lblAprovador" runat="server" /><br />
            <asp:Label ID="lblCargoAprovador" runat="server" /><br />
            <br />
            
            <asp:Button ID="btnSalvar" runat="server" Text="Salvar Formulário" OnClick="btnSalvar_Click" />
            <asp:ImageButton ID="btnPdf" runat="server" ImageUrl="~/img/PDF.png" OnClientClick="return openCartaPdf()" Visible="false" Height="50px" />
        </div>
    </div>
</asp:Content>
