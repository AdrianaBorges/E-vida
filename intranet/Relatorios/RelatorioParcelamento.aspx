<%@ Page Title="PARCELAMENTOS" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioParcelamento.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioParcelamento" %>
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
        <td colspan="4" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Atualizar" OnClick="btnBuscar_Click" />
                <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
		    </td>        
	</tr>
    </table>


    <table>
	<tr style="height:300px">
		<td>
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				AllowSorting="false" CssClass="tabela" Width="1000px" OnSorting="gdvRelatorio_Sorting" 
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                <Columns>
					<asp:BoundField HeaderText="Mes/Ano Ref" DataField="dt_ano_mes_ref" DataFormatString="{0:dd/MM/yyyy}" />
					<asp:BoundField HeaderText="CARTEIRA" DataField="cd_alternativo"/>
                    <asp:BoundField HeaderText="NOME BENEFICIARIO" DataField="nm_beneficiario"/>
					<asp:BoundField HeaderText="GRUPO LANCTO" DataField="cd_grupo_lancto" />
                    <asp:BoundField HeaderText="PARCELAS GERADAS" DataField="nr_parcelas_geradas" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="TOTAL DE PARCELAS" DataField="nr_parcelas" DataFormatString="{0:0}" />
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
