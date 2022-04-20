<%@ Page Title="Travamento ISA" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="RelatorioTravamentoISA.aspx.cs" Inherits="eVidaIntranet.Relatorios.RelatorioTravamentoISA" %>
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
				AllowSorting="false" CssClass="tabela" Width="1200px" AllowPaging="False"
                OnRowCommand="gdvRelatorio_RowCommand" OnRowDataBound="gdvRelatorio_RowDataBound" >
                <Columns>
					<asp:BoundField HeaderText="SID" DataField="SID" />
                    <asp:BoundField HeaderText="LOCKWAIT" DataField="LOCKWAIT"/>
                    <asp:BoundField HeaderText="STATUS" DataField="STATUS" />
                    <asp:BoundField HeaderText="USERNAME" DataField="USERNAME"/>
					<asp:BoundField HeaderText="OSUSER" DataField="OSUSER"/>
					<asp:BoundField HeaderText="TERMINAL" DataField="TERMINAL" />
					<asp:BoundField HeaderText="PROGRAM" DataField="PROGRAM" />
					<asp:BoundField HeaderText="SQL_ADDRESS" DataField="SQL_ADDRESS" />
					<asp:BoundField HeaderText="LOGON_TIME" DataField="LOGON_TIME" />
                    <asp:BoundField HeaderText="OBJECT_NAME" DataField="OBJECT_NAME" />
                    <asp:TemplateField>
                        <ItemTemplate>
                           <asp:Button ID="btnKill" runat="server" CommandName="CmdKill" CommandArgument='<%# Eval("SID") + "," + Eval("serialnumber") %>' Text="Finalizar" />
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
