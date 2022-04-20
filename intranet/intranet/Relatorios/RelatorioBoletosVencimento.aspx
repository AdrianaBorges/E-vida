<%@ Page Title="Boletos por Vencimento" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioBoletosVencimento.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioBoletosVencimento" %>
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
		<td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	</tr>
	<tr>
		<td style="width:140px; text-align: right">Data de Vencimento:</td>
		<td align="left"><asp:TextBox ID="txtVencimento" runat="server" CssClass="calendario" MaxLength="10" Width="100px" /></td>
	</tr>
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
                    <asp:BoundField HeaderText="EMPRESA" DataField="cd_empresa" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="MATRICULA" DataField="cd_funcionario" DataFormatString="{0:0}" />
                    <asp:BoundField HeaderText="NOME TITULAR" DataField="nm_titular"/>
					<asp:BoundField HeaderText="CPF" DataField="nr_cpf" />
					<asp:BoundField HeaderText="E-MAIL" DataField="ds_email" />
					<asp:BoundField HeaderText="NR. SEQ. BOLETO" DataField="nr_seq_boleto" DataFormatString="{0:0}"  />
					<asp:BoundField HeaderText="CÓD. CONVÊNIO" DataField="cd_convenio" />
					<asp:BoundField HeaderText="NOSSO NÚMERO" DataField="nr_nosso_numero" />
                    <asp:BoundField HeaderText="VENCIMENTO" DataField="dt_vencimento" DataFormatString="{0:dd/MM/yyyy}"/>
                    <asp:BoundField HeaderText="VALOR BOLETO" DataField="vl_boleto" DataFormatString="{0:C}" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" />
                    <asp:BoundField HeaderText="CÓD PLANO" DataField="cd_plano_vinculado" />
					<asp:BoundField HeaderText="PLANO" DataField="ds_plano" />
                    <asp:BoundField HeaderText="CÓD CATEGORIA" DataField="cd_categoria" />
					<asp:BoundField HeaderText="CATEGORIA" DataField="ds_categoria" />

				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
