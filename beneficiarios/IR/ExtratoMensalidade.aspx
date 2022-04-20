<%@ Page Title="EXTRATO DE MENSALIDADE E COPARTICIPAÇÕES" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="ExtratoMensalidade.aspx.cs" Inherits="eVidaBeneficiarios.IR.ExtratoMensalidade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		function ConfigControlsCustom() {
		
    	}

		function openPdf() {
		    var id = $('#<%= hidAno.ClientID%>').val();
		    if (id == "") {
		        alert('Realize a busca');
		        return false;
		    }
        	openReport(RELATORIO_MENSALIDADE_IR, "ANO=" + id);
        	return false;
		}

	    function openPdfFile() {
	        var id = $('#<%= hidAno.ClientID%>').val();
	        if (id == "") {
	            alert('Realize a busca');
	            return false;
	        }
	        openFile('<%= eVidaGeneralLib.Util.FileUtil.FileDir.MENSALIDADE_IR %>', "ID=" + id);
	        return false;
	    }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server" >
    <table width="850px" cellspacing="10px" id="tbFitro">
		<tr>
			<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
		</tr>
		<tr>
			<td style="width:140px; text-align: right">Ano Ref.:</td>
			<td align="left" colspan="3"><asp:DropDownList ID="dpdAno" runat="server" Width="120px" /> </td>
		</tr>
		<tr>
			<td colspan="4" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
                <asp:HiddenField ID="hidAno" runat="server" />
                <asp:ImageButton ID="btnExportar" runat="server" Text="Imprimir" OnClientClick="return openPdf()" 
					Visible="false" ImageUrl="~/img/PDF.png" Height="35px" ImageAlign="AbsMiddle" />
		    </td>        
		</tr>
	</table>
	
    <table>
		<tr style="height:300px">
			<td>
				<% if (false) { %><h3>Total</h3><%} %>

				<asp:GridView ID="gdvTotal" runat="server" AutoGenerateColumns="false"
					AllowSorting="false" CssClass="tabela" Width="800px" AllowPaging="false" Visible="false" >
					<Columns>
						<asp:BoundField HeaderText="Nome" DataField="nm_beneficiario" SortExpression="nm_beneficiario" />
						<asp:BoundField HeaderText="Co-Participação" DataField="vl_despesa_copart" SortExpression="vl_despesa_copart" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="Mensalidade" DataField="vl_despesa_mens" SortExpression="vl_despesa_mens" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="CPF" DataField="nr_cpf" SortExpression="nr_cpf" />
					</Columns>
				</asp:GridView>
			</td>
		</tr>
		<tr>
			<td>
				<% if (false) { %><h3>Detalhes</h3><%} %>
				<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
					AllowSorting="false" CssClass="tabela" Width="800px" AllowPaging="false" Visible="false" >
					<Columns>
						<asp:BoundField HeaderText="Mês" DataField="mes_ano" SortExpression="mes_ano" />
						<asp:BoundField HeaderText="Nome" DataField="nm_beneficiario" SortExpression="nm_beneficiario" />
						<asp:BoundField HeaderText="Co-Participação" DataField="vl_despesa_copart" SortExpression="vl_despesa_copart" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="Mensalidade" DataField="vl_despesa_mens" SortExpression="vl_despesa_mens" DataFormatString="{0:C}" />
						<asp:BoundField HeaderText="CPF" DataField="nr_cpf" SortExpression="nr_cpf" />
					</Columns>
				</asp:GridView>
			</td>
		</tr>

	</table>
</asp:Content>
