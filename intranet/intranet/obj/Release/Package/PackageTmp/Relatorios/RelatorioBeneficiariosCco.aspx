<%@ Page Title="Beneficiários por CCO" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioBeneficiariosCco.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioBeneficiariosCco" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">

    <table width="850px" cellspacing="10px">
	    <tr>
		    <td colspan="4"><h2 class="componentheading">Filtros</h2></td>
	    </tr>
	    <tr>
		    <td style="width:140px; text-align: right">CCO:</td>
		    <td align="left"><asp:TextBox ID="txtCco" runat="server" CssClass="inteiro" MaxLength="12" Width="120px" /></td>
	    </tr>
        <tr>
            <td colspan="2" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
		    </td>        
	    </tr>
    </table>

    
    <table>
	    <tr style="height:300px">
		    <td>
                <asp:Label ID="lblCount" runat="server" />
			    <asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false"
				    AllowSorting="false" CssClass="tabela" Width="1100px" 
                    AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="20" PagerSettings-Position="TopAndBottom" onpageindexchanging="gdvRelatorio_PageIndexChanging" >
                    <EmptyDataTemplate>
                        Não há beneficiários com este CCO
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField HeaderText="CCO" DataField="BA1_CODCCO" />
                        <asp:BoundField HeaderText="Nº DA CARTEIRA" DataField="BA1_MATANT" />
                        <asp:BoundField HeaderText="NOME" DataField="BA1_NOMUSR" />
                        <asp:BoundField HeaderText="COD. PLANO" DataField="BI3_CODIGO" />
                        <asp:BoundField HeaderText="NOME PLANO" DataField="BI3_DESCRI" />
                        <asp:BoundField HeaderText="VALIDADE DA CARTEIRA" DataField="BA1_DTVLCR" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField HeaderText="INÍCIO VIGÊNCIA PLANO" DataField="BA3_DATBAS" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField HeaderText="TÉRMINO VIGÊNCIA" DataField="BA3_DATBLO" DataFormatString="{0:dd/MM/yyyy}" />
				    </Columns>
			    </asp:GridView>
		    </td>
	    </tr>

    </table>
</asp:Content>
