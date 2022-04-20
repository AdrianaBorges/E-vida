<%@ Page Title="CADASTRO DE RESPONSÁVEIS" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="CadastroResponsavel.aspx.cs" Inherits="eVidaIntranet.Gestao.CadastroResponsavel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfigControlsCustom() {
        	createLocator(600, 600, dlgOpen, dlgClose, dlgCallback);            
        }

        function dlgCallback(handler, response) {
        	var currentLine = handler.id;
        	var resp = response;
        	var id = currentLine.id + "_";
        	$('#' + id + currentLine.hid + "_" + currentLine.idx).val(resp.cdBeneficiario);
        	$('#' + id + currentLine.hidNome + "_" + currentLine.idx).val(resp.cdFuncionario + " - " + resp.nmFuncionario);
        	$('#' + id + currentLine.nome + "_" + currentLine.idx).text(resp.cdFuncionario + " - " + resp.nmFuncionario);
        	closeLocator();
		}

    	function dlgOpen(handler, ev, ui) {
    		var src = "PopResponsavel.aspx";
    		setLocatorUrl(src);
    	}

    	function dlgClose(ev, ui) {

    	}
    	function openPop(tipo, obj, id, row) {
    		var handler = new LocatorHandler(tipo, obj, row, obj);
    		var titulo = "Selecionar responsável";    		
    		openLocator(titulo, handler);
    		return false;
    	}

        function openPopResponsavel(pId, idx, hidId, hidNmId, nmId) {
            var currentLine = new Object();
            currentLine.id = pId;
            currentLine.idx = idx;
            currentLine.hid = hidId;
            currentLine.hidNome = hidNmId;
            currentLine.nome = nmId;

            openPop(1, currentLine, idx, idx);
            return false;
        }

        function confirmCancelar() {
            return confirm("Deseja realmente cancelar todas as alterações realizadas?");
        }

        function confirmRemover() {
            return confirm("Deseja realmente excluir este registro? A operação não será efetivada enquanto não for solicitado salvar.");
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
    
    <table width="950px" cellspacing="5px" style="vertical-align:top; min-height: 400px">
        <tr>
            <td colspan="4">
                <h2 class="componentheading">Responsáveis do beneficiário</h2>
            </td>
        </tr>
        <tr>
            <td>Nome:</td>
		    <td align="left" colspan="2"><asp:Label ID="lblNome" runat="server" Width="100%"  /></td>
        </tr>
	    <tr>
            <td >Matrícula:</td>
            <td><asp:Label ID="lblMatricula" runat="server" Width="75px" /></td>	    
		    <td>Cód Alternativo:</td>
		    <td align="left"><asp:Label ID="lblAlternativo" runat="server" Width="220px"  /></td>
		    
	    </tr>
        <tr>
            <td >Plano:</td>
            <td><asp:Label ID="lblPlano" runat="server" Width="150px" /></td>	    
		    <td>Vigência:</td>
		    <td align="left"><asp:Label ID="lblVigencia" runat="server" Width="220px"  /></td>
		    
	    </tr>
        <tr>
            <td colspan="4">
                <table class="tabela" style="width: 950px" cellpadding="0" cellspacing="0">
                <asp:Repeater ID="dlResponsavel" runat="server" OnItemDataBound="dlResponsavel_ItemDataBound">
                    <HeaderTemplate>
                        <tr>
                            <td rowspan="2"></td>
                            <td rowspan="2" style="width: 80px"><b>Início</b></td>
                            <td rowspan="2" style="width: 80px"><b>Fim</b></td>
                            <td colspan="2" align="center" width="250px" ><b>Responsável Financeiro</b></td>
                            <td colspan="2" align="center" width="250px" ><b>Responsável Família</b></td>
                            <td rowspan="2" width="250px"><b>Observação</b></td>
                        </tr>
                        <tr>
                            <td>Matrícula</td>
                            <td>Nome</td>
                            <td>Matrícula</td>
                            <td>Nome</td>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:ImageButton ID="btnRemover" runat="server" ImageUrl="~/img/remove.png" OnClick="btnRemover_Click" 
                                    OnClientClick="return confirmRemover()"/></td>
                            </td>
                            <td>
                                <asp:TextBox ID="txtInicio" runat="server" CssClass="calendario" MaxLength="10" Width="80px" />
                            </td>
                            <td>
                                <asp:TextBox ID="txtFim" runat="server" CssClass="calendario" MaxLength="10" Width="80px" />
                            </td>
                            <td width="250px" colspan="2">
                                <asp:ImageButton ID="btnRespFinanceiro" runat="server" ImageUrl="~/img/lupa.gif" />
                                <asp:HiddenField ID="hidRespFinanceiro" runat="server" />
                                <asp:HiddenField ID="hidNmRespFinanceiro" runat="server" />
                                <asp:Label ID="lblRespFinanceiro" runat="server"  />
                            </td>
                            <td width="250px" colspan="2">
                                <asp:ImageButton ID="btnRespFamilia" runat="server" ImageUrl="~/img/lupa.gif" />
                                <asp:HiddenField ID="hidRespFamilia" runat="server" />
                                <asp:HiddenField ID="hidNmRespFamilia" runat="server" />
                                <asp:Label ID="lblRespFamilia" runat="server" />
                            </td>
                            <td>
                                <asp:TextBox ID="txtObservacao" runat="server" MaxLength="255" Width="250px" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td colspan="8" align="center">Não existe responsáveis cadastrados para o beneficiário</td>
                        </tr>
                    </FooterTemplate>
                </asp:Repeater>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="4" align="center">
                <asp:Button ID="btnAdicionar" runat="server" Text="Adicionar" OnClick="btnAdicionar_Click" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" OnClientClick="return confirmCancelar()" />
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" /></td>
        </tr>
    </table>
</asp:Content>
