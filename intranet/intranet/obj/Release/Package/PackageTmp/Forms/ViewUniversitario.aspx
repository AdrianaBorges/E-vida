<%@ Page Title="Visualização da Declaração Universitário" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="ViewUniversitario.aspx.cs" Inherits="eVidaIntranet.Forms.ViewUniversitario" %>
<%@ Import Namespace="eVidaGeneralLib.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    	function openPdf() {
    		openReport(RELATORIO_UNIVERSITARIO, 'ID=<%= Request["ID"] %>');
    		return false;
        }
    	function openDownload() {
    		openFile('<%= FileUtil.FileDir.DECLARACAO_UNIVERSITARIO %>', "ID=<%= Request["ID"] %>");
    		return false;
		}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo" style="margin-left: 15px">
        <asp:Label ID="lblProtocolo" runat="server" Font-Bold="true" />
        <h2>1 - Dados do Titular</h2>
	    <table id="tbTitular" border="1">
            <tr>
			    <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Completo" Font-Bold="true" /><br />
				    <asp:Label ID="txtNomeTitular" runat="server" Width="400px"/>
                </td>
                <td rowspan="2" valign="top">
                    <asp:Label ID="lblCartao" runat="server" Text="Cartão" Font-Bold="true" /><br />
				    <asp:Label ID="txtCartao" runat="server" Width="150px"/>
                </td>
            </tr>
            <tr>
                <td>
				    <asp:Label ID="lblEmailTitular" runat="server" Text="E-mail" Font-Bold="true" /><br />
				    <asp:Label ID="txtEmailTitular" runat="server" Width="400px" MaxLength="150" />
                </td>
            </tr>
        </table>
        <br />
        

        <h2>2 - DEPENDENTE</h2>
		<table id="tbDependente" border="1" class="tabelaForm">
            <tr style="height:50px">
				<td><b>Nome</b><br />
					<asp:Label ID="txtNomeDep" runat="server" Width="500px"/>
				</td>
                <td><b>PARENTESCO</b><br />
                    <asp:Label ID="txtParentesco" runat="server" Width="150px"/>
                </td>
                <td><b>PLANO</b><br />
                    <asp:Label ID="txtPlano" runat="server" Width="200px" />
                </td>
                <td align="center"><b>IDADE</b><br />
                    <asp:Label ID="txtIdade" runat="server" Width="80px" />
                </td>
            </tr>       
        </table>
        <br />

		<br /><br /><br />
        <div>
            <table width="50%" style="margin:auto">
                <tr>                    
                    <td align="center"><asp:Button ID="btnVoltar" runat="server" Text="Voltar" OnClientClick="history.back(-1)" /></td>
					<td align="center"><asp:Button ID="btnArquivo" runat="server" Text="Abrir arquivo" ToolTip="Arquivo anexado" OnClientClick="return openDownload();"/></td>
                    <td align="center"><asp:ImageButton ID="btnPdf" ImageUrl="../img/pdf.png" runat="server" ToolTip="Gerar PDF" Width="50px" CssClass="printer" OnClientClick="openPdf()" /></td>
                </tr>
            </table>
	    
        
        </div>
    </div>
</asp:Content>
