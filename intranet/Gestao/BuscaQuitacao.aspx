<%@ Page Title="Quitação SAP X ISA" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaQuitacao.aspx.cs" Inherits="eVidaIntranet.Gestao.BuscaQuitacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    
    <table width="100%" cellspacing="10px" style="vertical-align:top;">
        <tr>
		    <td colspan="4"><h2 class="componentheading">Buscar Arquivos SAP</h2></td>
	    </tr>
		<tr>
            <td >Tipo Arquivo:</td>
            <td align="left" colspan="3"><asp:DropDownList ID="dpdTipoArquivo" runat="server" Width="200px"/></td>
        </tr>
	    <tr>
            <td >Mês Folha:</td>
            <td align="left" colspan="3"><asp:DropDownList ID="dpdMes" runat="server" Width="120px" /> <asp:DropDownList ID="dpdAno" runat="server" Width="90px" /></td>
        </tr>
        <tr>
            <td >Situação:</td>
            <td align="left" colspan="3"><asp:DropDownList ID="dpdStatus" runat="server" Width="120px" /></td>
        </tr>
        <tr>
            <td colspan="4" style="text-align:center"><asp:Button ID="btnPesquisar" runat="server" Text="Pesquisar" OnClick="btnPesquisar_Click" />
                <asp:Button ID="btnNovo" runat="server" Text="Novo Arquivo" OnClick="btnNovo_Click" />

            </td>
        </tr>
        <tr style="min-height: 500px">
            <td colspan="4" valign="top">
                <asp:Label ID="lblCount" runat="server" />
                <asp:GridView ID="gdvArquivos" runat="server" AutoGenerateColumns="false" DataKeyNames="ID_ARQUIVO"
				AllowSorting="false" CssClass="gridView" Width="100%"
                OnRowDataBound="gdvArquivos_RowDataBound">
                <Columns>
                    <asp:BoundField HeaderText="Mês Folha" DataField="DT_FOLHA" SortExpression="DT_FOLHA" DataFormatString="{0:MM/yyyy}" />
                    <asp:BoundField HeaderText="Seq" DataField="NR_SEQUENCIAL" SortExpression="NR_SEQUENCIAL" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="Tipo" DataField="TP_ARQUIVO" />
					<asp:BoundField HeaderText="Nome arquivo" DataField="NM_ARQUIVO" SortExpression="NM_ARQUIVO" />
					<asp:BoundField HeaderText="Data importação" DataField="DT_IMPORTACAO" SortExpression="DT_IMPORTACAO" DataFormatString="{0:dd/MM/yyyy}" />
					<asp:BoundField HeaderText="Data recebimento" DataField="DT_RECEBIMENTO" SortExpression="DT_RECEBIMENTO" DataFormatString="{0:dd/MM/yyyy}"  />                    
					<asp:BoundField HeaderText="Situação" DataField="CD_STATUS" SortExpression="CD_STATUS"   />
                    <asp:HyperLinkField HeaderText="" DataNavigateUrlFields="ID_ARQUIVO" DataNavigateUrlFormatString="Quitacao.aspx?id={0}"
						    Text="&lt;img src='../img/lupa.gif' alt='Visualizar detalhes' border='0'/&gt;" />
				</Columns>
			</asp:GridView>
            </td>
        </tr>
    </table>
    

</asp:Content>
