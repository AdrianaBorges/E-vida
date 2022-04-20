<%@ Page Title="Boletos não quitados" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioBoletosNaoQuitados.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioBoletosNaoQuitados" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
	    function ConfigControlsCustom() {

	    }

    </script>
    <style type="text/css">
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
	<table width="850px" cellspacing="10px">
    <tr>
        <td colspan="2" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
		</td>        
	</tr>
    </table>
    <table>
	<tr style="height:300px">
		<td>
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				AllowSorting="false" CssClass="tabela" Width="2500px" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
					<asp:BoundField HeaderText="MÊS" DataField="dt_ano_mes_ref" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="EMPRESA" DataField="cd_empresa" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="MATRICULA" DataField="cd_funcionario" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="NOME TITULAR" DataField="nm_titular"/>
					<asp:BoundField HeaderText="NOME BENEFICIÁRIO" DataField="nm_beneficiario"/>
					<asp:BoundField HeaderText="MÊS COBRANÇA" DataField="dt_ano_mes_ref_cob" DataFormatString="{0:dd/MM/yyyy}" />
					<asp:BoundField HeaderText="VENCIMENTO" DataField="dt_vencimento" DataFormatString="{0:dd/MM/yyyy}" />
					<asp:BoundField HeaderText="VALOR" DataField="vl_parcela" DataFormatString="{0:C}"  />
					<asp:BoundField HeaderText="NOSSO NÚMERO" DataField="nr_nosso_numero" />
                    <asp:BoundField HeaderText="MATRICULA RESP. FAMILIA" DataField="cd_mat_resp_familia" DataFormatString="{0:0}"/>
                    <asp:BoundField HeaderText="MATRICULA RESP. FINANCEIRO" DataField="cd_mat_resp_financeiro" DataFormatString="{0:0}" />                    
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
