<%@ Page Title="Relatório Gerencial" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioGerencial.aspx.cs" Inherits="eVidaIntranet.Telefonia.RelatorioGerencial" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

    <table width="950px" cellspacing="10px">
	    <tr>
		    <td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	    </tr>
        <tr>
            <td style="width:140px; text-align: right">Setor:</td>
            <td colspan="3"><asp:CheckBoxList ID="chkSetor" runat="server" DataValueField="Id" DataTextField="Nome" RepeatColumns="3" Width="800px" BorderStyle="Solid" /></td>
        </tr>
        <tr>
            <td style="text-align: right">Ramal:</td>
            <td colspan="3"><asp:CheckBoxList ID="chkRamal" runat="server" DataValueField="NrRamal" DataTextField="NrRamal" RepeatColumns="3" Width="800px" BorderStyle="Solid" /></td>
        </tr>
	    <tr>
		    <td style="text-align: right">Período:</td>
		    <td colspan="3"> de <asp:TextBox ID="txtDataInicial" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  />
                 até 
                <asp:TextBox ID="txtDataFinal" runat="server" Width="80px" MaxLength="10" CssClass="calendario"  /></td>
	    </tr>
        <tr>
            <td style="text-align: right">Tipo:</td>
            <td colspan="3"><asp:DropDownList ID="dpdTipo" runat="server">
                <asp:ListItem Text="AMBOS" Value="" />
                <asp:ListItem Text="Recebida" Value="R" />
                <asp:ListItem Text="Originada" Value="O" /></asp:DropDownList></td>
        </tr>
        <tr>
            <td style="text-align: right">Estado:</td>
            <td colspan="3"><asp:CheckBoxList ID="chkEstado" runat="server" RepeatColumns="3" Width="800px" BorderStyle="Solid" >                
                <asp:ListItem Value="ATENDIDA" />           
                <asp:ListItem Value="DESVIADA" />           
                <asp:ListItem Value="TRANSFERIDA" />           
                <asp:ListItem Value="OCUPADO" />       
                <asp:ListItem Value="SEM RESPOSTA" />       
                <asp:ListItem Value="FALHOU" /></asp:CheckBoxList></td>
        </tr>
        <tr>
            <td colspan="4" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
                    <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
                    <asp:Button ID="btnGrafico" runat="server" Text="Gráfico do Período" OnClick="btnGrafico_Click" Visible="false" />
		        </td>        
	    </tr>
    </table>
    <table>
		<tr style="height:300px">
			<td colspan="4">
				<asp:UpdatePanel ID="pnlGrid" runat="server" UpdateMode="Conditional">
					<ContentTemplate>
						<asp:Label ID="lblCount" runat="server" Visible="false" />
						<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" OnRowDataBound="gdvRelatorio_RowDataBound"
							AllowSorting="false" CssClass="tabela" Width="1500px" OnSorting="gdvRelatorio_Sorting" 
							AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
							<Columns>
								<asp:BoundField HeaderText="Data/Hora" DataField="dt_bilhetagem" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"/>
								<asp:BoundField HeaderText="Tipo" DataField="tp_direcao"/>
								<asp:BoundField HeaderText="Duração" DataField="ds_duracao" />
								<asp:BoundField HeaderText="Origem" DataField="RORIGEM_DS_RAMAL" />
								<asp:BoundField HeaderText="Destino" DataField="RDESTINO_DS_RAMAL" />
								<asp:BoundField HeaderText="Estado" DataField="ds_estado" />
							</Columns>
						</asp:GridView>
					</ContentTemplate>
				</asp:UpdatePanel>
			</td>
		</tr>

	</table>
</asp:Content>
