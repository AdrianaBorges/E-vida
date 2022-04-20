<%@ Page Title="Indisponibilidade de Rede" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="BuscaIndisponibilidadeRede.aspx.cs" Inherits="eVidaIntranet.Forms.BuscaIndisponibilidadeRede" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        var POP_ENCERRAR = 0;

        function ConfigControlsCustom() {
            $('#<%= txtProtocolo.ClientID %>').ForceNumericOnly();
            $('#<%= txtProtocoloAns.ClientID %>').ForceNumericOnly();
            $('#<%= txtMatricula.ClientID %>').ForceNumericOnly();

            createLocator(650, 550, dlgOpen, dlgClose, dlgCallback);
        }

        function dlgCallback(handler, response) {
            closeLocator();
            if (handler.btn.href)
                eval(handler.btn.href);
            else if ($(handler.btn).attr('href'))
                eval($(handler.btn).attr('href'));
            else
                __doPostBack(handler.btn.name, '');
        }

        function dlgOpen(handler, ev, ui) {
            var src = "";
            switch (handler.tipo) {
                case POP_ENCERRAR: src = '../FormsPop/PopCancelIndisponibilidadeRede.aspx?TIPO=ENCERRAR'; break;
            }
            src += '&ID=' + handler.id;
            setLocatorUrl(src);
        }

        function dlgClose(ev, ui) {

        }

        function openEncerrar(obj, id, row) {
            openPop(POP_ENCERRAR, obj, id, row);
            return false;
        }

        function openPop(tipo, obj, id, row) {
            var handler = new LocatorHandler(tipo, id, row, obj);
            var titulo = "";
            switch (tipo) {
                case POP_ENCERRAR: titulo = "Encerrar Solicitação"; break;
            }

            openLocator(titulo, handler);
            return false;
        }
        function openEdit(btn, id) {
            window.location = "IndisponibilidadeRede.aspx?id=" + id;
            return false;
        }

        function executarCobranca(obj, id, row) {
            if (confirm("Deseja realmente enviar ao financeiro para executar a cobrança integral?")) {
                return true;
            }
            return false;
        }
        function goNova(id) {
            window.location = 'IndisponibilidadeRede.aspx';
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">    
<table width="1100px" cellspacing="10px">
	<tr>
		<td colspan="6"><h2 class="componentheading">Filtros</h2></td>
	</tr>
    <tr>
        <td style="width:100px; text-align: right">Requerimento:</td>
		<td align="left"><asp:TextBox ID="txtProtocolo" runat="server" Width="120px" MaxLength="10"  /></td>
        <td style="width:200px; text-align: right">Protocolo ANS:</td>
        <td align="left"><asp:TextBox ID="txtProtocoloAns" runat="server" Width="220px" MaxLength="20"  /></td>
        <td style="width:200px; text-align: right">Matrícula:</td>
        <td align="left"><asp:TextBox ID="txtMatricula" runat="server" Width="120px" MaxLength="20"  /></td>
    </tr>
	<tr>
        <td style="width:100px; text-align: right">Setor atual:</td>
		<td align="left"><asp:DropDownList ID="dpdSetorAtual" runat="server" Width="220px" /></td>
		<td style="width:150px; text-align: right">Situação: </td>
		<td align="left"><asp:DropDownList ID="dpdSituacao" runat="server" Width="220px" /></td>        
        
	</tr>    
    <tr>
        <td style="width:100px; text-align: right">UF:</td>
		<td align="left"><asp:DropDownList ID="dpdUf" runat="server" Width="220px" DataValueField="Value" OnSelectedIndexChanged="dpdUf_SelectedIndexChanged" AutoPostBack="true" /></td>
		<td style="width:150px; text-align: right">Município: </td>
		<td align="left"><asp:DropDownList ID="dpdMunicipio" runat="server" Width="320px" /></td>  
        <td style="width:150px; text-align: right">Pendência: </td>
		<td align="left"><asp:DropDownList ID="dpdPendencia" runat="server" Width="180px" /></td>              
    </tr>
	<tr>
		<td colspan="6" style="text-align:center"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"/>
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="btnExportar_Click" Visible="false" />
			<asp:Button ID="btnNovo" runat="server" Text="Nova Solicitação" OnClientClick="return goNova();" />
		</td>
	</tr>
    </table>
    <table>
	<tr style="height:300px">
		<td colspan="4">
            <asp:Label ID="lblCount" runat="server" />
			<asp:GridView ID="gdvRelatorio" runat="server" AutoGenerateColumns="false" DataKeyNames="CD_INDISPONIBILIDADE"
				AllowSorting="false" CssClass="gridView" Width="2000px" 
                OnRowDataBound="gdvRelatorio_RowDataBound" OnRowCommand="gdvRelatorio_RowCommand"
                PageSize="20" AllowPaging="true" OnPageIndexChanging="gdvRelatorio_PageIndexChanging">
                <Columns>
					<asp:BoundField HeaderText="Requerimento" DataField="CD_INDISPONIBILIDADE" DataFormatString="{0:000000000}" />
                    <asp:BoundField HeaderText="Protocolo ANS" DataField="NR_PROTOCOLO_ANS" />
                    <asp:BoundField HeaderText="Empresa" DataField="BA1_CODEMP" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="Matrícula" DataField="BA1_MATEMP" DataFormatString="{0:0}" />
					<asp:BoundField HeaderText="Beneficiário" DataField="BA1_NOMUSR" ItemStyle-Width="200px" />
					<asp:BoundField HeaderText="UF" DataField="SG_UF" />
                    <asp:BoundField HeaderText="Município" DataField="BID_DESCRI" />
                    <asp:BoundField HeaderText="Data da Solicitação" DataField="DT_SOLICITACAO" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField HeaderText="Especialidade" DataField="NM_ESPECIALIDADE" />
                    <asp:BoundField HeaderText="Serviço" DataField="NR_PRIORIDADE" />
                    <asp:BoundField HeaderText="Prazo Estimado (dias)" DataField="NR_DIAS_PRAZO" />
                    <asp:BoundField HeaderText="Vencimento" DataField="VENCIMENTO" />
                    <asp:BoundField HeaderText="Dias de Atraso" DataField="DIAS_ATRASO" />
                    <asp:BoundField HeaderText="Setor Atual" DataField="ID_SETOR_ENCAMINHAMENTO" />
                    <asp:TemplateField HeaderText="Responsável Atual">
                        <ItemTemplate>
                            <asp:Label ID="lblResponsavel" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
					<asp:BoundField HeaderText="Situação" DataField="ID_SITUACAO" />
                    <asp:TemplateField HeaderText="Pendência">
                        <ItemTemplate>
                            <asp:Label ID="lblPendencia" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemStyle Width="125px" />
                        <ItemTemplate>
                            <asp:ImageButton ID="btnEditar" runat="server" ImageUrl="~/img/ico_editar.gif" Height="25px" OnClientClick='<%# "return openEdit(this, "+ Eval("CD_INDISPONIBILIDADE") + ");" %>' AlternateText="Editar Solicitação" ToolTip="Editar Solicitação" />                            
                            <asp:LinkButton  ID="btnEncerrar" runat="server" CommandName="CmdEncerrar" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openEncerrar") %>'>
                                <asp:Image ID="imgEncerrar"  runat="server" ImageUrl="~/img/ok.jpg" Height="25px" AlternateText="Encerrar Solicitação" ToolTip="Encerrar Solicitação" />
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnExecutarCobranca" runat="server" CommandName="CmdExecutarCobranca" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "executarCobranca") %>'>
                                <asp:Image ID="imgExecutarCobranca"  runat="server" ImageUrl="~/img/process-info.png" Height="25px" AlternateText="Executar Cobrança" ToolTip="Executar Cobrança" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
				</Columns>
			</asp:GridView>
		</td>
	</tr>

</table>
</asp:Content>
