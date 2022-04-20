<%@ Page Title="CADASTRO DE RESPONSAVEL PLANO E RESPONSAVEL FINANCEIRO" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaBenefResponsavel.aspx.cs" Inherits="eVidaIntranet.Gestao.BuscaBenefResponsavel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfigControlsCustom() {

            $('#<%= txtMatricula.ClientID %>').ForceNumericOnly();
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
    <table width="650px" cellspacing="10px" style="vertical-align:top;">
        <tr>
		    <td colspan="4"><h2 class="componentheading">Buscar Beneficiários plano Família</h2></td>
	    </tr>
	    <tr>
            <td >Matrícula:</td>
            <td><asp:TextBox ID="txtMatricula" runat="server" Width="75px" MaxLength="20" /></td>	    
		    <td>Cód Alternativo:</td>
		    <td align="left"><asp:TextBox ID="txtAlternativo" runat="server" Width="150px"  /></td>
        </tr>
        <tr>
		    <td>Nome:</td>
		    <td align="left" colspan="3"><asp:TextBox ID="txtNome" runat="server" Width="450px"  /></td>
	    </tr>
        <tr>
            <td colspan="4"><asp:Button ID="btnPesquisar" runat="server" Text="Pesquisar" OnClick="btnPesquisar_Click" /></td>
        </tr>

        <tr style="min-height: 400px">
            <td colspan="4">
                <asp:Label ID="lblCount" runat="server" />
                <asp:GridView ID="gdvBeneficiario" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_BENEFICIARIO"
				AllowSorting="false" CssClass="gridView" Width="800px">
                <Columns>
					<asp:BoundField HeaderText="Cód Beneficiário" DataField="CD_BENEFICIARIO" SortExpression="CD_BENEFICIARIO" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="Matrícula" DataField="CD_FUNCIONARIO" SortExpression="CD_FUNCIONARIO" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="Nome Beneficiário" DataField="NM_BENEFICIARIO" SortExpression="NM_BENEFICIARIO" />
                    <asp:BoundField HeaderText="Cód. Alternativo" DataField="CD_ALTERNATIVO" SortExpression="CD_ALTERNATIVO" />
                    <asp:HyperLinkField HeaderText="" DataNavigateUrlFields="CD_BENEFICIARIO" DataNavigateUrlFormatString="CadastroResponsavel.aspx?id={0}"
						    Text="&lt;img src='../img/lupa.gif' alt='Visualizar responsáveis' border='0'/&gt;" />
				</Columns>
			</asp:GridView>
            </td>
        </tr>
    </table>
    
</asp:Content>
